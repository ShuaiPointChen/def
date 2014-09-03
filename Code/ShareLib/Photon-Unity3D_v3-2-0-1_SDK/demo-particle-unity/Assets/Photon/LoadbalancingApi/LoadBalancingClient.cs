// -----------------------------------------------------------------------
// <copyright file="LoadBalancingClient.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides the operations and a state for games using the 
//   Photon LoadBalancing server.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    #region Enums

    /// <summary>Possible states for a LoadBalancingClient.</summary>
    public enum ClientState
    {
        /// <summary>Peer is created but not used yet.</summary>
        Uninitialized,
        /// <summary>Not used currently.</summary>
        PeerCreated,
        /// <summary>Connecting to master (includes connect, authenticate and joining the lobby)</summary>
        ConnectingToMasterserver,
        /// <summary>Connected to master server.</summary>
        ConnectedToMaster,
        /// <summary>Currently not used.</summary>
        Queued,
        /// <summary>Usually when Authenticated, the client will join a game or the lobby (if AutoJoinLobby is true).</summary>
        Authenticated,
        /// <summary>Connected to master and joined lobby. Display room list and join/create rooms at will.</summary>
        JoinedLobby,
        /// <summary>Transition from master to game server.</summary>
        DisconnectingFromMasterserver,
        /// <summary>Transition to gameserver (client will authenticate and join/create game).</summary>
        ConnectingToGameserver,
        /// <summary>Connected to gameserver (going to auth and join game).</summary>
        ConnectedToGameserver,
        /// <summary>Joining game on gameserver.</summary>
        Joining,
        /// <summary>The client arrived inside a room. CurrentRoom and Players are known. Send events with OpRaiseEvent.</summary>
        Joined,
        /// <summary>Currently not used. Instead of OpLeave, the client disconnects from a server (which also triggers a leave immediately).</summary>
        Leaving,
        /// <summary>Currently not used.</summary>
        Left,
        /// <summary>Transition from gameserver to master (after leaving a room/game).</summary>
        DisconnectingFromGameserver,
        /// <summary>Currently not used.</summary>
        QueuedComingFromGameserver,
        /// <summary>The client disconnects (from any server).</summary>
        Disconnecting,
        /// <summary>The client is no longer connected (to any server). Connect to master to go on.</summary>
        Disconnected,
    }

    /// <summary>Ways a room can be created or joined.</summary>
    public enum JoinType
    {
        /// <summary>This client creates a room, gets into it (no need to join) and can set room properties.</summary>
        CreateRoom,
        /// <summary>The room existed already and we join into it (not setting room properties).</summary>
        JoinRoom,
        /// <summary>Done on Master Server and (if successful) followed by a Join on Game Server.</summary>
        JoinRandomRoom
    }

    /// <summary>Enumaration of causes for Disconnects (used in LoadBalancingClient.DisconnectedCause).</summary>
    /// <remarks>Read the individual descriptions to find out what to do about this type of disconnect.</remarks>
    public enum DisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,
        /// <summary>OnStatusChanged: The CCUs count of your Photon Server License is exausted (temporarily).</summary>
        DisconnectByServerUserLimit,
        /// <summary>OnStatusChanged: The server is not available or the address is wrong. Make sure the port is provided and the server is up.</summary>
        ExceptionOnConnect,
        /// <summary>OnStatusChanged: The server disconnected this client. Most likely the server's send buffer is full (receiving too much from other clients).</summary>
        DisconnectByServer,
        /// <summary>OnStatusChanged: This client detected that the server's responses are not received in due time. Maybe you send / receive too much?</summary>
        TimeoutDisconnect,
        /// <summary>OnStatusChanged: Some internal exception caused the socket code to fail. Contact Exit Games.</summary>
        Exception,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.</summary>
        InvalidAuthentication,
        /// <summary>OnOperationResponse: Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.</summary>
        MaxCcuReached,
        /// <summary>OnOperationResponse: Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or master server address.</summary>
        InvalidRegion,
        /// <summary>OnOperationResponse: Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.</summary>
        OperationNotAllowedInCurrentState,
    }

    #endregion

    /// <summary>
    /// This class implements the Photon LoadBalancing workflow by using a LoadBalancingPeer.
    /// It keeps a state and will automatically execute transitions between the Master and Game Servers.
    /// </summary>
    /// <remarks>
    /// This class (and the Player class) should be extended to implement your own game logic.
    /// You can override CreatePlayer as "factory" method for Players and return your own Player instances.
    /// The State of this class is essential to know when a client is in a lobby (or just on the master)
    /// and when in a game where the actual gameplay should take place.
    /// Extension notes:
    /// An extension of this class should override the methods of the IPhotonPeerListener, as they 
    /// are called when the state changes. Call base.method first, then pick the operation or state you
    /// want to react to and put it in a switch-case.
    /// We try to provide demo to each platform where this api can be used, so lookout for those.
    /// </remarks>
    public class LoadBalancingClient : IPhotonPeerListener
    {
        /// <summary>
        /// The client uses a LoadBalancingPeer as API to communicate with the server. 
        /// This is public for ease-of-use: Some methods like OpRaiseEvent are not relevant for the connection state and don't need a override.
        /// </summary>
        public LoadBalancingPeer loadBalancingPeer;

        /// <summary>The version of your client. A new version also creates a new "virtual app" to separate players from older client versions.</summary>
        public string AppVersion { get; set; }

        /// <summary>The AppID as assigned from the Photon Cloud. If you host yourself, this is the "regular" Photon Server Application Name (most likely: "LoadBalancing").</summary>
        public string AppId { get; set; }

        /// <summary>The master server's address. Defaults to "app.exitgamescloud.com:5055". Can be changed before call of Connect.</summary>
        public string MasterServerAddress { get; internal protected set; }

        /// <summary>The game server's address for a particular room. In use temporarily, as assigned by master.</summary>
        public string GameServerAddress { get; internal protected set; }

        /// <summary>Backing field for property.</summary>
        private ClientState state = ClientState.Uninitialized;

        /// <summary>Current state this client is in. Careful: several states are "transitions" that lead to other states.</summary>
        public ClientState State
        {
            get
            {
                return this.state;
            }

            protected internal set
            {
                this.state = value;
            }
        }

        /// <summary>Summarizes (aggregates) the different causes for disconnects of a client.</summary>
        /// <remarks>
        /// A disconnect can be caused by: errors in the network connection or some vital operation failing 
        /// (which is considered "high level"). While operations always trigger a call to OnOperationResponse, 
        /// connection related changes are treated in OnStatusChanged.
        /// The DisconnectCause is set in either case and summarizes the causes for any disconnect in a single
        /// state value which can be used to display (or debug) the cause for disconnection.
        /// </remarks>
        public DisconnectCause DisconnectedCause { get; protected set; }

        /// <summary>Available server (types) for internally used field: server.</summary>
        private enum ServerConnection
        {
            MasterServer,
            GameServer
        }

        /// <summary>The server this client is currently connected or connecting to.</summary>
        private ServerConnection server;

        /// <summary>Backing field for property.</summary>
        private bool autoJoinLobby = true;

        /// <summary>If your client should join random games, you can skip joining the lobby. Call OpJoinRandomRoom and create a room if that fails.</summary>
        public bool AutoJoinLobby
        {
            get
            {
                return this.autoJoinLobby;
            }

            set
            {
                this.autoJoinLobby = value;
            }
        }

        /// <summary>
        /// Same as client.LocalPlayer.Name
        /// </summary>
        public string PlayerName 
        {
            get
            {
                return this.LocalPlayer.Name;
            }

            set
            {
                if (this.LocalPlayer == null)
                {
                    return;
                }

                this.LocalPlayer.Name = value;
            }
        }

        /// <summary>This "list" is populated while being in the lobby of the Master. It contains RoomInfo per roomName (keys).</summary>
        public Dictionary<string, RoomInfo> RoomInfoList = new Dictionary<string, RoomInfo>();

        /// <summary>The current room this client is connected to (null if none available).</summary>
        public Room CurrentRoom;

        /// <summary>The local player is never null but not valid unless the client is in a room, too. The ID will be -1 outside of rooms.</summary>
        public Player LocalPlayer { get; set; }

        /// <summary>Statistic value available on master server: Players on master (looking for games).</summary>
        public int PlayersOnMasterCount { get; set; }

        /// <summary>Statistic value available on master server: Players in rooms (playing).</summary>
        public int PlayersInRoomsCount { get; set; }

        /// <summary>Statistic value available on master server: Rooms currently created.</summary>
        public int RoomsCount { get; set; }

        /// <summary>Internally used to decide if a room must be created or joined on game server.</summary>
        private JoinType lastJoinType;

        /// <summary>Internally used field to make identification of (multiple) clients possible.</summary>
        private static int clientCount;
        
        /// <summary>Internally used identification of clients. Useful to prefix debug output.</summary>
        private int clientId;

        /// <summary>Internally used to trigger OpAuthenticate when encryption was established after a connect.</summary>
        private bool didAuthenticate;

        public LoadBalancingClient()
        {
            this.clientId = ++clientCount;
            this.MasterServerAddress = "app.exitgamescloud.com:5055";
            this.LocalPlayer = this.CreatePlayer(string.Empty, -1, true, null);
            
            this.loadBalancingPeer = new LoadBalancingPeer(this, ConnectionProtocol.Udp);
        }

        public LoadBalancingClient(string masterAddress, string appId, string gameVersion) : this()
        {
            this.MasterServerAddress = masterAddress;
            this.AppId = appId;
            this.AppVersion = gameVersion;
        }

        #region Operations and Commands

        /// <summary>
        /// Starts the "process" to connect to the master server (initial connect).
        /// This includes connecting, establishing encryption, authentification and joining a lobby (if AutoJoinLobby is true).
        /// </summary>
        /// <param name="appId">Your application's name or ID assigned by Photon Cloud (webpage).</param>
        /// <param name="appVersion">The client's version (clients with differing client appVersions are separated and players don't meet).</param>
        /// <param name="playerName">This player's name.</param>
        /// <returns>If the operation could be send.</returns>
        public bool ConnectToMaster(string appId, string appVersion, string playerName)
        {
            this.AppId = appId;
            this.AppVersion = appVersion;
            this.PlayerName = playerName;

            return Connect();
        }

        /// <summary>
        /// Starts the "process" to connect to the master server (initial connect).
        /// This includes connecting, establishing encryption, authentification and joining a lobby (if AutoJoinLobby is true).
        /// </summary>
        /// <remarks>
        /// To connect to the Photon Cloud, a valid AppId must be provided. This is shown in the Photon Cloud Dashboard.
        /// https://cloud.exitgames.com/dashboard
        /// Connecting to the Photon Cloud might fail due to:
        /// - Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        /// - Region not available (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.InvalidRegion)
        /// - Subscription CCU limit reached (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached)
        /// More about the connection limitations:
        /// http://doc.exitgames.com/photon-cloud/SubscriptionErrorCases/#cat-references
        /// </remarks>
        public bool Connect(string serverAddress, string appId)
        {
            this.MasterServerAddress = serverAddress;
            this.AppId = appId;

            return this.Connect();
        }

        /// <summary>
        /// Starts the "process" to connect to the master server (initial connect).
        /// This includes connecting, establishing encryption, authentification and joining a lobby (if AutoJoinLobby is true).
        /// </summary>
        /// <remarks>
        /// To connect to the Photon Cloud, a valid AppId must be provided. This is shown in the Photon Cloud Dashboard.
        /// https://cloud.exitgames.com/dashboard
        /// Connecting to the Photon Cloud might fail due to:
        /// - Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        /// - Region not available (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.InvalidRegion)
        /// - Subscription CCU limit reached (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached)
        /// More about the connection limitations:
        /// http://doc.exitgames.com/photon-cloud/SubscriptionErrorCases/#cat-references
        /// </remarks>
        public virtual bool Connect()
        {
            this.DisconnectedCause = DisconnectCause.None;

            if (this.loadBalancingPeer.Connect(this.MasterServerAddress, this.AppId))
            {
                this.State = ClientState.ConnectingToMasterserver;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disconnects this client from any server.
        /// </summary>
        public void Disconnect()
        {
            this.State = ClientState.Disconnecting;
            this.loadBalancingPeer.Disconnect();
        }

        /// <summary>
        /// This method excutes DispatchIncomingCommands and SendOutgoingCommands in your applications Thread-context.
        /// </summary>
        /// <seealso cref="PhotonPeer.Service"/>
        /// <seealso cref="PhotonPeer.DispatchIncomingCommands"/>
        /// <seealso cref="PhotonPeer.SendOutgoingCommands"/>
        public void Service()
        {
            if (this.loadBalancingPeer != null)
            {
                this.loadBalancingPeer.Service();
            }
        }

        /// <summary>
        /// Internally used only.
        /// </summary>
        private void DisconnectToReconnect()
        {
            this.State = (this.server == ServerConnection.MasterServer)
                             ? ClientState.DisconnectingFromMasterserver
                             : ClientState.DisconnectingFromGameserver;
            this.loadBalancingPeer.Disconnect();
        }

        /// <summary>
        /// Internally used only.
        /// Starts the "process" to connect to the game server (connect before a game is joined).
        /// </summary>
        private bool ConnectToGameServer()
        {
            if (this.loadBalancingPeer.Connect(this.GameServerAddress, this.AppId))
            {
                this.State = ClientState.ConnectingToGameserver;
                return true;
            }

            // TODO: handle error "cant connect to GS"
            return false;
        }

        /// <summary>
        /// Operation to join a random, available room. 
        /// This operation fails if all rooms are closed or full.
        /// If successful, the result contains a gameserver address and the name of some room.
        /// </summary>
        /// <remarks>This override sets the state of the client.</remarks>
        /// <param name="expectedCustomRoomProperties">Optional. A room will only be joined, if it matches these custom properties (with string keys).</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <returns>If the operation could be sent currently (requires connection).</returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
        {
            return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom);
        }

        /// <summary>
        /// Operation to join a random, available room. 
        /// This operation fails if all rooms are closed or full.
        /// If successful, the result contains a gameserver address and the name of some room.
        /// </summary>
        /// <remarks>This override sets the state of the client.</remarks>
        /// <param name="expectedCustomRoomProperties">Optional. A room will only be joined, if it matches these custom properties (with string keys).</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="matchmakingMode">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
        /// <returns>If the operation could be sent currently (requires connection).</returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinRandomRoom;
            this.CurrentRoom = CreateRoom(null);
            
            Hashtable playerPropsToSend = null;
            if (this.server == ServerConnection.GameServer)
            {
                playerPropsToSend = this.LocalPlayer.AllProperties;
            }

            return this.loadBalancingPeer.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerPropsToSend, matchmakingMode);
        }

        /// <summary>
        /// Joins a room by name and sets this player's properties.
        /// </summary>
        /// <remarks>This override sets the state of the client.</remarks>
        /// <param name="roomName">The name of the room to join. Must be existing already, open and non-full or can't be joined.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpJoinRoom(string roomName)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinRoom;
            this.CurrentRoom = CreateRoom(roomName);

            Hashtable playerPropsToSend = null;
            if (this.server == ServerConnection.GameServer)
            {
                playerPropsToSend = this.LocalPlayer.AllProperties;
            }

            return this.loadBalancingPeer.OpJoinRoom(roomName, playerPropsToSend);
        }

        /// <summary>
        /// Creates a new room on the server (or fails when the name is already taken).
        /// </summary>
        /// <remarks>
        /// This override sets the state of the client.
        /// 
        /// The response depends on the server the peer is connected to: 
        /// Master will return a Game Server to connect to.
        /// Game Server will return the Room's data.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </remarks>
        /// <param name="roomName">The name to create a room with. Must be unique and not in use or can't be created. If null, the server will assign a GUID as name.</param>
        /// <param name="isVisible">Shows the room in the lobby's room list.</param>
        /// <param name="isOpen">Keeps players from joining the room (or opens it to everyone).</param>
        /// <param name="maxPlayers">Max players before room is considered full (but still listed).</param>
        /// <param name="customGameProperties">Custom properties to apply to the room on creation (use string-typed keys but short ones).</param>
        /// <param name="propsListedInLobby">Defines the custom room properties that get listed in the lobby. Null defaults to "none", a string[0].</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpCreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers, Hashtable customGameProperties, string[] propsListedInLobby)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.CreateRoom;
            this.CurrentRoom = this.CreateRoom(roomName, isVisible, isOpen, maxPlayers, customGameProperties, propsListedInLobby);

            Hashtable playerPropsToSend = null;
            if (this.server == ServerConnection.GameServer)
            {
                playerPropsToSend = this.LocalPlayer.AllProperties;
            }

            return this.loadBalancingPeer.OpCreateRoom(roomName, isVisible, isOpen, maxPlayers, customGameProperties, propsListedInLobby, playerPropsToSend);
        }

        /// <summary>
        /// Leaves the CurrentRoom and returns to the Master server (back to the lobby).
        /// OpLeaveRoom skips execution when the room is null or the server is not GameServer or the client is disconnecting from GS already.
        /// OpLeaveRoom returns false in those cases and won't change the state, so check return of this method.
        /// </summary>
        /// <remarks>
        /// This method actually is not an operation per se. It sets a state and calls Disconnect(). 
        /// This is is quicker than calling OpLeave and then disconnect (which also triggers a leave).
        /// </remarks>
        /// <returns>If the current room could be left (impossible while not in a room).</returns>
        public bool OpLeaveRoom()
        {
            if (this.CurrentRoom == null || this.server != ServerConnection.GameServer || this.State == ClientState.DisconnectingFromGameserver)
            {
                return false;
            }

            this.State = ClientState.DisconnectingFromGameserver;
            this.loadBalancingPeer.Disconnect();

            return true;
        }

        /// <summary>
        /// Sets custom properties of a player / actor (only passing on the string-typed custom properties).
        /// Use this only when in state Joined.
        /// </summary>
        /// <param name="actorNr">ID of player to update/set properties for.</param>
        /// <param name="actorProperties">The properties to set for target actor.</param>
        /// <returns>If sending the properties to the server worked (not if the operation was executed successfully).</returns>
        public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
        {
            Hashtable customActorProperties = new Hashtable();
            customActorProperties.MergeStringKeys(actorProperties);

            return this.OpSetPropertiesOfActor(actorNr, customActorProperties);
        }

        /// <summary>
        /// This updates the local cache of a player's properties before sending them to the server.
        /// Use this only when in state Joined.
        /// </summary>
        /// <param name="actorNr">ID of player to update/set properties for.</param>
        /// <param name="actorProperties">The properties to set for target actor.</param>
        /// <returns>If sending the properties to the server worked (not if the operation was executed successfully).</returns>
        public bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties)
        {
            Player target = this.CurrentRoom.GetPlayer(actorNr);
            if (target != null)
            {
                target.CacheProperties(actorProperties);
            }

            return this.loadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties);
        }

        /// <summary>
        /// Sets only custom game properties (which exclusively use strings as key-type in hash).
        /// </summary>
        /// <param name="gameProperties">The roomProperties to udpate or set.</param>
        /// <returns></returns>
        public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties)
        {
            Hashtable customGameProps = new Hashtable();
            customGameProps.MergeStringKeys(gameProperties);

            return this.OpSetPropertiesOfRoom(customGameProps);
        }

        /// <summary>
        /// This updates the current room's properties before sending them to the server.
        /// Use this only while in state Joined.
        /// </summary>
        /// <param name="gameProperties">The roomProperties to udpate or set.</param>
        /// <returns>If sending the properties to the server worked (not if the operation was executed successfully).</returns>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties)
        {
            this.CurrentRoom.CacheProperties(gameProperties);
            return this.loadBalancingPeer.OpSetPropertiesOfRoom(gameProperties);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Internally used only.
        /// Reads out properties coming from the server in events and operation responses (which might be a bit tricky).
        /// </summary>
        private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
        {
            // Debug.LogWarning("ReadoutProperties game=" + gameProperties + " actors(" + actorProperties + ")=" + actorProperties + " " + targetActorNr);
            // read game properties and cache them locally
            if (this.CurrentRoom != null && gameProperties != null)
            {
                this.CurrentRoom.CacheProperties(gameProperties);
            }

            if (actorProperties != null && actorProperties.Count > 0)
            {
                if (targetActorNr > 0)
                {
                    // we have a single entry in the actorProperties with one user's name
                    // targets MUST exist before you set properties
                    Player target = this.CurrentRoom.GetPlayer(targetActorNr);
                    if (target != null)
                    {
                        target.CacheProperties(this.ReadoutPropertiesForActorNr(actorProperties, targetActorNr));
                    }
                }
                else
                {
                    // in this case, we've got a key-value pair per actor (each
                    // value is a hashtable with the actor's properties then)
                    int actorNr;
                    Hashtable props;
                    string newName;
                    Player target;

                    foreach (object key in actorProperties.Keys)
                    {
                        actorNr = (int)key;
                        props = (Hashtable)actorProperties[key];
                        newName = (string)props[ActorProperties.PlayerName];
                        
                        target = this.CurrentRoom.GetPlayer(actorNr);
                        if (target == null)
                        {
                            target = this.CreatePlayer(newName, actorNr, false, props);
                            this.CurrentRoom.StorePlayer(target);
                        }
                        else
                        {
                            target.CacheProperties(props);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Internally used only to read properties for a distinct actor (which might be the hashtable OR a key-pair value IN the actorProperties).
        /// </summary>
        private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
        {
            if (actorProperties.ContainsKey(actorNr))
            {
                return (Hashtable)actorProperties[actorNr];
            }

            return actorProperties;
        }

        /// <summary>
        /// Internally used to set the LocalPlayer's ID (from -1 to the actual in-room ID).
        /// </summary>
        /// <param name="newID">New actor ID (a.k.a actorNr) assigned when joining a room.</param>
        protected internal void ChangeLocalID(int newID)
        {
            if (this.LocalPlayer == null)
            {
                this.DebugReturn(DebugLevel.WARNING, string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.LocalPlayer, this.CurrentRoom.Players == null, newID));
            }

            if (this.CurrentRoom == null)
            {
                // change to new actor/player ID and make sure the player does not have a room reference left
                this.LocalPlayer.ChangeLocalID(newID);
                this.LocalPlayer.RoomReference = null;
            }
            else
            {
                // remove old actorId from actor list
                this.CurrentRoom.RemovePlayer(this.LocalPlayer);

                // change to new actor/player ID
                this.LocalPlayer.ChangeLocalID(newID);

                // update the room's list with the new reference
                this.CurrentRoom.StorePlayer(this.LocalPlayer);

                // make this client known to the local player (used to get state and to sync values from within Player)
                this.LocalPlayer.LoadBalancingClient = this;
            }
        }

        /// <summary>
        /// Internally used to clean up local instances of players and room.
        /// </summary>
        private void CleanCachedValues()
        {
            this.ChangeLocalID(-1);

            // if this is called on the gameserver, we clean the room we were in. on the master, we keep the room to get into it
            if (this.server == ServerConnection.GameServer)
            {
                this.CurrentRoom = null;    // players get cleaned up inside this, too, except LocalPlayer (which we keep)
            }

            // when we leave the master, we clean up the rooms list (which might be updated by the lobby when we join again)
            if (this.server == ServerConnection.MasterServer)
            {
                this.RoomInfoList.Clear();
            }
        }

        /// <summary>
        /// Called internally, when a game was joined or created on the game server. 
        /// This reads the response, finds out the local player's actorNumber (a.k.a. Player.ID) and applies properties of the room and players.
        /// </summary>
        /// <param name="operationResponse">Contains the server's response for an operation called by this peer.</param>
        private void GameEnteredOnGameServer(OperationResponse operationResponse)
        {
            if (operationResponse.ReturnCode != 0)
            {
                switch (operationResponse.OperationCode)
                {
                    case OperationCode.CreateGame:
                        this.DebugReturn(DebugLevel.ERROR, "Create failed on GameServer. Changing back to MasterServer.");
                        break;
                    case OperationCode.JoinGame:
                    case OperationCode.JoinRandomGame:
                        this.DebugReturn(DebugLevel.ERROR, "Join failed on GameServer. Changing back to MasterServer.");

                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            this.DebugReturn(DebugLevel.INFO, "Most likely the game became empty during the switch to GameServer.");
                        }

                        // TODO: add callback to join failed
                        break;
                }

                this.DisconnectToReconnect();
                return;
            }

            this.State = ClientState.Joined;
            this.CurrentRoom.LoadBalancingClient = this;
            this.CurrentRoom.IsLocalClientInside = true;

            // the local player's actor-properties are not returned in join-result. add this player to the list
            int localActorNr = (int)operationResponse[ParameterCode.ActorNr];
            this.ChangeLocalID(localActorNr);

            Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
            Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
            this.ReadoutProperties(gameProperties, actorProperties, 0);
            
            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    // TODO: add callback "game created"
                    break;
                case OperationCode.JoinGame:
                case OperationCode.JoinRandomGame:
                    // TODO: add callback "game joined"
                    break;
            }
        }

        /// <summary>
        /// Factory method to create a player instance - override to get your own player-type with custom features.
        /// </summary>
        /// <param name="actorName">The name of the player to be created. </param>
        /// <param name="actorNumber">The player ID (a.k.a. actorNumber) of the player to be created.</param>
        /// <param name="isLocal">Sets the distinction if the player to be created is your player or if its assigned to someone else.</param>
        /// <param name="actorProperties">The custom properties for this new player</param>
        /// <returns>The newly created player</returns>
        protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
        {
            Player newPlayer = new Player(actorName, actorNumber, isLocal, actorProperties);
            return newPlayer;
        }

        protected internal virtual Room CreateRoom(string roomName)
        {
            return this.CreateRoom(roomName, true, true, 0, null, null);
        }

        protected internal virtual Room CreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers, Hashtable customGameProperties, string[] propsListedInLobby)
        {
            return new Room(roomName, customGameProperties, isVisible, isOpen, maxPlayers, propsListedInLobby);
        }

        #endregion

        #region IPhotonPeerListener

        /// <summary>
        /// Debug output of low level api (and this client).
        /// </summary>
        /// <remarks>This method is not responsible to keep up the state of a LoadBalancingClient. Calling base.DebugReturn on overrides is optional.</remarks>
        public virtual void DebugReturn(DebugLevel level, string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Uses the operationResponse's provided by the server to advance the internal state and call ops as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnOperationResponse.</remarks>
        /// <param name="operationResponse">Contains the server's response for an operation called by this peer.</param>
        public virtual void OnOperationResponse(OperationResponse operationResponse)
        {
            // if (operationResponse.ReturnCode != 0) this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());

            switch (operationResponse.OperationCode)
            {
                case OperationCode.Authenticate:
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            switch (operationResponse.ReturnCode)
                            {
                                case ErrorCode.InvalidAuthentication:
                                    this.DisconnectedCause = DisconnectCause.InvalidAuthentication;
                                    break;
                                case ErrorCode.InvalidRegion:
                                    this.DisconnectedCause = DisconnectCause.InvalidRegion;
                                    break;
                                case ErrorCode.MaxCcuReached:
                                    this.DisconnectedCause = DisconnectCause.MaxCcuReached;
                                    break;
                                case ErrorCode.OperationNotAllowedInCurrentState:
                                    this.DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
                                    break;
                            }
                            this.State = ClientState.Disconnecting;
                            this.Disconnect();
                            break;  // if auth didn't succeed, we disconnect (above) and exit this operation's handling
                        }

                        if (this.State == ClientState.ConnectedToMaster)
                        {
                            this.State = ClientState.Authenticated;
                            if (this.AutoJoinLobby)
                            {
                                this.loadBalancingPeer.OpJoinLobby();
                            }
                        }
                        else if (this.State == ClientState.ConnectedToGameserver)
                        {
                            this.State = ClientState.Joining;
                            if (this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom)
                            {
                                // if we just "join" the game, do so
                                this.OpJoinRoom(this.CurrentRoom.Name);
                            }
                            else if (this.lastJoinType == JoinType.CreateRoom)
                            {
                                this.OpCreateRoom(
                                    this.CurrentRoom.Name,
                                    this.CurrentRoom.IsVisible,
                                    this.CurrentRoom.IsOpen,
                                    this.CurrentRoom.MaxPlayers,
                                    this.CurrentRoom.CustomProperties,
                                    this.CurrentRoom.PropsListedInLobby);
                            }
                            break;
                        }
                        break;
                    }

                case OperationCode.Leave:
                    this.CleanCachedValues();
                    break;

                case OperationCode.JoinLobby:
                    this.State = ClientState.JoinedLobby;
                    break;

                case OperationCode.JoinRandomGame:  // this happens only on the master server. on gameserver this is a "regular" join
                case OperationCode.CreateGame:
                case OperationCode.JoinGame:
                    {
                        if (this.server == ServerConnection.GameServer)
                        {
                            this.GameEnteredOnGameServer(operationResponse);
                        }
                        else
                        {
                            if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                            {
                                // this happens only for JoinRandomRoom
                                // TODO: implement callback/reaction when no random game could be found (this is no bug and can simply happen if no games are open)
                                this.state = ClientState.JoinedLobby; // TODO: maybe we have to return to another state here (if we didn't join a lobby)
                                break;
                            }

                            // TODO: handle more error cases

                            if (operationResponse.ReturnCode != 0)
                            {
                                if (this.loadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                                {
                                    this.DebugReturn(DebugLevel.ERROR, string.Format("Getting into game failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
                                }

                                this.state = ClientState.JoinedLobby; // TODO: maybe we have to return to another state here (if we didn't join a lobby)
                                break;
                            }

                            this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                            string gameId = operationResponse[ParameterCode.RoomName] as string;
                            if (!string.IsNullOrEmpty(gameId))
                            {
                                // is only sent by the server's response, if it has not been sent with the client's request before!
                                this.CurrentRoom.Name = gameId;
                            }
                            
                            this.DisconnectToReconnect();
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Uses the connection's statusCodes to advance the internal state and call operations as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnStatusChanged.</remarks>
        public virtual void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    if (this.State == ClientState.ConnectingToGameserver)
                    {
                        if (this.loadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            this.DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
                        }

                        this.State = ClientState.ConnectedToGameserver;
                        this.server = ServerConnection.GameServer;
                    }

                    if (this.State == ClientState.ConnectingToMasterserver)
                    {
                        if (this.loadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            this.DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
                        }

                        this.State = ClientState.ConnectedToMaster;
                        this.server = ServerConnection.MasterServer;
                    }

                    this.loadBalancingPeer.EstablishEncryption();
                    break;

                case StatusCode.Disconnect:
                    // disconnect due to connection exception is handled below (don't connect to GS or master in that case)

                    this.CleanCachedValues();
                    this.didAuthenticate = false;   // on connect, we know that we didn't 

                    if (this.State == ClientState.Disconnecting)
                    {
                        this.State = ClientState.Disconnected;
                    }
                    else if (this.State == ClientState.Uninitialized)
                    {
                        this.State = ClientState.Disconnected;
                    }
                    else if (this.State != ClientState.Disconnected)
                    {
                        if (this.server == ServerConnection.GameServer)
                        {
                            this.ConnectToMaster(this.AppId, this.AppVersion, this.PlayerName);
                        }
                        else if (this.server == ServerConnection.MasterServer)
                        {
                            this.ConnectToGameServer();
                        }
                    }
                    break;

                case StatusCode.EncryptionEstablished:
                    // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                    if (!this.didAuthenticate)
                    {
                        this.didAuthenticate = this.loadBalancingPeer.OpAuthenticate(this.AppId, this.AppVersion);
                        if (!this.didAuthenticate)
                        {
                            this.DebugReturn(DebugLevel.ERROR, "Error Authenticating! Did not work. Check log output and OpAuthenticate return code.");
                        }
                    }
                    break;

                case StatusCode.DisconnectByServerUserLimit:
                    this.DebugReturn(DebugLevel.ERROR, "The Photon license's CCU Limit was reached. Server rejected this connection. Wait and re-try.");
                    this.DisconnectedCause = DisconnectCause.DisconnectByServerUserLimit;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.ExceptionOnConnect:
                    this.DisconnectedCause = DisconnectCause.ExceptionOnConnect;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.DisconnectByServer:
                    this.DisconnectedCause = DisconnectCause.DisconnectByServer;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.TimeoutDisconnect:
                    this.DisconnectedCause = DisconnectCause.TimeoutDisconnect;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.Exception:
                    this.DisconnectedCause = DisconnectCause.Exception;
                    this.State = ClientState.Disconnected;
                    break;
            }
        }

        /// <summary>
        /// Uses the photonEvent's provided by the server to advance the internal state and call ops as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnEvent.</remarks>
        public virtual void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case EventCode.GameList:
                case EventCode.GameListUpdate:
                    if (photonEvent.Code == EventCode.GameList)
                    {
                        this.RoomInfoList = new Dictionary<string, RoomInfo>();
                    }

                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (string gameName in games.Keys)
                    {
                        RoomInfo game = new RoomInfo(gameName, (Hashtable)games[gameName]);
                        if (game.removedFromList)
                        {
                            this.RoomInfoList.Remove(gameName);
                        }
                        else
                        {
                            this.RoomInfoList[gameName] = game;
                        }
                    }
                    break;

                case EventCode.Join:
                    int actorNr = (int)photonEvent[ParameterCode.ActorNr];  // actorNr (a.k.a. playerNumber / ID) of sending player
                    bool isLocal = this.LocalPlayer.ID == actorNr;

                    Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];

                    if (!isLocal)
                    {
                        Player newPlayer = this.CreatePlayer(string.Empty, actorNr, false, actorProperties);
                        this.CurrentRoom.StorePlayer(newPlayer);
                    }
                    else
                    {
                        // in this player's own join event, we get a complete list of players in the room, so check if we know each of the
                        int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                        foreach (int actorNrToCheck in actorsInRoom)
                        {
                            if (this.LocalPlayer.ID != actorNrToCheck && !this.CurrentRoom.Players.ContainsKey(actorNrToCheck))
                            {
                                this.CurrentRoom.StorePlayer(this.CreatePlayer(string.Empty, actorNrToCheck, false, null));
                            }
                        }
                    }
                    break;

                case EventCode.Leave:
                    int actorID = (int)photonEvent[ParameterCode.ActorNr];
                    this.CurrentRoom.RemovePlayer(actorID);
                    break;

                case EventCode.PropertiesChanged:
                    // whenever properties are sent in-room, they can be broadcasted as event (which we handle here)
                    // we get PLAYERproperties if actorNr > 0 or ROOMproperties if actorNumber is not set or 0
                    int targetActorNr = 0;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.TargetActorNr))
                    {
                        targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
                    }
                    Hashtable props = (Hashtable)photonEvent[ParameterCode.Properties];

                    if (targetActorNr > 0)
                    {
                        this.ReadoutProperties(null, props, targetActorNr);
                    }
                    else 
                    {
                        this.ReadoutProperties(props, null, 0);
                    }
                    
                    break;

                case EventCode.AppStats:
                    // only the master server sends these in (1 minute) intervals
                    this.PlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                    this.RoomsCount = (int)photonEvent[ParameterCode.GameCount];
                    this.PlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                    break;
            }
        }

        #endregion
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//   The "Particle" demo is a load balanced and Photon Cloud compatible "coding" demo.
//   The focus is on showing how to use the Photon features without too much "game" code cluttering the view.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.DemoParticle
{
    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;
    using ExitGames.Client.Photon.Lite;
    using System;
    using System.Text;
    using System.Collections;
	
	/// <summary>Delegate to get notified of joining/leaving players (see OnEventJoin and OnEventLeave).</summary>
	public delegate void EventPlayerListChangeDelegate(ParticlePlayer particlePlayer);
	
    /// <summary>
    /// Central class of the Photon Particle Demo which makes simple use of several Photon features to show how to use it.
    /// </summary>
    /// <returns>
    /// Instead of implementing a complex game, this simple demo shows how to use Photon features "in principle". 
    /// To implement a game, you could rewrite this demo and insert whatever other info you need to pass between clients.
    /// 
    /// The classes of this project make use of the Photon LoadBalancing workflow which distributes games (Rooms) over 
    /// several Game Servers. The LoadBalancingClient (extended by this class) implements the basic state and workflow 
    /// with "cloud" servers.
    /// 
    /// The demo is compatible with the Photon Cloud and its "self hosting" alternative "LoadBalancing" (in the Server SDK).
    /// 
    /// 
    /// GameLogic does not have it's own GUI or game loop. Instead, it must be integrated into a game loop, depending on the 
    /// platform. Unity3d (e.g.) calls a Update method per frame, while Windows Forms could use a Thread to run this.
    /// 
    /// The method GameLoop should be called as often as possible and in turn triggers all updates and uses the networking 
    /// classes to establish and keep a connection.
    /// Start will connect to the server and prints out some feedback if that fails.
    /// 
    /// In networked games, there is info that has to be sent to other clients very often, on event or only once per game.
    /// Photon uses Events to do so. Additionally, events can carry "short-lived" or "permanent" info. 
    /// This demo shows how achieve either in several different ways with Photon.
    /// 
    /// Read through the code comments and feel free to experiment with this code or use it as basis for your games.
    /// </returns>
    public class GameLogic : LoadBalancingClient
    {
		/// <summary>Can be used to be notified when a player joins the room (Photon: EvJoin).</summary>
		/// <remarks>
		/// Keep in mind: When joining an existing room, this client does not get EvJoin for 
		/// those players already in the room! To initiate each player, go through the list of 
		/// players in the room on join.
		/// The event join for this client is called, however.
		/// </remarks>
        public EventPlayerListChangeDelegate OnEventJoin;
		
		/// <summary>Can be used to be notified when a player leaves the room (Photon: EvLeave).</summary>
		public EventPlayerListChangeDelegate OnEventLeave;
		
        /// <summary>Provides the LocalPlayer cast to ParticlePlayer.</summary>
        public new ParticlePlayer LocalPlayer { get { return (ParticlePlayer)base.LocalPlayer; } }
        
        /// <summary>Provides the CurrentRoom cast to ParticleRoom.</summary>
        /// <remarks>This could also be names CurrentRoom (with new keywork) but this way it better matches LocalPlayer.</remarks>
        public ParticleRoom LocalRoom { get { return (ParticleRoom)base.CurrentRoom; } }
		
		/// <summary>Fetches the custom room-property "grid size" or returns ParticleRoom.GridSizeDefault if that is not available.</summary>
		/// <remarks>Set via LocalRoom.GridSize.</remarks>
		public int GridSize { get { return (this.LocalRoom == null) ? ParticleRoom.GridSizeDefault : this.LocalRoom.GridSize; } }

        /// <summary>When true, update the screen (display the new info) and set to false when done.</summary>
        /// <remarks>Set to true when some state changed or an event or result was dispatched.</remarks>
        public bool UpdateVisuals;

        /// <summary>Send EvColor and EvMove as reliable (or not).</summary>
        /// <remarks>You can set per operation/event if it's reliable or not but this demo simplifies this.</remarks>
        public bool SendReliable;

        /// <summary>Suppress this demo's automatic joining of a random game (to get into the lobby and show a rooms list, e.g.).</summary>
        public bool JoinRandomGame;
        
        /// <summary>The particle demo runs with or without Interest Groups and this is the toggle.</summary>
        /// <remarks>
        /// Clients (GameLogic instances) can turn this on or off independently. Depending on the combination, one game logic
        /// might get no updates at all (if others use the groups) or all info despite using the groups (others might send to all).
        /// </remarks>
        public bool UseInterestGroups { get; private set; }
        
        /// <summary>This is the list of custom room properties that we want listed in the lobby.</summary>
        protected static readonly string[] RoomPropsInLobby = new string[] { DemoConstants.MapProp, DemoConstants.GridSizeProp };

        
        /// <summary>Tracks the interval in which the local player should move (unless disabled).</summary>
        public TimeKeeper MoveInterval { get; set; }

        /// <summary>Tracks the interval in which the current position should be broadcasted.</summary>
        /// <remarks>This actually defines how many updates per second this player creates by position updates.</remarks>
        public TimeKeeper UpdateOthersInterval { get; set; }

        /// <summary>Tracks the interval in which PhotonPeer.DispatchIncomingCommands should be called.</summary>
        /// <remarks>Instead of dispatching incoming info every frame, this demo will do find with a slightly lower rate.</remarks>
        public TimeKeeper DispatchInterval { get; set; }

        /// <summary>Tracks the interval in which PhotonPeer.SendOutgoingCommands should be called.</summary>
        /// <remarks>You can send in fixed intervals and additionally send when some update was created (to speed up delivery).</remarks>
        public TimeKeeper SendInterval { get; set; }


        /// <summary>Internally used property to get some timestamp.</summary>
        /// <remarks>Could be exchanged, if some platform doesn't provide Environment.TickCount or if more precision is needed</remarks>
        public static int Timestamp { get { return Environment.TickCount; } }

        /// <summary>Logging coming from the Photon Library or this demo for debugging.</summary>
        public StringBuilder Log = new StringBuilder();

        public GameLogic(string masterAddress, string appId, string gameVersion) : base(masterAddress, appId, gameVersion)
        {
            this.LocalPlayer.Name = "usr" + SupportClass.ThreadSafeRandom.Next() % 99;

            this.AutoJoinLobby = false;
            this.UseInterestGroups = true;
            this.JoinRandomGame = true;

            this.DispatchInterval = new TimeKeeper(10);
            this.SendInterval = new TimeKeeper(100);
            this.MoveInterval = new TimeKeeper(500);
            this.UpdateOthersInterval = new TimeKeeper(this.MoveInterval.Interval);
        }

        /// <summary>
        /// Override of the factory method used by the base LoadBalancing framework (which we extend here) to create Players.
        /// </summary>
        /// <remarks>
        /// When clients join a room they become "Players" in that room. This is done by the LoadBalancing API, 
        /// so this demo only needs to override the creation method.
        /// 
        /// This method is used by a LoadBalancingClient automatically whenever a new player joins a room.
        /// We override it to produce a ParticlePlayer which has more features than just name and custom properties.
        /// </remarks>
        protected internal override Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
        {
            return new ParticlePlayer(actorName, actorNumber, isLocal, actorProperties);
        }

        /// <summary>
        /// Override of the factory method used by the LoadBalancing framework (which we extend here) to create a Room instance.
        /// </summary>
        /// <remarks>
        /// While CreateParticleDemoRoom will make the server create the room, this method creates a local object to represent that room.
        /// 
        /// This method is used by a LoadBalancingClient automatically whenever this client joins or creates a room.
        /// We override it to produce a ParticleRoom which has more features like Map and GridSize.
        /// </remarks>
        protected internal override Room CreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers, Hashtable customGameProperties, string[] propsListedInLobby)
        {
            return new ParticleRoom(roomName, isVisible, isOpen, maxPlayers, customGameProperties, propsListedInLobby);
        }

        /// <summary>Start connects to the server or prints out some feedback if that fails.</summary>
        /// <remarks>
        /// Connection might fail even before anything network-specific happened, so check the result.
        /// Example: If the address can't be resolved or no network is available or similar.
        /// </remarks>
        public void Start()
        {
            if (!this.Connect())
            {
                this.DebugReturn(DebugLevel.ERROR, "Can't connect. Either server is unavailable or policy doesn't allow it. Server: " + this.MasterServerAddress);
            }
        }

        /// <summary>This game loop should be called as often as possible - it will do it's work in intervals only.</summary>
        public void UpdateLoop()
        {
            // Dispatch means received messages are executed - one by one when you call dispatch.
            // You could also dispatch each frame!
            if (this.DispatchInterval.ShouldExecute)
            {
                while (this.loadBalancingPeer.DispatchIncomingCommands())
                {
                    // You could count dispatch calls to limit them to X, if they take too much time of a single frame
                }
                this.DispatchInterval.Reset();  // we dispatched, so reset the timer
            }

            // If the client is in a room, we might move our LocalPlayer and update others of our position
            if (this.State == ClientState.Joined)
            {
                if (this.MoveInterval.ShouldExecute)
                {
                    this.LocalPlayer.MoveRandom();
                    this.MoveInterval.Reset();

                    this.UpdateOthersInterval.ShouldExecute = true; // we just moved. this should produce a update (in this demo)
                    UpdateVisuals = true; // update visuals to show new pos
                }

                // This demo sends updates in intervals and when the player was moved
                // In a game you could send ~10 times per second or only when the user did some input, too
                if (this.UpdateOthersInterval.ShouldExecute)
                {
                    this.SendPositionUpdate();
                    this.UpdateInterestGroups();

                    this.UpdateOthersInterval.Reset();
                }
            }

            // With the Photon API you can fine-control sending data, which allows the library to aggregate several messages into one package
            // Keep in mind that reliable messages from the server will need a reply (ack), so send more often than needed.
            // If nothing is waiting to be sent, SendOutgoingCommands won't do anything.
            if (this.SendInterval.ShouldExecute)
            {
                this.loadBalancingPeer.SendOutgoingCommands();
                this.SendInterval.Reset();
            }
        }

        /// <summary>Turns on/off usage of Interest Groups and takes care of the settings necessary on the server.</summary>
        /// <remarks>OpChangeGroups sets this client's interests on the server (read that method's description).</remarks>
        /// <param name="useGroups">On or off for User Group usage.</param>
        public void SetUseInterestGroups(bool useGroups)
        {
            this.UseInterestGroups = useGroups;
            if (!useGroups)
            {
                this.loadBalancingPeer.OpChangeGroups(new byte[0], null);   // remove all group-"subscriptions"
                this.LocalPlayer.VisibleGroup = 0;  // group 0 is never used for actual grouping, so we can "flag" this as as unused
            }
            else
            {
                this.UpdateInterestGroups();    // this method does what we need to subscribe to certain group(s)
            }
        }

        /// <summary>Takes care of Interest Group "subscriptions" for the local player.</summary>
        /// <remarks>In this demo, groups are based on position only but you can make up any rule to divide players into groups.</remarks>
        private void UpdateInterestGroups()
        {
            if (this.UseInterestGroups)
            {
                byte currentGroup = this.LocalPlayer.GetGroup();
                if (currentGroup != this.LocalPlayer.VisibleGroup)
                {
                    this.loadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { currentGroup });    // config the server to only send this group
                    this.LocalPlayer.VisibleGroup = currentGroup;   // store which group we now are interested in (server side)
                }
            }
        }

        /// <summary>Makes use of the peer (connection to server) to send an Event containing our (local) positon.</summary>
        /// <remarks>
        /// In Photon, by default, events go to everyone in the same Room. Outside of Rooms, you can't send events, usually.
        /// There is an option to use Interest Groups to send to just those players interested in a certain group. 
        /// This can be used to reduce the number of events each player gets or to hide information from users not in the same group.
        /// </remarks>
        private void SendPositionUpdate()
        {
            if (this.UseInterestGroups)
            {
                // if groups are enabled for this player, we send to the group specific to our position only. note the group parameter
                byte playerGroup = this.LocalPlayer.GetGroup();
                this.loadBalancingPeer.OpRaiseEvent(DemoConstants.EvPosition, playerGroup, this.LocalPlayer.WriteEvMove(), this.SendReliable);
            }
            else
            {
                // this overload of OpRaiseEvent sends to everyone in the room - even those who didn't subscribe to any group
                this.loadBalancingPeer.OpRaiseEvent(DemoConstants.EvPosition, this.LocalPlayer.WriteEvMove(), this.SendReliable, 0);
            }
        }

        /// <summary>
        /// Implementation of a callback that's used by the Photon library to update the application / game of incoming events.
        /// </summary>
        /// <remarks>
        /// When you override this method, it's very important to call base.OnEvent to keep the state.
        /// 
        /// Photon uses events to add or remove players from this client's lists. When we call base.OnEvent() 
        /// and it adds a player, we want to fetch this player afterwards, if this removes a player, this 
        /// player will be gone after base.OnEvent(). 
        /// To get the added/removed player in any case, we might have to fetch it before or after running base code.
        /// </remarks>
        /// <param name="photonEvent">The event someone (or the server) sent.</param>
        public override void OnEvent(Photon.EventData photonEvent)
        {
            // most events have a sender / origin (but not all) - let's find the player sending this
            int actorNr = 0;
            Player origin = null;
            if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
            {
                actorNr = (int)photonEvent[ParameterCode.ActorNr];  // actorNr (a.k.a. playerNumber / ID) of sending player
            }
			
			if (actorNr > 0)
			{
				this.LocalRoom.Players.TryGetValue(actorNr, out origin);
			}

            base.OnEvent(photonEvent);  // important to call, to keep state up to date
			
			if (actorNr > 0 && origin == null)
			{
				this.LocalRoom.Players.TryGetValue(actorNr, out origin);
			}
			
			// the list of players will only store Player references (not the derived class). simply cast:
			ParticlePlayer originatingPlayer = (ParticlePlayer)origin;

            // this demo logic doesn't handle any events from the server (that is done in the base class) so we could return here
            if (originatingPlayer == null)
            {
                this.DebugReturn(DebugLevel.WARNING, photonEvent.Code + " ev. We didn't find a originating player for actorId: " + actorNr);
                return;
            }

            // this demo defined 2 events: Position and Color. additionally, a event is triggered when players join or leave
            switch (photonEvent.Code)
            {
                case DemoConstants.EvPosition:
                    originatingPlayer.ReadEvMove((Hashtable)photonEvent[ParameterCode.CustomEventContent]);
                    break;
                case DemoConstants.EvColor:
                    originatingPlayer.ReadEvColor((Hashtable)photonEvent[ParameterCode.CustomEventContent]);
                    break;
				
				// in this demo, we want a callback when players join or leave (so we can update their representation)
                case LiteEventCode.Join:
                    if (OnEventJoin != null) 
					{
						OnEventJoin(originatingPlayer);
					}
                    break;
                case LiteEventCode.Leave:
                    if (OnEventLeave != null) 
					{
						OnEventLeave(originatingPlayer);
					}
                    break;
            }

            UpdateVisuals = true;
        }

        /// <summary>
        /// Implementation of a callback that's used by the Photon library to update the application / game of operation responses by server.
        /// </summary>
        /// <remarks>When you override this method, it's very important to call base.OnEvent to keep the state.</remarks>
        /// <param name="operationResponse">The response to some operation we called on the server.</param>
        public override void OnOperationResponse(Photon.OperationResponse operationResponse)
        {
            base.OnOperationResponse(operationResponse);  // important to call, to keep state up to date

            if (operationResponse.ReturnCode != ErrorCode.Ok)
            {
                //this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull() + " " + this.State);
            }

            // this demo connects when you call start and then it automatically executes a certain operation workflow to get you in a room
            switch (operationResponse.OperationCode)
            {
                case OperationCode.Authenticate:
                    // authentication concludes connecting to the master server (it sends the appId and identifies your game)
                    // when that's done, this demo asks the Master for any game. the result is handled below
                    if (this.JoinRandomGame && this.State == ClientState.Authenticated)
                    {
                        this.OpJoinRandomRoom(null, 0);
                    }
                    break;

                case OperationCode.JoinRandomGame:
                    // OpJoinRandomRoom is called above. the response to that is handled here
                    // if the Master Server didn't find a room, simply create one. the result is handled below
                    if (this.JoinRandomGame && operationResponse.ReturnCode != ErrorCode.Ok)
                    {
                        this.CreateParticleDemoRoom(DemoConstants.MapType.Forest, 4);
                    }
                    break;

                case OperationCode.JoinGame:
                case OperationCode.CreateGame:
                    // the master server will respond to join and create but this is handled in the base class
                    if (this.State == ClientState.Joined)
                    {
                        // no matter if we joined or created a game, when we arrived in state "Joined", we are on the game server in a room and
                        // this client could start moving and update others of it's color
                        this.LocalPlayer.RandomizePosition();
                        this.loadBalancingPeer.OpRaiseEvent(DemoConstants.EvColor, this.LocalPlayer.WriteEvColor(), true, 0, null, EventCaching.AddToRoomCache);
                    }
                    break;
            }

            UpdateVisuals = true;
        }

        /// <summary>
        /// In this demo we only update the visuals when the status changes. The base class does everything else.
        /// </summary>
        /// <remarks>When you override this method, it's very important to call base.OnStatusChanged to keep the state.</remarks>
        /// <param name="statusCode"></param>
        public override void OnStatusChanged(Photon.StatusCode statusCode)
        {
            base.OnStatusChanged(statusCode);  // important to call, to keep state up to date

            if (statusCode == StatusCode.Disconnect && this.DisconnectedCause != DisconnectCause.None)
            {
                DebugReturn(DebugLevel.ERROR, this.DisconnectedCause + " caused a disconnect. State: " + this.State + " statusCode: " + statusCode + ".");
            }
            
            UpdateVisuals = true;
        }

        /// <summary>Logging method called by the client library.</summary>
        /// <remarks>
        /// This method is not responsible to keep up the state of a LoadBalancingClient, so calling base.DebugReturn is optional.
        /// The amount of logging can be controlled by property PhotonPeer.DebugOut (try: this.loadBalancingPeer.DebugOut).
        /// </remarks>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public override void DebugReturn(Photon.DebugLevel level, string message)
        {
            Log.AppendLine(message);
            base.DebugReturn(level, message);
        }

        /// <summary>
        /// Changes the GridSize and stores the new value as room property in the server (synced with anyone in this room).
        /// </summary>
        /// <remarks>
        /// This is a sample of room properties being used.
        /// Simply put, custom room properties can be set via Room.SetCustomProperties and on creation of a room.
        /// When you join a room, you can't set them before the client is "in". Once joined, any client is allowed to change any.
        /// </remarks>
        public void ChangeGridSize()
        {
            int newGridSize = this.LocalRoom.GridSize * 2;
            if (newGridSize > 128)
            {
                newGridSize = 2;
            }
            Hashtable newGridSizeProp = new Hashtable() {{DemoConstants.GridSizeProp, newGridSize}};
            this.LocalRoom.SetCustomProperties(newGridSizeProp);
        }

        /// <summary>
        /// Sends this player's color as a buffered event (color changes stack up and new players get this on join).
        /// </summary>
        /// <remarks>
        /// This is a sample of cached events (EventCaching parameter) and how to use them.
        /// A player's color could alternatively implemented with custom player properties. 
        /// Cached events are just nice, as they behave the same for players in the room and for those joining later.
        /// </remarks>
        public void ChangeLocalPlayercolor()
        {
            if (this.LocalPlayer != null)
            {
                this.LocalPlayer.RandomizeColor();
                this.loadBalancingPeer.OpRaiseEvent(DemoConstants.EvColor, this.LocalPlayer.WriteEvColor(), true, 0, null, EventCaching.AddToRoomCache);
            }
        }

        /// <summary>
        /// Tells the server to create a new room, randomly named but with some defult settings (properties).
        /// </summary>
        /// <remarks>
        /// This method shows how to create a room without assigning a name.
        /// Unless you want to show a list of rooms, this is the best workflow for random matchmaking.
        /// Showing a list of rooms to make users pick one is not always much more than a random picking, also.
        /// 
        /// Note the maxPlayers being 0. This means "any number of players". In your game, you would set some value > 0 here.
        /// </remarks>
        /// <seealso cref="http://doc.exitgames.com/photon-cloud/MatchmakingAndLobby/#cat-references"/>
        /// <param name="maptype">Any value of DemoConstants.MapType</param>
        /// <param name="gridSize"></param>
        public void CreateParticleDemoRoom(DemoConstants.MapType maptype, int gridSize)
        {
            // custom room properties to use when this client creates a room. Note: Not all are listed in the lobby.
            Hashtable roomPropsForCreation = new Hashtable() { { DemoConstants.MapProp, maptype.ToString() }, { DemoConstants.GridSizeProp, gridSize } };
            this.OpCreateRoom(null, true, true, 0, roomPropsForCreation, RoomPropsInLobby);
        }
    }

}
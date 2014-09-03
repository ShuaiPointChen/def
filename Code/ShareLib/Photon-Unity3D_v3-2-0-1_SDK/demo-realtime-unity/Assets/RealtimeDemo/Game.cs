// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This demo should show how to get a player's position across to other players in the same room.
//   Each running Game instance will connect to Photon (with a local player / peer), go into the same room 
//   and move around.
//   Players have positions (updated regularly), name and color (updated only when someone joins).
//
//   Server side, Photon with the default Lite Application is used.
//   A LitePeer is not thread safe, so make sure Update() is only called by one thread.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.RealtimeDemo;

/// <summary>
/// This class contains the "game logic" for this simple demo. In Update, the local player's
/// position is randomly changed and in OnEvent, incoming events from other players are
/// handled. Each remote player in the room (this demo uses a fixed one) is represented as a
/// Player instance.
/// This Game class is built on top of the PhotonClient class. This makes it a MonoBehaviour. 
/// The idea here is to make Game run fully automatic, so many Game instances can simulate many
/// clients in a single process. This is not used yet, however.
/// Input is handled externally. The DemoGuiAndInput class knows a GameInstance and will "feed"
/// input to it.
/// This class is similar to the source used in the DotNet Realtime Demo but modified 
/// to fit into the Unity Engine.
/// </summary>
public class Game : PhotonClient
{
    #region Members

    // Check PhotonClient for several inherited members like ServerAddress!

    /// <summary>Reliable UDP is the preferred and more flexible protocol for Photon but TCP is supported as well.</summary>
    /// <remarks>If you use TCP, make sure to use the correct ServerAddress, too. The TCP port on Photon is usually 4530.</remarks>
    public ConnectionProtocol UsedProtocol = ConnectionProtocol.Udp;

    /// <summary>All Realtime Demo clients meet in this room by default. This is also used on other platforms.</summary>
    public string RoomName = "realtimeDemoGame000";

    /// <summary>List of Player instances in this room ("cache" for position, name and color).</summary>
    public Dictionary<int, Player> Players;

    /// <summary>The local Player instance, which can be modified (moved, renamed) directly.</summary>
    public Player LocalPlayer;

    /// <summary>Interval between DispatchIncomingCommands() calls</summary>
    public int intervalDispatch = 50;

    /// <summary>Timestamp of last time something was dispatched.</summary>
    private int lastDispatch = Environment.TickCount;

    /// <summary>Interval between SendOutgoingCommands() calls. This directly affects the number of packages sent.</summary>
    public int intervalSend = 50;

    /// <summary>Timestamp of last time anything was sent.</summary>
    private int lastSend = Environment.TickCount;

    /// <summary>Interval for auto-movement. Each movement will also call LitePeer.OpRaiseEvent() to send an event of the new position.</summary>
    public int intervalMove = 500;

    /// <summary>Timestamp of last automatic movement.</summary>
    private int lastMove = Environment.TickCount;

    /// <summary>If true, an encrypted operation is used to send movement updates to the server</summary>
    public static bool RaiseEncrypted = false;

    /// <summary>A the Peer is not public (we don't want that just anyone could mess with it), we provide internal state with a property.</summary>
    public bool PeerIsEncryptionAvailable
    {
        get { return (this.Peer == null) ? false : this.Peer.IsEncryptionAvailable; }
    }

    /// <summary>A the Peer is not public (we don't want that just anyone could mess with it), we provide RoundtripTime with a property.</summary>
    public int PeerRoundTripTime
    {
        get { return (this.Peer == null) ? -1 : this.Peer.RoundTripTime; }
    }

    #endregion

    #region Start and Gameloop

    /// <summary>
    /// Called by Unity on start. We create and connect a new LitePeer.
    /// This makes the PhotonClient relatively autonomous.
    /// </summary>
    public override void Start()
    {
        this.Peer = new LitePeer(this, this.UsedProtocol);
        this.SetPeerForGuiElements();
    }

    /// <summary>The "helper" gui scripts need to knwo the peer, so we set them. This is optional.</summary>
    public void SetPeerForGuiElements()
    {
        PhotonNetSimSettingsGui pnssg = FindObjectOfType(typeof(PhotonNetSimSettingsGui)) as PhotonNetSimSettingsGui;
        if (pnssg != null)
        {
            pnssg.Peer = this.Peer;
        }

        PhotonStatsGui psg = FindObjectOfType(typeof(PhotonStatsGui)) as PhotonStatsGui;
        if (psg != null)
        {
            psg.Peer = this.Peer;
        }
    }

    /// <summary>
    /// Update must be called by a gameloop (a single thread), so it can handle
    /// automatic movement and networking.
    /// </summary>
    /// <remarks>
    /// A simpler variant of how to integrate Photon into your game loop is done
    /// in the PhotonClient.Update, which we override and replace here with something
    /// that gives us more control.
    /// </remarks>
    public override void Update()
    {
        // Check interval for Peer.DispatchIncomingCommands() calls. 
        // These will in turn cause calls to OnEvent, OnStatusChanged, OperationResult and DebugReturn!
        if (Environment.TickCount - this.lastDispatch > this.intervalDispatch)
        {
            this.lastDispatch = Environment.TickCount;
            while (this.Peer != null && this.Peer.DispatchIncomingCommands())
            {
                // call until the received events are all dispatched
            }
        }

        // Check interval for automatic movement. This enqueues Operations for sending.
        if (Environment.TickCount - this.lastMove > this.intervalMove)
        {
            this.lastMove = Environment.TickCount;
            this.MoveRandom();
            this.SendPosition();    // will not immediately SEND the operation but queue it (wrapped in a command)
        }

        // Check interval for sending outgoing commands: creates a UDP package and sends anything that's ready.
        if (Environment.TickCount - this.lastSend > this.intervalSend)
        {
            this.lastSend = Environment.TickCount;
            this.Peer.SendOutgoingCommands();    // will send pending, outgoing commands
        }
    }

    #endregion

    #region IPhotonPeerListener Members

    /// <summary>
    /// Photon library callback for state changes (connect, disconnect, etc.)
    /// Processed within PhotonPeer.DispatchIncomingCommands()!
    /// </summary>
    /// <param name="statusCode">A code for the reason of the status change (not the status itself).</param>
    public override void OnStatusChanged(StatusCode statusCode)
    {
        // We override PhotonClient.OnStatusChanged, but want to keep it's state functionality, so call the base classes' function:
        base.OnStatusChanged(statusCode);

        // handle status for connect, disconnect and errors (non-operations)
        switch (statusCode)
        {
            case StatusCode.Connect:
                this.Peer.OpJoin(this.RoomName);
                break;

            case StatusCode.Disconnect:
                this.LocalPlayer.actorNr = 0;
                this.Players.Clear();
                break;

            case StatusCode.ExceptionOnConnect:
                this.LocalPlayer.actorNr = 0;
                this.Players.Clear();
                break;

            case StatusCode.Exception:
                this.Players.Clear();
                this.LocalPlayer.actorNr = 0;
                break;

            case StatusCode.SendError:
                this.Players.Clear();
                this.LocalPlayer.actorNr = 0;
                break;

            default:
                this.DebugReturn("StatusCode not handled: " + statusCode);
                break;
        }
    }

    /// <summary>
    /// Photon library callback to get us operation results (if our operation was executed server-side)
    /// Only called for reliable commands! Anything sent unreliable will not produce a result.
    /// Processed within PhotonPeer.DispatchIncomingCommands()!
    /// </summary>
    /// <remarks>
    /// When joining a room, this is where we learn "this" player's actorNumber. See case Join!
    /// </remarks>
    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        // We override PhotonClient.OperationResult, but want to keep it's state functionality, so call the base classes' function:
        base.OnOperationResponse(operationResponse);

        // Handling operation results can be done in a switch by operation-code.
        // For this demo, the return of OpJoin is most interesting, as this tells this game the local player's actorNumber in the room
        switch (operationResponse.OperationCode)
        {
            case (byte)LiteOpCode.Join:
                // this.DebugReturn("Join result: " + SupportClass.HashtableToString(returnValues));

                // This is the result for any OpJoin this client/game did. It tells us the actorNr this
                // client was assigned in the room. As the actorNumber references any player in a room,
                // we are going to memorize it.
                int actorNrReturnedForOpJoin = (int)operationResponse[(byte)LiteOpKey.ActorNr];
                this.LocalPlayer.actorNr = actorNrReturnedForOpJoin;

                // The Players dict is organized by actorNr as well. The local player (handled by this Game)
                // is also a player, so add it to the list.
                this.Players[this.LocalPlayer.actorNr] = this.LocalPlayer;

                this.DebugReturn("LocalPlayer: " + this.LocalPlayer);
                break;
        }
    }

    /// <summary>
    /// Called by Photon lib for each incoming event (player- and position-data in this demo, as well as joins and leaves).
    /// Processed within PhotonPeer.DispatchIncomingCommands()!
    /// </summary>
    public override void OnEvent(EventData photonEvent)
    {
        // We override PhotonClient.OnEvent. The base classe's implementation prints out the event. Let's keep that by calling:
        base.OnEvent(photonEvent);

        // Custom events (defined and sent by a client) encapsulate the sent data in a separate Hashtable. This avoids duplicate usage of keys.
        // The event content a client sends is under key Data. This demo mostly handles custom data, so let's "grab" our content for later use
        Hashtable evData = null;
        if (photonEvent.Parameters.ContainsKey(LiteEventKey.CustomContent))
        {
            evData = photonEvent[(byte)LiteEventKey.CustomContent] as Hashtable;
        }

        // Most events are sent by (other) users. Their origin can be read from the ActorNr key
        int originatingActorNr = 0;
        if (photonEvent.Parameters.ContainsKey((byte)LiteEventKey.ActorNr))
        {
            originatingActorNr = (int)photonEvent[(byte)LiteEventKey.ActorNr];
        }

        // get the player object associated with the actorNumber (a remote player) that raised this event
        Player originatingPlayer;
        this.Players.TryGetValue(originatingActorNr, out originatingPlayer);

        switch (photonEvent.Code)
        {
            case (byte)LiteEventCode.Join:
                // Event is defined by Lite. A peer entered the room. It could be this peer!
                // This event provides the current list of actors and a actorNumber of the player who is new.

                // get the list of current players and check it against local list - create any that's not yet there
                int[] actorsInGame = (int[])photonEvent[(byte)LiteEventKey.ActorList];
                foreach (int i in actorsInGame)
                {
                    if (!this.Players.ContainsKey(i))
                    {
                        this.Players[i] = new Player(i);
                    }
                }
                this.PrintPlayers();

                this.LocalPlayer.SendPlayerInfo(this.Peer); // the new peers does not have our info, so send it again
                break;

            case (byte)LiteEventCode.Leave:
                // Event is defined by Lite. Someone left the room.
                this.Players.Remove(originatingActorNr);
                break;

            case (byte)Player.DemoEventCode.PlayerInfo:
                // this is a custom event, which is defined by this application.
                // if player is known (and it should be known!), update info
                if (originatingPlayer != null)
                {
                    originatingPlayer.SetInfo(evData); // we got the custom event content above
                }
                else
                {
                    this.DebugReturn("did not find player to set info: " + originatingActorNr);
                }

                this.PrintPlayers();
                break;

            case (byte)Player.DemoEventCode.PlayerMove:
                // this is a custom event, which is defined by this application.
                // if player is known (and it should be known) update position
                if (originatingPlayer != null)
                {
                    originatingPlayer.SetPosition(evData);
                }
                else
                {
                    this.DebugReturn("did not find player to move: " + originatingActorNr);
                }

                break;
        }
    }

    #endregion

    #region Game Handling

    /// <summary>
    /// Here the connection to Photon is established (if not already connected).
    /// </summary>
    internal override void Connect()
    {
        if (this.Peer == null)
        {
            this.Peer = new LitePeer(this, this.UsedProtocol);
        }
        else if (this.Peer.PeerState != PeerStateValue.Disconnected)
        {
            this.DebugReturn("already connected! disconnect first.");
            return;
        }

        //the next two lines show how you could activate the new Network Simulation (described in doc)
        //this.Peer.NetworkSimulationSettings.IsSimulationEnabled = true;
        //this.Peer.NetworkSimulationSettings.OutgoingLag = 200;

        this.Players = new Dictionary<int, Player>();
        this.LocalPlayer = new Player(0);

        this.DebugReturn("Trying to connect...");

        // This method overrides PhotonClient.Connect() but we want to keep it's state handling. Call base:
        base.Connect();

        // The amount of debugging/logging from the Photon library can be set this way:
        this.Peer.DebugOut = DebugLevel.ERROR;
    }

    // Disconnect from the server.
    internal void Disconnect()
    {
        if (this.Peer != null)
        {
            this.Peer.Disconnect();	//this will dump all prepared outgoing data and immediately send a "disconnect"
        }
    }

    /// <summary>
    /// In this demo, we separated the input (DemoGuiAndInput script) from the actual game. Cause of this
    /// and because the Peer is protected, we "wrap" exchanging the encryption keys with this method.
    /// </summary>
    public void OpExchangeKeysForEncryption()
    {
        this.Peer.EstablishEncryption(); // TODO: check OnStatusChanged()
    }

    // Moves the local player. Only changes the position but does not send it, which is triggered in Update().
    public void MoveRandom()
    {
        // dont move if player does not have a number or peer is not connected
        if (this.LocalPlayer == null || this.LocalPlayer.actorNr == 0)
        {
            return;
        }

        this.LocalPlayer.MoveRandom();
    }

    // Will create the operation OpRaiseEvent with local player's position, calling peer.OpRaiseEvent(). 
    // This does not yet send the Operation but queue it in the peer. Sending is done by calling SendOutgoingCommands().
    public void SendPosition()
    {

        // dont move if player does not have a number or peer is not connected
        if (this.LocalPlayer == null || this.LocalPlayer.actorNr == 0 || this.Peer == null)
        {
            return;
        }

        this.LocalPlayer.SendEvMove(this.Peer);
    }

    // Will create and queue the operation OpRaiseEvent with local player's color and name (not position).
    // Actually sent by a call to SendOutgoingCommands().
    // At this point, we could also use properties (so we don't have to re-send this data when someone joins).
    public void SendPlayerInfo()
    {
        // dont move if player does not have a number or peer is not connected
        if (this.LocalPlayer == null || this.LocalPlayer.actorNr == 0 || this.Peer == null)
        {
            return;
        }

        this.LocalPlayer.SendPlayerInfo(this.Peer);
    }

    // Simple "help" function to print current list of players.
    // As this function uses the players list, make sure it's not called while 
    // peer.DispatchIncomingCommands() might modify the list!! (e.g. by lock(this))
    public void PrintPlayers()
    {
        string players = "Players: ";
        foreach (Player p in this.Players.Values)
        {
            players += p.ToString() + ", ";
        }

        this.DebugReturn(players);
    }

    #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <summary>
// Part of the "Demo Lobby" for Photon in Unity.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;

using UnityEngine;

/// <summary>
/// This class encapsulates the Lite Application's workflow to keep a connection and 
/// dispatch operation-results and events into the game logic.
/// This (more or less) resembles a game-loop and is a simple base for this project.
/// The PhotonClient also keeps track of it's state. In this layer, we (currently) only care
/// about the states: Disconnected, Connected, InRoom. More detail could be aquired by directly
/// accessing the LitePeer.PeerState.
/// </summary>
/// <remarks>
/// Having this autonomous "game-loop" for each client makes it easier to simulate multiple
/// clients in the Editor or in the players. It's usually a good way to test out features 
/// that require multiple connections.
/// </remarks>
public class PhotonClient : MonoBehaviour, IPhotonPeerListener 
{
    /// <summary>
    /// The PhotonPeer or LitePeer (extends PhotonPeer) is the central class to communicate with the Photon Server.
    /// In this case, we use a LiteLobbyPeer and make it protected, so not everyone cann directly access it.
    /// It's an essential point that a PhotonPeer (or derived class) is not thread safe - it should only be used from a single thread context!
    /// </summary>
    protected LiteLobbyPeer Peer;

    /// <summary>All clients will connect to this server address. Set as: <hostname>:<ip>. Edit before building for iPhone!</summary>
    public string ServerAddress = "localhost:5055";
    
    /// <summary>The name of the server application this client uses. These are defined in photonsocketserver.config.</summary>
    protected string ServerApplication = "LiteLobby";

    /// <summary>used as send and dispatch interval. simple but ok for a chat.</summary>
    public int SendIntervalMs = 100;

    /// <summary>The next timestamp when a Service() call is due.</summary>
    private int NextSendTickCount = Environment.TickCount;

    // Gets the peer's "low level" state. This client also has it's own connection states.
    public PeerStateValue LitePeerState { get { return this.Peer.PeerState; } }
    
    /// <summary>The current state of this client.</summary>
    public ClientState State = ClientState.Disconnected;
    
    /// <summary>This client's actor number in a room. It's only valid per room.</summary>
    public int ActorNumber;

    // All debug output is collected and buffered, line by line. This could be removed later on.
    private StringBuilder DebugBuffer = new StringBuilder();
    
    // Converts the DebugBuffer to a single string
    public string DebugOutput { get { return DebugBuffer.ToString(); } }
    
    /// <summary>If false, Debug.Log() won't be used but debug callbacks will still be cached.</summary>
    public bool DebugOutputToConsole = true;
    
    /// <summary>User-friendly description of the various conditions that lead to a disconnect. Shown while disconnected.</summary>
    public string OfflineReason = String.Empty;

    // A simpler set of states is enough for this level. The ChatPhotonClient has it's own set.
    public enum ClientState : byte
    {
        Disconnected, Connected, InRoom
    }

    /// <summary>
    /// Called by Unity on start. We create and connect a new LiteLobbyPeer. 
    /// This makes the PhotonClient relatively autonomous.
    /// </summary>
    public virtual void Start() 
    {
        this.Peer = new LiteLobbyPeer(this);
        this.Connect();
    }

    /// <summary>
    /// Update is called once per frame by Unity. Every SendIntervalMs, we send a 
    /// UDP package (if anything must be sent) and dispatch incoming events and operation-results.
    /// </summary>
    public virtual void Update() 
    {
        if (Environment.TickCount > this.NextSendTickCount)
        {
            this.Peer.Service();
            this.NextSendTickCount = Environment.TickCount + this.SendIntervalMs;
        }
    }

    /// <summary>
    /// Called by Unity when when the application closes. We use that to disconnect.
    /// Disconnect will immediately send a package to the server telling it that
    /// the connection is being closed. This way, the server is informed and this peer
    /// is not considered connected until a timeout happens.
    /// </summary>
    public virtual void OnApplicationQuit()
    {
        this.Peer.Disconnect();
    }

    /// <summary>
    /// Aside from calling Peer.Connect() the OfflineReason is also reset.
    /// If Unity's policy request fails, we set the corresponding "friendly" message.
    /// </summary>
    /// <remarks>
    /// If Unity's policy request fails (for webplayer builds), OnStatusChanged() is called
    /// nearly immediately after Connect(). Check for StatusCode.SecurityExceptionOnConnect there.
    /// </remarks>
    internal virtual void Connect()
    {
        this.OfflineReason = String.Empty;
        // PhotonPeer.Connect() is described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf
        this.Peer.Connect(this.ServerAddress, this.ServerApplication);
    }

    /// <summary>
    /// This method is from the IPhotonPeerListener interface and called by the library with 
    /// information during development.
    /// </summary>
    /// <remarks>Described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf.</remarks>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public void DebugReturn(DebugLevel level, string message)
    {
        this.DebugReturn(message);
    }

    /// <summary>
    /// This will append the debug out to a buffer (for display on screen, if needed) and
    /// can log it out to the console.
    /// This method is also used by all classes of the Demo, so all debug out is available.
    /// </summary>
    /// <param name="message"></param>
    public void DebugReturn(string message)
    {
        this.DebugBuffer.AppendLine(message);

        if (this.DebugOutputToConsole)
        {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// On this level, only Join and Leave are handled and set the corresponding state. 
    /// This whole class is more or less just automating the Peer / connection and leaves anything
    /// else to classes that extend it.
    /// </summary>
    /// <remarks>Described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf.</remarks>
    public virtual void OnOperationResponse(OperationResponse operationResponse)
    {
        this.DebugReturn(String.Format("OnOperationResponse: {0}", operationResponse.ToStringFull()));

        switch (operationResponse.OperationCode)
        {
            case (byte)LiteOpCode.Join:
                this.State = ClientState.InRoom;
                this.ActorNumber = (int)operationResponse[(byte)LiteOpKey.ActorNr];
                break;
            case (byte)LiteOpCode.Leave:
                this.State = ClientState.Connected;
                break;
        }
    }

    /// <summary>
    /// This method is from the IPhotonPeerListener interface and on this level primarily handles error-states. 
    /// </summary>
    /// <remarks>
    /// Error conditions that lead to a disconnect get two callbacks from the Photon client library:
    /// a) The error itself
    /// b) The following disconnect - due to the reason of a)
    /// 
    /// This allows a client to just check ClientState.Disconnected and still reliably detect a disconnect state.
    /// </remarks>
    /// <param name="statusCode"></param>
    public virtual void OnStatusChanged(StatusCode statusCode)
    {
        this.DebugReturn(String.Format("OnStatusChanged: {0}", statusCode));

        switch (statusCode)
        {
            case StatusCode.Connect:
                this.State = ClientState.Connected;
                break;
            case StatusCode.Disconnect:
                this.State = ClientState.Disconnected;
                this.ActorNumber = 0;
                break;
            case StatusCode.ExceptionOnConnect:
                this.OfflineReason = "Connection failed.\nIs the server online? Firewall open?";
                break;
            case StatusCode.SecurityExceptionOnConnect:
                this.OfflineReason = "Security Exception on connect.\nMost likely, the policy request failed.\nIs Photon and the Policy App running?";
                break;
            case StatusCode.Exception:
                this.OfflineReason = "Communication terminated by Exception.\nProbably the server shutdown locally.\nOr the network connection terminated.";
                break;
            case StatusCode.TimeoutDisconnect:
                this.OfflineReason = "Disconnect due to timeout.\nProbably the server shutdown locally.\nOr the network connection terminated.";
                break;
            case StatusCode.DisconnectByServer:
                this.OfflineReason = "Timeout Disconnect by server.\nThe server did not get responses in time.";
                break;
            case StatusCode.DisconnectByServerLogic:
                this.OfflineReason = "Disconnect by server.\nThe servers logic (application) disconnected this client for some reason.";
                break;
            case StatusCode.DisconnectByServerUserLimit:
                this.OfflineReason = "Server reached it's user limit.\nThe server is currently not accepting connections.\nThe license does not allow it.";
                break;
            default:
                this.DebugReturn("StatusCode not handled: " + statusCode);
                break;
        }
    }

    /// <summary>
    /// This "generic" PhotonClient does not handle any events but simply prints them out.
    /// The Game extends and overrides this method with something meaningful.
    /// </summary>
    /// <remarks>Described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf.</remarks>
    /// <param name="photonEvent">The dispatched event.</param>
    public virtual void OnEvent(EventData photonEvent)
    {
        this.DebugReturn(String.Format("OnEvent: {0}", photonEvent.ToStringFull()));
    }
}

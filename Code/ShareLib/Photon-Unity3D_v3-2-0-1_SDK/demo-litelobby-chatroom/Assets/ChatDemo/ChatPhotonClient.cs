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
using System.Text;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;

/// <summary>
/// This class implements most of the chat-related functionality on top of a PhotonClient, 
/// but does not implement the GUI. It keeps track of the current state in respect to the 
/// chat demo's features.
/// </summary>
/// <remarks>
/// By extending PhotonClient, this class is also a MonoBehaviour and it can be attached to
/// objects. The engine's methods Start, Update, and 
/// </remarks>
public class ChatPhotonClient : PhotonClient
{
    public StringBuilder ChatLines = new StringBuilder();
    public string RoomName;
    public string LobbyName;
    public string UserName = String.Empty;

    /// <summary>Current state of the chat application.</summary>
    public ChatStateOption ChatState = ChatStateOption.Offline;

    /// <summary>Caches a lobby's room info. Initialized when a lobby is joined and updated after that when changes happen.</summary>
    public Hashtable RoomHashtable = new Hashtable();

    /// <summary>A cache of user/actor properties in a chat room.</summary>
    public Hashtable ActorProperties = new Hashtable();

    /// <summary>Caches the list of players in a room. To make this more clean, it should be reset on leave/disconnect.</summary>
    public int[] ActorNumbersInRoom = new int[0];

    /// <summary>This demo defines new custom events (sent via Peer.OpRaiseEvent) and these codes are used for them.</summary>
    public enum ChatEventCode : byte { Message = 50, PrivateMessage = 51 }

    /// <summary>All chat events have only one custom event-key (the message). The ActorNumber (originating user in room) is part of these events by default (LiteLobby conventions).</summary>
    public enum ChatEventKey : byte { TextLine = 1 }

    /// <summary>This demo supports only Name but could be extended with more properties easily.</summary>
    public enum ChatActorProperties : byte { Name = (byte)'n' }

    /// <summary>On this (chat) application level, additional states are used.</summary>
    public enum ChatStateOption : byte
    {
        /// <summary>Used on start and on disconnect.</summary>
        Offline,
        /// <summary>Used on connect and when a room is left or on disconnect.</summary>
        OutsideOfRooms,
        /// <summary>Used as transition from OutsideOfRooms to InLobbyRoom. Checked when the result for OpJoin is handled. Has no GUI.</summary>
        JoiningLobbyRoom,
        /// <summary>Used in the lobby room, where no chat is possible and a user just selects a chat room to join.</summary>
        InLobbyRoom,
        /// <summary>Used as transition to InChatRoom. Checked when the result for OpJoin is handled. Has no GUI.</summary>
        JoiningChatRoom,
        /// <summary>Used while in a chat room.</summary>
        InChatRoom
    }

    /// <summary>
    /// On this level, only the connect and disconnect status callbacks are interesting and used.
    /// But we call the base-classes callback as well, which handles states in more detail.
    /// </summary>
    /// <remarks>Described in the Unity Reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf</remarks>
    /// <param name="statusCode"></param>
    public override void OnStatusChanged(StatusCode statusCode)
    {
        base.OnStatusChanged(statusCode);

        switch (statusCode)
        {
            case StatusCode.Connect:
                ChatState = ChatStateOption.OutsideOfRooms;
                break;
            case StatusCode.Disconnect:
                ChatState = ChatStateOption.Offline;
                break;
            default:
                DebugReturn("StatusCode not handled: " + statusCode);
                break;
        }
    }

    /// <summary>
    /// For this client, we change states after joining a lobby or room was successfuly done. 
    /// Also, properties of other users in a chat room (fetched with GetProperties) are cached.
    /// </summary>
    /// <remarks>
    /// Called when an operation from this client was executed and produced a result.
    /// Will be called when this client calls Peer.Service() or DispatchIncomingCommands().
    /// </remarks>
    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        base.OnOperationResponse(operationResponse);

        // to take a look at the inside of any operation result, we could log it out as string this way
        //this.DebugReturn(SupportClass.HashtableToString(returnValues));

        switch (operationResponse.OperationCode)
        {
            case (byte)LiteOpCode.Join:
                if (this.ChatState == ChatStateOption.JoiningChatRoom)
                {
                    // on joining a chatroom: get properties of everyone else in the room
                    // the result of this operation is handled below
                    this.Peer.OpGetProperties(0);
                    this.ChatState = ChatStateOption.InChatRoom;
                }
                if (this.ChatState == ChatStateOption.JoiningLobbyRoom)
                {
                    this.ChatState = ChatStateOption.InLobbyRoom;
                }
                break;
            case (byte)LiteOpCode.GetProperties:
                // the result of operation GetProperties contains two interesting hashtables: one for actors and one for the room properties
                // this demo uses a property per actor for the name, so we are caching the complete list we just got

                Hashtable actorProps = operationResponse[(byte)LiteOpKey.ActorProperties] as Hashtable;
                this.ActorProperties = actorProps;  // updates (name changes, new users) are sent by the server as event and merged in to this
                break;
        }
    }

    /// <summary>
    /// Most of the "work" in the chat demo is done by events. 
    /// The events Join, Leave, RoomList and RoomListUpdate are the ones pre-defined by LiteLobby. 
    /// The Message event is a "custom event" that is defined, sent and used only by the Chat Demo.
    /// </summary>
    /// <remarks>
    /// Described in the Unity Reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf
    /// </remarks>
    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);

        // Custom events (defined and sent by a client) encapsulate the sent data in a separate Hashtable. This avoids duplicate usage of keys.
        // The event content a client sends is under key Data. This demo mostly handles custom data, so let's "grab" our content for later use
        Hashtable evData = photonEvent[(byte)LiteEventKey.Data] as Hashtable;

        // Most events are sent by (other) users. Their origin can be read from the ActorNr key
        int originatingActorNr = 0;
        if (photonEvent.Parameters.ContainsKey((byte)LiteEventKey.ActorNr))
        {
            originatingActorNr = (int)photonEvent[(byte)LiteEventKey.ActorNr];
        }

        switch (photonEvent.Code)
        {
            // this client or any other joined the room (lobbies will not send this event but chat rooms do)
            case (byte)LiteEventCode.Join:
                // update the list of actor numbers in room
                this.ActorNumbersInRoom = (int[])photonEvent[(byte)LiteOpKey.ActorList];

                // update the list of actorProperties if any were set on join
                Hashtable actorProps = photonEvent[(byte)LiteEventKey.ActorProperties] as Hashtable;
                if (actorProps != null)
                {
                    this.ActorProperties[originatingActorNr] = actorProps;
                }
                break;

            // some other user left this room - remove his data
            case (byte)LiteEventCode.Leave:
                // update the list of actor numbers in room
                this.ActorNumbersInRoom = (int[])photonEvent[(byte)LiteOpKey.ActorList];

                // update the list of actorProperties we cache
                if (this.ActorProperties.ContainsKey(originatingActorNr))
                {
                    this.ActorProperties.Remove(originatingActorNr);
                }
                break;
            case (byte)ChatEventCode.Message:
                // this event "type" is created on the fly by this demo. Lite does not define it but allows us to send "custom events"
                // the content of this event is up to this demo as well, so take a look at SendChatMessage() in this class
                string sender = this.GetActorPropertyNameOf(originatingActorNr);
                string message = evData[(byte) ChatEventKey.TextLine] as string;
                ChatLines.AppendLine(String.Format("{0}: {1}", sender, message));
                break;

            case (byte)LiteLobbyPeer.LiteLobbyEventCode.RoomList:
            case (byte)LiteLobbyPeer.LiteLobbyEventCode.RoomListUpdate:
                // these two events are sent by lobby rooms
                // the first is a complete list (provided on join) and the latter an update to the list (when some room's user count changed)
                Hashtable roomData;
                roomData = (Hashtable)photonEvent[(byte)LiteLobbyPeer.LiteLobbyEventKey.RoomsArray];

                // updates exclude rooms in which the user count is not changed
                // so we merge and update new data into a local cache
                foreach (string key in roomData.Keys)
                {
                    //each key is a room name. each value of a key is the current player count
                    //we still list rooms when they are known and have 0 players. this could be changed easily
                    this.RoomHashtable[key] = roomData[key];
                }
                break;
        }
    }

    /// <summary>
    /// Wraps a call to LiteLobbyPeer.OpJoinFromLobby with an even nicer signature and adds the properties as needed.
    /// This method changes the ChatState to JoiningChatRoom.
    /// </summary>
    /// <remarks>
    /// Obviously the OpJoinFromLobby method could also be changed to accept the name, instead of the property hash.
    /// I wanted to keep that method's signature generic. Not it simply accepts any properties and this class provides them.
    /// </remarks>
    /// <param name="roomName">A chat room name to join.</param>
    public void JoinRoomFromLobby(string roomName)
    {
        this.RoomName = roomName;
        this.ChatState = ChatStateOption.JoiningChatRoom;
        this.ActorProperties = new Hashtable();

        Hashtable props = new Hashtable() { { ChatActorProperties.Name, this.UserName } };
        this.Peer.OpJoinFromLobby(this.RoomName, this.LobbyName, props, true);
    }

    /// <summary>
    /// This wrapper method joins a lobby and set the fitting state (this helps us remember what sort of room we joined).
    /// </summary>
    /// <remarks>
    /// By convention of the LiteLobby server, any room that ends on "_lobby" is a lobby, not a regular room. 
    /// We don't enforce the ending here, so calling this method must adhere to conventions, too.
    /// </remarks>
    /// <param name="lobbyName"></param>
    public void JoinLobby(string lobbyName)
    {
        this.LobbyName = lobbyName;
        this.RoomName = lobbyName;
        this.ChatState = ChatStateOption.JoiningLobbyRoom;
        this.RoomHashtable = new Hashtable();
        this.Peer.OpJoin(lobbyName);
    }

    /// <summary>
    /// Calls OpLeave (which can be used in lobby and any other room alike) and sets a state.
    /// </summary>
    public void LeaveRoom()
    {
        this.Peer.OpLeave();
        this.ChatState = ChatStateOption.OutsideOfRooms;
    }

    /// <summary>
    /// In our demo, you can't really disconnect. Only "surplus" clients that we don't want to use anymore do disconnect.
    /// </summary>
    public void Disconnect()
    {
        this.Peer.Disconnect();
    }
    
    /// <summary>
    /// Uses OpRaiseEvent to get the message across and adds it to the local chat buffer. 
    /// This event is not not defined by the server's logic. The Lite application we use does not "know" what a chat message is
    /// but allows any client to send any event type with any content. This is called a "custom event". Lite will simply forward
    /// this event to the other users in the room. It will add this user's actorNumber as origin, so we don't have to send that.
    /// Remember: All clients cache the actorNumbers of everyone in the same room and their properties (names).
    /// 
    /// Our custom event gets the event code (byte)51. Why 51? It could be anything that doesn't "collide" with the event codes
    /// that LiteLobby already defines. So 51 is more or less random.
    /// 
    /// The content of this event is up to our needs. A single TextLine is enough. We put it into key (byte)1. A byte key is 
    /// small and effective but the demo could also use strings or any other datatype. It could be 1 or "one" or "ChatDemoTextLine".
    /// 
    /// Lite and LiteLobby allow you to use any key in your event's data. The resulting event (on the receiving end) is already
    /// a Hashtable. What we send here is in that event under key (byte)LiteEventKey.Data. Check out how OnEvent() in this demo
    /// treats the event.
    /// </summary>
    /// <remarks>
    /// In the LiteLobby logic, events are not sent back to the originating client. We already know what the event looks like.
    /// </remarks>
    /// <param name="line"></param>
    public void SendChatMessage(string line)
    {
        Hashtable chatEvent = new Hashtable();  // the custom event's data. content we want to send
        chatEvent.Add((byte)ChatEventKey.TextLine, line);   // add some content
        this.Peer.OpRaiseEvent((byte)ChatEventCode.Message, chatEvent, true);   // call raiseEvent with our content and a event code

        // because Photon won't send this event back to this client, we will add the chat line locally
        this.ChatLines.AppendLine(String.Format("{0}: {1}", this.UserName, line));
    }

    /// <summary>
    /// Helper method to find a name inside the cached actor properties.
    /// </summary>
    /// <param name="actorNr">The actorNumber of someone in the same room as this client.</param>
    /// <returns>Either the name or "<actorNr> unknown"</returns>
    public string GetActorPropertyNameOf(int actorNr)
    {
        if (this.ActorProperties == null)
        {
            return "ActorProperties == null";
        }

        // we cache all actor properties in this.ActorProperties and each actor has it's own hashtable of properties
        Hashtable actorProps = this.ActorProperties[actorNr] as Hashtable;
        if (actorProps != null)
        {
            // if we found this user's properties, get the name property from it (we defined this in this demo ourselfs)
            string name = actorProps[(byte)ChatActorProperties.Name] as string;
            if (!String.IsNullOrEmpty(name))
            {
                return name;
            }
        }

        // fallback if user is not found
        return String.Format("{0} unknown", actorNr);
    }

    /// <summary>
    /// Helper method to be able to print out the actornumber list of users currently in a room.
    /// </summary>
    private string IntArrayToString(int[] returnValue)
    {
        if (returnValue == null)
        {
            return "[] = null";
        }
        string ret = String.Empty;
        for (int i = 0; i < returnValue.Length; i++)
        {
            ret = String.Format("{0}, {1}", ret, returnValue[i]);
        }
        return ret;
    }
}


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

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;

/// <summary>
/// The LiteLobbyPeer extends a "regular" LitePeer by several Lobby specific Operations and Events.
/// The enumerations simplify access to the byte codes used for operation parameters, event codes and keys and more.
/// The method OpJoinFromLobby() wraps up a custom operation "Join" with the additional parameter of a lobby.
/// </summary>
public class LiteLobbyPeer : LitePeer
{

    /// <summary>LiteLobby uses the same operation code for Join but extends it with another parameter.</summary>
    public enum LiteLobbyOpCode : byte
    {
        /// <summary>Same as Lite join.</summary>
        Join = LiteOpCode.Join
    }

    /// <summary>LiteLobbyPeer effectively only adds the LobbyName parameter but let's define the other operation parameter keys, too.</summary>
    public enum LiteLobbyOpKey : byte
    {
        /// <summary>RoomName is a better name, instead of ActorSessionInstanceId (Asid).</summary>
        RoomName = LiteOpKey.GameId,

        /// <summary>(242) A lobby-name to connect this room to.</summary>
        LobbyName = 242,

        /// <summary>This actor's properties for this room.</summary>
        ActorProperties = LiteOpKey.ActorProperties
    }

    /// <summary>The lobby's event contains these keys.</summary>
    public enum LiteLobbyEventKey : byte
    {
        /// <summary>A Hashtable of rooms (key = name) and their current user-count.</summary>
        RoomsArray = LiteEventKey.Data
    }

    /// <summary>The LiteLobby server application defines events to update clients about currently available rooms.</summary>
    public enum LiteLobbyEventCode : byte
    {
        /// <summary>A complete roomlist, sent to "this client" when a lobby is joined.</summary>
        RoomList = 252,

        /// <summary>Updates of the roomlist.</summary>
        RoomListUpdate = 251
    }

    public LiteLobbyPeer(IPhotonPeerListener listener) : base(listener)
    {
    }

    /// <summary>
    /// Sends an OpJoin with the additional parameter of a lobby-name. 
    /// The server-side LiteLobby application will join a room and make it update the lobby with it's actor-count.
    /// </summary>
    /// <remarks>
    /// The application on the server defines which operation will join a room, which parameters that uses and 
    /// all the codes for operation and parameters as well as their value type. 
    /// This client is developed, taking a look at the server's operation join.
    /// </remarks>
    /// <param name="gameName"></param>
    /// <param name="lobbyName"></param>
    /// <param name="actorProperties"></param>
    /// <param name="broadcastActorProperties"></param>
    /// <returns></returns>
    public virtual bool OpJoinFromLobby(string gameName, string lobbyName, Hashtable actorProperties, bool broadcastActorProperties)
    {
        if (this.DebugOut >= DebugLevel.ALL)
        {
            this.Listener.DebugReturn(DebugLevel.ALL, String.Format("OpJoin({0}/{1})", gameName, lobbyName));
        }

        // All operations get their parameters as key-value set (a Hashtable)
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[(byte)LiteLobbyOpKey.RoomName] = gameName;
        opParameters[(byte)LiteLobbyOpKey.LobbyName] = lobbyName;

        if (actorProperties != null)
        {
            opParameters[(byte)LiteOpKey.ActorProperties] = actorProperties;
            if (broadcastActorProperties)
            {
                opParameters[(byte)LiteOpKey.Broadcast] = broadcastActorProperties;
            }
        }

        return OpCustom((byte)LiteLobbyOpCode.Join, opParameters, true);
    }
}
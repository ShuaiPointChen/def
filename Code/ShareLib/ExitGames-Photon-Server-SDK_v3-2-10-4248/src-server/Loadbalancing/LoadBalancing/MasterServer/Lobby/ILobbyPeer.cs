// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILobbyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ILobbyPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Collections.Generic;

    using Photon.SocketServer;

    public interface ILobbyPeer
    {
        HashSet<string> BlockedUsers { get; }

        NetworkProtocolType NetworkProtocol { get; }

        string UserId { get; }
    }
}
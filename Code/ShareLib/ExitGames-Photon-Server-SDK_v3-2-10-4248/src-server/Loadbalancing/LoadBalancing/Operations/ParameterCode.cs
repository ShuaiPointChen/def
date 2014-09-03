// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    public enum ParameterCode : byte
    {
        Address = 230,
        PeerCount = 229,
        GameCount = 228,
        MasterPeerCount = 227,
        GameId = Lite.Operations.ParameterKey.GameId, // (226)
        UserId = 225,
        ApplicationId = 224,
        Position = 223,
        GameList = 222,
        Secret = 221,
        AppVersion = 220,
        NodeId = 219,
        Info = 218
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    public enum OperationCode : byte
    {
        Authenticate = 230, 
        JoinLobby = 229, 
        LeaveLobby = 228, 
        CreateGame = 227, 
        JoinGame = 226, 
        JoinRandomGame = 225, 
        CancelJoinRandomGame = 224, 
        DebugGame = 223,
    }
}
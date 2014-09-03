// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinGameRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    #region using directives

    using System.Collections;

    using Lite.Operations;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    public class JoinGameRequest : Operation
    {
        public JoinGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public JoinGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = false)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }
    }
}
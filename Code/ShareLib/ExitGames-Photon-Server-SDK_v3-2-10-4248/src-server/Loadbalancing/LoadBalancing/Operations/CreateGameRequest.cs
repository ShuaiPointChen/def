// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateGameRequest type.
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

    public class CreateGameRequest : Operation
    {
        public CreateGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public CreateGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }
    }
}
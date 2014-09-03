// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRandomGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinRandomGameRequest type.
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

    public class JoinRandomGameRequest : Operation
    {
        public JoinRandomGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public JoinRandomGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position, IsOptional = true)]
        public byte JoinRandomType { get; set; }
    }
}
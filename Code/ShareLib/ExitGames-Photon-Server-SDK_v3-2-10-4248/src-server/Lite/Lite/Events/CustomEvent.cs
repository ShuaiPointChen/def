// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implementation of a custom event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Events
{
    using System.Collections;

    using Lite.Operations;

    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Implementation of a custom event.
    /// </summary>
    public class CustomEvent : LiteEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEvent"/> class.
        /// </summary>
        /// <param name="actorNr">
        /// The actor nr.
        /// </param>
        /// <param name="eventCode">
        /// The event code.
        /// </param>
        /// <param name="data">
        /// The event data.
        /// </param>
        public CustomEvent(int actorNr, byte eventCode, Hashtable data)
            : base(actorNr)
        {
            this.Code = eventCode;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        /// <value>The event data.</value>
        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public Hashtable Data { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating how the event updates.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        [DataMember(Code = (byte)ParameterKey.Cache, IsOptional = true)]
        public byte Cache { get; set; }
    }
}
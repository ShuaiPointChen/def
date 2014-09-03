// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Room.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A room has <see cref="Actor" />s, can have properties, and provides an <see cref="ExecutionFiber" /> with a few wrapper methods to solve otherwise complicated threading issues:
//   All actions enqueued to the <see cref="ExecutionFiber" /> are executed in a serial order. Operations of all Actors in a room are handled via ExecutionFiber.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Lite.Common;
    using Lite.Events;
    using Lite.Messages;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    ///   A room has <see cref = "Actor" />s, can have properties, and provides an <see cref = "ExecutionFiber" /> with a few wrapper methods to solve otherwise complicated threading issues:
    ///   All actions enqueued to the <see cref = "ExecutionFiber" /> are executed in a serial order. Operations of all Actors in a room are handled via ExecutionFiber.
    /// </summary>
    public class Room : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   The room name.
        /// </summary>
        private readonly string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Room" /> class without a room name.
        /// </summary>
        /// <param name = "name">
        ///   The room name.
        /// </param>
        public Room(string name)
            : this(name, new PoolFiber())
        {
            this.ExecutionFiber.Start();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Room" /> class.
        /// </summary>
        /// <param name = "name">
        ///   The room name.
        /// </param>
        /// <param name = "executionFiber">
        ///   The execution fiber used to synchronize access to this instance.
        /// </param>
        protected Room(string name, PoolFiber executionFiber)
        {
            this.name = name;
            this.ExecutionFiber = executionFiber;
            this.Actors = new ActorCollection();
            this.Properties = new PropertyBag<object>();
        }

        /// <summary>
        ///   Finalizes an instance of the <see cref = "Room" /> class. 
        ///   This destructor will run only if the Dispose method does not get called.
        ///   It gives your base class the opportunity to finalize.
        ///   Do not provide destructors in types derived from this class.
        /// </summary>
        ~Room()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a <see cref = "PoolFiber" /> instance used to synchronize access to this instance.
        /// </summary>
        /// <value>A <see cref = "PoolFiber" /> instance.</value>
        public PoolFiber ExecutionFiber { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether IsDisposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///   Gets the name (id) of the room.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        ///   Gets a PropertyBag instance used to store custom room properties.
        /// </summary>
        public PropertyBag<object> Properties { get; private set; }

        /// <summary>
        ///   Gets an <see cref = "ActorCollection" /> containing the actors in the room
        /// </summary>
        protected ActorCollection Actors { get; private set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Room name: {0}, Actors: {1}, Properties: {2}", this.name, this.Actors.Count, this.Properties == null ? 0 : this.Properties.Count).AppendLine(); 
            
            foreach (var actor in this.Actors)
            {
                sb.AppendLine(actor.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Enqueues an <see cref = "IMessage" /> to the end of the execution queue.
        /// </summary>
        /// <param name = "message">
        ///   The message to enqueue.
        /// </param>
        /// <remarks>
        ///   <see cref = "ProcessMessage" /> is called sequentially for each operation request 
        ///   stored in the execution queue.
        ///   Using an execution queue ensures that messages are processed in order
        ///   and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueMessage(IMessage message)
        {
            this.ExecutionFiber.Enqueue(() => this.ProcessMessage(message));
        }

        /// <summary>
        ///   Enqueues an <see cref = "OperationRequest" /> to the end of the execution queue.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request to enqueue.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <remarks>
        ///   <see cref = "ExecuteOperation" /> is called sequentially for each operation request 
        ///   stored in the execution queue.
        ///   Using an execution queue ensures that operation request are processed in order
        ///   and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }

        /// <summary>
        ///   Schedules a message to be processed after a specified time.
        /// </summary>
        /// <param name = "message">
        ///   The message to schedule.
        /// </param>
        /// <param name = "timeMs">
        ///   The time in milliseconds to wait before the message will be processed.
        /// </param>
        /// <returns>
        ///   an <see cref = "IDisposable" />
        /// </returns>
        public IDisposable ScheduleMessage(IMessage message, long timeMs)
        {
            return this.ExecutionFiber.Schedule(() => this.ProcessMessage(message), timeMs);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name = "dispose">
        ///   <c>true</c> to release both managed and unmanaged resources; 
        ///   <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool dispose)
        {
            this.IsDisposed = true;

            if (dispose)
            {
                this.ExecutionFiber.Dispose();
            }
        }

        /// <summary>
        ///   This method is invoked sequentially for each operation request 
        ///   enqueued in the <see cref = "ExecutionFiber" /> using the 
        ///   <see cref = "EnqueueOperation" /> method.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "operation">
        ///   The operation request.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void ExecuteOperation(LitePeer peer, OperationRequest operation, SendParameters sendParameters)
        {
        }

        /// <summary>
        ///   This method is invoked sequentially for each message enqueued 
        ///   by the <see cref = "EnqueueMessage" /> or <see cref = "ScheduleMessage" />
        ///   method.
        /// </summary>
        /// <param name = "message">
        ///   The message to process.
        /// </param>
        protected virtual void ProcessMessage(IMessage message)
        {
        }

        /// <summary>
        ///   Publishes an event to a single actor on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actor">
        ///   The <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(LiteEventBase e, Actor actor, SendParameters sendParameters)
        {
            var eventData = new EventData(e.Code, e);
            actor.Peer.SendEvent(eventData, sendParameters);
        }

        /// <summary>
        ///   Publishes an event to a list of actors on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actorList">
        ///   A list of <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(LiteEventBase e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            IEnumerable<PeerBase> peers = actorList.Select(actor => actor.Peer);
            var eventData = new EventData(e.Code, e);
            ApplicationBase.Instance.BroadCastEvent(eventData, peers, sendParameters);
        }

        /// <summary>
        ///   Publishes an event to a list of actors on a specified channel.
        /// </summary>
        /// <param name = "e">
        ///   The event to publish.
        /// </param>
        /// <param name = "actorList">
        ///   A list of <see cref = "Actor" /> who should receive the event.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected void PublishEvent(EventData e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            IEnumerable<PeerBase> peers = actorList.Select(actor => actor.Peer);
            ApplicationBase.Instance.BroadCastEvent(e, peers, sendParameters);
        }
        #endregion
    }
}
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LatencyMonitor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LatencyMonitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.LoadShedding
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.LoadBalancing.GameServer;
    using Photon.LoadBalancing.LoadShedding.Diagnostics;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using PhotonHostRuntimeInterfaces;

    public sealed class LatencyMonitor : ServerPeerBase, ILatencyMonitor
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly int intervalMs;

        private readonly ValueHistory latencyHistory;

        private readonly byte operationCode;

        private readonly WorkloadController workloadController;

        private readonly ThreadSafeStopwatch stopWatch;
        
        private int averageLatencyMs;

        private int lastLatencyMs;

        private IDisposable pingTimer;

        #endregion

        #region Constructors and Destructors

        public LatencyMonitor(
            IRpcProtocol protocol, IPhotonPeer nativePeer, byte operationCode, int maxHistoryLength, int intervalMs, WorkloadController workloadController)
            : base(protocol, nativePeer)
        {
            this.operationCode = operationCode;
            this.intervalMs = intervalMs;
            this.workloadController = workloadController;
            this.latencyHistory = new ValueHistory(maxHistoryLength);
            this.averageLatencyMs = 0;
            this.lastLatencyMs = 0;

            log.InfoFormat("{1} connection for latency monitoring established (id={0}), serverId={2}", this.ConnectionId, this.NetworkProtocol, GameApplication.ServerId);

            if (!Stopwatch.IsHighResolution)
            {
                log.InfoFormat("No hires stopwatch!");
            }

            this.stopWatch = new ThreadSafeStopwatch();

            this.pingTimer = this.RequestFiber.ScheduleOnInterval(this.Ping, 0, this.intervalMs);
        }

        #endregion

        #region Properties

        public int AverageLatencyMs
        {
            get
            {
                return this.averageLatencyMs;
            }
        }

        public int LastLatencyMs
        {
            get
            {
                return this.lastLatencyMs;
            }
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.pingTimer != null)
                {
                    this.pingTimer.Dispose();
                    this.pingTimer = null;
                }
            }

            base.Dispose(disposing);
        }

        ////protected override void OnConnectFailed(int errorCode, string errorMessage)
        ////{
        ////    log.WarnFormat("Connect Error {0}: {1}", errorCode, errorMessage);
        ////    if (!this.Disposed)
        ////    {
        ////        // wait a second and try again
        ////        this.RequestFiber.Schedule(this.Connect, 1000);
        ////    }
        ////}

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat("{1} connection for latency monitoring closed (id={0}, reason={2}, detail={3}, serverId={4})", this.ConnectionId, this.NetworkProtocol, reasonCode, reasonDetail, GameApplication.ServerId);
            }

            if (this.pingTimer != null)
            {
                this.pingTimer.Dispose();
                this.pingTimer = null;
            }

            if (this.stopWatch != null)
            {
                this.stopWatch.Stop();
            }

            if (!this.Disposed)
            {
                IPAddress address = IPAddress.Parse(this.RemoteIP);
                this.workloadController.OnLatencyConnectClosed(new IPEndPoint(address, this.RemotePort));
                //this.workloadController.Start();
            }
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            if (operationResponse.ReturnCode == 0)
            {
                var contract = new LatencyOperation(this.Protocol, operationResponse.Parameters);
                if (!contract.IsValid)
                {
                    log.Error("LatencyOperation contract error: " + contract.GetErrorMessage());
                    return;
                }

                long now = this.stopWatch.GetTimestamp();

                var sentTime = contract.SentTime;
                long latencyTicks = now - sentTime.GetValueOrDefault();
                double time = (double)latencyTicks / (double)Stopwatch.Frequency * (double)1000;
                var latencyMs = (int)time;

                Interlocked.Exchange(ref this.lastLatencyMs, latencyMs);
                this.latencyHistory.Add(latencyMs);
                if (this.NetworkProtocol == NetworkProtocolType.Udp)
                {
                    Counter.LatencyUdp.RawValue = latencyMs;
                }
                else
                {
                    Counter.LatencyTcp.RawValue = latencyMs; 
                }
                var newAverage = (int)this.latencyHistory.Average();
                if (newAverage == 0)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("LatancyAvg=0!");
                    }
                }

                Interlocked.Exchange(ref this.averageLatencyMs, newAverage);
            }
            else
            {
                log.ErrorFormat("Received Ping Response with Error {0}: {1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
            }
        }

        private void Ping()
        {
            var contract = new LatencyOperation { SentTime = this.stopWatch.GetTimestamp() };
            var request = new OperationRequest(this.operationCode, contract);
            this.SendOperationRequest(request, new SendParameters());
        }

        #endregion
    }
}
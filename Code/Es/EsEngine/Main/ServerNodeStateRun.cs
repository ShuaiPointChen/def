using System;
using System.Collections.Generic;
using System.Text;
using log4net.Config;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using ExitGames.Client.Photon;
using Eb;

namespace Es
{
    public class ServerNodeStateRun : EbState, IDisposable
    {
        //---------------------------------------------------------------------
        private static readonly ILogger mLog = LogManager.GetCurrentClassLogger();
        private ServerNode mServerNode = null;

        //---------------------------------------------------------------------
        public ServerNodeStateRun(ServerNode server_node)
        {
            mServerNode = server_node;

            _defState("ServerNodeStateRun", "CFsm", 0, false);
            _bindAction("update", new EbAction(this.evUpdate));
            _bindAction("close", new EbAction(this.evClose));
        }

        //---------------------------------------------------------------------
        ~ServerNodeStateRun()
        {
            this.Dispose(false);
        }

        //-----------------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        //-----------------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        //---------------------------------------------------------------------
        public override void enter()
        {
            mLog.Info("ServerNodeStateRun.enter()");

            IServerNodeListener listener = mServerNode.Listener;
            if (listener != null) listener.onRun();
        }

        //---------------------------------------------------------------------
        public override void exit()
        {
            mLog.Info("ServerNodeStateRun.exit()");
        }

        //-----------------------------------------------------------------------------
        public string evUpdate(IEbEvent ev)
        {
            for (int i = 0; i < mServerNode.ListPeerBefore.Count; i++)
            {
                PhotonServerPeerC peer = mServerNode.ListPeerBefore[i];
                //peer.Service();
            }

            return "";
        }

        //-----------------------------------------------------------------------------
        public string evClose(IEbEvent ev)
        {
            return "ServerNodeStateStop";
        }
    }
}
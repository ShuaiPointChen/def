using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ServerNodeStateStop : EbState, IDisposable
    {
        //---------------------------------------------------------------------
        private static readonly ILogger mLog = LogManager.GetCurrentClassLogger();
        private ServerNode mServerNode = null;

        //---------------------------------------------------------------------
        public ServerNodeStateStop(ServerNode server_node)
        {
            mServerNode = server_node;

            _defState("ServerNodeStateStop", "CFsm", 0, false);
            _bindAction("update", new EbAction(this.evUpdate));
        }

        //---------------------------------------------------------------------
        ~ServerNodeStateStop()
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
            mLog.Info("ServerNodeStateStop.enter()");

            for (int i = 0; i < mServerNode.ListPeerBefore.Count; i++)
            {
                PhotonServerPeerC peer = mServerNode.ListPeerBefore[i];
                peer.Disconnect();
                //peer.Service();
            }
        }

        //---------------------------------------------------------------------
        public override void exit()
        {
            mLog.Info("ServerNodeStateStop.exit()");
        }

        //-----------------------------------------------------------------------------
        public string evUpdate(IEbEvent ev)
        {
            return "";
        }
    }
}
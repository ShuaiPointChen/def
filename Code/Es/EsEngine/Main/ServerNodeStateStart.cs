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
    public class ServerNodeStateStart : EbState, IDisposable
    {
        //---------------------------------------------------------------------
        private static readonly ILogger mLog = LogManager.GetCurrentClassLogger();
        private ServerNode mServerNode = null;

        //---------------------------------------------------------------------
        public ServerNodeStateStart(ServerNode server_node)
        {
            mServerNode = server_node;

            _defState("ServerNodeStateStart", "CFsm", 0, true);
            _bindAction("on_peer_status_changed", new EbAction(this.evOnPeerStatusChanged));
            _bindAction("update", new EbAction(this.evUpdate));
            _bindAction("close", new EbAction(this.evClose));
        }

        //---------------------------------------------------------------------
        ~ServerNodeStateStart()
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
            mLog.Info("ServerNodeStateStart.enter()");

            int servernode_id = mServerNode.getServerNodeId();
            _tServerNode server_node = mServerNode.getServerNodeCfg().getServerNode(servernode_id);

            // 连接前置，并统计连接信息
            foreach (var i in server_node.vec_before)
            {
                //_tServerNode node = mServerNode.getServerNodeCfg().getServerNode(i);
                //var peer = new CPhotonServerPeerC(mServerNode, mServerNode.EntityMgr, node.id, mServerNode.getServerNodeId());
                //mServerNode.ListPeerBefore.Add(peer);
            }
        }

        //---------------------------------------------------------------------
        public override void exit()
        {
            mLog.Info("ServerNodeStateStart.exit()");
        }

        //-----------------------------------------------------------------------------
        public string evOnPeerStatusChanged(IEbEvent ev)
        {
            //TEvent2<int, StatusCode> evt = ev as TEvent2<int, StatusCode>;
            //int servernode_id = evt.param1;
            //StatusCode statusCode = evt.param2;

            return "";
        }

        //-----------------------------------------------------------------------------
        public string evUpdate(IEbEvent ev)
        {
            int before_connected_count = 0;
            for (int i = 0; i < mServerNode.ListPeerBefore.Count; i++)
            {
                PhotonServerPeerC peer = mServerNode.ListPeerBefore[i];
                //peer.Service();

                //if (peer.PeerState == PeerStateValue.Disconnected)
                //{
                //    string s = peer.RemoteServerNodeInfo.ip;
                //    s += ":";
                //    s += peer.RemoteServerNodeInfo.port.ToString();
                //    peer.Connect(s, peer.RemoteServerNodeInfo.app_name);
                //}
                //else if (peer.PeerState == PeerStateValue.Connected)
                //{
                //    before_connected_count++;
                //}
            }

            // 等待后置连接，并统计连接信息
            if (mServerNode.ListPeerBefore.Count == before_connected_count &&
                mServerNode.getServerNode().vec_after.Count == mServerNode.ListPeerAfter.Count)
            {
                return "ServerNodeStateRun";
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
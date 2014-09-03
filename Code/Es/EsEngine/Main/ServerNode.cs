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
    public class ServerNode : EbFsm, IDisposable
    {
        //---------------------------------------------------------------------
        private static readonly ILogger mLog = LogManager.GetCurrentClassLogger();
        private IServerNodeListener mListener = null;
        private ServerNodeCfg mServerNodeCfg = new ServerNodeCfg();
        private int mServerNodeId = 0;
        private EntityMgr mEntityMgr = null;
        private List<PhotonServerPeerC> mListPeerBefore = new List<PhotonServerPeerC>();
        private List<PhotonServerPeerS> mListPeerAfter = new List<PhotonServerPeerS>();

        //---------------------------------------------------------------------
        public ServerNode(IServerNodeListener listener, int servernode_id, EntityMgr entity_mgr)
            : base()
        {
            mListener = listener;
            mServerNodeId = servernode_id;
            mEntityMgr = entity_mgr;
            mServerNodeCfg.load("..\\Config\\ServerNode.xml");

            addState(new ServerNodeStateStart(this));
            addState(new ServerNodeStateRun(this));
            addState(new ServerNodeStateStop(this));

            setupFsm();
        }

        //---------------------------------------------------------------------
        ~ServerNode()
        {
            destroyFsm();

            this.Dispose(false);
        }

        //---------------------------------------------------------------------
        // 前置，该ServerNode依赖的ServerNode，如该ServerNode是GameServer，GameServer依赖前置DbServer
        public List<PhotonServerPeerC> ListPeerBefore
        {
            get { return this.mListPeerBefore; }
        }

        //---------------------------------------------------------------------
        // 后置，其他ServerNode依赖该ServerNode
        public List<PhotonServerPeerS> ListPeerAfter
        {
            get { return this.mListPeerAfter; }
        }

        //---------------------------------------------------------------------
        public EntityMgr EntityMgr
        {
            get { return this.mEntityMgr; }
        }

        //---------------------------------------------------------------------
        public IServerNodeListener Listener
        {
            get { return this.mListener; }
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

        //-----------------------------------------------------------------------------
        public void update()
        {
            processEvent("update");
        }

        //-----------------------------------------------------------------------------
        public ServerNodeCfg getServerNodeCfg()
        {
            return mServerNodeCfg;
        }

        //-----------------------------------------------------------------------------
        public int getServerNodeId()
        {
            return mServerNodeId;
        }

        //-----------------------------------------------------------------------------
        public _tServerNode getServerNode()
        {
            return mServerNodeCfg.getServerNode(mServerNodeId);
        }

        //-----------------------------------------------------------------------------
        public PhotonServerPeerC getPeerClient(byte servernode_type)
        {
            //foreach (var i in mListPeerBefore)
            //{
            //    if (i.RemoteServerNodeInfo.servernode_type == servernode_type) return i;
            //}
            return null;
        }

        //-----------------------------------------------------------------------------
        public void close()
        {
            processEvent("close");
        }

        //-----------------------------------------------------------------------------
        public void addStaticPeerAfter(PhotonServerPeerS peer_server)
        {
            //int remote_id = peer_server.getRemoteServerNodeId();
            //if (getServerNode().vec_after.Contains(remote_id))
            //{
            //    mListPeerAfter.Add(peer_server);
            //}
        }

        //-----------------------------------------------------------------------------
        public void onPeerStatusChanged(int servernode_id, StatusCode statusCode)
        {
            processEvent("on_peer_status_changed", servernode_id, statusCode);
        }
    }
}
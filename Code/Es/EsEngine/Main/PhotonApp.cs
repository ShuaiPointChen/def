using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using log4net.Config;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using ExitGames.Diagnostics.Counter;
using ExitGames.Diagnostics.Monitoring;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using Eb;
using Zk;

namespace Es
{
    public struct SessionEvent
    {
        public bool connect;
        public byte node_type_local;
        public byte node_type_remote;
        public RpcSession session;
    }

    public struct ServerNodeZkInfo
    {
        public string ip_port;
        public string servernode_path;
    }

    public abstract class PhotonApp : ApplicationBase, RpcSessionListener
    {
        //---------------------------------------------------------------------
        protected static readonly ILogger mLog = LogManager.GetCurrentClassLogger();
        protected EntityMgr mEntityMgr = null;
        private volatile bool mSignDestroy = false;
        private float mTimeLogicGap = 100.0f;// 毫秒
        private Thread mThreadLogic = null;
        private Stopwatch mStopwatch = new Stopwatch();
        private ConcurrentQueue<SessionEvent> mQueSessionEvent = new ConcurrentQueue<SessionEvent>();
        private int mNodePort = 0;
        private uint mNodeId = 0;
        private string mNodeIdStr = "";
        private byte mNodeType = 0;
        private string mNodeTypeString = "";
        private string mProjectName = "";
        private IZkClient mZkClient;
        private ServerNodeZkInfo mServerNodeZkInfo;
        private string mLocalIpPort = "";

        //---------------------------------------------------------------------
        public int NodePort { get { return mNodePort; } }
        public uint NodeId { get { return mNodeId; } }
        public string NodeIdStr { get { return mNodeIdStr; } }
        public byte NodeType { get { return mNodeType; } }
        public string NodeTypeString { get { return mNodeTypeString; } }
        public string ProjectName { get { return mProjectName; } }
        public string LocalIpPort { get { return mLocalIpPort; } }
        public IZkClient ZkClient { get { return mZkClient; } }
        public EntityMgr EntityMgr { get { return mEntityMgr; } }

        //---------------------------------------------------------------------
        // Interface: RpcSessionListener.onSessionConnect
        public void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session)
        {
            SessionEvent se;
            se.connect = true;
            se.node_type_local = node_type_local;
            se.node_type_remote = node_type_remote;
            se.session = session;
            mQueSessionEvent.Enqueue(se);
        }

        //---------------------------------------------------------------------
        // Interface: RpcSessionListener.onSessionDisconnect
        public void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session)
        {
            SessionEvent se;
            se.connect = false;
            se.node_type_local = node_type_local;
            se.node_type_remote = node_type_remote;
            se.session = session;
            mQueSessionEvent.Enqueue(se);
        }

        //---------------------------------------------------------------------
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            PeerBase peer = new PhotonServerPeerS(initRequest.Protocol,
                initRequest.PhotonPeer, mEntityMgr, (RpcSessionListener)this);

            return peer;
        }

        //---------------------------------------------------------------------
        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            byte remote_node_type = (byte)state;
            ServerPeerBase peer = new PhotonServerPeerC(initResponse.Protocol, initResponse.PhotonPeer,
                mEntityMgr, remote_node_type, (RpcSessionListener)this);
            return peer;
        }

        //---------------------------------------------------------------------
        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            EbLog.Note("PhotonApp.OnServerConnectionFailed() errorCode=" + errorCode + " errorMessage=" + errorMessage);
        }

        //---------------------------------------------------------------------
        protected override void Setup()
        {
            // log4net
            string path = Path.Combine(this.BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            EbLog.NoteCallback = mLog.Info;
            EbLog.WarningCallback = mLog.Warn;
            EbLog.ErrorCallback = mLog.Error;

            Protocol.AllowRawCustomValues = true;

            EbLog.Note("PhotonApp.Setup()");

            // 获取初始化数据
            EntityMgrListener entitymgr_listener;
            string servercfg_filename;
            init(out entitymgr_listener, out servercfg_filename);
            _parseServerCfg(servercfg_filename);

            // 创建ZkClient
            string host_name = Dns.GetHostName();
            IPAddress ip_addr = Dns.Resolve(host_name).AddressList[0];//获得当前IP地址
            string localnode_ipport = ip_addr.ToString() + ":" + NodePort;
            mZkClient = new ZkClient(mServerNodeZkInfo.ip_port);
            mLocalIpPort = localnode_ipport;

            // 创建服务器zk节点.
            string tempNodeStr = "/"+ProjectName;
            if (!mZkClient.sexists(tempNodeStr, false))
            {
                mZkClient.screate(tempNodeStr, "", ZK_CONST.ZOO_DEFAULT_NODE);
            }
            tempNodeStr = tempNodeStr + "/" + NodeTypeString;
            if (!mZkClient.sexists(tempNodeStr, false))
            {
                mZkClient.screate(tempNodeStr, "", ZK_CONST.ZOO_DEFAULT_NODE);
            }
            tempNodeStr = tempNodeStr + "/" + localnode_ipport + ",";

            // 向ZkServer获取NodeId
            string zk_node = mZkClient.screate(tempNodeStr, "", ZK_CONST.ZOO_EPHEMERAL | ZK_CONST.ZOO_SEQUENCE);
            string last_str = zk_node.Substring(zk_node.LastIndexOf('/') + 1);
            char[] char_separators = new char[] { ',', ':' };
            string[] list_str = last_str.Split(char_separators);
            mNodeId = uint.Parse(list_str[2]);
            mNodeIdStr = list_str[2];
            EbLog.Note("NodeId=" + mNodeId);

            // 创建EntityMgr
            mEntityMgr = new EntityMgr();
            regComponentFactory(mEntityMgr);
            mEntityMgr.create(NodeType, NodeTypeString, (ushort)NodeId, entitymgr_listener);

            onInit(mEntityMgr);

            mSignDestroy = false;
            mThreadLogic = new Thread(new ThreadStart(_threadLogic));
            mThreadLogic.Name = "ThreadLogic";
            mThreadLogic.IsBackground = true;
            mThreadLogic.Start();
            mStopwatch.Reset();
        }

        //---------------------------------------------------------------------
        protected override void TearDown()
        {
            try
            {
                mSignDestroy = true;
                if (mThreadLogic != null)
                {
                    mThreadLogic.Join();
                    mThreadLogic = null;
                }

                mLog.Info("------------------------------------------------");
                mLog.Info("服务端开始停止!");
                mLog.Info("------------------------------------------------");

                release();

                if (mEntityMgr != null)
                {
                    mEntityMgr.Dispose();
                    mEntityMgr = null;
                }

                EbLog.Note("PhotonApp.TearDown()");
            }
            catch (Exception ec)
            {
                EbLog.Error("Exception: PhotonApp.TearDown() : " + ec);
            }
        }

        //---------------------------------------------------------------------
        protected abstract void regComponentFactory(EntityMgr entity_mgr);

        //---------------------------------------------------------------------
        protected abstract void init(out EntityMgrListener entitymgr_listener, out string servercfg_filename);

        //---------------------------------------------------------------------
        protected abstract void onInit(EntityMgr entity_mgr);

        //---------------------------------------------------------------------
        protected abstract void release();

        //---------------------------------------------------------------------
        public abstract void onSessionEvent(ref SessionEvent se);

        //---------------------------------------------------------------------
        void _threadLogic()
        {
            float elapsed_tm = 0;

            while (!mSignDestroy)
            {
                mStopwatch.Restart();

                //更新zk
                ZkClient.update();

                // session连接，断开通知
                SessionEvent se;
                while (!mQueSessionEvent.IsEmpty)
                {
                    if (mQueSessionEvent.TryDequeue(out se))
                    {
                        onSessionEvent(ref se);
                    }
                }

                // 每帧更新
                _update(elapsed_tm);

                mStopwatch.Stop();
                float watch_time = mStopwatch.ElapsedMilliseconds;
                if (watch_time > mTimeLogicGap)
                {
                    elapsed_tm = watch_time / 1000.0f;
                }
                else
                {
                    Thread.Sleep((int)(mTimeLogicGap - watch_time));
                    elapsed_tm = mTimeLogicGap / 1000.0f;
                }
            }
        }

        //---------------------------------------------------------------------
        void _update(float elapsed_tm)
        {
            try
            {
                if (mEntityMgr != null)
                {
                    mEntityMgr.update(elapsed_tm);
                }
            }
            catch (Exception ec)
            {
                EbLog.Error("Exception: PhotonApp._update() : " + ec);
            }
        }

        //---------------------------------------------------------------------
        void _parseServerCfg(string file_name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            XmlNode node_zkinfo = doc.SelectSingleNode("Server/ZkInfo");
            mServerNodeZkInfo.ip_port = node_zkinfo.Attributes["IpPort"].Value;
            mServerNodeZkInfo.servernode_path = node_zkinfo.Attributes["ServerNodePath"].Value;

            XmlNode node_serverinfo = doc.SelectSingleNode("Server/ServerInfo");
            mProjectName = node_serverinfo.Attributes["ProjectName"].Value;
            mNodeType = byte.Parse(node_serverinfo.Attributes["NodeType"].Value);
            mNodeTypeString = node_serverinfo.Attributes["NodeTypeString"].Value;
            mNodePort = int.Parse(node_serverinfo.Attributes["NodePort"].Value);
        }
    }
}

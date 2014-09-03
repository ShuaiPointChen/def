using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Eb;

namespace Ec
{
    public struct SessionEvent
    {
        public bool connect;
        public byte node_type_local;
        public byte node_type_remote;
        public RpcSession session;
    }

    public class PhotonClientPeer : PhotonPeer, IPhotonPeerListener, IRpcPeer
    {
        //---------------------------------------------------------------------
        private EntityMgr mEntityMgr = null;
        private RpcSession mRpcSession = null;
        private RpcSessionListener mListener = null;
        private byte mRemoteNodeType = 0;
        private object mQueSessionEventLock = new object();
        private Queue<SessionEvent> mQueSessionEvent = new Queue<SessionEvent>();

        //---------------------------------------------------------------------
        public PhotonClientPeer(EntityMgr entity_mgr, byte remote_node_type, RpcSessionListener listener)
            : base(ConnectionProtocol.Tcp)
        {
            Listener = this;
            mEntityMgr = entity_mgr;
            mRpcSession = new RpcSession(mEntityMgr, this);
            mListener = listener;
            mRemoteNodeType = remote_node_type;

            DebugOut = DebugLevel.ERROR;
            //WarningSize = 500;
            //CommandBufferSize = 500;
            //TrafficStatsEnabled = true;
            //this.IsSimulationEnabled = true;
            //NetworkSimulationSet NetworkSimulationSettings
        }

        //---------------------------------------------------------------------
        // Interface: IPhotonPeerListener.DebugReturn
        public void DebugReturn(DebugLevel level, string message)
        {
            if (level == DebugLevel.INFO) EbLog.Note(message);
            else if (level == DebugLevel.WARNING) EbLog.Warning(message);
            else if (level == DebugLevel.ERROR) EbLog.Error(message);
        }

        //---------------------------------------------------------------------
        // Interface: IPhotonPeerListener.OnEvent
        public void OnEvent(ExitGames.Client.Photon.EventData eventData)
        {
        }

        //---------------------------------------------------------------------
        // Interface: IPhotonPeerListener.OnOperationResponse
        public void OnOperationResponse(ExitGames.Client.Photon.OperationResponse operationResponse)
        {
            if (mRpcSession != null)
            {
                Dictionary<byte, object> p = operationResponse.Parameters;
                _tEntityRpcData data;
                data.rpc_id = (long)p[0];
                data.from_node = (byte)p[1];
                data.method_id = (short)p[2];
                data.map_param = (Dictionary<byte, object>)p[3];
                data.session_recv = null;

                mRpcSession.recvRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        // Interface: IPhotonPeerListener.OnStatusChanged
        public void OnStatusChanged(StatusCode statusCode)
        {
            //EbLog.Note("PhotonClientPeer.OnStatusChanged() StatusCode=" +
            //    statusCode.ToString() + " RemoteNodeType=" + mRemoteNodeType);

            if (statusCode == StatusCode.Connect)
            {
                Dictionary<byte, object> map_param = new Dictionary<byte, object>();
                map_param[0] = mEntityMgr.NodeType;
                map_param[1] = 0;// local_node_id
                OpCustom((byte)_eRpcCmd.SyncPeerInfo, map_param, true);

                SessionEvent se;
                se.connect = true;
                se.node_type_local = mEntityMgr.NodeType;
                se.node_type_remote = mRemoteNodeType;
                se.session = mRpcSession;
                lock (mQueSessionEventLock)
                {
                    mQueSessionEvent.Enqueue(se);
                }
            }
            else if (statusCode == StatusCode.Disconnect)
            {
                SessionEvent se;
                se.connect = false;
                se.node_type_local = mEntityMgr.NodeType;
                se.node_type_remote = mRemoteNodeType;
                se.session = mRpcSession;
                lock (mQueSessionEventLock)
                {
                    mQueSessionEvent.Enqueue(se);
                }
            }
            else if (statusCode == StatusCode.ExceptionOnConnect)
            {
                //bool is_client = true;
                //List<object> vec_param = new List<object>();
                //vec_param.Add(is_client);
                //vec_param.Add(this);
                //mEntityMgr.postMsg((int)_eEtMsg.PeerDisonnect, vec_param);
            }
            else if (statusCode == StatusCode.TimeoutDisconnect)
            {
            }
            else if (statusCode == StatusCode.DisconnectByServer)
            {
            }
            else if (statusCode == StatusCode.DisconnectByServerUserLimit)
            {
            }
            else if (statusCode == StatusCode.DisconnectByServerLogic)
            {
            }
            else if (statusCode == StatusCode.SendError)
            {
                //CUiMgr ui_mgr = CClientApp.get().getUiMgr();
                //ui_mgr.createMessageBox();
                //ui_mgr.getMessageBox().showMessageBox("发送网络包出现错误", 
                //false, "ok", 2, (int)_eMessageBoxLayer.Disconnect);
            }
        }

        //---------------------------------------------------------------------
        public void update()
        {
            Service();

            lock (mQueSessionEventLock)
            {
                if (mQueSessionEvent.Count == 0) return;
                SessionEvent se = mQueSessionEvent.Dequeue();

                if (mListener == null) return;

                if (se.connect)
                {
                    mListener.onSessionConnect(se.node_type_local, se.node_type_remote, se.session);
                }
                else
                {
                    mListener.onSessionDisconnect(se.node_type_local, se.node_type_remote, se.session);
                }
            }
        }

        //---------------------------------------------------------------------
        // Interface: IRpcPeer.getRpcSession
        public RpcSession getRpcSession()
        {
            return mRpcSession;
        }

        //---------------------------------------------------------------------
        // Interface: IRpcPeer.sendEntityRpcData
        public void sendEntityRpcData(ref _tEntityRpcData data)
        {
            Dictionary<byte, object> p = new Dictionary<byte, object>();
            p[0] = data.rpc_id;
            p[1] = data.from_node;
            p[2] = data.method_id;
            p[3] = data.map_param;

            if (PeerState != PeerStateValue.Connected)
            {
                // log error
                return;
            }

            try
            {
                bool send_queue_ok = OpCustom(data.getCmdId(), p, true, 0);
                if (!send_queue_ok)
                {
                    EbLog.Error("PhotonClientPeer.sendEntityRpcData() Failed! MethodId=" + data.method_id);
                }
            }
            catch (Exception ec)
            {
                EbLog.Error(ec.ToString());
            }
        }

        //---------------------------------------------------------------------
        public byte getRemoteNodeType()
        {
            return mRemoteNodeType;
        }
    }
}
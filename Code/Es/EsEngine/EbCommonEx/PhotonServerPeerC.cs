using System;
using System.Collections.Generic;
using System.Text;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using Eb;

namespace Es
{
    public class PhotonServerPeerC : ServerPeerBase, IRpcPeer
    {
        //---------------------------------------------------------------------
        private EntityMgr mEntityMgr;
        private RpcSession mRpcSession;
        private RpcSessionListener mListener;
        private byte mRemoteNodeType;

        //---------------------------------------------------------------------
        public PhotonServerPeerC(IRpcProtocol protocol, IPhotonPeer unmanaged_peer,
            EntityMgr entity_mgr, byte remote_node_type, RpcSessionListener listener)
            : base(protocol, unmanaged_peer)
        {
            mEntityMgr = entity_mgr;
            mRpcSession = new RpcSession(mEntityMgr, this);
            mListener = listener;
            mRemoteNodeType = remote_node_type;

            Dictionary<byte, object> map_param = new Dictionary<byte, object>();
            map_param[0] = mEntityMgr.NodeType;
            map_param[1] = 0;// local_node_id
            OperationRequest operation_request = new OperationRequest((byte)_eRpcCmd.SyncPeerInfo, map_param);
            SendResult r = SendOperationRequest(operation_request, new SendParameters { ChannelId = 0 });
            if (r != SendResult.Ok)
            {
                EbLog.Error("PhotonServerPeerC SyncPeerInfo Result=" + r);
            }

            if (mListener != null)
            {
                mListener.onSessionConnect(mEntityMgr.NodeType, mRemoteNodeType, mRpcSession);
            }
        }

        //---------------------------------------------------------------------
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (mListener != null)
            {
                mListener.onSessionDisconnect(mEntityMgr.NodeType, mRemoteNodeType, mRpcSession);
            }
        }

        //---------------------------------------------------------------------
        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
        }

        //---------------------------------------------------------------------
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (mRpcSession != null)
            {
                Dictionary<byte, object> p = operationRequest.Parameters;
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
        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
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

            try
            {
                OperationRequest operation_request = new OperationRequest(data.getCmdId(), p);
                SendResult r = SendOperationRequest(operation_request, new SendParameters { ChannelId = 0 });
                if (r != SendResult.Ok)
                {
                    EbLog.Error("PhotonServerPeerC.sendEntityRpcData() Result=" + r + " MethodId=" + data.method_id);
                }
            }
            catch (Exception ec)
            {
                EbLog.Error(ec.ToString());
            }
        }
    }
}

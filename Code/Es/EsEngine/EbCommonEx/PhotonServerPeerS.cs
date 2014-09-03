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
    public class PhotonServerPeerS : PeerBase, IRpcPeer
    {
        //---------------------------------------------------------------------
        private EntityMgr mEntityMgr = null;
        private RpcSession mRpcSession = null;
        private RpcSessionListener mListener = null;
        private byte mRemoteNodeType = 0;

        //---------------------------------------------------------------------
        public PhotonServerPeerS(IRpcProtocol rpc_protocol, IPhotonPeer native_peer,
            EntityMgr entity_mgr, RpcSessionListener listener)
            : base(rpc_protocol, native_peer)
        {
            mEntityMgr = entity_mgr;
            mRpcSession = new RpcSession(mEntityMgr, this);
            mListener = listener;
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
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            _eRpcCmd op_code = (_eRpcCmd)operationRequest.OperationCode;

            if (_eRpcCmd.SyncPeerInfo == op_code)
            {
                mRemoteNodeType = (byte)operationRequest.Parameters[0];

                if (mListener != null)
                {
                    mListener.onSessionConnect(mEntityMgr.NodeType, mRemoteNodeType, mRpcSession);
                }
            }
            else if (mRpcSession != null)
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

            if (!Connected)
            {
                // log error
                return;
            }

            try
            {
                OperationResponse operation_response = new OperationResponse(data.getCmdId(), p);
                SendResult r = SendOperationResponse(operation_response, new SendParameters { ChannelId = 0 });
                if (r != SendResult.Ok)
                {
                    EbLog.Error("PhotonServerPeerS.sendEntityRpcData() Result=" + r + " MethodId=" + data.method_id);
                }
            }
            catch (Exception ec)
            {
                EbLog.Error(ec.ToString());
            }
        }

        //---------------------------------------------------------------------
        public byte getLocalNodeType()
        {
            return mEntityMgr.NodeType;
        }

        //---------------------------------------------------------------------
        public byte getRemoteNodeType()
        {
            return mRemoteNodeType;
        }
    }
}
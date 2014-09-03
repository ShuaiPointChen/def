using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public interface IRpcPeer
    {
        //---------------------------------------------------------------------
        RpcSession getRpcSession();

        //---------------------------------------------------------------------
        void sendEntityRpcData(ref _tEntityRpcData data);
    }

    public interface RpcSessionListener
    {
        //---------------------------------------------------------------------
        void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session);

        //---------------------------------------------------------------------
        void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session);
    }

    public class RpcSession
    {
        //---------------------------------------------------------------------
        EntityMgr mEntityMgr = null;
        Entity mEntity = null;
        IRpcPeer mRpcPeer = null;

        //---------------------------------------------------------------------
        public RpcSession(EntityMgr entity_mgr, IRpcPeer rpc_peer)
        {
            mEntityMgr = entity_mgr;
            mRpcPeer = rpc_peer;
        }

        //---------------------------------------------------------------------
        public void sendEntityRpcData(ref _tEntityRpcData data)
        {
            if (mRpcPeer != null)
            {
                mRpcPeer.sendEntityRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        public void recvRpcData(ref _tEntityRpcData data)
        {
            if (mEntityMgr != null)
            {
                data.session_recv = this;
                mEntityMgr.onRecvRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        public void setProxyEntity(Entity proxy_entity)
        {
            mEntity = proxy_entity;
        }

        //---------------------------------------------------------------------
        public Entity getProxyEntity()
        {
            return mEntity;
        }

        //---------------------------------------------------------------------
        public IRpcPeer getRpcPeer()
        {
            return mRpcPeer;
        }
    }
}
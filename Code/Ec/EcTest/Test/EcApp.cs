using System;
using System.Collections.Generic;
using System.Text;
using Eb;
using Ec;
using ExitGames.Client.Photon;

public class EcApp<T> : Component<T>, RpcSessionListener where T : DefApp, new()
{
    //---------------------------------------------------------------------
    PhotonClientPeer mPeer = null;
    bool mDoRpc1 = false;

    //---------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("EcApp.init() entity_rpcid=" + Entity.getEntityRpcId());

        defRpcMethod((byte)_eNodeType.Es, (ushort)_eMethodType.es2ecOnLogin, es2ecOnLogin);

        mPeer = new PhotonClientPeer(EntityMgr, 1, (RpcSessionListener)this);
        Entity.setSession((byte)_eNodeType.Es, mPeer.getRpcSession());

        mPeer.Connect("127.0.0.1:5880", "EsTest");
    }

    //---------------------------------------------------------------------
    public override void release()
    {
        mPeer.Disconnect();

        EbLog.Note("EcApp.release()");
    }

    //---------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        if (mPeer != null)
        {
            mPeer.Service();
        }

        if (!mDoRpc1 && mPeer.PeerState == PeerStateValue.Connected)
        {
            mDoRpc1 = true;

            rpcOne((byte)_eNodeType.Es, (ushort)_eMethodType.es2ecOnLogin, null);

            EntityMgr.rpc(Entity.getSession((byte)_eNodeType.Es), (byte)_eNodeType.Es, (ushort)_eMethodType.nodeEc2EsLogin, null);
        }
    }

    //---------------------------------------------------------------------
    // Interface: RpcSessionListener.onSessionConnect
    public void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("EcApp.onSessionConnect() node_type_local=" + node_type_local
            + " node_type_remote=" + node_type_remote);
    }

    //---------------------------------------------------------------------
    // Interface: RpcSessionListener.onSessionDisconnect
    public void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("EcApp.onSessionDisconnect() node_type_local=" + node_type_local
            + " node_type_remote=" + node_type_remote);
    }

    //---------------------------------------------------------------------
    //[EntityRpcMethod(_eEtNode.Anonymous)]，定义NodeType，与Rpc委托比较校验函数参数
    public void es2ecOnLogin(RpcSession s, Dictionary<byte, object> map_param)
    {
        EbLog.Note("EsApp.es2ecOnLogin()");
    }
}
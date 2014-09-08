using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class ClientUCenter<T> : Component<T>, RpcSessionListener where T : ComponentDef, new()
{
    //-------------------------------------------------------------------------
    Ec.PhotonClientPeer mPeer;

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientUCenter.init()");

        mPeer = new Ec.PhotonClientPeer(EntityMgr, (byte)255, (RpcSessionListener)this);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        if (mPeer != null)
        {
            mPeer.Disconnect();
            mPeer = null;
        }

        EbLog.Note("ClientUCenter.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        mPeer.Service();
    }

    //-------------------------------------------------------------------------
    public override void onChildInit(Entity child)
    {
    }

    //-------------------------------------------------------------------------
    public override void onRpcPropSync(RpcSession session, byte from_node, ushort reason)
    {
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //---------------------------------------------------------------------
    public void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("ClientUCenter.onSessionConnect()");

        // 发送login请求
        //ushort methdo_id = 2;
        //Dictionary<byte, object> map_param = new Dictionary<byte, object>();
        //map_param[0] = "acc";
        //map_param[1] = "pwd";
        //map_param[2] = "app_name";
        //map_param[3] = "app_channel";
        //EntityMgr.rpc(session, (byte)255, (ushort)methdo_id, map_param);
    }

    //---------------------------------------------------------------------
    public void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("ClientUCenter.onSessionDisconnect()");
    }

    //---------------------------------------------------------------------
    public void login(string ip, int port, string acc, string pwd)
    {
        if (mPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            return;
        }

        string ipport = ip + ":" + port;
        EbLog.Note("ClientUCenter.login() " + ipport);
        mPeer.Connect(ipport, "EsUCenter");
    }
}

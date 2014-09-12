using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class ClientUCenter<T> : Component<T>, RpcSessionListener where T : ComponentDef, new()
{
    //-------------------------------------------------------------------------
    public delegate void login2ClientOnLoginHandler(string result, string token, Dictionary<byte, object> map_param);
    Ec.PhotonClientPeer mPeer;
    login2ClientOnLoginHandler mlogin2ClientHandler = null;
    string mAcc = "";
    string mPwd = "";
    string mProjectName = "";
    string mChannelName = "";

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientUCenter.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        mPeer = new Ec.PhotonClientPeer(EntityMgr, (byte)_eUCenterNodeType.UCenter, (RpcSessionListener)this);
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
        if (e is EvEntityCreateRemote)
        {
            EvEntityCreateRemote ev = (EvEntityCreateRemote)e;
            if (ev.entity != null && ev.entity_data.entity_type == _eUCenterEtType.EtUCenterSession.ToString())
            {
                var co_ucentersession = ev.entity.getComponent<ClientUCenterSession<ComponentDef>>();
                co_ucentersession.setCoUCenter(this);
                co_ucentersession.login(mAcc, mPwd, mProjectName, mChannelName);
            }
        }
    }

    //-------------------------------------------------------------------------
    public void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("ClientUCenter.onSessionConnect()");
    }

    //-------------------------------------------------------------------------
    public void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("ClientUCenter.onSessionDisconnect()");
    }

    //-------------------------------------------------------------------------
    public void login(string ip, int port, string acc, string pwd, string project_name, string channel_name)
    {
        mAcc = acc;
        mPwd = pwd;
        mProjectName = project_name;
        mChannelName = channel_name;

        if (mPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            return;
        }

        string ipport = ip + ":" + port;
        EbLog.Note("ClientUCenter.login() " + ipport);

        mPeer.Connect(ipport, "EsUCenter");
    }

    //-------------------------------------------------------------------------
    public void setonLoginHnadler(login2ClientOnLoginHandler handler)
    {
        mlogin2ClientHandler = handler;
    }

    //-------------------------------------------------------------------------
    internal void _onLogin(string result, string token, Dictionary<byte, object> map_param)
    {
        if (mlogin2ClientHandler != null)
        {
            mlogin2ClientHandler(result, token, map_param);
        }
        else
        {
            EbLog.Error("ClientUCenter._onLogin() mlogin2ClientHandler==null");
        }
    }
}

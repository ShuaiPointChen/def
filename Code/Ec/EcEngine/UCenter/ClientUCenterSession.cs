using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class ClientUCenterSession<T> : Component<T> where T : ComponentDef, new()
{
    //-------------------------------------------------------------------------
    ClientUCenter<ComponentDef> mCoUCenter;

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientUCenterSession.init()");

        defRpcMethod((byte)_eUCenterNodeType.UCenter, (ushort)_eUCenterMethodType.login2ClientOnLogin, login2ClientOnLogin);

        // 设置session
        RpcSession session = (RpcSession)Entity.getCacheData("RemoteSession");
        Entity.setSession((byte)_eUCenterNodeType.UCenter, session);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientUCenterSession.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
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

    //-------------------------------------------------------------------------
    // 响应登陆请求
    public void login2ClientOnLogin(RpcSession s, Dictionary<byte, object> map_param)
    {
        EbLog.Note("ClientUCenterSession.login2ClientOnLogin()");

        string result = (string)map_param[0];
        string token = (string)map_param[1];
        byte count = (byte)map_param[2];
        Dictionary<byte, object> param = new Dictionary<byte, object>();
        for (byte idx = 0; idx < count; idx++)
        {
            param[idx] = map_param[(byte)(3 + idx)];
        }

        mCoUCenter._onLogin(result, token, param);
    }

    //-------------------------------------------------------------------------
    public void login(string acc, string pwd, string project_name, string channel_name)
    {
        // 发送登陆请求
        Dictionary<byte, object> map_param = new Dictionary<byte, object>();
        map_param[0] = acc;
        map_param[1] = pwd;
        map_param[2] = project_name;
        map_param[3] = channel_name;
        //map_param[0] = "test1001";
        //map_param[1] = "1";
        //map_param[2] = "Dragon";
        //map_param[3] = "app_channel";
        rpcOne((byte)_eUCenterNodeType.UCenter, (ushort)_eUCenterMethodType.client2LoginLogin, map_param);
    }

    //-------------------------------------------------------------------------
    internal void setCoUCenter(IComponent co_ucenter)
    {
        mCoUCenter = (ClientUCenter<ComponentDef>)co_ucenter;
    }
}

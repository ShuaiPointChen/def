using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class ClientUCenterSession<T> : Component<T> where T : ComponentDef, new()
{
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientUCenterSession.init()");

        // 设置session
        RpcSession session = (RpcSession)Entity.getCacheData("RemoteSession");
        Entity.setSession(255, session);

        // 发送登陆请求
        Dictionary<byte, object> map_param = new Dictionary<byte, object>();
        map_param[0] = "test1001";
        map_param[1] = "1";
        map_param[2] = "Dragon";
        map_param[3] = "app_channel";
        rpcOne(255, 1000, map_param);
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


    }
}

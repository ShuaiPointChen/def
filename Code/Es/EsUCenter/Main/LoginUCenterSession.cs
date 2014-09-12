using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class LoginUCenterSession<T> : Component<T> where T : DefUCenterSession, new()
{
    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("LoginUCenterSession.init()");

        defRpcMethod((byte)_eUCenterNodeType.Client, (ushort)_eUCenterMethodType.client2LoginLogin, client2LoginLogin);

        // 设置session
        RpcSession session = (RpcSession)Entity.getCacheData("RemoteSession");
        Entity.setSession((byte)_eUCenterNodeType.Client, session);

        // Create Client Remote
        rpcOneCreateRemote((byte)_eUCenterNodeType.Client, false);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("LoginUCenterSession.release()");
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
    public void client2LoginLogin(RpcSession s, Dictionary<byte, object> map_param)
    {
        EbLog.Note("LoginUCenterSession.client2LoginLogin()");

        string account = (string)map_param[0];
        string password = (string)map_param[1];
        string serverGroup = (string)map_param[2];
        string channel = (string)map_param[3];

        LoginApp<ComponentDef> login = EntityMgr.findFirstEntity("EtApp").getComponent<LoginApp<ComponentDef>>();
        login.addLoginPlayer(serverGroup, account, password, channel, this);
    }

    //-------------------------------------------------------------------------
    // 反馈登陆请求
    public void login2ClientLogin(string result, string token, Dictionary<byte, object> map_param)
    {
        EbLog.Note("LoginUCenterSession.client2LoginLogin()");

        Dictionary<byte, object> map_ret = new Dictionary<byte, object>();
        map_ret[0] = result;
        map_ret[1] = token;
        map_ret[2] = (byte)map_param.Count;
        for (byte idx = 0; idx < map_param.Count; idx++)
        {
            map_ret[(byte)(3 + idx)] = map_param[idx];
        }

        rpcOne((byte)_eUCenterNodeType.Client, (ushort)_eUCenterMethodType.login2ClientOnLogin, map_ret);
    }
}

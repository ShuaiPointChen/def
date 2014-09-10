using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class LoginUCenterSession<T> : Component<T> where T : DefUCenterSession, new()
{
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("LoginUCenterSession.init()");

        int client_part = 1;

        // 设置session
        RpcSession session = (RpcSession)Entity.getCacheData("RemoteSession");
        Entity.setSession((byte)client_part, session);

        defRpcMethod((byte)client_part, (ushort)1000, client2LoginLogin);

        // Create Client Remote
        rpcOneCreateRemote((byte)client_part, false);
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

       string account  = (string)map_param[0];
       string password = (string)map_param[1];
       string serverGroup = (string)map_param[2];
       string channel = (string)map_param[3];

      LoginApp<ComponentDef> login = EntityMgr.findFirstEntity("EtApp").getComponent<LoginApp<ComponentDef>>();
      login.addLoginPlayer(serverGroup, account, password, channel ,  s);

    }
}

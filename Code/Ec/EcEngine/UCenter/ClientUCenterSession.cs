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

        defRpcMethod((byte)255, (ushort)1001, login2ClientOnLogin);

        // 设置session
        RpcSession session = (RpcSession)Entity.getCacheData("RemoteSession");
        Entity.setSession(255, session);

        Entity et_app=EntityMgr.findFirstEntity("EtApp");
        mCoUCenter = et_app.getComponent<ClientUCenter<ComponentDef>>();

        if (mCoUCenter == null)
        {
            EbLog.Note("mCoUCenter == null");
        }

        //mCoUCenter = Entity.getComponent<ClientUCenter<ComponentDef>>();
        //if (mCoUCenter == null)
        //{
        //    mCoUCenter = Entity.getComponent<ClientUCenter<ComponentDef>>();
        //}

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
        string result = (string)map_param[0];
        string token = (string)map_param[1];
        byte count = (byte)map_param[2];

        Dictionary<byte, object> param = new  Dictionary<byte, object>();

        for (byte idx = 0; idx < count; idx++)
        {
            param[idx] = map_param[(byte)(3 + idx)];
        }
        
        mCoUCenter._onLogin(result, token, param);
    }
}

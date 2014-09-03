using System;
using System.Collections.Generic;
using System.Text;
using Eb;
using Es;

public class EsApp<T> : Component<T>, RpcSessionListener where T : DefApp, new()
{
    //---------------------------------------------------------------------
    bool mDoStep1 = false;

    //---------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("EsApp.init() entity_rpcid=" + Entity.getEntityRpcId());

        defRpcMethod((byte)_eNodeType.Ec, (ushort)_eMethodType.ec2esLogin, ec2esLogin);

        //EntityMgr.defRpcMethod((byte)_eNodeType.Ec, (ushort)_eMethodType.nodeEc2EsLogin, nodeEc2EsLogin);

        EnableUpdate = true;
        Def.mPropNickName.set("test_nickname");
    }

    //---------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("EsApp.release()");
    }

    //---------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        if (!mDoStep1)
        {
            mDoStep1 = true;
            //EbLog.Note("EsApp.update() DoStep1");
        }
    }

    //---------------------------------------------------------------------
    // Interface: RpcSessionListener.onSessionConnect
    public void onSessionConnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("EsApp.onSessionConnect() node_type_local=" + node_type_local
            + " node_type_remote=" + node_type_remote);
    }

    //---------------------------------------------------------------------
    // Interface: RpcSessionListener.onSessionDisconnect
    public void onSessionDisconnect(byte node_type_local, byte node_type_remote, RpcSession session)
    {
        EbLog.Note("EsApp.onSessionDisconnect() node_type_local=" + node_type_local
            + " node_type_remote=" + node_type_remote);
    }

    //---------------------------------------------------------------------
    //[EntityRpcMethod(_eEtNode.Anonymous)]，定义NodeType，与Rpc委托比较校验函数参数
    public void ec2esLogin(RpcSession s, Dictionary<byte, object> map_param)
    {
        EbLog.Note("EsApp.ec2esLogin()");
    }

    //---------------------------------------------------------------------
    public void nodeEc2EsLogin(RpcSession s, Dictionary<byte, object> map_param)
    {
        EbLog.Note("EsApp.nodeEc2EsLogin()");
    }
}
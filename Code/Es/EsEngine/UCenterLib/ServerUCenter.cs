using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class ServerUCenter<T> : Component<T> where T : ComponentDef, new()
{
    //---------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ServerUCenter.init()");
    }

    //---------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ServerUCenter.release()");
    }

    //---------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
    }

    //---------------------------------------------------------------------
    public override void onChildInit(Entity child)
    {
    }

    //---------------------------------------------------------------------
    public override void onRpcPropSync(RpcSession session, byte from_node, ushort reason)
    {
    }

    //---------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }
}

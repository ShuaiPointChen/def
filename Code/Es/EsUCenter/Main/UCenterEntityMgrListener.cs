using System;
using System.Collections.Generic;
using Eb;
using Es;

public class UCenterEntityMgrListener : EntityMgrListener
{
    //-------------------------------------------------------------------------
    EntityMgr mEntityMgr;

    //-------------------------------------------------------------------------
    public UCenterEntityMgrListener()
    {
        PhotonApp photon_app = (PhotonApp)PhotonApp.Instance;
        mEntityMgr = photon_app.EntityMgr;
    }

    //-------------------------------------------------------------------------
    public void onRpcNodeMethod(RpcSession session, byte from_node, ushort method_id, Dictionary<byte, object> map_param)
    {
    }

    //-------------------------------------------------------------------------
    public void onRpcEntityCreateRemote(RpcSession session, EntityData entity_data, bool from_db)
    {
    }
}

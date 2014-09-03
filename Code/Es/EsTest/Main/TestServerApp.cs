using System;
using System.Collections.Generic;
using System.Text;
using Eb;
using Es;
using Zk;

public class TestServerApp : PhotonApp
{
    //---------------------------------------------------------------------
    protected override void regComponentFactory(EntityMgr entity_mgr)
    {
        entity_mgr.regComponentFactory(new ComponentFactory<EsApp<DefApp>>(entity_mgr));
    }

    //---------------------------------------------------------------------
    protected override void init(out IZkOnOpeResult zk_listener,
        out EntityMgrListener entitymgr_listener, out string servercfg_filename)
    {
        zk_listener = null;
        entitymgr_listener = null;
        servercfg_filename = "";
    }

    //---------------------------------------------------------------------
    protected override void onInit(EntityMgr entity_mgr)
    {
        Entity et = entity_mgr.createEmptyEntity("EtApp", null);
        et.addComponent<EsApp<DefApp>>();
    }

    //---------------------------------------------------------------------
    protected override void release()
    {
    }

    //---------------------------------------------------------------------
    public override void onSessionEvent(ref SessionEvent se)
    {
    }
}
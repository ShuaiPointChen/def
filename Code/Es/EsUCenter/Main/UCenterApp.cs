using System;
using System.Collections.Generic;
using System.IO;
using Photon.SocketServer;
using Eb;
using Es;
using Zk;

public class EvMainSessionEvent : EntityEvent
{
    public EvMainSessionEvent() : base() { }
    public SessionEvent Se;
}

public class UCenterApp : PhotonApp
{
    //-------------------------------------------------------------------------
    protected override void regComponentFactory(EntityMgr entity_mgr)
    {
        entity_mgr.regComponentFactory(new ComponentFactory<LoginApp<ComponentDef>>(entity_mgr));
        entity_mgr.regComponentFactory(new ComponentFactory<LoginNode<ComponentDef>>(entity_mgr));
        entity_mgr.regComponentFactory(new ComponentFactory<LoginUCenterSession<DefUCenterSession>>(entity_mgr));
    }

    //-------------------------------------------------------------------------
    protected override void init(out EntityMgrListener entitymgr_listener, out string servercfg_filename)
    {
        entitymgr_listener = new UCenterEntityMgrListener();

        string path = Path.Combine(this.BinaryPath, "../../../Media/EsUCenter/Config/EsUCenter.xml");
        servercfg_filename = Path.GetFullPath(path);
    }

    //-------------------------------------------------------------------------
    protected override void onInit(EntityMgr entity_mgr)
    {
        Entity et_app = entity_mgr.createEmptyEntity("EtApp", null);
        et_app.addComponent<LoginApp<ComponentDef>>();
    }

    //-------------------------------------------------------------------------
    protected override void release()
    {
    }

    //-------------------------------------------------------------------------
    public override void onSessionEvent(ref SessionEvent se)
    {
        var ev = mEntityMgr.getDefaultEventPublisher().genEvent<EvMainSessionEvent>();
        ev.Se = se;
        ev.send(null);
    }
}

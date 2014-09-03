using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    class TestEntity : Test
    {
        //---------------------------------------------------------------------
        EntityMgr mEntityMgr = null;

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("TestEntity.init()");

            if (mEntityMgr == null)
            {
                mEntityMgr = new EntityMgr();
                mEntityMgr.create(1, "", 1, null);

                mEntityMgr.regComponentFactory(new ComponentFactory<CellPlayer<DefPlayer>>(mEntityMgr));
            }

            Entity et = mEntityMgr.createEmptyEntity("EtPlayer", null);
            et.addComponent<CellPlayer<DefPlayer>>();
            mEntityMgr.asyncSaveEntity(new EntitySaverJson(mEntityMgr, "EtPlayer.json", et, true));
            
            //mEntityMgr.asyncLoadEntity(new EntityLoaderJson(mEntityMgr, "EtPlayer.json"));
        }

        //---------------------------------------------------------------------
        public override void release()
        {
            if (mEntityMgr != null)
            {
                mEntityMgr.destroy();
                mEntityMgr = null;
            }

            EbLog.Note("TestEntity.release()");
        }

        //---------------------------------------------------------------------
        public override void update()
        {
            if (mEntityMgr != null)
            {
                mEntityMgr.update(0.0f);
            }
        }
    }
}

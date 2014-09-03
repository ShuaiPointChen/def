using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    // 场景中的地图网格中的一格
    public class EbGridRegion
    {
        //---------------------------------------------------------------------
        public EntityEventPublisher Publisher { get; private set; }

        //---------------------------------------------------------------------
        public EbGridRegion(EntityMgr entity_mgr)
        {
            Publisher = new EntityEventPublisher(entity_mgr);
        }

        //---------------------------------------------------------------------
        public void entityEnterRegion(Entity entity)
        {
            entity.setUserData<EbGridRegion>(this);

            // 发送有新Entity进入Region的消息
            Publisher.genEvent<EvSceneEntityEnterRegion>().send(entity);

            Publisher.addHandler(entity);
        }

        //---------------------------------------------------------------------
        public void entityMove(Entity entity)
        {
            // 发送有新Entity在Region移动的消息
            Publisher.genEvent<EvSceneEntityMoveOnRegion>().send(entity);
        }

        //---------------------------------------------------------------------
        public void entityLeaveRegion(Entity entity)
        {
            entity.setUserData<EbGridRegion>(null);

            Publisher.removeHandler(entity);

            // 发送有Entity离开Region的消息
            Publisher.genEvent<EvSceneEntityLeaveRegion>().send(entity);
        }
    }
}

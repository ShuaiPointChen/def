using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public abstract class EntityLoader
    {
        //---------------------------------------------------------------------
        protected EntityMgr mEntityMgr;
        protected EntityAsyncStatus mEntityAsyncStatus = new EntityAsyncStatus();
        protected EntityData mEntityData;
        protected Entity mParentEntity;
        protected bool mRecursive = true;

        //---------------------------------------------------------------------
        public EntityLoader(EntityMgr entity_mgr, Entity parent, bool recursive)
        {
            mEntityMgr = entity_mgr;
            mParentEntity = parent;
            mRecursive = recursive;
        }

        //---------------------------------------------------------------------
        public EntityAsyncStatus getEntityAsyncStatus()
        {
            return mEntityAsyncStatus;
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public abstract void handleLoad();

        //---------------------------------------------------------------------
        // 运行在主线程中
        public void onLoadFinished(Entity entity)
        {
            if (entity == null)
            {
                mEntityAsyncStatus.is_done = false;
                mEntityAsyncStatus.is_error = true;
                mEntityAsyncStatus.error_str = "";
                mEntityAsyncStatus.obj = null;
            }
            else
            {
                mEntityAsyncStatus.is_done = true;
                mEntityAsyncStatus.is_error = false;
                mEntityAsyncStatus.error_str = "";
                mEntityAsyncStatus.obj = entity;
            }
        }

        //---------------------------------------------------------------------
        public EntityData getEntityData()
        {
            return mEntityData;
        }

        //---------------------------------------------------------------------
        public Entity getEntityParent()
        {
            return mParentEntity;
        }

        //---------------------------------------------------------------------
        public bool isRecursive()
        {
            return mRecursive;
        }
    }

    public abstract class EntitySaver
    {
        //---------------------------------------------------------------------
        protected EntityMgr mEntityMgr = null;
        protected EntityData mEntityData = null;

        //---------------------------------------------------------------------
        public EntitySaver(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public abstract void handleSave();
    }
}
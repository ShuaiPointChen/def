using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Eb
{
    internal class EntitySerializerMgr
    {
        //---------------------------------------------------------------------
        private EntityMgr mEntityMgr = null;
        private Thread mThreadSerializer = null;
        private volatile bool mSignDestroy = false;
        private Queue<EntityLoader> mQueEntityLoadRequest = new Queue<EntityLoader>();
        private object mLockQueEntityLoadRequest = new object();
        private Queue<EntityLoader> mQueEntityLoadResponse = new Queue<EntityLoader>();
        private object mLockQueEntityLoadResponse = new object();
        private Queue<EntitySaver> mQueEntitySaveRequest = new Queue<EntitySaver>();
        private object mLockQueEntitySaveRequest = new object();

        //---------------------------------------------------------------------
        public void create(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;

            if (mThreadSerializer == null)
            {
                mSignDestroy = false;
                mThreadSerializer = new Thread(new ThreadStart(_threadSerializer));
                mThreadSerializer.Name = "ThreadSerializer";
                mThreadSerializer.IsBackground = true;
                mThreadSerializer.Start();
            }
        }

        //---------------------------------------------------------------------
        public void destroy()
        {
            if (mThreadSerializer != null)
            {
                mSignDestroy = true;
                mThreadSerializer.Join();
                mThreadSerializer = null;
            }
        }

        //---------------------------------------------------------------------
        public void update(float elapsed_tm)
        {
            bool done = false;
            EntityLoader loader = null;
            int count = 0;

            while (!done)
            {
                count++;
                if (count > 20) break;

                // GetEntityData
                lock (mLockQueEntityLoadResponse)
                {
                    if (mQueEntityLoadResponse.Count > 0)
                    {
                        loader = mQueEntityLoadResponse.Dequeue();
                    }
                    else
                    {
                        done = true;
                    }
                }

                // CreateEntity
                if (!done)
                {
                    Entity entity = null;
                    EntityData entity_data = loader.getEntityData();
                    if (entity_data != null)
                    {
                        entity = mEntityMgr._createEntityImpl(entity_data, loader.getEntityParent(), loader.isRecursive());
                    }
                    loader.onLoadFinished(entity);
                }
            }
        }

        //---------------------------------------------------------------------
        public void addLoadRequest(EntityLoader loader)
        {
            lock (mLockQueEntityLoadRequest)
            {
                mQueEntityLoadRequest.Enqueue(loader);
            }
        }

        //---------------------------------------------------------------------
        public void addSaverRequest(EntitySaver saver)
        {
            lock (mLockQueEntitySaveRequest)
            {
                mQueEntitySaveRequest.Enqueue(saver);
            }
        }

        //---------------------------------------------------------------------
        void _threadSerializer()
        {
            while (true)
            {
                // 处理Entity Load
                if (!mSignDestroy)
                {
                    EntityLoader loader = null;
                    lock (mLockQueEntityLoadRequest)
                    {
                        if (mQueEntityLoadRequest.Count > 0)
                        {
                            loader = mQueEntityLoadRequest.Dequeue();
                        }
                    }

                    if (loader != null)
                    {
                        loader.handleLoad();

                        lock (mLockQueEntityLoadResponse)
                        {
                            mQueEntityLoadResponse.Enqueue(loader);
                        }
                    }
                }

                // 处理Entity Save
                {
                    EntitySaver saver = null;

                    lock (mLockQueEntitySaveRequest)
                    {
                        if (mQueEntitySaveRequest.Count > 0)
                        {
                            saver = mQueEntitySaveRequest.Dequeue();
                        }
                    }

                    if (saver != null)
                    {
                        saver.handleSave();
                    }

                    if (mSignDestroy)
                    {
                        lock (mLockQueEntitySaveRequest)
                        {
                            if (mQueEntitySaveRequest.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}

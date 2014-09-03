using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Eb
{
    public class EntityMgr : IDisposable
    {
        //---------------------------------------------------------------------
        byte mNodeType;
        string mNodeTypeString = "";
        ushort mNodeId;
        EntityMgrListener mEntityMgrListener;
        Dictionary<string, IComponentFactory> mMapComponentFactory = new Dictionary<string, IComponentFactory>();
        EntitySerializerMgr mEntitySerializerMgr = new EntitySerializerMgr();
        EbFileStream mEntityFileStream = new EbFileStreamDefault();
        RpcDispatch mRpcDispatch = new RpcDispatch();
        EntityEventMgr mEntityEventMgr;
        EntityEventPublisher mEntityEventPublisherDefault;
        // key1=entity_type key2=entity_rpcid
        Dictionary<string, Dictionary<ulong, Entity>> mMapAllEntity4Search1 = new Dictionary<string, Dictionary<ulong, Entity>>();
        Dictionary<ulong, Entity> mMapAllEntity4Search2 = new Dictionary<ulong, Entity>();
        // key=entity_rpcid
        Dictionary<ulong, Entity> mMapAllEntity4Update = new Dictionary<ulong, Entity>();
        // 每帧创建的Entity队列
        Queue<Entity> mQueCreateEntity = new Queue<Entity>();
        // 每帧标识为销毁状态的Entity队列
        Queue<ulong> mQueSignDestroyEntity = new Queue<ulong>();
        static uint mMaxEntityId = 0;
        static Queue<uint> mQueFreeEntityId = new Queue<uint>();

        //---------------------------------------------------------------------
        public byte NodeType { get { return mNodeType; } }
        public string NodeTypeAsString { get { return mNodeTypeString; } }
        public ushort NodeId { get { return mNodeId; } }

        //---------------------------------------------------------------------
        public EntityMgr()
        {
        }

        //---------------------------------------------------------------------
        ~EntityMgr()
        {
            this.Dispose(false);
        }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        //---------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            destroy();

            if (disposing)
            {
            }
        }

        //---------------------------------------------------------------------
        public void create(byte node_type, string nodetype_string, ushort node_id, EntityMgrListener listener)
        {
            mNodeType = node_type;
            mNodeTypeString = nodetype_string;
            mNodeId = node_id;
            mEntityMgrListener = listener;

            mRpcDispatch.create(this);
            mEntitySerializerMgr.create(this);

            mEntityEventMgr = new EntityEventMgr(this);
            mEntityEventPublisherDefault = new EntityEventPublisher(this);
        }

        //---------------------------------------------------------------------
        public void destroy()
        {
            mEntitySerializerMgr.destroy();
            mRpcDispatch.destroy();
            mEntityEventMgr = null;
            mEntityEventPublisherDefault = null;

            List<Entity> list_top_entity = new List<Entity>();
            foreach (var i in mMapAllEntity4Search2)
            {
                if (i.Value.getParent() == null)
                {
                    list_top_entity.Add(i.Value);
                }
            }
            foreach (var i in list_top_entity)
            {
                destroyEntity(i);
            }

            mMapAllEntity4Search1.Clear();
            mMapAllEntity4Search2.Clear();
            mMapAllEntity4Update.Clear();
            mQueCreateEntity.Clear();
            mQueSignDestroyEntity.Clear();
        }

        //---------------------------------------------------------------------
        public void update(float elapsed_tm)
        {
            // RpcMgr每帧更新
            mRpcDispatch.update(elapsed_tm);

            // SerializerMgr每帧更新
            mEntitySerializerMgr.update(elapsed_tm);

            // 每帧新销毁的Entity从mMapAllEntity4Update中移除
            while (mQueSignDestroyEntity.Count > 0)
            {
                ulong entity_rpcid = mQueSignDestroyEntity.Dequeue();
                mMapAllEntity4Update.Remove(entity_rpcid);
            }

            // 每帧新创建的Entity添加到mMapAllEntity4Update中
            while (mQueCreateEntity.Count > 0)
            {
                Entity entity_create = mQueCreateEntity.Dequeue();
                mMapAllEntity4Update[entity_create.getEntityRpcId()] = entity_create;
            }

            // 每帧更新
            foreach (var i in mMapAllEntity4Update)
            {
                if (i.Value != null)
                {
                    i.Value._update(elapsed_tm);
                }
            }
        }

        //---------------------------------------------------------------------
        public void regComponentFactory(IComponentFactory factory)
        {
            mMapComponentFactory[factory.getName()] = factory;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, IComponentFactory> getMapComponentFactory()
        {
            return mMapComponentFactory;
        }

        //---------------------------------------------------------------------
        public IComponentFactory getComponentFactory(string name)
        {
            if (mMapComponentFactory.ContainsKey(name))
            {
                return mMapComponentFactory[name];
            }
            else
            {
                return null;
            }
        }

        //---------------------------------------------------------------------
        public string getComponentName<T>() where T : IComponent
        {
            Type t = typeof(T);
            return _getComponentName(t);
        }

        //---------------------------------------------------------------------
        public string getComponentName(IComponent component)
        {
            Type t = component.GetType();
            return _getComponentName(t);
        }

        //---------------------------------------------------------------------
        public void setFileStream(EbFileStream file_stream)
        {
            mEntityFileStream = file_stream;
        }

        //---------------------------------------------------------------------
        public void rpc(RpcSession s, byte to_node, ushort method_id, Dictionary<byte, object> map_param)
        {
            uint id = 0;
            byte local_node_type = NodeType;
            ulong entity_rpcid = 0xffffffffffffffff;
            entity_rpcid &= ((ulong)local_node_type) << 32;
            entity_rpcid |= (ulong)id;

            _tEntityRpcData data;
            data.rpc_id = _tEntityRpcData.from((byte)_eRpcCmd.NodeMethod, to_node, entity_rpcid);
            data.from_node = NodeType;
            data.method_id = (short)method_id;
            data.map_param = map_param;
            data.session_recv = null;

            if (s != null)
            {
                // 直接发送
                s.sendEntityRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        // 该函数可以运行在另一个线程
        public void onRecvRpcData(ref _tEntityRpcData data)
        {
            if (mRpcDispatch != null)
            {
                mRpcDispatch.onRecvRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        public Entity createEmptyEntity(string entity_type, Dictionary<string, object> cache_data, Entity parent = null)
        {
            EntityData entity_data = new EntityData();
            entity_data.entity_type = entity_type;
            entity_data.entity_guid = Guid.NewGuid().ToString();
            entity_data.entity_rpcid = 0;
            entity_data.entity_transform = null;
            entity_data.list_component = null;
            entity_data.cache_data = cache_data;

            return _createEntityImpl(entity_data, parent);
        }

        //---------------------------------------------------------------------
        public Entity createEmptyEntity(string entity_type, string entity_guid,
            ulong entity_rpcid, Dictionary<string, object> cache_data, Entity parent = null)
        {
            EntityData entity_data = new EntityData();
            entity_data.entity_type = entity_type;
            entity_data.entity_guid = entity_guid;
            entity_data.entity_rpcid = entity_rpcid;
            entity_data.entity_transform = null;
            entity_data.list_component = null;
            entity_data.cache_data = cache_data;

            return _createEntityImpl(entity_data, parent);
        }

        //---------------------------------------------------------------------
        public Entity createEmptyEntity(EntityData entity_data, Entity parent = null, bool recursive = false)
        {
            return _createEntityImpl(entity_data, parent, recursive);
        }

        //---------------------------------------------------------------------
        public void destroyEntity(Entity entity)
        {
            // 先销毁所有子Entity
            Dictionary<string, Dictionary<ulong, Entity>> map_children = entity.getChildren();
            if (map_children != null)
            {
                foreach (var i in map_children)
                {
                    foreach (var j in i.Value)
                    {
                        destroyEntity(j.Value);
                    }
                }
                map_children.Clear();
            }

            // 通知本Entity逻辑层做点事情
            entity._release();

            string entity_type = entity.getEntityType();
            ulong entity_rpcid = entity.getEntityRpcId();

            // 从查找map中移除
            if (mMapAllEntity4Search1.ContainsKey(entity_type))
            {
                Dictionary<ulong, Entity> m = mMapAllEntity4Search1[entity_type];
                m.Remove(entity_rpcid);
            }

            mMapAllEntity4Search2.Remove(entity_rpcid);

            mQueSignDestroyEntity.Enqueue(entity_rpcid);

            // delete object
            entity.Dispose();
        }

        //---------------------------------------------------------------------
        public void destroyEntity(ulong entity_rpcid)
        {
            Entity et = null;

            if (mMapAllEntity4Search2.ContainsKey(entity_rpcid))
            {
                et = mMapAllEntity4Search2[entity_rpcid];
            }

            if (et == null) return;

            destroyEntity(et);
        }

        //---------------------------------------------------------------------
        public EntityAsyncStatus asyncLoadCreateEntity(EntityLoader loader)
        {
            mEntitySerializerMgr.addLoadRequest(loader);
            return loader.getEntityAsyncStatus();
        }

        //---------------------------------------------------------------------
        public void asyncSaveEntity(EntitySaver saver)
        {
            mEntitySerializerMgr.addSaverRequest(saver);
        }

        //---------------------------------------------------------------------
        public Entity findFirstEntity(string entity_type)
        {
            Entity et = null;

            if (mMapAllEntity4Search1.ContainsKey(entity_type))
            {
                Dictionary<ulong, Entity> m = mMapAllEntity4Search1[entity_type];
                foreach (var i in m)
                {
                    et = i.Value;
                    break;
                }
            }

            return et;
        }

        //---------------------------------------------------------------------
        // 返回的集合中可能包含已标记为销毁的entity，需要手动剔除
        public Dictionary<ulong, Entity> findEntity(string entity_type)
        {
            Dictionary<ulong, Entity> m = null;

            if (mMapAllEntity4Search1.ContainsKey(entity_type))
            {
                m = mMapAllEntity4Search1[entity_type];
            }

            return m;
        }

        //---------------------------------------------------------------------
        public Entity findEntity(string entity_type, ulong entity_rpcid)
        {
            Entity et = null;
            Dictionary<ulong, Entity> m = null;
            if (mMapAllEntity4Search1.TryGetValue(entity_type, out m))
            {
                m.TryGetValue(entity_rpcid, out et);
            }

            return et;
        }

        //---------------------------------------------------------------------
        public Entity findEntity(ulong entity_rpcid)
        {
            Entity et = null;

            if (mMapAllEntity4Search2.ContainsKey(entity_rpcid))
            {
                et = mMapAllEntity4Search2[entity_rpcid];
            }

            return et;
        }

        //---------------------------------------------------------------------
        public EntityEventPublisher genEventPublisher()
        {
            return new EntityEventPublisher(this);
        }

        //---------------------------------------------------------------------
        public EntityEventPublisher getDefaultEventPublisher()
        {
            return mEntityEventPublisherDefault;
        }

        //---------------------------------------------------------------------
        internal T _genEvent<T>() where T : EntityEvent, new()
        {
            return mEntityEventMgr._genEvent<T>();
        }

        //---------------------------------------------------------------------
        internal void _freeEvent(EntityEvent ev)
        {
            mEntityEventMgr._freeEvent(ev);
        }

        //---------------------------------------------------------------------
        internal EntityMgrListener _getListener()
        {
            return mEntityMgrListener;
        }

        //---------------------------------------------------------------------
        internal EbFileStream _getFileStream()
        {
            return mEntityFileStream;
        }

        //---------------------------------------------------------------------
        internal EntitySerializerMgr _getSerializerMgr()
        {
            return mEntitySerializerMgr;
        }

        //---------------------------------------------------------------------
        internal Entity _createEntityImpl(EntityData entity_data, Entity parent = null, bool recursive = false)
        {
            Entity entity = new Entity(this, entity_data);
            string entity_type = entity.getEntityType();
            ulong entity_rpcid = entity.getEntityRpcId();

            if (mMapAllEntity4Search1.ContainsKey(entity_type))
            {
                Dictionary<ulong, Entity> m = mMapAllEntity4Search1[entity_type];
                m[entity_rpcid] = entity;
            }
            else
            {
                Dictionary<ulong, Entity> m = new Dictionary<ulong, Entity>();
                m[entity_rpcid] = entity;
                mMapAllEntity4Search1[entity_type] = m;
            }

            mMapAllEntity4Search2[entity_rpcid] = entity;

            mQueCreateEntity.Enqueue(entity);

            // 调用Entity._init()，让逻辑层有机会做事情
            if (parent != null) entity.setParent(parent);
            entity._init();
            if (parent != null) parent._onChildInit(entity);

            // 递归创建子Entity
            if (recursive && entity_data.entity_children != null)
            {
                foreach (var i in entity_data.entity_children)
                {
                    _createEntityImpl(i, entity, recursive);
                }
            }

            return entity;
        }

        //---------------------------------------------------------------------
        internal ulong _genEntityRpcId()
        {
            if (mQueFreeEntityId.Count > 0)
            {
                uint id = mQueFreeEntityId.Dequeue();
                byte local_node_type = NodeType;
                ulong entity_rpcid = 0xffffffffffffffff;
                entity_rpcid &= ((ulong)local_node_type) << 32;
                entity_rpcid |= (ulong)id;
                return entity_rpcid;
            }
            else
            {
                uint id = ++mMaxEntityId;
                if (id == 0) id = ++mMaxEntityId;
                byte local_node_type = NodeType;
                ulong entity_rpcid = 0xffffffffffffffff;
                entity_rpcid &= ((ulong)local_node_type) << 32;
                entity_rpcid |= (ulong)id;
                return entity_rpcid;
            }
        }

        //---------------------------------------------------------------------
        internal void _freeEntityRpcId(ulong entity_rpcid)
        {
            uint id = (uint)(entity_rpcid & 0x00000000ffffffff);
            byte local_node_type = (byte)(entity_rpcid >> 32);

            mQueFreeEntityId.Enqueue(id);
        }

        //---------------------------------------------------------------------
        internal string _getComponentName(Type t)
        {
            Type[] list_t_arg = t.GetGenericArguments();
            Type t_arg = list_t_arg[0];
            if (t_arg.Name == "ComponentDef")
            {
                // 本地Commpent
                string name = "Def" + t.Name;
                return name;
            }
            else
            {
                // 分布式Commpent
                return t_arg.Name;
            }
        }
    }
}
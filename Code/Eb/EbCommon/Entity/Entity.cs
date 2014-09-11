using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public sealed class Entity : IDisposable
    {
        //---------------------------------------------------------------------
        private string mEntityType = "";
        private string mEntityGuid = "";
        private ulong mEntityRpcId;
        private Entity mParent;
        private EntityDef mEntityDef = new EntityDef();
        private Dictionary<string, Dictionary<ulong, Entity>> mMapChildEntity;
        private Dictionary<string, object> mMapCacheData;
        private Dictionary<byte, RpcSession> mMapSessionOne2One = new Dictionary<byte, RpcSession>();
        private Dictionary<byte, Dictionary<ulong, Entity>> mMapSessionN2One = null;
        private Dictionary<string, IComponent> mMapComponent = new Dictionary<string, IComponent>();
        private List<string> mListEventPublisher;

        //---------------------------------------------------------------------
        public EntityMgr EntityMgr { get; private set; }
        public EntityTransform Transform { get; private set; }

        //---------------------------------------------------------------------
        internal Entity(EntityMgr entity_mgr, EntityData entity_data)
        {
            EntityMgr = entity_mgr;
            mEntityType = entity_data.entity_type;
            mEntityGuid = entity_data.entity_guid;
            mEntityRpcId = entity_data.entity_rpcid;
            mMapCacheData = entity_data.cache_data;
            Transform = entity_data.entity_transform;

            if (Transform == null)
            {
                Transform = new EntityTransform();
                Transform.position.x = 0.0f;
                Transform.position.y = 0.0f;
                Transform.position.z = 0.0f;
                Transform.rotation.x = 0.0f;
                Transform.rotation.y = 0.0f;
                Transform.rotation.z = 0.0f;
            }

            if (mEntityRpcId == 0)
            {
                mEntityRpcId = EntityMgr._genEntityRpcId();
            }

            if (entity_data.list_component != null)
            {
                foreach (var i in entity_data.list_component)
                {
                    ComponentData component_data = i;
                    _addComponent(component_data.component_name, component_data.def_propset, false);
                }
            }
        }

        //---------------------------------------------------------------------
        ~Entity()
        {
            this._dispose(false);
        }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            this._dispose(true);
            GC.SuppressFinalize(this);
        }

        //---------------------------------------------------------------------
        public string getEntityType()
        {
            return mEntityType;
        }

        //---------------------------------------------------------------------
        public string getEntityGuid()
        {
            return mEntityGuid;
        }

        //---------------------------------------------------------------------
        public ulong getEntityRpcId()
        {
            return mEntityRpcId;
        }

        //---------------------------------------------------------------------
        public EntityData genEntityData4SaveDb(bool recursive = false)
        {
            EntityData entity_data = new EntityData();
            entity_data.entity_type = getEntityType();
            entity_data.entity_guid = getEntityGuid();
            entity_data.entity_rpcid = getEntityRpcId();
            entity_data.entity_transform = Transform;
            entity_data.cache_data = null;
            entity_data.list_component = new List<ComponentData>();

            foreach (var i in mMapComponent)
            {
                ComponentData component_data = new ComponentData();
                component_data.component_name = i.Key;
                component_data.def_propset = i.Value.getDef().getMapProp4SaveDb(EntityMgr.NodeType);
                entity_data.list_component.Add(component_data);
            }

            if (recursive && mMapChildEntity != null)
            {
                entity_data.entity_children = new List<EntityData>();
                foreach (var i in mMapChildEntity)
                {
                    foreach (var j in i.Value)
                    {
                        entity_data.entity_children.Add(j.Value.genEntityData4SaveDb(recursive));
                    }
                }
            }

            return entity_data;
        }

        //---------------------------------------------------------------------
        public EntityData genEntityData4NetSync(byte to_node, bool recursive = false)
        {
            EntityData entity_data = new EntityData();
            entity_data.entity_type = getEntityType();
            entity_data.entity_guid = getEntityGuid();
            entity_data.entity_rpcid = getEntityRpcId();
            entity_data.entity_transform = Transform;
            entity_data.cache_data = null;
            entity_data.list_component = new List<ComponentData>();

            foreach (var i in mMapComponent)
            {
                ComponentData component_data = new ComponentData();
                component_data.component_name = i.Key;
                component_data.def_propset = i.Value.getDef().getMapProp4NetSync(EntityMgr.NodeType, to_node);
                entity_data.list_component.Add(component_data);
            }

            if (recursive && mMapChildEntity != null)
            {
                entity_data.entity_children = new List<EntityData>();
                foreach (var i in mMapChildEntity)
                {
                    foreach (var j in i.Value)
                    {
                        entity_data.entity_children.Add(j.Value.genEntityData4NetSync(to_node, recursive));
                    }
                }
            }

            return entity_data;
        }

        //---------------------------------------------------------------------
        public EntityData genEntityData4All(bool recursive = false)
        {
            EntityData entity_data = new EntityData();
            entity_data.entity_type = getEntityType();
            entity_data.entity_guid = getEntityGuid();
            entity_data.entity_rpcid = getEntityRpcId();
            entity_data.entity_transform = Transform;
            entity_data.cache_data = null;
            entity_data.list_component = new List<ComponentData>();

            foreach (var i in mMapComponent)
            {
                ComponentData component_data = new ComponentData();
                component_data.component_name = i.Key;
                component_data.def_propset = i.Value.getDef().getMapProp4All();
                entity_data.list_component.Add(component_data);
            }

            if (recursive && mMapChildEntity != null)
            {
                entity_data.entity_children = new List<EntityData>();
                foreach (var i in mMapChildEntity)
                {
                    foreach (var j in i.Value)
                    {
                        entity_data.entity_children.Add(j.Value.genEntityData4All(recursive));
                    }
                }
            }

            return entity_data;
        }

        //---------------------------------------------------------------------
        public T addComponent<T>() where T : IComponent, new()
        {
            string type_name = EntityMgr.getComponentName<T>();
            T component = (T)_addComponent(type_name, null, true);
            return component;
        }

        //---------------------------------------------------------------------
        public IComponent addComponent(string type_name)
        {
            return _addComponent(type_name, null, true);
        }

        //---------------------------------------------------------------------
        public void removeComponent(string type_name)
        {
            if (mMapComponent.ContainsKey(type_name))
            {
                mMapComponent[type_name].release();
                mMapComponent.Remove(type_name);
            }
        }

        //---------------------------------------------------------------------
        public void removeComponent(IComponent component)
        {
            if (component == null) return;

            string type_name = EntityMgr.getComponentName(component);
            removeComponent(type_name);
        }

        //---------------------------------------------------------------------
        public T getComponent<T>() where T : IComponent
        {
            string type_name = EntityMgr.getComponentName<T>();

            if (mMapComponent.ContainsKey(type_name))
            {
                return (T)mMapComponent[type_name];
            }
            else
            {
                return default(T);
            }
        }

        //---------------------------------------------------------------------
        public IComponent getComponent(string type)
        {
            if (mMapComponent.ContainsKey(type))
            {
                return mMapComponent[type];
            }
            else
            {
                return null;
            }
        }

        //---------------------------------------------------------------------
        public Dictionary<string, IComponent> getAllComponents()
        {
            return mMapComponent;
        }

        //---------------------------------------------------------------------
        public void setSession(byte to_node, RpcSession session)
        {
            mMapSessionOne2One[to_node] = session;
        }

        //---------------------------------------------------------------------
        public RpcSession getSession(byte to_node)
        {
            if (mMapSessionOne2One.ContainsKey(to_node))
            {
                return mMapSessionOne2One[to_node];
            }
            else
            {
                return null;
            }
        }

        //---------------------------------------------------------------------
        public void setMapSessionSameAsEntity(Entity entity)
        {
            Dictionary<byte, RpcSession> map_session = entity._getMapSession();
            foreach (var i in map_session)
            {
                mMapSessionOne2One[i.Key] = i.Value;
            }
        }

        //---------------------------------------------------------------------
        public void addProxyEntity(byte to_node, Entity entity)
        {
            if (mMapSessionN2One == null)
            {
                mMapSessionN2One = new Dictionary<byte, Dictionary<ulong, Entity>>();
            }

            if (mMapSessionN2One.ContainsKey(to_node))
            {
                Dictionary<ulong, Entity> m = mMapSessionN2One[to_node];
                m[entity.getEntityRpcId()] = entity;
            }
            else
            {
                Dictionary<ulong, Entity> m = new Dictionary<ulong, Entity>();
                m[entity.getEntityRpcId()] = entity;
                mMapSessionN2One[to_node] = m;
            }
        }

        //---------------------------------------------------------------------
        public void delProxyEntity(byte to_node, Entity entity)
        {
            if (mMapSessionN2One == null) return;

            if (mMapSessionN2One.ContainsKey(to_node))
            {
                mMapSessionN2One[to_node].Remove(entity.getEntityRpcId());
            }
        }

        //---------------------------------------------------------------------
        public void clearProxyEntity(byte to_node)
        {
            if (mMapSessionN2One == null) return;

            if (mMapSessionN2One.ContainsKey(to_node))
            {
                mMapSessionN2One[to_node].Clear();
            }
        }

        //---------------------------------------------------------------------
        public Entity getProxyEntity(byte to_node, uint et_player_rpcid)
        {
            if (mMapSessionN2One == null) return null;

            if (mMapSessionN2One.ContainsKey(to_node))
            {
                Dictionary<ulong, Entity> m = mMapSessionN2One[to_node];
                if (m.ContainsKey(et_player_rpcid))
                {
                    return m[et_player_rpcid];
                }
            }

            return null;
        }

        //---------------------------------------------------------------------
        public void setUserData<T>(T data)
        {
            if (mMapCacheData == null)
            {
                mMapCacheData = new Dictionary<string, object>();
            }

            mMapCacheData[data.GetType().Name] = data;
        }

        //---------------------------------------------------------------------
        public T getUserData<T>()
        {
            string key = typeof(T).Name;
            if (mMapCacheData == null || !mMapCacheData.ContainsKey(key)) return default(T);
            else return (T)mMapCacheData[key];
        }

        //---------------------------------------------------------------------
        public void setCacheData(string key, object v)
        {
            if (mMapCacheData == null)
            {
                mMapCacheData = new Dictionary<string, object>();
            }

            mMapCacheData[key] = v;
        }

        //---------------------------------------------------------------------
        public object getCacheData(string key)
        {
            if (mMapCacheData == null || !mMapCacheData.ContainsKey(key)) return null;
            else return mMapCacheData[key];
        }

        //---------------------------------------------------------------------
        public bool hasCacheData(string key)
        {
            return mMapCacheData.ContainsKey(key);
        }

        //---------------------------------------------------------------------
        public void setParent(Entity parent)
        {
            mParent = parent;
            mParent._addChild(this);
        }

        //---------------------------------------------------------------------
        public void removeChild(Entity child)
        {
            if (mMapChildEntity == null)
            {
                mMapChildEntity = new Dictionary<string, Dictionary<ulong, Entity>>();
            }

            string et_type = child.getEntityType();
            ulong et_rpcid = child.getEntityRpcId();
            if (mMapChildEntity.ContainsKey(et_type))
            {
                mMapChildEntity[et_type].Remove(et_rpcid);
            }
        }

        //---------------------------------------------------------------------
        public Dictionary<ulong, Entity> getChildrenByType(string et_type)
        {
            if (mMapChildEntity == null) return null;

            if (mMapChildEntity.ContainsKey(et_type))
            {
                return mMapChildEntity[et_type];
            }

            return null;
        }

        //---------------------------------------------------------------------
        public Entity getChild(string et_type, ulong et_rpcid)
        {
            if (mMapChildEntity == null) return null;

            if (mMapChildEntity.ContainsKey(et_type))
            {
                Dictionary<ulong, Entity> map_child = mMapChildEntity[et_type];
                if (map_child.ContainsKey(et_rpcid))
                {
                    return map_child[et_rpcid];
                }
            }

            return null;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, Dictionary<ulong, Entity>> getChildren()
        {
            return mMapChildEntity;
        }

        //---------------------------------------------------------------------
        public Entity getParent()
        {
            return mParent;
        }

        //---------------------------------------------------------------------
        internal void _init()
        {
            Dictionary<string, IComponent> m = new Dictionary<string, IComponent>(mMapComponent);
            foreach (var i in m)
            {
                i.Value.init();
            }
        }

        //---------------------------------------------------------------------
        internal void _release()
        {
            Dictionary<string, IComponent> m = new Dictionary<string, IComponent>(mMapComponent);
            foreach (var i in m)
            {
                i.Value.release();
            }
        }

        //---------------------------------------------------------------------
        internal void _update(float elapsed_tm)
        {
            foreach (var i in mMapComponent)
            {
                if (i.Value.EnableUpdate) i.Value.update(elapsed_tm);
            }
        }

        //---------------------------------------------------------------------
        internal void _onChildInit(Entity child)
        {
            foreach (var i in mMapComponent)
            {
                i.Value.onChildInit(child);
            }
        }

        //---------------------------------------------------------------------
        internal void _handleEvent(object sender, EntityEvent e)
        {
            foreach (var i in mMapComponent)
            {
                i.Value.handleEvent(sender, e);
            }
        }

        //---------------------------------------------------------------------
        internal void _addChild(Entity child)
        {
            if (mMapChildEntity == null)
            {
                mMapChildEntity = new Dictionary<string, Dictionary<ulong, Entity>>();
            }

            string et_type = child.getEntityType();
            ulong et_rpcid = child.getEntityRpcId();
            if (mMapChildEntity.ContainsKey(et_type))
            {
                Dictionary<ulong, Entity> map_child = mMapChildEntity[et_type];
                map_child[et_rpcid] = child;
            }
            else
            {
                Dictionary<ulong, Entity> map_child = new Dictionary<ulong, Entity>();
                map_child[et_rpcid] = child;
                mMapChildEntity[et_type] = map_child;
            }
        }

        //---------------------------------------------------------------------
        internal IComponent _addComponent(string type_name, Dictionary<string, string> map_param, bool need_init)
        {
            IComponentFactory component_factory = EntityMgr.getComponentFactory(type_name);
            if (component_factory == null)
            {
                EbLog.Error("Entity.addComponent() failed! can't find component_factory, component=" + type_name);
                return null;
            }

            IComponent component = null;
            if (mMapComponent.TryGetValue(type_name, out component))
            {
                return component;
            }

            component = component_factory.createComponent(this, map_param);
            mMapComponent[type_name] = component;

            component.awake();

            if (need_init)
            {
                component.init();
            }

            return component;
        }

        //---------------------------------------------------------------------
        internal Dictionary<byte, RpcSession> _getMapSession()
        {
            return mMapSessionOne2One;
        }

        //---------------------------------------------------------------------
        internal Dictionary<ulong, Entity> _getMapProxyEntity(byte to_node)
        {
            if (mMapSessionN2One == null) return null;

            if (mMapSessionN2One.ContainsKey(to_node))
            {
                return mMapSessionN2One[to_node];
            }

            return null;
        }

        //---------------------------------------------------------------------
        internal EntityDef _getEntityDef()
        {
            return mEntityDef;
        }

        //---------------------------------------------------------------------
        internal void _rpcOnEntityMethod(RpcSession session, byte from_node,
            ushort method_id, Dictionary<byte, object> map_param)
        {
            mEntityDef._onRpcMethod(session, from_node, method_id, map_param);
        }

        //---------------------------------------------------------------------
        internal void _rpcOnComponentSyncProp(RpcSession session, byte from_node, ushort component_id,
            ushort reason, Dictionary<string, string> map_prop)
        {
            foreach (var i in mMapComponent)
            {
                if (i.Value.getDef().getComponentId() == component_id)
                {
                    i.Value._onRpcPropSync(session, from_node, reason, map_prop);
                    return;
                }
            }
        }

        //---------------------------------------------------------------------
        internal void _addEvPublisher(string ev_handler_name)
        {
            if (mListEventPublisher == null)
            {
                mListEventPublisher = new List<string>();
            }

            mListEventPublisher.Add(ev_handler_name);
        }

        //---------------------------------------------------------------------
        internal void _removeEvPublisher(string ev_publisher_name)
        {
            if (mListEventPublisher != null)
            {
                mListEventPublisher.Remove(ev_publisher_name);
            }
        }

        //---------------------------------------------------------------------
        internal bool _existEvPublisher(string ev_publisher_name)
        {
            if (mListEventPublisher != null)
            {
                return mListEventPublisher.Contains(ev_publisher_name);
            }

            return false;
        }

        //---------------------------------------------------------------------
        private void _dispose(bool disposing)
        {
            EntityMgr._freeEntityRpcId(mEntityRpcId);

            if (mMapCacheData != null)
            {
                mMapCacheData.Clear();
            }

            //mMapSessionOne2One.Clear();
            //mMapSessionN2One = null;
            //mMapComponent.Clear();
        }
    }
}
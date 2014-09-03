using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Eb
{
    public abstract class IComponent
    {
        //---------------------------------------------------------------------
        public Entity Entity { internal set; get; }
        public EntityMgr EntityMgr { internal set; get; }
        internal EntityDef EntityDef { set; get; }
        public bool EnableUpdate { set; get; }

        //---------------------------------------------------------------------
        public abstract void awake();

        //---------------------------------------------------------------------
        public abstract void init();

        //---------------------------------------------------------------------
        public abstract void release();

        //---------------------------------------------------------------------
        public abstract void update(float elapsed_tm);

        //---------------------------------------------------------------------
        public abstract void onChildInit(Entity child);

        //---------------------------------------------------------------------
        public abstract void onRpcPropSync(RpcSession session, byte from_node, ushort reason);

        //---------------------------------------------------------------------
        public abstract void handleEvent(object sender, EntityEvent e);

        //---------------------------------------------------------------------
        public abstract ComponentDef getDef();

        //---------------------------------------------------------------------
        public abstract bool isDistributed();

        //---------------------------------------------------------------------
        public abstract void rpcOne(byte to_node, ushort method_id, Dictionary<byte, object> map_param);

        //---------------------------------------------------------------------
        public abstract void rpcOneSyncProp(byte to_node, ushort reason);

        //---------------------------------------------------------------------
        public abstract void rpcOneCreateRemote(byte to_node, bool load_from_db = false);

        //---------------------------------------------------------------------
        public abstract void rpcOneDestroyRemote(byte to_node);

        //---------------------------------------------------------------------
        public abstract void rpcProxy(byte to_node, Entity proxy_entity, ushort method_id,
            Dictionary<byte, object> map_param);

        //---------------------------------------------------------------------
        public abstract void rpcProxyGroup(byte to_node, List<Entity> list_proxy,
            ushort method_id, Dictionary<byte, object> map_param);

        //---------------------------------------------------------------------
        public abstract void rpcProxyAll(byte to_node, ushort method_id, Dictionary<byte, object> map_param);

        //---------------------------------------------------------------------
        internal abstract void _genDef(Dictionary<string, string> map_param);

        //---------------------------------------------------------------------
        internal abstract void _onRpcPropSync(RpcSession session, byte from_node,
            ushort reason, Dictionary<string, string> map_prop);
    }

    public class Component<T> : IComponent where T : ComponentDef, new()
    {
        //---------------------------------------------------------------------
        private T mDef = null;

        //---------------------------------------------------------------------
        public T Def { private set { mDef = value; } get { return mDef; } }

        //---------------------------------------------------------------------
        public override void awake()
        {
        }

        //---------------------------------------------------------------------
        public override void init()
        {
        }

        //---------------------------------------------------------------------
        public override void release()
        {
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

        //---------------------------------------------------------------------
        public override ComponentDef getDef()
        {
            return mDef;
        }

        //---------------------------------------------------------------------
        public override bool isDistributed()
        {
            return typeof(T).IsSubclassOf(typeof(ComponentDef));
        }

        //---------------------------------------------------------------------
        public override void rpcOne(byte to_node, ushort method_id, Dictionary<byte, object> map_param)
        {
            _tEntityRpcData data = _packRpcDataMethod(to_node, method_id, map_param);
            _sendRpcOne(to_node, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcOneSyncProp(byte to_node, ushort reason)
        {
            // todo，优化为只提取dirty属性
            Dictionary<string, string> map_prop = mDef.getMapProp4NetSync(EntityMgr.NodeType, to_node);

            _tEntityRpcData data;
            data.rpc_id = _tEntityRpcData.from((byte)_eRpcCmd.ComponentSyncProp, to_node, Entity.getEntityRpcId());
            data.from_node = EntityMgr.NodeType;
            data.method_id = (short)mDef.getComponentId();
            data.map_param = new Dictionary<byte, object>();
            data.map_param[0] = (short)reason;
            data.map_param[1] = map_prop;
            data.session_recv = null;

            _sendRpcOne(to_node, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcOneCreateRemote(byte to_node, bool load_from_db = false)
        {
            _tEntityRpcData data = _packRpcDataCreateRemote(to_node, load_from_db);
            _sendRpcOne(to_node, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcOneDestroyRemote(byte to_node)
        {
            _tEntityRpcData data = _packRpcDataDestroyRemote(to_node);
            _sendRpcOne(to_node, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcProxy(byte to_node, Entity proxy_entity,
            ushort method_id, Dictionary<byte, object> map_param)
        {
            _tEntityRpcData data = _packRpcDataMethod(to_node, method_id, map_param);
            _sendRpcProxy(to_node, proxy_entity, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcProxyGroup(byte to_node, List<Entity> list_proxy,
            ushort method_id, Dictionary<byte, object> map_param)
        {
            _tEntityRpcData data = _packRpcDataMethod(to_node, method_id, map_param);
            _sendRpcProxyGroup(to_node, list_proxy, ref data);
        }

        //---------------------------------------------------------------------
        public override void rpcProxyAll(byte to_node, ushort method_id, Dictionary<byte, object> map_param)
        {
            _tEntityRpcData data = _packRpcDataMethod(to_node, method_id, map_param);
            _sendRpcProxyAll(to_node, ref data);
        }

        //---------------------------------------------------------------------
        public void defRpcMethod(byte from_node, ushort method_id, RpcSlot rpc_slot)
        {
            EntityDef._defRpcMethod(from_node, method_id, rpc_slot);
        }

        //---------------------------------------------------------------------
        _tEntityRpcData _packRpcDataMethod(byte to_node, ushort method_id, Dictionary<byte, object> map_param)
        {
            _tEntityRpcData data;
            data.rpc_id = _tEntityRpcData.from((byte)_eRpcCmd.EntityMethod, to_node, Entity.getEntityRpcId());
            data.from_node = EntityMgr.NodeType;
            data.method_id = (short)method_id;
            data.map_param = map_param;
            data.session_recv = null;

            return data;
        }

        //---------------------------------------------------------------------
        _tEntityRpcData _packRpcDataCreateRemote(byte to_node, bool load_from_db = false)
        {
            _tEntityRpcData data;
            data.rpc_id = _tEntityRpcData.from((byte)_eRpcCmd.EntityCreate, to_node, Entity.getEntityRpcId());
            data.from_node = EntityMgr.NodeType;
            data.method_id = (short)(load_from_db ? 1 : 0);
            data.map_param = new Dictionary<byte, object>();
            data.map_param[0] = Entity.getEntityType();
            data.map_param[1] = Entity.getEntityGuid();
            data.map_param[2] = (long)Entity.getEntityRpcId();
            data.map_param[3] = Entity.Transform.toDic();
            data.map_param[4] = EntityMgr._getComponentName(GetType());
            data.session_recv = null;

            return data;
        }

        //---------------------------------------------------------------------
        _tEntityRpcData _packRpcDataDestroyRemote(byte to_node)
        {
            _tEntityRpcData data;
            data.rpc_id = _tEntityRpcData.from((byte)_eRpcCmd.EntityDestroy, to_node, Entity.getEntityRpcId());
            data.from_node = EntityMgr.NodeType;
            data.method_id = 0;
            data.map_param = null;
            data.session_recv = null;

            return data;
        }

        //---------------------------------------------------------------------
        void _sendRpcOne(byte to_node, ref _tEntityRpcData data)
        {
            RpcSession s = Entity.getSession(to_node);
            if (s != null)
            {
                s.sendEntityRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        void _sendRpcProxy(byte to_node, Entity proxy_entity, ref _tEntityRpcData data)
        {
            RpcSession session = proxy_entity.getSession(to_node);
            if (session != null)
            {
                session.sendEntityRpcData(ref data);
            }
        }

        //---------------------------------------------------------------------
        void _sendRpcProxyGroup(byte to_node, List<Entity> list_proxy, ref _tEntityRpcData data)
        {
            if (list_proxy == null) return;

            foreach (var i in list_proxy)
            {
                RpcSession session = i.getSession(to_node);
                if (session != null)
                {
                    session.sendEntityRpcData(ref data);
                }
            }
        }

        //---------------------------------------------------------------------
        void _sendRpcProxyAll(byte to_node, ref _tEntityRpcData data)
        {
            Dictionary<ulong, Entity> m = Entity._getMapProxyEntity(to_node);
            if (m == null) return;

            foreach (var i in m)
            {
                RpcSession session = i.Value.getSession(to_node);
                if (session != null)
                {
                    session.sendEntityRpcData(ref data);
                }
            }
        }

        //---------------------------------------------------------------------
        internal override void _genDef(Dictionary<string, string> map_param)
        {
            mDef = new T();
            mDef.defAllProp(map_param);
        }

        //---------------------------------------------------------------------
        internal override void _onRpcPropSync(RpcSession session, byte from_node,
            ushort reason, Dictionary<string, string> map_prop)
        {
            if (map_prop != null)
            {
                foreach (var i in map_prop)
                {
                    IProp prop = mDef.getProp(i.Key);
                    if (prop == null) continue;
                    prop.fromJsonString(i.Value);
                }
            }

            onRpcPropSync(session, from_node, reason);
        }
    }
}

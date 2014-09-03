using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Eb
{
    //-------------------------------------------------------------------------
    public delegate void RpcSlot(RpcSession s, Dictionary<byte, object> map_param);

    //-------------------------------------------------------------------------
    public enum _eRpcCmd : byte
    {
        Dummy = 0,
        NodeMethod,// 节点远程调用
        EntityMethod,// 远程调用
        EntityCreate,// 远程创建
        EntityDestroy,// 远程销毁
        ComponentSyncProp,// 请求同步属性
        SyncPeerInfo,// 同步Peer信息
        Unknown
    }

    //-------------------------------------------------------------------------
    public struct _tEntityRpcData
    {
        public long rpc_id;
        public byte from_node;
        public short method_id;
        public Dictionary<byte, object> map_param;
        public RpcSession session_recv;

        public static long from(byte cmd_id, byte to_node, ulong entity_rpcid)
        {
            byte method_id = 0;
            long id = 0x0000000000000000;
            id |= ((long)cmd_id) << 56;
            id |= ((long)method_id) << 48;
            id |= ((long)to_node) << 40;
            id |= (long)entity_rpcid;
            return id;
        }

        public static void to(long rpc_id, out byte cmd_id, out byte to_node, out ulong entity_rpcid)
        {
            byte method_id = 0;
            cmd_id = (byte)(rpc_id >> 56);
            rpc_id &= 0x00ffffffffffffff;
            method_id = (byte)(rpc_id >> 48);
            rpc_id &= 0x0000ffffffffffff;
            to_node = (byte)(rpc_id >> 40);
            rpc_id &= 0x000000ffffffffff;
            entity_rpcid = (ulong)rpc_id;
            //from_node = (byte)(rpc_id >> 32);
        }

        public byte getCmdId()
        {
            return (byte)(rpc_id >> 56);
        }
    }

    //-------------------------------------------------------------------------
    public class EntityAsyncStatus
    {
        public bool is_done;
        public bool is_error;
        public string error_str;
        public object obj;
    }

    [Serializable]
    public class EntityTransform
    {
        //---------------------------------------------------------------------
        public EbVector3 position;
        public EbVector3 rotation;

        //---------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is EntityTransform)
            {
                var other = (EntityTransform)obj;
                return this.position.Equals(other.position) && this.rotation.Equals(other.rotation);
            }
            return false;
        }

        //---------------------------------------------------------------------
        public static bool operator ==(EntityTransform left, EntityTransform right)
        {
            if (System.Object.ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null)
            {
                if ((object)right == null)
                {
                    return false;
                }
            }

            return left.Equals(right);
        }

        //---------------------------------------------------------------------
        public static bool operator !=(EntityTransform left, EntityTransform right)
        {
            return !left.Equals(right);
        }

        //---------------------------------------------------------------------
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //---------------------------------------------------------------------
        public Dictionary<byte, object> toDic()
        {
            Dictionary<byte, object> m = new Dictionary<byte, object>();
            m[0] = position.x;
            m[1] = position.y;
            m[2] = position.z;
            m[3] = rotation.x;
            m[4] = rotation.y;
            m[5] = rotation.z;
            return m;
        }

        //---------------------------------------------------------------------
        public void fromDic(Dictionary<byte, object> m)
        {
            position.x = (float)m[0];
            position.y = (float)m[1];
            position.z = (float)m[2];
            rotation.x = (float)m[3];
            rotation.y = (float)m[4];
            rotation.z = (float)m[5];
        }
    }

    //-------------------------------------------------------------------------
    [Serializable]
    public class ComponentData
    {
        public string component_name;// 组件类工厂名
        public Dictionary<string, string> def_propset;// 组件定义类属性集
    }

    //-------------------------------------------------------------------------
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class EntityData
    {
        public string entity_type;
        public string entity_guid;
        public EntityTransform entity_transform;
        [JsonIgnoreAttribute]
        public ulong entity_rpcid;
        [JsonIgnoreAttribute]
        [NonSerialized]
        public Dictionary<string, object> cache_data;
        public List<ComponentData> list_component;
        public List<EntityData> entity_children;
    }
}
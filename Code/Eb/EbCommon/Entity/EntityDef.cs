using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public sealed class EntityDef
    {
        //---------------------------------------------------------------------
        // key1=node_type key2=method_id
        Dictionary<byte, Dictionary<ushort, RpcSlot>> mMapRpcSlot = new Dictionary<byte, Dictionary<ushort, RpcSlot>>();

        //---------------------------------------------------------------------
        internal void _defRpcMethod(byte from_node, ushort method_id, RpcSlot rpc_slot)
        {
            if (mMapRpcSlot.ContainsKey(from_node))
            {
                Dictionary<ushort, RpcSlot> m = mMapRpcSlot[from_node];
                m[method_id] = rpc_slot;
            }
            else
            {
                Dictionary<ushort, RpcSlot> m = new Dictionary<ushort, RpcSlot>();
                m[method_id] = rpc_slot;
                mMapRpcSlot[from_node] = m;
            }
        }
        
        //---------------------------------------------------------------------
        internal void _onRpcMethod(RpcSession session, byte from_node, ushort method_id, Dictionary<byte, object> map_param)
        {
            if (!mMapRpcSlot.ContainsKey(from_node))
            {
                EbLog.Error("EntityDef._onRpcMethod() not found from_node. from_node = " + from_node);
                return;
            }

            Dictionary<ushort, RpcSlot> m = mMapRpcSlot[from_node];
            if (!m.ContainsKey(method_id))
            {
                EbLog.Error("EntityDef._onRpcMethod() not found method_id. method_id = " + method_id);
                return;
            }

            try
            {
                if (m[method_id] != null)
                {
                    m[method_id](session, map_param);
                }
            }
            catch (System.Exception ec)
            {
                EbLog.Error(ec.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public interface EntityMgrListener
    {
        //---------------------------------------------------------------------
        void onRpcNodeMethod(RpcSession session, byte from_node, ushort method_id, Dictionary<byte, object> map_param);

        //---------------------------------------------------------------------
        Entity onRpcEntityCreateRemote(RpcSession session, EntityData entity_data, bool from_db);
    }
}

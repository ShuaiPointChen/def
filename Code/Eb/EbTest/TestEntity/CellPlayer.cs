using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public class CellPlayer<T> : Component<T> where T : DefPlayer, new()
    {
        //---------------------------------------------------------------------
        int mCount = 0;

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("CellPlayer.init()");

            EnableUpdate = true;
            Def.mPropNickName.set("test_nickname");
        }

        //---------------------------------------------------------------------
        public override void release()
        {
            EbLog.Note("CellPlayer.release()");
        }

        //---------------------------------------------------------------------
        public override void update(float elapsed_tm)
        {
        }

        //---------------------------------------------------------------------
        //[EntityRpcMethod(_eEtNode.Anonymous)]，定义NodeType，与Rpc委托比较校验函数参数
        public void cell2clientLogin(RpcSession s, Dictionary<byte, object> map_param)
        {

        }
    }
}

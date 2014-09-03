using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Eb
{
    //-------------------------------------------------------------------------
    public delegate void RpcSlotTest();

    class TestDummy : Test
    {
        //---------------------------------------------------------------------
        Dictionary<byte, RpcSlotTest> mMapRpcSlot = new Dictionary<byte, RpcSlotTest>();

        //---------------------------------------------------------------------
        public override void init()
        {
            Type t = this.GetType();
            //EbLog.Note(t.ToString());
            //EbLog.Note(t.FullName);
            //EbLog.Note(t.Name);

            MethodInfo[] method_list = t.GetMethods();
            foreach(var i in method_list)
            {
                //EbLog.Note(i.Name);
            }

            //Type t1 = typeof(release);

            //mMapRpcSlot[0] = release;
            //EbLog.Note(mMapRpcSlot[0].Method.Name);

            BindingFlags BindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = t.GetFields(BindingFlag);

            //byte cmd_id = 101;
            //byte method_id = 73;
            //byte to_node = 0;
            //ulong entity_rpcid = 12345;

            //long id = 0x0000000000000000;
            //id |= ((long)cmd_id) << 56;
            //id |= ((long)method_id) << 48;
            //id |= ((long)to_node) << 40;
            //id |= (long)entity_rpcid;

            //cmd_id = (byte)(id >> 56);
            //id &= 0x00ffffffffffffff;
            //method_id = (byte)(id >> 48);
            //id &= 0x0000ffffffffffff;
            //to_node = (byte)(id >> 40);
            //id &= 0x000000ffffffffff;
            //entity_rpcid = (ulong)id;

            //EbLog.Note("id=" + id.ToString());
            //EbLog.Note("cmd_id=" + cmd_id.ToString());
            //EbLog.Note("method_id=" + method_id.ToString());
            //EbLog.Note("to_node=" + to_node.ToString());
            //EbLog.Note("entity_rpcid=" + entity_rpcid.ToString());
        }

        //---------------------------------------------------------------------
        public override void release()
        {
        }

        //---------------------------------------------------------------------
        public override void update()
        {
        }
    }
}

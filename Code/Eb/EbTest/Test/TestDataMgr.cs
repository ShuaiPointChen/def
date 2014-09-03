using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    class TestDataMgr : Test
    {
        //---------------------------------------------------------------------
        EbDataMgr mDataMgr = new EbDataMgr();

        //---------------------------------------------------------------------
        public override void init()
        {
            mDataMgr.genTestDbDescFile("..//..//..//Media//EbCommon//Config//DragonForeignKey.json");
            //mDataMgr.setup("Dragon",
            //    "..//..//..//Media//EbCommon//Config//Dragon.db",
            //    "..//..//..//Media//EbCommon//Config//DragonDbInfo",
            //    "..//..//..//Media//EbCommon//Config//DragonForeignKey.json");
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

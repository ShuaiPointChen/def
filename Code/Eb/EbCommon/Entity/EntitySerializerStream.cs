using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eb
{
    public class CEntitySerializerStream : CEntitySerializer
    {
        //-----------------------------------------------------------------------------
        public CEntitySerializerStream(CEntityMgr entity_mgr)
            : base(entity_mgr)
        {
            mpEntityMgr = entity_mgr;
        }

        //-----------------------------------------------------------------------------
        public override string getName()
        {
            return "Stream";
        }

        //-----------------------------------------------------------------------------
        public override CEntity load(string entity_type, object param, Dictionary<string, object> cache_data)
        {
            return null;
        }

        //-----------------------------------------------------------------------------
        public override object save(string serializer_name, CEntity entity, CEntityDef entity_def, object param)
        {
            return null;
        }
    }
}

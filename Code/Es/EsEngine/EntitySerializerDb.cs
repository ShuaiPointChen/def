using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using log4net.Config;
//using ExitGames.Logging;
//using ExitGames.Logging.Log4Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Enyim.Caching.Memcached;
using Couchbase;
using Eb;

namespace Es
{
    public class CEntitySerializerDb : CEntitySerializer
    {
        //-----------------------------------------------------------------------------
        //private static readonly ILogger mLog = LogManager.GetCurrentClassLogger();

        //-----------------------------------------------------------------------------
        public CEntitySerializerDb(CEntityMgr entity_mgr)
            : base(entity_mgr)
        {
        }

        //-----------------------------------------------------------------------------
        public override string getName()
        {
            return "Db";
        }

        //-----------------------------------------------------------------------------
        public override CEntity load(string entity_type, object param, Dictionary<string, object> cache_data)
        {
            string entity_id = (string)param;
            string db_key = entity_type + "_" + entity_id;

            Enyim.Caching.Memcached.Results.IGetOperationResult<string> ret = CCouchbaseClient.Instance.ExecuteGet<string>(db_key);
            if (ret.Success)
            {
                string json_map_prop = ret.Value;
                //mLog.Info("-------- Db Begin --------");
                //mLog.Info("CEntitySerializerDb load entity from db: entity_type=" + entity_type + " entity_id=" + entity_id);
                //mLog.Info(json_map_prop);
                //mLog.Info("-------- Db End --------");

                Dictionary<string, object> map_param = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_map_prop);
                return mpEntityMgr.createEntity(entity_type, entity_id, map_param, cache_data);
            }
            else
            {
                //mLog.Error("CEntitySerializerDb::load() error! ");
                //mLog.Error("error load entity from db: entity_type=" + entity_type + " entity_id=" + entity_id);
                //mLog.Error(ret.Message);

                return mpEntityMgr.createEntity(entity_type, entity_id, null, cache_data);
            }
        }

        //-----------------------------------------------------------------------------
        public override object save(string serializer_name, CEntity entity, CEntityDef entity_def, object param)
        {
            string db_key = entity.getEntityType() + "_" + entity.getEntityId();

            Dictionary<string, object> map_prop = entity_def.getMapPropertyValue();
            string json_map_prop = JsonConvert.SerializeObject(map_prop);
            Enyim.Caching.Memcached.Results.IStoreOperationResult ret =
                CCouchbaseClient.Instance.ExecuteStore(StoreMode.Set, db_key, json_map_prop, Couchbase.Operations.PersistTo.Zero);

            if (ret.Success)
            {
                //mLog.Info("-------- Db Begin --------");
                //mLog.Info("CEntitySerializerDb save entity to db: entity_type=" + entity.getEntityType() + " entity_id=" + entity.getEntityId());
                //mLog.Info("-------- Db End --------");
            }
            else
            {
                //mLog.Error("CEntitySerializerDb::save() error! ");
                //mLog.Error("save entity to db: entity_type=" + entity.getEntityType() + " entity_id=" + entity.getEntityId());
                //mLog.Error(ret.Message);
                //mLog.Error(json_map_prop);
            }

            return null;
        }
    }
}
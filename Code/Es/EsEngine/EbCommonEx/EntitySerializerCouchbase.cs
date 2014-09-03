using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Eb;

namespace Es
{
    public class EntityLoaderCouchbase : EntityLoader
    {
        //---------------------------------------------------------------------
        private IDbCouchbase mDbCouchbase;
        private string mDbKey;
        private ulong mEntityRpcId = 0;

        //---------------------------------------------------------------------
        public EntityLoaderCouchbase(EntityMgr entity_mgr, Entity parent,
            string entity_type, string entity_guid, IDbCouchbase db_couchbase, bool recursive = false)
            : base(entity_mgr, parent, recursive)
        {
            mDbCouchbase = db_couchbase;
            mDbKey = entity_type + mEntityMgr.NodeTypeAsString + "_" + entity_guid;
        }

        //---------------------------------------------------------------------
        public EntityLoaderCouchbase(EntityMgr entity_mgr, Entity parent,
            string entity_type, string entity_guid, ulong entity_rpcid,
            IDbCouchbase db_couchbase, bool recursive = false)
            : base(entity_mgr, parent, recursive)
        {
            mDbCouchbase = db_couchbase;
            mDbKey = entity_type + mEntityMgr.NodeTypeAsString + "_" + entity_guid;
            mEntityRpcId = entity_rpcid;
        }

        //---------------------------------------------------------------------
        public EntityLoaderCouchbase(EntityMgr entity_mgr, Entity parent, EntityData entity_data,
            IDbCouchbase db_couchbase, bool recursive = false)
            : base(entity_mgr, parent, recursive)
        {
            mDbCouchbase = db_couchbase;
            mDbKey = entity_data.entity_type + mEntityMgr.NodeTypeAsString + "_" + entity_data.entity_guid;
            mEntityData = entity_data;
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleLoad()
        {
            string json_data;
            bool result = mDbCouchbase.executeGet(mDbKey, out json_data);
            if (result)
            {
                mEntityData = JsonConvert.DeserializeObject<EntityData>(json_data);
                if (mEntityRpcId != 0) mEntityData.entity_rpcid = mEntityRpcId;
            }
            else if (mEntityData == null)
            {
                EbLog.Error("EntityLoaderCouchbase.handleLoad() Error！mEntityData == null");
            }
        }
    }

    public class EntitySaverCouchbase : EntitySaver
    {
        //---------------------------------------------------------------------
        private IDbCouchbase mDbCouchbase;
        bool mRecursive = true;

        //---------------------------------------------------------------------
        public EntitySaverCouchbase(EntityMgr entity_mgr, Entity entity, IDbCouchbase db_couchbase, bool recursive = false)
            : base(entity_mgr)
        {
            mDbCouchbase = db_couchbase;
            mRecursive = recursive;
            mEntityData = entity.genEntityData4SaveDb();
        }

        //---------------------------------------------------------------------
        public EntitySaverCouchbase(EntityMgr entity_mgr)
            : base(entity_mgr)
        {
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleSave()
        {
            string db_key = mEntityData.entity_type + mEntityMgr.NodeTypeAsString + "_" + mEntityData.entity_guid;
            string json_data = JsonConvert.SerializeObject(mEntityData);
            bool result = mDbCouchbase.executeStore(db_key, json_data);
            if (result)
            {
            }
            else
            {
                // log error
                // mLog.Error("CEntitySerializerCouchbase.handleSave() error! ");
                // mLog.Error("save entity to db: entity_type=" + data.entity_type + " entity_id=" + data.entity_guid);
                // mLog.Error(ret.Message);
                // mLog.Error(json_map_prop);
            }
        }
    }
}
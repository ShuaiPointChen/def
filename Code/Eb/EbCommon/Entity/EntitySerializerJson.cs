using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Eb
{
    public class EntityLoaderJson : EntityLoader
    {
        //---------------------------------------------------------------------
        JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string mFileName = "";

        //---------------------------------------------------------------------
        public EntityLoaderJson(EntityMgr entity_mgr, string file_name, Entity parent = null, bool recursive = true)
            : base(entity_mgr, parent, recursive)
        {
            mFileName = file_name;
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleLoad()
        {
            EbFileStream file_stream = mEntityMgr._getFileStream();

            bool result = file_stream.load(mFileName);
            if (result)
            {
                string buf = file_stream.getDataAsString();
                mEntityData = JsonConvert.DeserializeObject<EntityData>(buf);
            }
            else
            {
                // log error
            }
        }
    }

    public class EntitySaverJson : EntitySaver
    {
        //---------------------------------------------------------------------
        JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string mFileName = "";
        bool mRecursive = true;

        //---------------------------------------------------------------------
        public EntitySaverJson(EntityMgr entity_mgr, string file_name, Entity entity, bool recursive = true)
            : base(entity_mgr)
        {
            mFileName = file_name;
            mRecursive = recursive;
            mEntityData = entity.genEntityData4SaveDb();
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleSave()
        {
            string str_json = JsonConvert.SerializeObject(mEntityData);
            byte[] str = System.Text.Encoding.Default.GetBytes(str_json);

            EbFileStream file_stream = mEntityMgr._getFileStream();
            bool result = file_stream.save(mFileName, str);
            if (!result)
            {
                // log error
            }
        }
    }
}
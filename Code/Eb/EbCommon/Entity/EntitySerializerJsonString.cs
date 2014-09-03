using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Eb
{
    public class EntityLoaderJsonString : EntityLoader
    {
        //---------------------------------------------------------------------
        JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string mJsonString = "";

        //---------------------------------------------------------------------
        public EntityLoaderJsonString(EntityMgr entity_mgr, string json_string, Entity parent = null, bool recursive = true)
            : base(entity_mgr, parent, recursive)
        {
            mJsonString = json_string;
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleLoad()
        {
            JsonReader reader = new JsonTextReader(new StringReader(mJsonString));
            JsonSerializer serializer = new JsonSerializer();
            mEntityData = serializer.Deserialize<EntityData>(reader);
        }
    }

    public class EntitySaverJsonString : EntitySaver
    {
        //---------------------------------------------------------------------
        JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string mJsonString = "";
        bool mRecursive = true;
        bool mIsDone = false;

        //---------------------------------------------------------------------
        public EntitySaverJsonString(EntityMgr entity_mgr, Entity entity, bool recursive = true)
            : base(entity_mgr)
        {
            mRecursive = recursive;
            mEntityData = entity.genEntityData4SaveDb();
        }

        //---------------------------------------------------------------------
        // 运行在"ThreadSerializer"线程中
        public override void handleSave()
        {
            JsonSerializer serializer = new JsonSerializer();
            StringWriter sw = new StringWriter();
            serializer.Serialize(new JsonTextWriter(sw), mEntityData);
            mJsonString = sw.ToString();
            mIsDone = true;
        }

        //---------------------------------------------------------------------
        public bool isDone()
        {
            return mIsDone;
        }

        //---------------------------------------------------------------------
        public string getData()
        {
            return mJsonString;
        }
    }
}
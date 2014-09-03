using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Enyim.Caching.Memcached;
using Couchbase;
using Couchbase.Configuration;
using log4net;
using Es;


public class DbCouchbase : IDbCouchbase
{
    //---------------------------------------------------------------------
    CouchbaseClient mCouchbaseClient = null;
    
    //---------------------------------------------------------------------
    public DbCouchbase()
    {
    }

    //---------------------------------------------------------------------
    public void setup(ref _tCouchbaseInfo couchbase_info)
    {
        try
        {
            CouchbaseClientConfiguration cfg = new CouchbaseClientConfiguration();
            cfg.HttpRequestTimeout = new TimeSpan(0, 0, 60);
            cfg.Bucket = couchbase_info.bucket_name;
            cfg.BucketPassword = couchbase_info.bucket_pwd;
            cfg.Username = couchbase_info.user;
            cfg.Password = couchbase_info.pwd;
            cfg.Urls.Add(new Uri(couchbase_info.url));
            mCouchbaseClient = new CouchbaseClient(cfg);
        }
        catch (System.Exception ex)
        {
        }
    }

    //---------------------------------------------------------------------
    public bool executeGet(string db_key, out string json_data)
    {
        Enyim.Caching.Memcached.Results.IGetOperationResult<string> ret = mCouchbaseClient.ExecuteGet<string>(db_key);
        if (ret.Success)
        {
            json_data = ret.Value;
            return true;
        }
        else
        {
            json_data = null;
            return false;
        }
    }

    //---------------------------------------------------------------------
    public bool executeStore(string db_key, string json_data)
    {
        Enyim.Caching.Memcached.Results.IStoreOperationResult ret =
            mCouchbaseClient.ExecuteStore(StoreMode.Set, db_key, json_data, Couchbase.Operations.PersistTo.Zero);
        if (ret.Success)
        {
            return true;
        }
        else
        {
            //mLog.Error("CEntitySerializerCouchbase.handleSave() error! ");
            //mLog.Error("save entity to db: entity_type=" + data.entity_type + " entity_id=" + data.entity_guid);
            //mLog.Error(ret.Message);
            //mLog.Error(json_map_prop);
            return false;
        }
    }
}

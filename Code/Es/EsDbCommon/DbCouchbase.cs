using System;
using System.Collections.Generic;
using System.Text;

namespace Es
{
    public interface IDbCouchbase
    {
        //---------------------------------------------------------------------
        void setup(ref _tCouchbaseInfo couchbase_info);

        //---------------------------------------------------------------------
        bool executeGet(string db_key, out string json_data);

        //---------------------------------------------------------------------
        bool executeStore(string db_key, string json_data);
    }
}

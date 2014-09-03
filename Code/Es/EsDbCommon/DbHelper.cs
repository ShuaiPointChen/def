using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Es
{
    enum DbClass
    {
        DbCouchbase = 0,
    }

    public struct _tCouchbaseInfo
    {
        public string url;
        public string bucket_name;
        public string bucket_pwd;
        public string user;
        public string pwd;
    }

    public static class DbHelper
    {
        public static IDbCouchbase setupDbCouchbase(string db_dll_file, ref _tCouchbaseInfo couchbase_info)
        {
            Assembly ass = Assembly.LoadFrom(db_dll_file);// 加载dll文件
            Type tp = ass.GetType(DbClass.DbCouchbase.ToString());// 获取类名，命名空间+类名
            IDbCouchbase dc = (IDbCouchbase)Activator.CreateInstance(tp);// 建立实例
            dc.setup(ref couchbase_info);
            return dc;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;

namespace Zk
{
    public class CZkConnection
    {
        //---------------------------------------------------------------------
        private static readonly int DEFAULT_SESSION_TIMEOUT = 30000;
        private IntPtr mZookeeper = IntPtr.Zero;// ZooKeeper Handler.
        private string mServer;
        private int mSessionTimeOut;

        //---------------------------------------------------------------------
        public CZkConnection(String zkServers) : this(zkServers, DEFAULT_SESSION_TIMEOUT) { }

        //---------------------------------------------------------------------
        public CZkConnection(String zkServers, int sessionTimeOut)
        {
            mServer = zkServers;
            mSessionTimeOut = sessionTimeOut;
        }

        //---------------------------------------------------------------------
        public void connect(zookeeper.watcher_fn fn)
        {
            if (mZookeeper != IntPtr.Zero)
            {
                //mLog.Info("CZKClient  has already been created!");
                return;
            }
            //mLog.Info("Creating new ZookKeeper instance to connect to " + mServer + ".");
            mZookeeper = zookeeper.zookeeper_init(mServer, fn, mSessionTimeOut, IntPtr.Zero, null, 0);
        }

        //---------------------------------------------------------------------
        public void close()
        {
            if (mZookeeper != IntPtr.Zero)
            {
                //mLog.Info("Closing ZooKeeper connected to " + mServer);
                zookeeper.zookeeper_close(mZookeeper);
                mZookeeper = IntPtr.Zero;
            }
        }

        //---------------------------------------------------------------------
        //mode : ZK_CONST.ZOO_EPHEMERAL , ZK_CONST.ZOO_SEQUENCE
        public int acreate(string path, string data, int mode, zookeeper.string_completion_t handler)
        {
            if (data == null) data = "";
            return zookeeper.zoo_acreate(mZookeeper, path, data, data.Length,
                           ZK_CONST.ZOO_OPEN_ACL_UNSAFE, mode, handler,
                           Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int adelete(string path, zookeeper.void_completion_t handler)
        {
            return zookeeper.zoo_adelete(mZookeeper, path, -1, handler, Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int aexists(string path, bool watch, zookeeper.stat_completion_t handler)
        {
            return zookeeper.zoo_aexists(mZookeeper, path,
                    watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, handler, Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int agetChildren(string path, bool watch, zookeeper.strings_completion_t handler)
        {
            return zookeeper.zoo_aget_children(mZookeeper, path,
                watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, handler, Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int areadData(string path, bool watch, zookeeper.data_completion_t handler)
        {
            return zookeeper.zoo_aget(mZookeeper, path,
                watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, handler, Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int awriteData(string path, string data, zookeeper.stat_completion_t handler)
        {
            return zookeeper.zoo_aset(mZookeeper, path, data, data.Length, -1, handler, Marshal.StringToHGlobalAnsi(path));
        }

        //---------------------------------------------------------------------
        public int getState()
        {
            return mZookeeper != IntPtr.Zero ? zookeeper.zoo_state(mZookeeper) : 0;
        }

        //---------------------------------------------------------------------
        public string getServer()
        {
            return mServer;
        }

        //------------ 同步接口，用于初始化---------------------
        //---------------------------------------------------------------------
        public string screate(string path, string value, int flags)
        {
            string node = null;
            int rtCode = zookeeper.zoo_create(mZookeeper, path, value, value.Length,
                ZK_CONST.ZOO_OPEN_ACL_UNSAFE, flags, ZK_CONST.STR_BUFFER, ZK_CONST.ZOO_STR_BUF_LEN);
            if (rtCode == 0)
            {
                node = zookeeper.getString(ZK_CONST.STR_BUFFER);
            }
            return node;
        }

        //---------------------------------------------------------------------
        public bool sdelete(string path)
        {
            return zookeeper.zoo_delete(mZookeeper, path, -1) == 0;
        }

        //---------------------------------------------------------------------
        public string sread(string path, bool watch)
        {
            string data = null;
            int rtCode = zookeeper.zoo_get(mZookeeper, path, watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, ZK_CONST.STR_BUFFER, ZK_CONST.INT_BUFFER, IntPtr.Zero);
            if (rtCode == 0)
            {
                data = zookeeper.getString2(ZK_CONST.STR_BUFFER, ZK_CONST.INT_BUFFER);
            }
            return data;
        }

        //---------------------------------------------------------------------
        public int swrite(string path, string buffer)
        {
            int rtCode = zookeeper.zoo_set(mZookeeper, path, buffer, buffer.Length, -1);
            return rtCode;
        }

        //---------------------------------------------------------------------
        public string[] sget_children(string path, bool watch)
        {
            string[] rtVec = null;
            int rtCode = zookeeper.zoo_get_children(mZookeeper, path, watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, ZK_CONST.STRS_BUFFER);
            if (rtCode == 0)
            {
                rtVec = zookeeper.getString_vector(ZK_CONST.STRS_BUFFER);
            }
            return rtVec;
        }

        //---------------------------------------------------------------------
        public bool sexists(string path, bool watch)
        {
            return zookeeper.zoo_exists(mZookeeper, path, watch ? ZK_CONST.ADD_WATCH : ZK_CONST.NOT_WATCH, IntPtr.Zero) == 0;
        }
        //------------ 同步接口，用于初始化---------------------
    }
}

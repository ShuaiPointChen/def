using System;
using System.Collections.Generic;

namespace Zk
{
    // 当前ZooKeeper Clinet操作的反馈.
    public interface IZkOnOpeResult
    {
        //---------------------------------------------------------------------
        // 本地操作的反馈.
        void onCreated(string path, string fullPath, int result);

        //---------------------------------------------------------------------
        void onDeleted(string path, int result);

        //---------------------------------------------------------------------
        void onSet(string path, int result);

        //---------------------------------------------------------------------
        void onGet(string path, int result, string data);

        //---------------------------------------------------------------------
        void onExists(string path, int result);

        //---------------------------------------------------------------------
        void onGetChildren(string path, int result, string[] chdn);

        //---------------------------------------------------------------------
        // Zookeeper自动的反馈.
        void handler(WatchedEvent eve);
    }

    public class ResponseEvent
    {
        //---------------------------------------------------------------------
        public readonly int opeType;
        public readonly int returnCode;
        public readonly string path;
        public readonly string data;
        public readonly string[] children;

        //---------------------------------------------------------------------
        public ResponseEvent(int ope, int rc, string pat, string dat, string[] chr = null)
        {
            this.opeType = ope;
            this.returnCode = rc;
            this.path = pat;
            this.data = dat;
            this.children = chr;
        }
    }

    public interface IZkClient
    {
        //---------------------------------------------------------------------
        // 是否存在节点.
        int aexists(string path, bool watch);

        //---------------------------------------------------------------------
        // 创建节点,mode : ZK_CONST.ZOO_EPHEMERAL , ZK_CONST.ZOO_SEQUENCE
        int acreate(string path, string data, int mode);

        //---------------------------------------------------------------------
        // 删除节点
        int adelete(string path);

        //---------------------------------------------------------------------
        // 读取节点数据.
        int areadData(string path, bool watch);

        //---------------------------------------------------------------------
        // 写节点数据.
        int awriteData(string path, string data);

        //---------------------------------------------------------------------
        // 监视当前节点和子节点.
        int awatchForChilds(string path);

        //---------------------------------------------------------------------
        // 监视节点是否存在.
        void subscribeExists(string path);

        //---------------------------------------------------------------------
        // 取消监视节点是否存在.
        void unsubscribeExists(string path);

        //---------------------------------------------------------------------
        // 监视当前节点和子节点，并设置成持续监听.
        void subscribeChildChanges(string path);

        //---------------------------------------------------------------------
        // 取消child的监听.
        void unsubscribeChildChanges(string path);

        //---------------------------------------------------------------------
        // 监听当前节点的数据改变，并设置成持续监听.
        void subscribeDataChanges(string path);

        //---------------------------------------------------------------------
        // 取消data的监听.
        void unsubscribeDataChanges(string path);

        //---------------------------------------------------------------------
        // 关闭并销毁当前client.
        void close();

        //---------------------------------------------------------------------
        void Dispose();

        //---------------------------------------------------------------------
        // 要在心跳中调用.处理消息逻辑.
        void update();

        //---------------------------------------------------------------------
        // 设置关闭触发器.关闭后将不再处理Zookeeper主动发过来的消息.
        void setShutdownTrigger(bool triggerState);

        //---------------------------------------------------------------------
        bool getShutdownTrigger();

        //---------------------------------------------------------------------
        // 得到当前监听的节点数.
        int numberOfListeners();

        //---------------------------------------------------------------------
        // 获取当前连接的状态.
        int getState();

        //---------------------------------------------------------------------
        // 日志打印监听的节点.
        void dumpWatchPath();

        //---------------------------------------------------------------------
        string LoginQueueNode();

        //---------------------------------------------------------------------
        string LoginLockNode();

        //---同步接口----------
        //---------------------------------------------------------------------
        string screate(string path, string value, int flags);

        //---------------------------------------------------------------------
        bool sdelete(string path);

        //---------------------------------------------------------------------
        string sread(string path, bool watch);

        //---------------------------------------------------------------------
        int swrite(string path, string buffer);

        //---------------------------------------------------------------------
        string[] sget_children(string path, bool watch);

        //---------------------------------------------------------------------
        bool sexists(string path, bool watch);
        //---同步接口----------
    }
}

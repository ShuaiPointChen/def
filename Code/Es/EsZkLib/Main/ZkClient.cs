using System;
using System.Collections.Generic;

namespace Zk
{
    // 当前ZooKeeper Clinet操作的反馈.
    //public interface IZkOnOpeResult
    //{
        //---------------------------------------------------------------------
        // Zookeeper自动的反馈.
        //void handler(WatchedEvent eve);

    //}

    public delegate bool handler_func(WatchedEvent eve);

    public delegate void zkOpeHandler(int result, string data, string[] chdn, Dictionary<string, object> param);

    public class zkHandlerParam
    {
        public zkOpeHandler handler;
        public Dictionary<string, object> param;
    }

    public class ResponseEvent
    {
        //---------------------------------------------------------------------
        public readonly int opeType;
        public readonly int returnCode;
        //public readonly string path;
        public readonly string data;
        public readonly string[] children;
        public readonly int id;

        //---------------------------------------------------------------------
        public ResponseEvent(int ope, int rc,  string dat, int idx ,string[] chr = null)
        {
            this.opeType = ope;
            this.returnCode = rc;
            this.data = dat;
            this.children = chr;
            this.id = idx;
        }
    }

    public interface IZkClient
    {

        //---------------------------------------------------------------------
        // 获取处理操作类.
        //IZkOnOpeResult getZkOpeResult();

        /// <summary>
        /// 添加响应zk的函数.
        /// </summary>
        /// <param name="handler"></param>
        void addHandler(handler_func handler);

        //---------------------------------------------------------------------
        // 是否存在节点.
        int aexists(string path, bool watch, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 创建节点,mode : ZK_CONST.ZOO_EPHEMERAL , ZK_CONST.ZOO_SEQUENCE
        int acreate(string path, string data, int mode, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 删除节点
        int adelete(string path, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 读取节点数据.
        int areadData(string path, bool watch, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 写节点数据.
        int awriteData(string path, string data, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 监视当前节点和子节点.
        int awatchForChilds(string path, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 监视节点是否存在.
        void subscribeExists(string path, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 取消监视节点是否存在.
        void unsubscribeExists(string path);

        //---------------------------------------------------------------------
        // 监视当前节点和子节点，并设置成持续监听.
        void subscribeChildChanges(string path, zkOpeHandler handler, Dictionary<string, object> param = null);

        //---------------------------------------------------------------------
        // 取消child的监听.
        void unsubscribeChildChanges(string path);

        //---------------------------------------------------------------------
        // 监听当前节点的数据改变，并设置成持续监听.
        void subscribeDataChanges(string path, zkOpeHandler handler, Dictionary<string, object> param = null);

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

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
/************************************************************************/
/* ZooKeeper Version 1.0
 * 1. 
 *
 * 
 * 
/************************************************************************/

namespace Zk
{
    public class ZkClient : IZkClient
    {
        //---------------------------------------------------------------------
        protected CZkConnection mConnection = null;
        private bool _shutdownTriggered;
        private static volatile int _eventId = 0;
        private List<handler_func> _handlerList = new List<handler_func>();
        private Dictionary<string, zkHandlerParam> _childListener = new Dictionary<string, zkHandlerParam>();
        private Dictionary<string, zkHandlerParam> _dataListener = new Dictionary<string, zkHandlerParam>();
        private Dictionary<string, zkHandlerParam> _existsListener = new Dictionary<string, zkHandlerParam>();
        private ConcurrentQueue<ResponseEvent> _events = new ConcurrentQueue<ResponseEvent>();
        private ConcurrentQueue<WatchedEvent> _watchEvents = new ConcurrentQueue<WatchedEvent>();
        private ConcurrentDictionary<int, zkHandlerParam> _handlerDic = new ConcurrentDictionary<int, zkHandlerParam>();

        //private IZkOnOpeResult onOpeResult = null;

        //---------------------------------------------------------------------
        //public void setZkOpeResult(IZkOnOpeResult ope)
        //{
        //    onOpeResult = ope;
        //}

        //---------------------------------------------------------------------
        //public IZkOnOpeResult getZkOpeResult()
        //{
        //    return onOpeResult;
        //}
        //---------------------------------------------------------------------
        //public ZkClient(string serverstring, IZkOnOpeResult ope)
        public ZkClient(string serverstring)
            : this(serverstring, int.MaxValue >> 2)
        {
        }

        //---------------------------------------------------------------------
        //public ZkClient(string zkServers, int connectionTimeout, IZkOnOpeResult ope)
        public ZkClient(string zkServers, int connectionTimeout)
            : this(new CZkConnection(zkServers), connectionTimeout)
        {
        }

        //---------------------------------------------------------------------
        //public ZkClient(CZkConnection zkConnection, int connectionTimeout, IZkOnOpeResult ope)
        public ZkClient(CZkConnection zkConnection, int connectionTimeout)
        {
            ZK_CONST.Init();
            mConnection = zkConnection;
            //onOpeResult = ope;
            connect(connectionTimeout, watchFunction);
        }

        //---------------------------------------------------------------------
        public string LoginQueueNode() { return "/Queue"; }

        //---------------------------------------------------------------------
        public string LoginLockNode() { return "/Lock"; }

        //---------------------------------------------------------------------
        private void _deleteCompletion(int rc, IntPtr data)
        {
            //string path = zookeeper.getString(data);
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_deleteCompletion failed :" + id + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_DELETE_OP, rc, null, id, null));
            }
        }

        //---------------------------------------------------------------------
        private void _existsCompletion(int rc, IntPtr stat, IntPtr data)
        {
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {

            }
            _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_EXISTS_OP, rc,  null, id, null));
        }

        //---------------------------------------------------------------------
        private void _setCompletion(int rc, IntPtr stat, IntPtr data)
        {
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_setCompletion failed :" + id + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_SETDATA_OP, rc, null, id, null));
            }
        }

        //---------------------------------------------------------------------
        private void _createCompletion(int rc, IntPtr value, IntPtr data)
        {
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_createCompletion failed :" + id + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_CREATE_OP, rc, null, id, null));
            }
            else
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_CREATE_OP, rc, zookeeper.getString(value), id ,  null));
            }
        }

        //---------------------------------------------------------------------
        private void _getCompletion(int rc, IntPtr value, int value_len, IntPtr stat, IntPtr data)
        {
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETDATA_OP, rc, null, id ,null));
                EbLog.Note("_getCompletion failed :" + id + "reason:" + zookeeper.error2str(rc));
            }
            else
            {
                string val = zookeeper.getString2(value, value_len);
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETDATA_OP, rc, val, id, null));
            }
        }

        //---------------------------------------------------------------------
        private void _getChildrenCompletion(int rc, IntPtr strings, IntPtr data)
        {
            int id = zookeeper.getInt(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETCHILDREN_OP, rc, null, id, null));
                EbLog.Note("_getChildrenCompletion failed :" + id + "reason:" + zookeeper.error2str(rc));
            }
            else
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETCHILDREN_OP, rc, null, id, zookeeper.getString_vector(strings)));
            }
        }

        //---------------------------------------------------------------------
        protected int agetChildren(string path, bool watch, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd( id , hp);
            }
            return mConnection.agetChildren(path, watch, _getChildrenCompletion , id);
        }

        //---------------------------------------------------------------------
        public int aexists(string path, bool watch, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd(id, hp);
            }
            return mConnection.aexists(path, watch, _existsCompletion, id);
        }

        //---------------------------------------------------------------------
        public int acreate(string path, string data, int mode, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            if (path == null || data == null)
            {
                EbLog.Error("path must not be null.");
                return -1;
            }
            EbLog.Note("acreate node :" + path);
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd(id, hp);
            }
            return mConnection.acreate(path, data, mode, _createCompletion, id);
        }

        //---------------------------------------------------------------------
        public int adelete(string path, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd(id, hp);
            }
            return mConnection.adelete(path, _deleteCompletion, id);
        }

        //---------------------------------------------------------------------
        public int areadData(string path, bool watch, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd(id, hp);
            }
            return mConnection.areadData(path, watch, _getCompletion, id);
        }

        //---------------------------------------------------------------------
        public int awriteData(string path, string data, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            int id = 0;
            if (handler != null)
            {
                id = zookeeper.generateId();
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _handlerDic.TryAdd(id, hp);
            }
            return mConnection.awriteData(path, data, _setCompletion, id);
        }

        //---------------------------------------------------------------------
        public void subscribeChildChanges(string path, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            if (path != null && path != "" && !_childListener.ContainsKey(path))
            {
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _childListener.Add(path , hp);
                awatchForChilds(path, handler , param);
                EbLog.Note("Subscribed child changes for:" + path);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeChildChanges(string path)
        {
            if (_childListener.ContainsKey(path))
            {
                _childListener.Remove(path);
                EbLog.Note("unsubscribe child changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public void subscribeDataChanges(string path, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            if (path != null && path != "" && !_dataListener.ContainsKey(path))
            {
                EbLog.Note("Subscribed Data changes for:" + path);
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _dataListener.Add(path, hp);
                areadData(path, true , handler , param);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeDataChanges(string path)
        {
            if (_dataListener.ContainsKey(path))
            {
                _dataListener.Remove(path);
                EbLog.Note("unsubscribe Data changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public void subscribeExists(string path, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            if (path != null && path != "" && !_existsListener.ContainsKey(path))
            {
                EbLog.Note("Subscribed Exists changes for:" + path);
                zkHandlerParam hp = new zkHandlerParam();
                hp.handler = handler;
                hp.param = param;
                _existsListener.Add(path, hp);
                aexists(path, true , handler  , param);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeExists(string path)
        {
            if (_existsListener.ContainsKey(path))
            {
                _existsListener.Remove(path);
                EbLog.Note("unsubscribe Exists changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public int awatchForChilds(string path, zkOpeHandler handler = null, Dictionary<string, object> param = null)
        {
            //exists(path, true);
            return agetChildren(path, true, handler , param);
        }

        //---------------------------------------------------------------------
        private void watchFunction(IntPtr zh, int type, int state, string path, IntPtr watcherCtx)
        {
            if (getShutdownTrigger())
            {
                EbLog.Note("ignoring ev '{" + type + " | " + path + "}' since shutdown triggered");
                return;
            }

            if (type == (int)Zk.ZOO_EVENT.SESSION_EVENT_DEF)
            {
                if (state == (int)Zk.ZOO_STATE.CONNECTED_STATE_DEF)
                {
                    EbLog.Note("Connected to zookeeper service successfully!");
                }
                else if (state == (int)Zk.ZOO_STATE.EXPIRED_SESSION_STATE_DEF)
                {
                    EbLog.Error("Zookeeper session expired!");
                }
                return;
            }

            // 持续监听.
            if (_childListener.ContainsKey(path))
            {
                zkHandlerParam hp;
                _childListener.TryGetValue(path, out hp);
                if(hp != null)
                {
                    awatchForChilds(path, hp.handler, hp.param);
                }
                else
                {
                    awatchForChilds(path, null);
                }
                
            }
            if (_dataListener.ContainsKey(path))
            {
                zkHandlerParam hp;
                _childListener.TryGetValue(path, out hp);
                if(hp!=null)
                {
                    areadData(path, true, hp.handler, hp.param);
                }
                else 
                {
                    areadData(path, true, null);
                }
            }
            if (_existsListener.ContainsKey(path))
            {
                zkHandlerParam hp;
                _childListener.TryGetValue(path, out hp);
                if(hp != null)
                {
                    aexists(path, true, hp.handler, hp.param);
                }
            }

            WatchedEvent ev = new WatchedEvent(state, type, path);
            _watchEvents.Enqueue(ev);
        }

        //---------------------------------------------------------------------
        private void connect(int maxMsToWaitUntilConnected, zookeeper.watcher_fn watcher)
        {
            setShutdownTrigger(false);
            mConnection.connect(watcher);
            EbLog.Note("Awaiting connection to Zookeeper server");
        }

        //---------------------------------------------------------------------
        public void close()
        {
            if (mConnection == null)
            {
                return;
            }
            setShutdownTrigger(true);
            mConnection.close();
            mConnection = null;
            ZK_CONST.Release();
            EbLog.Note("Closing ZkClient...done");
        }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            close();
            ZK_CONST.Release();
        }

        //---------------------------------------------------------------------
        public int getEventCount()
        {
            return (_events.Count + _watchEvents.Count);
        }

        //---------------------------------------------------------------------
        public void update()
        {
            // 处理process消息.
            while (_events.Count > 0)
            {
                ResponseEvent response;
                _events.TryDequeue(out response);

                if (null == response) continue;

                //if (response.returnCode != (int)Zk.ZOO_ERRORS.ZOK)
                if(response.id <= 0)
                {
                    EbLog.Note("Zk.ZkClient.update ,path:" + response.id + ",Reason:" + ((Zk.ZOO_ERRORS)response.returnCode).ToString());
                    continue;
                }
                zkHandlerParam hp = null;
                //_handlerDic.TryGetValue(response.id, out handler);
                _handlerDic.TryRemove(response.id, out hp);
                if (null == hp || null == hp.handler)
                {
                    EbLog.Error("zk update error , hp or handler is null !");
                    continue;
                }
                int eventId = _eventId++;
                EbLog.Note("Delivering event #" + eventId + ",type:" + ((ZOO_OPE)(response.opeType)).ToString() + ",id:" + response.id);
                switch ((ZOO_OPE)response.opeType)
                {
                    case ZOO_OPE.ZOO_CREATE_OP:
                        //onOpeResult.onCreated(response.path, response.data, response.returnCode);
                        hp.handler(response.returnCode,  response.data, null , hp.param);
                        break;
                    case ZOO_OPE.ZOO_DELETE_OP:
                        //onOpeResult.onDeleted(response.path, response.returnCode);
                        hp.handler(response.returnCode, null, null, hp.param);
                        break;
                    case ZOO_OPE.ZOO_EXISTS_OP:
                        //onOpeResult.onExists(response.path, response.returnCode);
                        hp.handler(response.returnCode, null, null, hp.param);
                        break;
                    case ZOO_OPE.ZOO_GETDATA_OP:
                        //onOpeResult.onGet(response.path, response.returnCode, response.data);
                        hp.handler(response.returnCode, response.data, null, hp.param);
                        break;
                    case ZOO_OPE.ZOO_SETDATA_OP:
                        //onOpeResult.onSet(response.path, response.returnCode);
                        hp.handler(response.returnCode, null, null, hp.param);
                        break;
                    case ZOO_OPE.ZOO_GETCHILDREN_OP:
                        //onOpeResult.onGetChildren(response.path, response.returnCode, response.children);
                        hp.handler(response.returnCode, null, response.children, hp.param);
                        break;
                }

                EbLog.Note("Delivering event #" + eventId + " done");
            }

            while (_watchEvents.Count > 0)
            {
                WatchedEvent wcEvent;
                _watchEvents.TryDequeue(out wcEvent);
                if (null == wcEvent) continue;
                int eventId = _eventId++;
                EbLog.Note("Delivering event #" + eventId + "," + wcEvent);
                //if(null != onOpeResult)
                //{
                foreach (var func in _handlerList)
                {
                    // 如果已经处理了，就不用遍历了.
                    if (func(wcEvent))
                        break;
                }
                //onOpeResult.handler(wcEvent);
                //}
                EbLog.Note("Delivering event #" + eventId + " done");
            }
        }

        //---------------------------------------------------------------------
        public void setShutdownTrigger(bool triggerState)
        {
            _shutdownTriggered = triggerState;
        }

        //---------------------------------------------------------------------
        public bool getShutdownTrigger()
        {
            return _shutdownTriggered;
        }

        //---------------------------------------------------------------------
        public int numberOfListeners()
        {
            return (_childListener.Count + _dataListener.Count + _existsListener.Count);
        }

        //---------------------------------------------------------------------
        public int getState()
        {
            return mConnection.getState();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// 添加响应zk的函数.
        /// </summary>
        /// <param name="handler"></param>
        public void addHandler(handler_func handler)
        {
            _handlerList.Add(handler);
        }

        //---------------------------------------------------------------------
        public void dumpWatchPath()
        {
            EbLog.Note("watch for children, count:" + _childListener.Count);
            int index = 0;
            foreach (var child in _childListener)
            {
                EbLog.Note("index #" + index + ":" + child.Key);
            }

            index = 0;
            EbLog.Note("watch for data, count:" + _dataListener.Count);
            foreach (var data in _dataListener)
            {
                EbLog.Note("index #" + index + ":" + data.Key);
            }

            index = 0;
            EbLog.Note("watch for exists, count:" + _existsListener.Count);
            foreach (var data in _existsListener)
            {
                EbLog.Note("index #" + index + ":" + data.Key);
            }
        }

        //---------------------------------------------------------------------
        public string screate(string path, string value, int flags)
        {
            EbLog.Note("acreate node :" + path);
            return mConnection.screate(path, value, flags);
        }

        //---------------------------------------------------------------------
        public bool sdelete(string path)
        {
            return mConnection.sdelete(path);
        }

        //---------------------------------------------------------------------
        public string sread(string path, bool watch)
        {
            return mConnection.sread(path, watch);
        }

        //---------------------------------------------------------------------
        public int swrite(string path, string buffer)
        {
            return mConnection.swrite(path, buffer);
        }

        //---------------------------------------------------------------------
        public string[] sget_children(string path, bool watch)
        {
            return mConnection.sget_children(path, watch);
        }

        //---------------------------------------------------------------------
        public bool sexists(string path, bool watch)
        {
            return mConnection.sexists(path, watch);
        }
    }
}

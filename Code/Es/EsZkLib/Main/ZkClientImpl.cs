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
        private List<string> _childListener = new List<string>();
        private List<string> _dataListener = new List<string>();
        private List<string> _existsListener = new List<string>();
        private bool _shutdownTriggered;
        private static volatile int _eventId = 0;
        private ConcurrentQueue<ResponseEvent> _events = new ConcurrentQueue<ResponseEvent>();
        private ConcurrentQueue<WatchedEvent> _watchEvents = new ConcurrentQueue<WatchedEvent>();
        private IZkOnOpeResult onOpeResult = null;

        //---------------------------------------------------------------------
        public ZkClient(string serverstring, IZkOnOpeResult ope)
            : this(serverstring, int.MaxValue >> 2, ope)
        {
        }

        //---------------------------------------------------------------------
        public ZkClient(string zkServers, int connectionTimeout, IZkOnOpeResult ope)
            : this(new CZkConnection(zkServers), connectionTimeout, ope)
        {
        }

        //---------------------------------------------------------------------
        public ZkClient(CZkConnection zkConnection, int connectionTimeout, IZkOnOpeResult ope)
        {
            ZK_CONST.Init();
            mConnection = zkConnection;
            onOpeResult = ope;
            connect(connectionTimeout, watchFunction);
        }

        //---------------------------------------------------------------------
        public string LoginQueueNode() { return "/Queue"; }

        //---------------------------------------------------------------------
        public string LoginLockNode() { return "/Lock"; }

        //---------------------------------------------------------------------
        private void _deleteCompletion(int rc, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_deleteCompletion failed :" + path + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_DELETE_OP, rc, path, null, null));
            }
        }

        //---------------------------------------------------------------------
        private void _existsCompletion(int rc, IntPtr stat, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {

            }
            _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_EXISTS_OP, rc, path, null, null));
        }

        //---------------------------------------------------------------------
        private void _setCompletion(int rc, IntPtr stat, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_setCompletion failed :" + path + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_SETDATA_OP, rc, path, null, null));
            }
        }

        //---------------------------------------------------------------------
        private void _createCompletion(int rc, IntPtr value, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                EbLog.Note("_createCompletion failed :" + path + "reason:" + zookeeper.error2str(rc));
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_CREATE_OP, rc, path, null, null));
            }
            else
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_CREATE_OP, rc, path, zookeeper.getString(value), null));
            }
        }

        //---------------------------------------------------------------------
        private void _getCompletion(int rc, IntPtr value, int value_len, IntPtr stat, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETDATA_OP, rc, path, null, null));
                EbLog.Note("_getCompletion failed :" + path + "reason:" + zookeeper.error2str(rc));
            }
            else
            {
                string val = zookeeper.getString2(value, value_len);
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETDATA_OP, rc, path, val, null));
            }
        }

        //---------------------------------------------------------------------
        private void _getChildrenCompletion(int rc, IntPtr strings, IntPtr data)
        {
            string path = zookeeper.getString(data);
            if (data != IntPtr.Zero) zookeeper.FreeMem(data); data = IntPtr.Zero;
            if (rc != 0)
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETCHILDREN_OP, rc, path, null, null));
                EbLog.Note("_getChildrenCompletion failed :" + path + "reason:" + zookeeper.error2str(rc));
            }
            else
            {
                _events.Enqueue(new ResponseEvent((int)ZOO_OPE.ZOO_GETCHILDREN_OP, rc, path, null, zookeeper.getString_vector(strings)));
            }
        }

        //---------------------------------------------------------------------
        protected int agetChildren(string path, bool watch)
        {
            return mConnection.agetChildren(path, watch, _getChildrenCompletion);
        }

        //---------------------------------------------------------------------
        public int aexists(string path, bool watch)
        {
            return mConnection.aexists(path, watch, _existsCompletion);
        }

        //---------------------------------------------------------------------
        public int acreate(string path, string data, int mode)
        {
            if (path == null || data == null)
            {
                EbLog.Error("path must not be null.");
                return -1;
            }
            EbLog.Note("acreate node :" + path);
            return mConnection.acreate(path, data, mode, _createCompletion);
        }

        //---------------------------------------------------------------------
        public int adelete(string path)
        {
            return mConnection.adelete(path, _deleteCompletion);
        }

        //---------------------------------------------------------------------
        public int areadData(string path, bool watch)
        {
            return mConnection.areadData(path, watch, _getCompletion);
        }

        //---------------------------------------------------------------------
        public int awriteData(string path, string data)
        {
            return mConnection.awriteData(path, data, _setCompletion);
        }

        //---------------------------------------------------------------------
        public void subscribeChildChanges(string path)
        {
            if (path != null && path != "" && !_childListener.Contains(path))
            {
                _childListener.Add(path);
                awatchForChilds(path);
                EbLog.Note("Subscribed child changes for:" + path);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeChildChanges(string path)
        {
            if (_childListener.Contains(path))
            {
                _childListener.Remove(path);
                EbLog.Note("unsubscribe child changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public void subscribeDataChanges(string path)
        {
            if (path != null && path != "" && !_dataListener.Contains(path))
            {
                EbLog.Note("Subscribed Data changes for:" + path);
                _dataListener.Add(path);
                areadData(path, true);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeDataChanges(string path)
        {
            if (_dataListener.Contains(path))
            {
                _dataListener.Remove(path);
                EbLog.Note("unsubscribe Data changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public void subscribeExists(string path)
        {
            if (path != null && path != "" && !_existsListener.Contains(path))
            {
                EbLog.Note("Subscribed Exists changes for:" + path);
                _existsListener.Add(path);
                aexists(path, true);
            }
        }

        //---------------------------------------------------------------------
        public void unsubscribeExists(string path)
        {
            if (_existsListener.Contains(path))
            {
                _existsListener.Remove(path);
                EbLog.Note("unsubscribe Exists changes for " + path);
            }
        }

        //---------------------------------------------------------------------
        public int awatchForChilds(string path)
        {
            //exists(path, true);
            return agetChildren(path, true);
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
            if (_childListener.Contains(path))
            {
                awatchForChilds(path);
            }
            if (_dataListener.Contains(path))
            {
                areadData(path, true);
            }
            if (_existsListener.Contains(path))
            {
                aexists(path, true);
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

                if (response.returnCode != (int)Zk.ZOO_ERRORS.ZOK)
                {
                    EbLog.Error("Zk.ZkClient.update ,path:" + response.path + ",Reason:" + ((Zk.ZOO_ERRORS)response.returnCode).ToString());
                    continue;
                }

                int eventId = _eventId++;
                EbLog.Note("Delivering event #" + eventId + ",type:" + ((ZOO_OPE)(response.opeType)).ToString() + ",path:" + response.path);
                switch ((ZOO_OPE)response.opeType)
                {
                    case ZOO_OPE.ZOO_CREATE_OP:
                        onOpeResult.onCreated(response.path, response.data, response.returnCode);
                        break;
                    case ZOO_OPE.ZOO_DELETE_OP:
                        onOpeResult.onDeleted(response.path, response.returnCode);
                        break;
                    case ZOO_OPE.ZOO_EXISTS_OP:
                        onOpeResult.onExists(response.path, response.returnCode);
                        break;
                    case ZOO_OPE.ZOO_GETDATA_OP:
                        onOpeResult.onGet(response.path, response.returnCode, response.data);
                        break;
                    case ZOO_OPE.ZOO_SETDATA_OP:
                        onOpeResult.onSet(response.path, response.returnCode);
                        break;
                    case ZOO_OPE.ZOO_GETCHILDREN_OP:
                        onOpeResult.onGetChildren(response.path, response.returnCode, response.children);
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
                onOpeResult.handler(wcEvent);
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
        public void dumpWatchPath()
        {
            EbLog.Note("watch for children, count:" + _childListener.Count);
            int index = 0;
            foreach (string child in _childListener)
            {
                EbLog.Note("index #" + index + ":" + child);
            }

            index = 0;
            EbLog.Note("watch for data, count:" + _dataListener.Count);
            foreach (string data in _dataListener)
            {
                EbLog.Note("index #" + index + ":" + data);
            }

            index = 0;
            EbLog.Note("watch for exists, count:" + _existsListener.Count);
            foreach (string data in _existsListener)
            {
                EbLog.Note("index #" + index + ":" + data);
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

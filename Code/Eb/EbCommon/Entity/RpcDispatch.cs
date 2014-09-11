using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Eb
{
    class RpcDispatch
    {
        //---------------------------------------------------------------------
        private EntityMgr mEntityMgr;
        private volatile bool mSignDestroy = false;
        private Queue<_tEntityRpcData> mQueRecvData = new Queue<_tEntityRpcData>();
        private object mLockQueRecvData = new object();
        private _tEntityRpcData mEntityRpcData;

        //---------------------------------------------------------------------
        public void create(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;
            mSignDestroy = false;

            mEntityRpcData.rpc_id = 0;
            mEntityRpcData.map_param = null;
            mEntityRpcData.session_recv = null;
        }

        //---------------------------------------------------------------------
        public void destroy()
        {
            mSignDestroy = true;

            lock (mLockQueRecvData)
            {
                mQueRecvData.Clear();
            }
        }

        //---------------------------------------------------------------------
        public void update(float elapsed_tm)
        {
            if (mSignDestroy) return;

            bool done = false;
            while (!done)
            {
                lock (mLockQueRecvData)
                {
                    if (mQueRecvData.Count > 0)
                    {
                        mEntityRpcData = mQueRecvData.Dequeue();
                    }
                    else
                    {
                        done = true;
                    }
                }

                if (!done)
                {
                    _onEntityRpcData(ref mEntityRpcData);
                }
            }
        }

        //---------------------------------------------------------------------
        public void onRecvRpcData(ref _tEntityRpcData data)
        {
            if (mSignDestroy) return;

            lock (mLockQueRecvData)
            {
                mQueRecvData.Enqueue(data);
            }
        }

        //---------------------------------------------------------------------
        // 该函数执行在主线程中
        void _onEntityRpcData(ref _tEntityRpcData data)
        {
            byte cmd_id = 0;
            byte to_node = 0;
            ulong entity_rpcid = 0;
            _tEntityRpcData.to(data.rpc_id, out cmd_id, out to_node, out entity_rpcid);
            ushort method_id = (ushort)data.method_id;
            _eRpcCmd c = (_eRpcCmd)cmd_id;

            // 需要路由转发
            if (mEntityMgr.NodeType != to_node)
            {
                if (cmd_id == (byte)_eRpcCmd.NodeMethod)
                {
                    // log warning
                    return;
                }

                Entity et_proxy = data.session_recv.getProxyEntity();
                if (et_proxy != null)
                {
                    RpcSession s = et_proxy.getSession(to_node);
                    if (s == null) return;
                    s.sendEntityRpcData(ref data);
                    return;
                }

                Entity et = mEntityMgr.findEntity(entity_rpcid);
                if (et != null)
                {
                    RpcSession s = et.getSession(to_node);
                    if (s == null) return;
                    s.sendEntityRpcData(ref data);
                    return;
                }
            }

            // 无需路由转发
            if (cmd_id == (byte)_eRpcCmd.EntityMethod)
            {
                Entity et = mEntityMgr.findEntity(entity_rpcid);
                if (et == null)
                {
                    // log warning
                    return;
                }
                else
                {
                    et._rpcOnEntityMethod(data.session_recv, data.from_node, method_id, data.map_param);
                }
            }
            else if (cmd_id == (byte)_eRpcCmd.ComponentSyncProp)
            {
                Entity et = mEntityMgr.findEntity(entity_rpcid);
                if (et == null)
                {
                    // log warning
                    return;
                }
                else
                {
                    ushort reason = (ushort)(short)data.map_param[0];
                    Dictionary<string, string> map_prop = (Dictionary<string, string>)data.map_param[1];
                    et._rpcOnComponentSyncProp(data.session_recv, data.from_node, method_id, reason, map_prop);
                }
            }
            else if (cmd_id == (byte)_eRpcCmd.EntityCreate)
            {
                // 需要检查entity的创建权限，默认为不可远程创建

                Entity et = mEntityMgr.findEntity(entity_rpcid);
                if (et != null)
                {
                    // log warning
                    return;
                }

                EntityData entity_data = new EntityData();
                entity_data.entity_type = (string)data.map_param[0];
                entity_data.entity_guid = (string)data.map_param[1];
                entity_data.entity_rpcid = (ulong)(long)data.map_param[2];
                entity_data.cache_data = null;
                entity_data.entity_children = null;
                entity_data.entity_transform = new EntityTransform();
                entity_data.entity_transform.fromDic((Dictionary<byte, object>)data.map_param[3]);
                entity_data.list_component = new List<ComponentData>();
                ComponentData co_data = new ComponentData();
                co_data.component_name = (string)data.map_param[4];
                co_data.def_propset = null;
                entity_data.list_component.Add(co_data);

                EntityMgrListener listener = mEntityMgr._getListener();
                if (listener == null) return;
                Entity et_new = listener.onRpcEntityCreateRemote(data.session_recv, 
                    entity_data, data.method_id == 0 ? false : true);
                
                var ev = mEntityMgr.getDefaultEventPublisher().genEvent<EvEntityCreateRemote>();
                ev.entity = et_new;
                ev.entity_data = entity_data;
                ev.send(null);
            }
            else if (cmd_id == (byte)_eRpcCmd.EntityDestroy)
            {
                // 需要检查entity的销毁权限，默认为不可远程销毁

                Entity et = mEntityMgr.findEntity(entity_rpcid);
                if (et == null) return;

                mEntityMgr.destroyEntity(entity_rpcid);
            }
            else if (cmd_id == (byte)_eRpcCmd.NodeMethod)
            {
                EntityMgrListener listener = mEntityMgr._getListener();
                if (listener == null) return;
                listener.onRpcNodeMethod(data.session_recv, data.from_node, method_id, data.map_param);
            }
            else
            {
                EbLog.Error("RpcDispatch._onEntityRpcData() Error! cmd_id=" + cmd_id.ToString());
            }
        }
    }
}
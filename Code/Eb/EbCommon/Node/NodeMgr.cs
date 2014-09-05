using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using EventDataXML;
using Eb;

public class CNodeMgr : IDisposable
{
    //-------------------------------------------------------------------------
    INodeServerListener mpNodeServerListener;
    INodeClientListener mpNodeClientListener;
    CNodeSys mNodeSys;
    Entity mEtPlayer;
    Dictionary<int, CNode> mMapAllNode = new Dictionary<int, CNode>();// 包含运行时所有父和子Node
    Queue<_tNodeOp> mQueNodeOp = new Queue<_tNodeOp>();
    Queue<_tNodeOp> mQueNodeOpRemote = new Queue<_tNodeOp>();
    bool mbPaused = false;
    bool mIsClient = false;

    public bool IsHandledMsg { get; set; }

    //-------------------------------------------------------------------------
    ~CNodeMgr()
    {
        this.Dispose(false);
    }

    //-------------------------------------------------------------------------
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    //-------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
        _destroy();
    }

    //-------------------------------------------------------------------------
    // 初始化，根据List<_tNodeInfo>创建所有Node
    public void setListNodeInfo(List<_tNodeInfo> list_nodeinfo)
    {
        if (list_nodeinfo == null || list_nodeinfo.Count == 0)
        {
            if (!mIsClient)
            {
                CNodeConfig cfg = mNodeSys._getConfig();
                _opCreateNode(cfg.getStartEntity(), _eNodeState.Init, -1);
            }
        }
        else
        {
            _setListNodeInfo(list_nodeinfo);
        }

        updateQueNodeOp(mQueNodeOp);
    }

    //-------------------------------------------------------------------------
    // 获取所有NodeInfo
    public List<_tNodeInfo> getListNodeInfo()
    {
        List<_tNodeInfo> list_nodeinfo = new List<_tNodeInfo>();

        foreach (var i in mMapAllNode)
        {
            if (!mNodeSys.hasParentNodeDef(i.Value.getNodeId()))
            {
                list_nodeinfo.Add(i.Value._getNodeInfo());
            }
        }

        return list_nodeinfo;
    }

    //-------------------------------------------------------------------------
    public Dictionary<int, CNode> getAllNodes()
    {
        return mMapAllNode;
    }

    //-------------------------------------------------------------------------
    // 更新队列中所有Node操作
    public void updateQueNodeOp(Queue<_tNodeOp> que_node_op)
    {
        if (!mIsClient)
        {
            mQueNodeOpRemote.Clear();    
        }
        
        if (que_node_op == null) return;
        while (que_node_op.Count > 0)
        {
            _tNodeOp node_op = que_node_op.Dequeue();
            switch (node_op.op)
            {
                case _eNodeOp.CreateNode:
                    {
                        if (!mMapAllNode.ContainsKey(node_op.id))
                        {
                            CNode node = new CNode();
                            node.create(node_op.id, node_op.state, node_op.list_param,
                                mpNodeServerListener, mpNodeClientListener, mNodeSys, this);
                            mMapAllNode[node.getNodeId()] = node;

                            if (!mIsClient)
                            {
                                mQueNodeOpRemote.Enqueue(node_op);
                            }
                        }
                        else
                        {
                            EbLog.Error("CNodeMgr.updateQueNodeOp() _eNodeOp.CreateNode exist node_id=" + node_op.id);
                        }
                    }
                    break;
                case _eNodeOp.DestroyNode:
                    {
                        if (mMapAllNode.ContainsKey(node_op.id))
                        {
                            CNode node = mMapAllNode[node_op.id];
                            mMapAllNode.Remove(node_op.id);
                            node.Dispose();
                        }
                        else
                        {
                            EbLog.Error("CNodeMgr.updateQueNodeOp() _eNodeOp.DestroyNode not exist node_id=" + node_op.id);
                        }
                    }
                    break;
                case _eNodeOp.EnterState:
                    {
                        if (mMapAllNode.ContainsKey(node_op.id))
                        {
                            CNode node = mMapAllNode[node_op.id];
                            node.setNodeSate(node_op);

                            if (!mIsClient)
                            {
                                mQueNodeOpRemote.Enqueue(node_op);
                            }
                        }
                        else
                        {
                            EbLog.Error("CNodeMgr.updateQueNodeOp() _eNodeOp.EnterState not exist node_id=" + node_op.id);
                        }
                    }
                    break;
            }
        }
    }

    //-------------------------------------------------------------------------
    // 获取Entity
    public Entity getEtPlayer()
    {
        return mEtPlayer;
    }

    //-------------------------------------------------------------------------
    // 判定是否是客户端
    public bool isClient()
    {
        return mIsClient;
    }

    //-------------------------------------------------------------------------
    // 从远端接收到消息
    public void recvMsgFromRemote(int node_id, int msg_type, int msg_id, int msg_param, List<object> msg_paramlist)
    {
        foreach (var i in mMapAllNode)
        {
            if (i.Key == node_id)
            {
                i.Value._recvMsgFromRemote(msg_type, msg_id, msg_param, msg_paramlist);
                break;
            }
        }
    }

    //-------------------------------------------------------------------------
    public void handleMsg(int msg_id, List<object> vec_param)
    {
        IsHandledMsg = false;
        foreach (var i in mMapAllNode)
        {
            i.Value.processEvent("main_sendmsg", msg_id, vec_param);
        }
    }

    //-------------------------------------------------------------------------
    public CNode findNode(int node_id)
    {
        foreach (var i in mMapAllNode)
        {
            if (i.Key == node_id) return i.Value;
        }

        return null;
    }

    //-------------------------------------------------------------------------
    public CNode findFirstNodeByType(string node_type)
    {
        foreach (var i in mMapAllNode)
        {
            if (i.Value.getNodeType() == node_type) return i.Value;
        }

        return null;
    }

    //-------------------------------------------------------------------------
    public List<CNode> findMultipleNodeByType(string node_type)
    {
        List<CNode> l = new List<CNode>();
        foreach (var i in mMapAllNode)
        {
            if (i.Value.getNodeType() == node_type)
            {
                l.Add(i.Value);
            }
        }

        return l;
    }

    //-------------------------------------------------------------------------
    public void pause()
    {
        mbPaused = true;
    }

    //-------------------------------------------------------------------------
    public void resume()
    {
        mbPaused = false;
    }

    //-------------------------------------------------------------------------
    public bool isPaused()
    {
        return mbPaused;
    }

    //-------------------------------------------------------------------------
    // Server专用接口
    public void serverOnClientSelectExit(int node_id, bool all_exit, int exit_id)
    {
        foreach (var i in mMapAllNode)
        {
            if (i.Key == node_id)
            {
                if (all_exit)
                {
                    i.Value.serverSelectAllExits();
                }
                else
                {
                    i.Value.serverSelectExit(exit_id);
                }

                break;
            }
        }
    }

    //-------------------------------------------------------------------------
    internal void _create(CNodeSys node_sys, INodeServerListener server_listener, INodeClientListener client_listener, Entity et_player)
    {
        mNodeSys = node_sys;
        mpNodeServerListener = server_listener;
        mpNodeClientListener = client_listener;
        mEtPlayer = et_player;
        mIsClient = mNodeSys.isClient();
    }

    //-------------------------------------------------------------------------
    internal void _destroy()
    {
        // 保存List<_tNodeInfo>
        if (mpNodeServerListener != null)
        {
            List<_tNodeInfo> list_nodeinfo = getListNodeInfo();
            mpNodeServerListener.nodeSaveNodeInfoList(list_nodeinfo);
        }

        // 销毁NodeList中所有元素
        foreach (var i in mMapAllNode)
        {
            i.Value.Dispose();
        }
        mMapAllNode.Clear();
        mQueNodeOp.Clear();

        // 销毁其他对象
        mNodeSys = null;
        mpNodeServerListener = null;
        mpNodeClientListener = null;
    }

    //-------------------------------------------------------------------------
    internal void _update(float elapsed_tm)
    {
        // 暂定则返回
        if (mbPaused) return;

        // 服务端应用队列中所有操作
        bool need_save = false;
        if (mQueNodeOp.Count > 0)
        {
            // 处理mQueNodeOp中所有元素
            updateQueNodeOp(mQueNodeOp);

            // 把mQueNodeOp发送给Client
            if (!mIsClient && mpNodeServerListener != null)
            {
                mpNodeServerListener.nodeUpdateNodeOpQue(mQueNodeOpRemote);
            }

            need_save = true;
        }

        // 更新AllNode
        foreach (var i in mMapAllNode)
        {
            i.Value.processEvent("main_update", elapsed_tm);
        }

        // 保存List<_tNodeInfo>
        if (!mIsClient && need_save)
        {
            if (mpNodeServerListener != null)
            {
                List<_tNodeInfo> list_nodeinfo = getListNodeInfo();
                mpNodeServerListener.nodeSaveNodeInfoList(list_nodeinfo);
            }
        }
    }

    //-------------------------------------------------------------------------
    private void _setListNodeInfo(List<_tNodeInfo> list_nodeinfo)
    {
        if (list_nodeinfo == null) return;

        foreach (var i in list_nodeinfo)
        {
            _opCreateNode(i);

            _setListNodeInfo(i.list_child);
        }
    }

    //-------------------------------------------------------------------------
    internal void _opCreateNode(int node_id, _eNodeState state, int pre_node_id)
    {
        _tNodeOp node_op;
        node_op.op = _eNodeOp.CreateNode;
        node_op.id = node_id;
        node_op.state = state;
        node_op.list_param = new List<_tNodeParamPair>();
        _tNodeParamPair pp;
        pp.k = (byte)_eNodeParam.PreNodeId;
        pp.v = pre_node_id;
        node_op.list_param.Add(pp);

        mQueNodeOp.Enqueue(node_op);
    }

    //-------------------------------------------------------------------------
    internal void _opCreateNode(_tNodeInfo node_info)
    {
        _tNodeOp node_op;
        node_op.op = _eNodeOp.CreateNode;
        node_op.id = node_info.id;
        node_op.state = node_info.state;
        node_op.list_param = new List<_tNodeParamPair>();
        if (node_info.list_param != null)
        {
            foreach (var i in node_info.list_param)
            {
                node_op.list_param.Add(i);
            }
        }

        mQueNodeOp.Enqueue(node_op);
    }

    //-------------------------------------------------------------------------
    internal void _opDestroyNode(int node_id)
    {
        _tNodeOp node_op;
        node_op.op = _eNodeOp.DestroyNode;
        node_op.id = node_id;
        node_op.state = _eNodeState.Release;
        node_op.list_param = null;

        mQueNodeOp.Enqueue(node_op);
    }

    //-------------------------------------------------------------------------
    internal void _opEnterState(int node_id, _eNodeState state, List<_tNodeParamPair> list_param = null)
    {
        _tNodeOp node_op;
        node_op.op = _eNodeOp.EnterState;
        node_op.id = node_id;
        node_op.state = state;
        node_op.list_param = list_param;

        mQueNodeOp.Enqueue(node_op);
    }
}
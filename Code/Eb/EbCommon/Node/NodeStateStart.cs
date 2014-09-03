using System;
using System.Collections.Generic;
using System.Text;
using EventDataXML;
using Eb;

class CNodeStateStart : EbState
{
    //-------------------------------------------------------------------------
    CNode mNode = null;
    INodeServerScript mNodeServerScript = null;
    INodeClientScript mNodeClientScript = null;

    //-------------------------------------------------------------------------
    public CNodeStateStart(CNode node)
    {
        mNode = node;

        _defState("CNodeStateStart", "Fsm", 0, false);

        _bindAction("main_update", new EbAction(this.evMainUpdate));
        _bindAction("main_sendmsg", new EbAction(this.evMainSendMsg));
        _bindAction("evSetRunState", new EbAction(this.evSetRunState));
    }

    //-------------------------------------------------------------------------
    public override void enter()
    {
        EbLog.Note("CNodeStateStart.enter() NodeType=" + mNode.getNodeType() + " NodeId=" + mNode.getNodeId());

        mNodeServerScript = mNode._getNodeServerScript();
        mNodeClientScript = mNode._getNodeClientScript();
        mNode._setNodeState(_eNodeState.Start);

        // 广播NodeEnterStart消息
        if (!mNode.getNodeSys().isClient())
        {
            List<object> list_param = new List<object>();
            list_param.Add(mNode.getNodeType());
            list_param.Add(mNode.getNodeId());
            mNode._getNodeServerListener().nodeSendMsg((int)_eNodeMsg.NodeEnterStart, list_param);
        }

        // 执行脚本函数
        if (mNodeServerScript != null)
        {
            mNodeServerScript.onEnterStartState(mNode);
        }
        else if (mNodeClientScript != null)
        {
            mNodeClientScript.onEnterStartState(mNode);
        }
    }

    //-------------------------------------------------------------------------
    public override void exit()
    {
        EbLog.Note("CNodeStateStart.exit() NodeType=" + mNode.getNodeType() + " NodeId=" + mNode.getNodeId());
    }

    //-------------------------------------------------------------------------
    // event: 每帧更新
    public string evMainUpdate(IEbEvent ev)
    {
        if (!mNode.getNodeSys().isClient())
        {
            // 更新所有触发器
            EbEvent1<float> evt = ev as EbEvent1<float>;
            float elasped_tm = evt.param1;
            List<INodeTrigger> trigger_list = mNode._getTriggerList();
            foreach (var i in trigger_list)
            {
                i.update(elasped_tm);
            }

            // 检查有没有触发器触发
            bool triggered = false;
            foreach (var i in trigger_list)
            {
                triggered = i.triggered();
                if (triggered) break;
            }
            if (trigger_list.Count == 0) triggered = true;
            foreach (var i in trigger_list)
            {
                i.reset();
            }

            // 检查触发条件是否满足
            bool triggered_condition = false;
            if (triggered)
            {
                triggered_condition = _checkTriggerCondition();
            }

            // 触发器触发了即执行脚本on_trigger函数
            bool trigger_ss = true;
            if (triggered)
            {
                if (mNodeServerScript != null)
                {
                    trigger_ss = mNodeServerScript.canExitStartState(mNode);
                }
            }

            // 触发结果
            bool can_run = false;
            if (triggered && triggered_condition && trigger_ss)
            {
                can_run = true;
            }

            if (can_run)
            {
                mNode.getNodeMgr()._opEnterState(mNode.getNodeId(), _eNodeState.Run);
            }
        }

        return "";
    }

    //-------------------------------------------------------------------------
    // event: 响应消息
    public string evMainSendMsg(IEbEvent ev)
    {
        EbEvent2<int, List<object>> evt = ev as EbEvent2<int, List<object>>;
        int msg_id = evt.param1;
        List<object> vec_param = evt.param2;

        List<INodeTrigger> trigger_list = mNode._getTriggerList();
        foreach (var i in trigger_list)
        {
            i.handleMsg(msg_id, vec_param);
        }

        // 检查有没有触发器触发
        foreach (var i in trigger_list)
        {
            if (i.triggered())
            {
                mNode.getNodeMgr().IsHandledMsg = true;
            }
        }

        return "";
    }

    //-------------------------------------------------------------------------
    // event: 设置进入运行状态
    public string evSetRunState(IEbEvent ev)
    {
        return "CNodeStateRun";
    }

    //-------------------------------------------------------------------------
    // 检查触发条件
    private bool _checkTriggerCondition()
    {
        EventDef entity_def = mNode.getDefXml();
        var trigger_group = entity_def.GetGroup("TrigCondition");
        if (trigger_group == null) return true;
        if (trigger_group.Groups.Count == 0) return true;

        bool blank = true;
        bool and = true;
        {
            Property p = trigger_group.GetValue("Conjunction");
            if (p.Value == "2") and = false;
        }

        foreach (Group g in trigger_group.Groups)
        {
            INodeTriggerCondition tc = mNode.getNodeSys().getEntityTriggerCondition(g.Key);
            if (tc != null)
            {
                tc.setEntity(mNode);
                bool result = tc.excute(g);
                if (and)
                {
                    if (!result) return false;
                }
                else
                {
                    if (result) return true;
                }
                blank = false;
            }
        }

        if (blank)
        {
            return true;
        }

        if (and)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
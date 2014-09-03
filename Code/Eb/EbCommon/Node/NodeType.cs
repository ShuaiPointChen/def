using System;
using System.Collections.Generic;
using System.Text;
using Eb;

//-----------------------------------------------------------------------------
[Serializable]
public enum _eNodeState : byte
{
    Init = 0,
    Start,
    Run,
    Stop,
    Release
}

//-----------------------------------------------------------------------------
[Serializable]
public enum _eNodeParam : byte
{
    PreNodeId = 0,
    AllExit,
    ExitId
}

//-----------------------------------------------------------------------------
[Serializable]
public enum _eNodeOp : byte
{
    CreateNode = 0,
    DestroyNode,
    EnterState
}

//-----------------------------------------------------------------------------
[Serializable]
public struct _tNodeParamPair
{
    public byte k;
    public object v;
}

//-----------------------------------------------------------------------------
[Serializable]
public struct _tNodeInfo
{
    public int id;
    public _eNodeState state;
    public List<_tNodeParamPair> list_param;
    public List<_tNodeInfo> list_child;
}

//-----------------------------------------------------------------------------
[Serializable]
public struct _tNodeOp
{
    public _eNodeOp op;
    public int id;
    public _eNodeState state;
    public List<_tNodeParamPair> list_param;
}

//-----------------------------------------------------------------------------
public enum _eNodeMsg
{
    NodeEnterStart = 0,
    NodeEnterRun,
    NodeLeaveStop
}

//-----------------------------------------------------------------------------
public struct _tMsgInfo
{
    public int msg_type;
    public int msg_id;
}
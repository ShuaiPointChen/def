using System;


public abstract class ViAsynDelegateInterface
{
	public static void Update()
	{
		while (_AsynExecList.IsNotEmpty())
		{
			ViAsynDelegateInterface asynCallback = _AsynExecList.GetHead().Data;
			ViDebuger.AssertError(asynCallback);
			asynCallback._node.Detach();
			asynCallback._node.Data = null;
			asynCallback._AsynExec();
		}
		_AsynExecList.Clear();
		ViDebuger.AssertWarning(_AsynExecList.IsEmpty());
	}
	static private ViDoubleLink2<ViAsynDelegateInterface> _AsynExecList = new ViDoubleLink2<ViAsynDelegateInterface>();
	//
	public bool Active { get { return _node.IsAttach(); } }
	public void End()
	{
		_node.Data = null;
		_node.Detach();
	}

	protected void _AttachAsyn()
	{
		_node.Data = this;
		_AsynExecList.PushBack(_node);
	}
	internal abstract void _AsynExec();
	ViDoubleLinkNode2<ViAsynDelegateInterface> _node = new ViDoubleLinkNode2<ViAsynDelegateInterface>();
}
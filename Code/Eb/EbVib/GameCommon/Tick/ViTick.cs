using System;



public class ViTickNode : ViRefNode1<ViTickNode>
{
	//+----------------------------------------------------------------------------------------------------------------
	static public void Update(float deltaTime)
	{
		_tickList.BeginIterator();
		while (!_tickList.IsEnd())
		{
			ViTickNode tickNode = _tickList.CurrentNode as ViTickNode;
			ViDebuger.AssertError(tickNode);
			_tickList.Next();
			tickNode._delegate(deltaTime);
		}
	}
	static private ViRefList1<ViTickNode> _tickList = new ViRefList1<ViTickNode>();

	//+----------------------------------------------------------------------------------------------------------------
	public delegate void Callback(float deltaTime);
	public new bool IsAttach()
	{
		return base.IsAttach();
	}
	public void Attach(Callback dele)
	{
		_delegate = dele;
		_tickList.PushBack(this);
	}
	public new void Detach()
	{
		_delegate = null;
		base.Detach();
	}
	//
	private Callback _delegate;
}
using System;



public class ViLodTickNode : ViRefNode1<ViLodTickNode>
{
	//+----------------------------------------------------------------------------------------------------------------
	static public void Update(float deltaTime)
	{
		_tickList.BeginIterator();
		while (!_tickList.IsEnd())
		{
			ViLodTickNode tickNode = _tickList.CurrentNode as ViLodTickNode;
			ViDebuger.AssertError(tickNode);
			_tickList.Next();
			tickNode.Exec(deltaTime);
		}
	}
	static private ViRefList1<ViLodTickNode> _tickList = new ViRefList1<ViLodTickNode>();

	//+----------------------------------------------------------------------------------------------------------------
	public delegate void Callback(float deltaTime);

	public float AccumulateTime { get { return _accumulateTime; } }

	public void SetSpan(float span)
	{
		_span = span;
	}
	public void ClearAccumulateTime()
	{
		_accumulateTime = 0.0f;
	}
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
		_accumulateTime = 0.0f;
		base.Detach();
	}
	//
	private void Exec(float deltaTime)
	{
		_accumulateTime += deltaTime;
		if (_accumulateTime >= _span)
		{
			_accumulateTime -= _span;
			_delegate(_span);
		}
	}
	private Callback _delegate;
	private float _span;
	private float _accumulateTime;
}

public class Demo_LodTickNode
{
#pragma warning disable 0219
	public class Listener
	{
		public void Func(float deltaTime)
		{
			Console.Write("Func");
			Console.WriteLine(deltaTime);
		}
		public ViLodTickNode _node = new ViLodTickNode();
	}

	public static void Test()
	{
		Listener listener = new Listener();
		listener._node.Attach(listener.Func);
		listener._node.SetSpan(1.0f);
		ViLodTickNode.Update(1.1f);
		ViLodTickNode.Update(0.1f);
	}
#pragma warning restore 0219
}
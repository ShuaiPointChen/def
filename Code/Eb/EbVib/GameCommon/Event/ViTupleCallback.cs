using System;

public interface ViTupleCallbackInterface
{
	bool Active { get; }
	void End();
	void OnCallerClear();
	void Exec(UInt32 eventID, ViTupleInterface tuple);
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTupleCallback : ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID);
	public Callback Delegate { get { return _delegate; } }
	public bool Active { get { return _node.IsAttach(); } }
	//
	public void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViDebuger.AssertError(_delegate);
		_delegate(eventID);
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	Callback _delegate;
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViAsynTupleCallback : ViAsynDelegateInterface, ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID);
	public Callback Delegate { get { return _delegate; } }
	public new bool Active { get { return _node.IsAttach(); } }
	public bool AsynActive { get { return base.Active; } }
	//
	public new void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
		//
		_asynDele = null;
		base.End();
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec()
	{
		if (AsynActive)
		{
			base.End();
			_AsynExec();
		}
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViDebuger.AssertError(_delegate);
		_asynDele = _delegate;
		_eventID = eventID;
		_AttachAsyn();
	}
	internal override void _AsynExec()
	{
		ViDebuger.AssertError(_asynDele);
		Callback dele = _asynDele;
		_asynDele = null;
		dele(_eventID);
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		End();
		//
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	private Callback _delegate;
	Callback _asynDele;
	UInt32 _eventID;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTupleCallback<T0> : ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID, T0 param0);
	public Callback Delegate { get { return _delegate; } }
	public bool Active { get { return _node.IsAttach(); } }
	//
	public void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViTuple<T0> tupleAlias = tuple as ViTuple<T0>;
		if (tupleAlias != null)
		{
			ViDebuger.AssertError(_delegate);
			_delegate(eventID, tupleAlias._value0);
		}
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	Callback _delegate;
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViAsynTupleCallback<T0> : ViAsynDelegateInterface, ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID, T0 param0);
	public Callback Delegate { get { return _delegate; } }
	public new bool Active { get { return _node.IsAttach(); } }
	public bool AsynActive { get { return base.Active; } }
	//
	public new void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
		//
		_asynDele = null;
		_param0 = default(T0);
		base.End();
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec()
	{
		if (AsynActive)
		{
			base.End();
			_AsynExec();
		}
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViDebuger.AssertError(_delegate);
		ViTuple<T0> tupleAlias = tuple as ViTuple<T0>;
		if (tupleAlias != null)
		{
			_asynDele = _delegate;
			_eventID = eventID;
			_param0 = tupleAlias._value0;
			_AttachAsyn();
		}
	}
	internal override void _AsynExec()
	{
		ViDebuger.AssertError(_asynDele);
		Callback dele = _asynDele;
		T0 param0 = _param0;
		_asynDele = null;
		_param0 = default(T0);
		dele(_eventID, param0);
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		End();
		//
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	private Callback _delegate;
	Callback _asynDele;
	UInt32 _eventID;
	T0 _param0;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTupleCallback<T0, T1> : ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID, T0 param0, T1 param1);
	public Callback Delegate { get { return _delegate; } }
	public bool Active { get { return _node.IsAttach(); } }
	//
	public void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViTuple<T0, T1> tupleAlias = tuple as ViTuple<T0, T1>;
		if (tupleAlias != null)
		{
			ViDebuger.AssertError(_delegate);
			_delegate(eventID, tupleAlias._value0, tupleAlias._value1);
		}
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	Callback _delegate;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViAsynTupleCallback<T0, T1> : ViAsynDelegateInterface, ViTupleCallbackInterface
{
	public delegate void Callback(UInt32 eventID, T0 param0, T1 param1);
	public Callback Delegate { get { return _delegate; } }
	public new bool Active { get { return _node.IsAttach(); } }
	public bool AsynActive { get { return base.Active; } }
	//
	public new void End()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
		//
		_asynDele = null;
		_param0 = default(T0);
		_param1 = default(T1);
		base.End();
	}
	public void OnCallerClear()
	{
		_delegate = null;
		_node.Detach();
		_node.Data = null;
	}
	public void Exec()
	{
		if (AsynActive)
		{
			base.End();
			_AsynExec();
		}
	}
	public void Exec(UInt32 eventID, ViTupleInterface tuple)
	{
		ViDebuger.AssertError(_delegate);
		ViTuple<T0, T1> tupleAlias = tuple as ViTuple<T0, T1>;
		if (tupleAlias != null)
		{
			_asynDele = _delegate;
			_eventID = eventID;
			_param0 = tupleAlias._value0;
			_param1 = tupleAlias._value1;
			_AttachAsyn();
		}
	}
	internal override void _AsynExec()
	{
		ViDebuger.AssertError(_asynDele);
		Callback dele = _asynDele;
		T0 param0 = _param0;
		T1 param1 = _param1;
		_asynDele = null;
		_param0 = default(T0);
		_param1 = default(T1);
		dele(_eventID, param0, param1);
	}
	internal void Attach(Callback dele, ViRefList2<ViTupleCallbackInterface> list)
	{
		End();
		//
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViTupleCallbackInterface> _node = new ViRefNode2<ViTupleCallbackInterface>();
	private Callback _delegate;
	Callback _asynDele;
	UInt32 _eventID;
	T0 _param0;
	T1 _param1;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViAsynEventTupleList
{
	public void _Invoke(UInt32 eventID, ViTupleInterface tuple)
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViTupleCallbackInterface callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.Exec(eventID, tuple);
		}
	}
	public void Invoke(UInt32 eventID)
	{
		_Invoke(eventID, null);
	}
	public void Invoke<T0>(UInt32 eventID, T0 param0)
	{
		ViTuple<T0> tuple = new ViTuple<T0>();
		tuple._value0 = param0;
		_Invoke(eventID, tuple);
	}
	public void Invoke<T0, T1>(UInt32 eventID, T0 param0, T1 param1)
	{
		ViTuple<T0, T1> tuple = new ViTuple<T0, T1>();
		tuple._value0 = param0;
		tuple._value1 = param1;
		_Invoke(eventID, tuple);
	}
	public void Clear()
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViTupleCallbackInterface callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.OnCallerClear();
		}
		ViDebuger.AssertWarning(_eventList.IsEmpty());
	}
	public void Attach(ViAsynTupleCallback node, ViAsynTupleCallback.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Attach<T0>(ViAsynTupleCallback<T0> node, ViAsynTupleCallback<T0>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Attach<T0, T1>(ViAsynTupleCallback<T0, T1> node, ViAsynTupleCallback<T0, T1>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	//
	protected ViRefList2<ViTupleCallbackInterface> _eventList = new ViRefList2<ViTupleCallbackInterface>();
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViEventTupleList : ViAsynEventTupleList
{
	public void Attach(ViTupleCallback node, ViTupleCallback.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Attach<T0>(ViTupleCallback<T0> node, ViTupleCallback<T0>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Attach<T0, T1>(ViTupleCallback<T0, T1> node, ViTupleCallback<T0, T1>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViAsynTupleCaller
{
	public void SetSize(UInt32 size)
	{
		_eventList.Resize(size);
	}
	public void Clear()
	{
		for (UInt32 idx = 0; idx < _eventList.Size; ++idx)
		{
			_eventList.Get(idx).Clear();
		}
		_eventList.Clear();
	}
	public void _Invoke(UInt32 eventId, ViTupleInterface tuple)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList._Invoke(eventId, tuple);
	}
	public void Invoke(UInt32 eventId)
	{
		_Invoke(eventId, null);
	}
	public void Invoke<T0>(UInt32 eventId, T0 param0)
	{
		ViTuple<T0> tuple = new ViTuple<T0>();
		tuple._value0 = param0;
		_Invoke(eventId, tuple);
	}
	public void Invoke<T0, T1>(UInt32 eventId, T0 param0, T1 param1)
	{
		ViTuple<T0, T1> tuple = new ViTuple<T0, T1>();
		tuple._value0 = param0;
		tuple._value1 = param1;
		_Invoke(eventId, tuple);
	}
	public void Attach(UInt32 eventId, ViAsynTupleCallback node, ViAsynTupleCallback.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
	public void Attach<T0>(UInt32 eventId, ViAsynTupleCallback<T0> node, ViAsynTupleCallback<T0>.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
	public void Attach<T0, T1>(UInt32 eventId, ViAsynTupleCallback<T0, T1> node, ViAsynTupleCallback<T0, T1>.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
	//
	protected ViSimpleVector<ViEventTupleList> _eventList = new ViSimpleVector<ViEventTupleList>();
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTupleCaller : ViAsynTupleCaller
{
	public void Attach(UInt32 eventId, ViTupleCallback node, ViTupleCallback.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
	public void Attach<T0>(UInt32 eventId, ViTupleCallback<T0> node, ViTupleCallback<T0>.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
	public void Attach<T0, T1>(UInt32 eventId, ViTupleCallback<T0, T1> node, ViTupleCallback<T0, T1>.Callback dele)
	{
		ViEventTupleList eventList = _eventList.Get(eventId);
		if (eventList == null)
		{
			return;
		}
		eventList.Attach(node, dele);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class Demo_TupleCallback
{
#pragma warning disable 0219
	class Listener
	{
		public void Func(UInt32 eventID, int i) { }
		public void Func(UInt32 eventID, int i, float f) { }
		public ViTupleCallback<int> _node1a = new ViTupleCallback<int>();
		public ViAsynTupleCallback<int> _node1b = new ViAsynTupleCallback<int>();
		public ViTupleCallback<int, float> _node2a = new ViTupleCallback<int, float>();
		public ViAsynTupleCallback<int, float> _node2b = new ViAsynTupleCallback<int, float>();
	}

	public static void Test()
	{
		Listener listener = new Listener();
		ViTupleCaller caller = new ViTupleCaller();
		caller.SetSize(10);
		caller.Attach(0, listener._node1a, listener.Func);
		caller.Attach(0, listener._node1b, listener.Func);
		caller.Attach(0, listener._node2a, listener.Func);
		caller.Attach(0, listener._node2b, listener.Func);
		int i = 3;
		float f = 6.0f;
		caller.Invoke(0, i, f);
		ViAsynDelegateInterface.Update();
	}
#pragma warning restore 0219
}
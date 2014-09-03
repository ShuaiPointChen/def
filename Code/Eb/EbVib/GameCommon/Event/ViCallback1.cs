using System;


public interface ViCallbackInterface<T0>
{
	bool Active { get; }
	void End();
	void OnCallerClear();
	void Exec(UInt32 eventID, T0 param0);
}

public class ViCallback<T0> : ViCallbackInterface<T0>
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
	public void Exec(UInt32 eventID, T0 param0)
	{
		ViDebuger.AssertError(_delegate);
		_delegate(eventID, param0);
	}
	internal void Attach(Callback dele, ViRefList2<ViCallbackInterface<T0>> list)
	{
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViCallbackInterface<T0>> _node = new ViRefNode2<ViCallbackInterface<T0>>();
	Callback _delegate;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViAsynCallback<T0> : ViAsynDelegateInterface, ViCallbackInterface<T0>
{
	public delegate void Callback(UInt32 eventId, T0 param0);
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
	public void Exec(UInt32 eventID, T0 param0)
	{
		ViDebuger.AssertError(_delegate);
		_asynDele = _delegate;
		_eventID = eventID;
		_param0 = param0;
		_AttachAsyn();
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
	internal void Attach(Callback dele, ViRefList2<ViCallbackInterface<T0>> list)
	{
		End();
		//
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViCallbackInterface<T0>> _node = new ViRefNode2<ViCallbackInterface<T0>>();
	Callback _delegate;
	Callback _asynDele;
	UInt32 _eventID;
	T0 _param0;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViEventAsynList<T0>
{
	public void Invoke(UInt32 eventId, T0 param0)
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViCallbackInterface<T0> callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.Exec(eventId, param0);
		}
	}
	public void Attach(ViAsynCallback<T0> node, ViAsynCallback<T0>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Clear()
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViCallbackInterface<T0> callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.OnCallerClear();
		}
		ViDebuger.AssertWarning(_eventList.IsEmpty());
	}
	//
	protected ViRefList2<ViCallbackInterface<T0>> _eventList = new ViRefList2<ViCallbackInterface<T0>>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViEventList<T0> : ViEventAsynList<T0>
{
	public void Attach(ViCallback<T0> node, ViCallback<T0>.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
}

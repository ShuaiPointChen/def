using System;



public interface ViCallbackInterface
{
	bool Active { get; }
	void End();
	void OnCallerClear();
	void Exec(UInt32 eventID);
}

public class ViCallback : ViCallbackInterface
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
	public void Exec(UInt32 eventID)
	{
		ViDebuger.AssertError(_delegate);
		_delegate(eventID);
	}
	internal void Attach(Callback dele, ViRefList2<ViCallbackInterface> list)
	{
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViCallbackInterface> _node = new ViRefNode2<ViCallbackInterface>();
	Callback _delegate;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViAsynCallback : ViAsynDelegateInterface, ViCallbackInterface
{
	public delegate void Callback(UInt32 eventId);
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
	public void Exec(UInt32 eventID)
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
	internal void Attach(Callback dele, ViRefList2<ViCallbackInterface> list)
	{
		End();
		//
		_delegate = dele;
		_node.Data = this;
		list.PushBack(_node);
	}
	//
	ViRefNode2<ViCallbackInterface> _node = new ViRefNode2<ViCallbackInterface>();
	Callback _delegate;
	Callback _asynDele;
	UInt32 _eventID;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViEventAsynList
{
	public void Invoke(UInt32 eventId)
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViCallbackInterface callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.Exec(eventId);
		}
	}
	public void Attach(ViAsynCallback node, ViAsynCallback.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
	public void Clear()
	{
		_eventList.BeginIterator();
		while (!_eventList.IsEnd())
		{
			ViCallbackInterface callback = _eventList.CurrentNode.Data;
			ViDebuger.AssertError(callback);
			_eventList.Next();
			callback.OnCallerClear();
		}
		ViDebuger.AssertWarning(_eventList.IsEmpty());
	}
	//
	protected ViRefList2<ViCallbackInterface> _eventList = new ViRefList2<ViCallbackInterface>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViEventList : ViEventAsynList
{
	public void Attach(ViCallback node, ViCallback.Callback dele)
	{
		node.Attach(dele, _eventList);
	}
}
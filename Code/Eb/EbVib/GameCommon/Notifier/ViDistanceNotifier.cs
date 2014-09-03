using System;



//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViRootRanger
{
	public ViVector3 Root
	{
		get { return _rootPos; }
		set { _rootPos = value; }
	}
	public void SetRange(float fRange)
	{
		_range2 = fRange * fRange;
	}
	public bool IsIn(ref ViVector3 pos)
	{
		return (_range2 >= ViVector3.Distance2(_rootPos, pos));
	}
	//
	private ViVector3 _rootPos;
	private float _range2;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDistNotifierInterface
{
	public enum State
	{
		NONE,
		INSIDE,
		OUTSIDE,
	}
	public static State GetState(ViDistNotifierInterface notifier)
	{
		if (notifier._target0 == null || notifier._target1 == null)
		{
			return State.NONE;
		}
		float distance2 = ViVector3.Distance2(notifier._target0.Value, notifier._target1.Value);
		if (distance2 < notifier._range2)
		{
			return State.INSIDE;
		}
		else
		{
			return State.OUTSIDE;
		}
	}
	public void SetTarget(ViProvider<ViVector3> target0, ViProvider<ViVector3> target1)
	{
		ViDebuger.AssertWarning(target0 != null && target1 != null);
		_target0 = target0;
		_target1 = target1;
	}
	public void SetRange(float fRange)
	{
		_range2 = fRange * fRange;
	}
	public float GetDistance()
	{
		if (_target0 != null && _target1 != null)
		{
			return ViVector3.Distance(_target0.Value, _target1.Value);
		}
		else
		{
			return 0.0f;
		}
	}
	public float GetDistance2()
	{
		if (_target0 != null && _target1 != null)
		{
			return ViVector3.Distance2(_target0.Value, _target1.Value);
		}
		else
		{
			return 0.0f;
		}
	}
	public bool IsIn()
	{
		if (_target0 != null && _target1 != null)
		{
			return ViVector3.Distance2(_target0.Value, _target1.Value) < _range2;
		}
		else
		{
			return false;
		}
	}
	public bool IsOut()
	{
		if (_target0 != null && _target1 != null)
		{
			return ViVector3.Distance2(_target0.Value, _target1.Value) > _range2;
		}
		else
		{
			return false;
		}
	}
	public float Range { get { return ViMathDefine.Sqrt(_range2); } }
	public float Range2 { get { return _range2; } }
	//
	protected ViProvider<ViVector3> _target0;
	protected ViProvider<ViVector3> _target1;
	protected float _range2;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDistNotifier : ViDistNotifierInterface, ViNotifierInterface
{
	public delegate void Callback();
	public static void UpdateList()
	{
		_listInner.BeginIterator();
		while (!_listInner.IsEnd())
		{
			ViRefNode2<ViDistNotifier> iter = _listInner.CurrentNode;
			_listInner.Next();
			ViDistNotifier notifier = iter.Data;
			ViDebuger.AssertError(notifier);
			notifier._UpdateInside();
		}
		_listOutor.BeginIterator();
		while (!_listOutor.IsEnd())
		{
			ViRefNode2<ViDistNotifier> iter = _listOutor.CurrentNode;
			_listOutor.Next();
			ViDistNotifier _this = iter.Data;
			ViDebuger.AssertError(_this);
			_this._UpdateOutside();
		}
		_listInner.PushBack(_listWaitingInner);
		_listOutor.PushBack(_listWaitingOutor);
	}
	private static ViRefList2<ViDistNotifier> _listInner = new ViRefList2<ViDistNotifier>();
	private static ViRefList2<ViDistNotifier> _listOutor = new ViRefList2<ViDistNotifier>();
	private static ViRefList2<ViDistNotifier> _listWaitingInner = new ViRefList2<ViDistNotifier>();
	private static ViRefList2<ViDistNotifier> _listWaitingOutor = new ViRefList2<ViDistNotifier>();
	//
	public bool IsAttach()
	{
		return _node.IsAttach();
	}
	public void AttachUpdate(Callback deleIn, Callback deleOut)
	{
		if (_target0 == null || _target1 == null)
		{
			return;
		}
		_node.Data = this;
		_delegateIn = deleIn;
		_delegateOut = deleOut;
		float distance2 = ViVector3.Distance2(_target0.Value, _target1.Value);
		if (distance2 < _range2)
		{
			_listInner.PushBack(_node);
		}
		else
		{
			_listOutor.PushBack(_node);
		}
	}
	public void DetachUpdate()
	{
		_delegateIn = null;
		_delegateOut = null;
		_node.Data = null;
		_node.Detach();
	}
	//
	public void Notify()
	{
		if (IsIn())
		{
			if (_delegateIn != null) { _delegateIn(); }
		}
		else if (IsOut())
		{
			if (_delegateOut != null) { _delegateOut(); }
		}
	}
	//
	private void _UpdateOutside()
	{
		ViDebuger.AssertError(_target0 != null && _target1 != null);
		if (ViVector3.Distance2(_target0.Value, _target1.Value) < _range2)
		{
			_listWaitingInner.PushBack(_node);
			if (_delegateIn != null) { _delegateIn(); }
		}
	}
	private void _UpdateInside()
	{
		ViDebuger.AssertError(_target0 != null && _target1 != null);
		if (ViVector3.Distance2(_target0.Value, _target1.Value) > _range2)
		{
			_listWaitingOutor.PushBack(_node);
			if (_delegateOut != null) { _delegateOut(); }
		}
	}
	//
	private Callback _delegateIn;
	private Callback _delegateOut;
	private ViRefNode2<ViDistNotifier> _node = new ViRefNode2<ViDistNotifier>();
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDistOutNotifier : ViDistNotifierInterface, ViNotifierInterface
{
	public delegate void Callback();
	public static void UpdateList()
	{
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViRefNode2<ViDistOutNotifier> iter = _list.CurrentNode;
			_list.Next();
			ViDistOutNotifier notifier = iter.Data;
			ViDebuger.AssertError(notifier);
			notifier._Update();
		}
	}
	private static ViRefList2<ViDistOutNotifier> _list = new ViRefList2<ViDistOutNotifier>();
	//
	public bool IsAttach()
	{
		return _node.IsAttach();
	}
	public void AttachUpdate(Callback dele)
	{
		if (_target0 == null || _target1 == null)
		{
			return;
		}
		_node.Data = this;
		_delegate = dele;
		_list.PushBack(_node);
	}
	public void DetachUpdate()
	{
		_delegate = null;
		_node.Data = null;
		_node.Detach();
	}
	//
	public void Notify()
	{
		if (IsOut())
		{
			if (_delegate != null) { _delegate(); }
		}
	}
	//
	private void _Update()
	{
		ViDebuger.AssertError(_target0 != null && _target1 != null);
		if (ViVector3.Distance2(_target0.Value, _target1.Value) > _range2)
		{
			_node.Detach();
			if (_delegate != null) { _delegate(); }
		}
	}
	//
	private Callback _delegate;
	private ViRefNode2<ViDistOutNotifier> _node = new ViRefNode2<ViDistOutNotifier>();
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDistInNotifier : ViDistNotifierInterface, ViNotifierInterface
{
	public delegate void Callback();
	public static void UpdateList()
	{
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViRefNode2<ViDistInNotifier> iter = _list.CurrentNode;
			_list.Next();
			ViDistInNotifier notifier = iter.Data;
			ViDebuger.AssertError(notifier);
			notifier._Update();
		}
	}
	private static ViRefList2<ViDistInNotifier> _list = new ViRefList2<ViDistInNotifier>();
	//
	public bool IsAttach()
	{
		return _node.IsAttach();
	}
	public void AttachUpdate(Callback dele)
	{
		if (_target0 == null || _target1 == null)
		{
			return;
		}
		_node.Data = this;
		_delegate = dele;
		_list.PushBack(_node);
	}
	public void DetachUpdate()
	{
		_delegate = null;
		_node.Data = null;
		_node.Detach();
	}
	//
	public void Notify()
	{
		if (IsIn())
		{
			if (_delegate != null) { _delegate(); }
		}
	}
	//
	private void _Update()
	{
		ViDebuger.AssertError(_target0 != null && _target1 != null);
		if (ViVector3.Distance2(_target0.Value, _target1.Value) < _range2)
		{
			_node.Detach();
			if (_delegate != null) { _delegate(); }
		}
	}
	//
	private Callback _delegate;
	private ViRefNode2<ViDistInNotifier> _node = new ViRefNode2<ViDistInNotifier>();
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class Demo_DistanceNotifier
{
#pragma warning disable 0219
	class Listener
	{
		public void OnIn()
		{

		}
		public void OnOut()
		{

		}
	}

	public static void Test()
	{
		Listener listener = new Listener();
		ViSimpleProvider<ViVector3> provider0 = new ViSimpleProvider<ViVector3>();
		ViSimpleProvider<ViVector3> provider1 = new ViSimpleProvider<ViVector3>();
		provider0.SetValue(new ViVector3(0, 0, 0));
		provider0.SetValue(new ViVector3(0, 0, 0));
		ViDistNotifier distanceNotifier = new ViDistNotifier();
		distanceNotifier.SetRange(10.0f);
		distanceNotifier.SetTarget(provider0, provider1);
		distanceNotifier.AttachUpdate(listener.OnIn, listener.OnOut);
		ViDistNotifier.UpdateList();
		provider1.SetValue(new ViVector3(11, 0, 0));
		ViDistNotifier.UpdateList();
		provider1.SetValue(new ViVector3(9, 0, 0));
		ViDistNotifier.UpdateList();
	}
#pragma warning restore 0219
}
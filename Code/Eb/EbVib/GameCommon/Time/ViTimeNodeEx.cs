using System;



using ViTime64 = System.Int64;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViTimeNode1 : ViTimeNodeInterface
{
	public delegate void Callback(ViTimeNodeInterface node);
	public Callback Delegate { get { return _delegate; } }
	public new void Detach()
	{
		_delegate = null;
		_execTime = 0;
		base.Detach();
	}
	//
	internal void _SetDelegate(Callback dele)
	{
		_delegate = dele;
	}
	internal override void _Exce(ViTimer timer)
	{
		Callback dele = _delegate;
		_delegate = null;
		_execTime = 0;
		dele(this);
	}
	private Callback _delegate;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViTimeNodeEx1<T> : ViTimeNodeInterface
{
	public delegate void Callback(ViTimeNodeInterface node, T param);
	//
	public Callback Delegate { get { return _delegate; } }
	public T Value
	{
		get { return _param; }
		set { _param = value; }
	}
	//
	public new void Detach()
	{
		_delegate = null;
		_execTime = 0;
		_param = default(T);
		base.Detach();
	}
	public T _param;
	//
	internal override void _Exce(ViTimer timer)
	{
		Callback dele = _delegate;
		_delegate = null;
		dele(this, _param);
		_execTime = 0;
		_param = default(T);
	}
	internal void _SetDelegate(Callback dele)
	{
		_delegate = dele;
	}
	//
	private Callback _delegate;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViTimeNode2 : ViTimeNodeInterface
{
	public delegate void Callback(ViTimeNodeInterface node);
	//
	public Callback Delegate
	{
		get { return _delegate; }
		set { _delegate = value; }
	}
	public UInt32 ReserveCnt { get { return _reserveCnt; } }
	public UInt32 Span { get { return _span; } }
	//
	public void Start(ViTimer timer, UInt32 cnt, UInt32 span)
	{
		if (cnt > 0)
		{
			_reserveCnt = cnt - 1;
			_span = span;
			SetTime(timer.Time + _span);
			timer.Add(this);
		}
	}
	public new bool GetReserveDuration(ViTimer timer, ref ViTime64 reserveTime)
	{
		if (base.GetReserveDuration(timer, ref reserveTime))
		{
			ViDebuger.AssertError(reserveTime > 0);
			reserveTime += _span * _reserveCnt;
			return true;
		}
		else
			return false;
	}
	public void SetReserveTime(ViTimer timer, UInt32 span, ViTime64 reserveTime)
	{
		if (span != 0)
		{
			_span = span;
			ViTime64 reserveTimeMod = (reserveTime > 0) ? (reserveTime - 1) : 0;
			_reserveCnt = (UInt32)(reserveTimeMod / _span);
			UInt32 reserveSpan = (UInt32)(reserveTimeMod % _span + 1);
			SetTime(timer.Time + (Int64)reserveSpan);
			timer.Add(this);
		}
	}
	public new void Detach()
	{
		_delegate = null;
		_span = 0;
		_reserveCnt = 0;
		base.Detach();
	}
	//
	internal override void _Exce(ViTimer timer)
	{
		if (_reserveCnt > 0)
		{
			--_reserveCnt;
			_delegate(this);
			SetTime(timer.Time + _span);
			timer.Add(this);
		}
		else
		{
			Callback dele = _delegate;
			_delegate = null;
			dele(this);
			_execTime = 0;
			_span = 0;
		}
	}
	//
	private UInt32 _span;
	private UInt32 _reserveCnt;
	private Callback _delegate;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViTimeNode3 : ViTimeNodeInterface
{
	public delegate void Callback(ViTimeNodeInterface node);
	//
	public Callback TickDelegate
	{
		get { return _tickDelegate; }
		set { _tickDelegate = value; }
	}
	public Callback EndDelegate
	{
		get { return _endDelegate; }
		set { _endDelegate = value; }
	}
	public UInt32 ReserveCnt { get { return _reserveCnt; } }
	public UInt32 Span { get { return _span; } }
	//
	public void Start(ViTimer timer, UInt32 cnt, UInt32 span)
	{
		if (cnt > 0)
		{
			_reserveCnt = cnt - 1;
			_span = span;
			SetTime(timer.Time + _span);
			timer.Add(this);
		}
	}
	internal override void _Exce(ViTimer timer)
	{
		if (_reserveCnt == 0)
		{
			Callback dele = _endDelegate;
			_tickDelegate = null;
			_endDelegate = null;
			dele(this);
			//
			_execTime = 0;
			_span = 0;
		}
		else
		{
			_tickDelegate(this);
			--_reserveCnt;
			SetTime(timer.Time + _span);
			timer.Add(this);
		}
	}
	public new bool GetReserveDuration(ViTimer timer, ref ViTime64 reserveTime)
	{
		if (base.GetReserveDuration(timer, ref reserveTime))
		{
			ViDebuger.AssertError(reserveTime > 0);
			reserveTime += (Int64)_span * _reserveCnt;
			return true;
		}
		else
			return false;
	}
	public void SetReserveTime(ViTimer timer, UInt32 span, ViTime64 reserveTime)
	{
		if (span != 0)
		{
			_span = span;
			ViTime64 reserveTimeMod = (reserveTime > 0) ? (reserveTime - 1) : 0;
			_reserveCnt = (UInt32)(reserveTimeMod / _span);
			UInt32 reserveSpan = (UInt32)(reserveTimeMod % _span + 1);
			SetTime(timer.Time + reserveSpan);
			timer.Add(this);
		}
	}
	public new void Detach()
	{
		_tickDelegate = null;
		_endDelegate = null;
		_execTime = 0;
		_span = 0;
		_reserveCnt = 0;
		base.Detach();
	}
	//
	private UInt32 _span;
	private UInt32 _reserveCnt;
	private Callback _tickDelegate;
	private Callback _endDelegate;
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class Demo_TimeNode
{
#pragma warning disable 0219
	public class Listener
	{
		public void Func(ViTimeNodeInterface node)
		{

		}
		public ViTimeNode1 _node = new ViTimeNode1();
	}

	public static void Test()
	{
		ViTimer timer = new ViTimer();
		Listener listener = new Listener();
		timer.Start(0, 10, 10, 10);
		listener._node.SetTime(10);
		listener._node._SetDelegate(listener.Func);
		timer.Add(listener._node);
		timer.Update(9);
		timer.Update(11);
		listener._node.Detach();
		timer.End();
	}
#pragma warning restore 0219
}
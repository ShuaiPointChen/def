using System;



using ViTime64 = System.Int64;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public abstract class ViTimeNodeInterface : ViDoubleLinkNode1<ViTimeNodeInterface>
{
	public ViTime64 Time
	{
		get { return _execTime; }
	}
	//
	public new bool IsAttach() { return base.IsAttach(); }
	public void SetTime(ViTime64 time) { _execTime = time; }
	public ViTime64 GetReserveDuration(ViTimer timer)
	{
		if (base.IsAttach())
		{
			return (_execTime > timer.Time) ? (_execTime - timer.Time) : 1;
		}
		else
		{
			return 0;
		}
	}
	public bool GetReserveDuration(ViTimer timer, ref ViTime64 reserveTime)
	{
		if (base.IsAttach())
		{
			reserveTime = (_execTime > timer.Time) ? (_execTime - timer.Time) : 1;
			return true;
		}
		else
		{
			reserveTime = 0;
			return false;
		}
	}
	internal abstract void _Exce(ViTimer timer);
	//
	protected ViTime64 _execTime;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

internal class TimeRoll
{
	public UInt32 Span { get { return _span; } }
	public ViTime64 TimeInf { get { return _timeInf; } }
	public ViTime64 TimeISup { get { return _timeSup; } }
	//
	public void Init(ViTime64 startTime, UInt32 rollSize, UInt32 span)
	{
		ViDebuger.AssertError(rollSize != 0);
		_timeListArray.Resize(rollSize);
		_span = span;
		_timeInf = startTime;
		_timeSup = startTime + span * rollSize;
	}
	public bool InRange(ViTime64 time)
	{
		return (_timeInf <= time && time < _timeSup);
	}
	public bool IsRoll()
	{
		return _idx == 0;
	}
	public void Add(ViTimeNodeInterface node)
	{
		ViTime64 time = node.Time;
		ViDebuger.AssertError(_timeInf <= time && time < _timeSup);
		UInt32 slot = _idx;
		if (time > _timeInf)
		{
			UInt32 deltaSlot = (UInt32)((time - _timeInf) / _span);
			slot = deltaSlot + _idx;
			ViDebuger.AssertError(deltaSlot < _timeListArray.Size);
			if (slot >= _timeListArray.Size)
			{
				slot -= _timeListArray.Size;
			}
		}
		ViDebuger.AssertError(slot < _timeListArray.Size);
		_timeListArray.Get(slot).PushBack(node);
	}
	public ViDoubleLink1<ViTimeNodeInterface> Current { get { return _timeListArray.Get(_idx); } }
	public UInt32 Next()
	{
		++_idx;
		if (_idx == _timeListArray.Size)
		{
			_idx = 0;
		}
		_timeInf += _span;
		_timeSup += _span;
		return _idx;
	}
	public void Clear()
	{
		for (UInt32 idx = 0; idx < _timeListArray.Size; ++idx)
		{
			_timeListArray.Get(idx).Clear();
		}
		_timeListArray.Clear();
		_timeInf = 0;
		_timeSup = 0;
		_timeListArray = null;
		_span = 0;
		_idx = 0;
	}
	//
	private ViTime64 _timeInf;
	private ViTime64 _timeSup;
	private UInt32 _span;
	private UInt32 _idx;
	ViSimpleVector<ViDoubleLink1<ViTimeNodeInterface>> _timeListArray = new ViSimpleVector<ViDoubleLink1<ViTimeNodeInterface>>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViTimer
{
	public ViTime64 Time { get { return _time; } }

	public void Start(ViTime64 startTime, UInt32 span, UInt32 rollSize0, UInt32 rollSize1)
	{
		_time = startTime;
		_roll0.Init(startTime, rollSize0, span);
		_roll1.Init(startTime, rollSize1, span * rollSize0);
	}
	public void End()
	{
		_roll0.Clear();
		_roll0 = null;
		_roll1.Clear();
		_roll1 = null;
		_reserveList.Clear();
		_reserveList = null;
		_time = 0;
	}
	public void Update(ViTime64 time)
	{
		if (_time >= time)
		{
			return;
		}
		//! Update迭代
		ViTime64 updateTime = _time;
		ViTime64 span = _roll0.Span;
		ViTime64 topTime = time - span;
		while (updateTime <= topTime)
		{
			//! 更新时间
			updateTime += span;
			if (_roll0.IsRoll())
			{
				_AddFastEvent(_roll1.Current, _roll0);
				if (_roll1.IsRoll())
				{
					_AddEvent(_reserveList, _roll1);
				}
				_roll1.Next();
			}
			_time = updateTime;
			ViDoubleLink1<ViTimeNodeInterface> currentTimeNodes = _roll0.Current;//必须先取出来
			_roll0.Next();
			_UpdateTimeList(currentTimeNodes);
		}
	}
	public void Add(ViTimeNodeInterface node)
	{
		if (node.Time < _roll0.TimeInf)
		{
			node.SetTime(_roll0.TimeInf);
		}
		if (_roll0.InRange(node.Time))
		{
			_roll0.Add(node);
			return;
		}
		if (_roll1.InRange(node.Time))
		{
			_roll1.Add(node);
			return;
		}
		_AddEvent(_reserveList, node);
	}
	//
	private static void _AddEvent(ViDoubleLink1<ViTimeNodeInterface> list, ViTimeNodeInterface node)
	{
		ViDoubleLinkNode1<ViTimeNodeInterface> iter = list.GetHead();
		while (!list.IsEnd(iter))
		{
			ViTimeNodeInterface timeNode = iter as ViTimeNodeInterface;
			ViDebuger.AssertError(timeNode);
			ViDoubleLink1<ViTimeNodeInterface>.Next(ref iter);
			if (timeNode.Time > node.Time)
			{
				ViDoubleLink1<ViTimeNodeInterface>.PushBefore(iter, node);
				return;
			}
		}
		list.PushBack(node);
	}
	private static void _AddFastEvent(ViDoubleLink1<ViTimeNodeInterface> list, TimeRoll timeRoll)
	{
		ViDoubleLinkNode1<ViTimeNodeInterface> iter = list.GetHead();
		while (!list.IsEnd(iter))
		{
			ViTimeNodeInterface timeNode = iter as ViTimeNodeInterface;
			ViDebuger.AssertError(timeNode);
			ViDoubleLink1<ViTimeNodeInterface>.Next(ref iter);
			if (timeRoll.InRange(timeNode.Time))
			{
				timeRoll.Add(timeNode);
			}
			else
			{
				ViDebuger.Error(string.Empty);
			}
		}
	}
	private static void _AddEvent(ViDoubleLink1<ViTimeNodeInterface> list, TimeRoll timeRoll)
	{
		ViDoubleLinkNode1<ViTimeNodeInterface> iter = list.GetHead();
		while (!list.IsEnd(iter))
		{
			ViTimeNodeInterface timeNode = iter as ViTimeNodeInterface;
			ViDebuger.AssertError(timeNode);
			ViDoubleLink1<ViTimeNodeInterface>.Next(ref iter);
			if (timeRoll.InRange(timeNode.Time))
			{
				timeRoll.Add(timeNode);
			}
			else
			{
				break;
			}
		}
	}
	private void _UpdateTimeList(ViDoubleLink1<ViTimeNodeInterface> list)
	{
		while (list.IsNotEmpty())
		{
			ViTimeNodeInterface timeNode = list.GetHead() as ViTimeNodeInterface;
			ViDebuger.AssertError(timeNode);
			timeNode.Detach();
			timeNode._Exce(this);
		}
		ViDebuger.AssertError(list.IsEmpty());
	}
	//
	private ViTime64 _time;
	private TimeRoll _roll0 = new TimeRoll();
	private TimeRoll _roll1 = new TimeRoll();
	private ViDoubleLink1<ViTimeNodeInterface> _reserveList = new ViDoubleLink1<ViTimeNodeInterface>();
}

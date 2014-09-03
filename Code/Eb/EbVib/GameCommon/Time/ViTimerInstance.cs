using System;


using ViTime64 = System.Int64;

public static class ViTickCount
{
	public static readonly ViTime64 SECOND = 100;
	public static readonly ViTime64 MINUTE = SECOND * 60;
	public static readonly ViTime64 HOUR = MINUTE * 60;
	public static readonly ViTime64 DAY = HOUR * 24;
	public static readonly ViTime64 WEEK = DAY * 7;

	static System.DateTime time1970 = new System.DateTime(1970, 1, 1);
	public static System.DateTime Date(ViTime64 time)
	{
		return time1970.AddMilliseconds(time * 10);
	}

	public static ViTime64 GetCount(Int32 week, Int32 day, Int32 hour, Int32 minute, Int32 second)
	{
		ViTime64 count = WEEK * week;
		count += day * DAY;
		count += hour * HOUR;
		count += minute * MINUTE;
		count += second * SECOND;
		return count;
	}
	public static ViTime64 GetCount(Int32 day, Int32 hour, Int32 minute, Int32 second)
	{
		ViTime64 count = day * DAY;
		count += hour * HOUR;
		count += minute * MINUTE;
		count += second * SECOND;
		return count;
	}
	public static ViTime64 GetCount(Int32 hour, Int32 minute, Int32 second)
	{
		ViTime64 count = hour * HOUR;
		count += minute * MINUTE;
		count += second * SECOND;
		return count;
	}
	public static ViTime64 GetCount(Int32 minute, Int32 second)
	{
		ViTime64 count = minute * MINUTE;
		count += second * SECOND;
		return count;
	}
	public static ViTime64 GetCount(Int32 second)
	{
		return second * SECOND;
	}
	public static void GetTime(ViTime64 count, out Int32 week, out Int32 day, out Int32 hour, out Int32 minute, out Int32 second)
	{
		ViTime64 reserveCount = count + 50;
		week = (Int32)(reserveCount / WEEK);
		reserveCount -= week * WEEK;
		day = (Int32)(reserveCount / DAY);
		reserveCount -= day * DAY;
		hour = (Int32)(reserveCount / HOUR);
		reserveCount -= hour * HOUR;
		minute = (Int32)(reserveCount / MINUTE);
		reserveCount -= minute * MINUTE;
		second = (Int32)(reserveCount / SECOND);
	}
	public static void GetTime(ViTime64 count, out Int32 day, out Int32 hour, out Int32 minute, out Int32 second)
	{
		ViTime64 reserveCount = count + 50;
		day = (Int32)(reserveCount / DAY);
		reserveCount -= day * DAY;
		hour = (Int32)(reserveCount / HOUR);
		reserveCount -= hour * HOUR;
		minute = (Int32)(reserveCount / MINUTE);
		reserveCount -= minute * MINUTE;
		second = (Int32)(reserveCount / SECOND);
	}
	public static void GetTime(ViTime64 count, out Int32 hour, out Int32 minute, out Int32 second)
	{
		ViTime64 reserveCount = count + 50;
		hour = (Int32)(reserveCount / HOUR);
		reserveCount -= hour * HOUR;
		minute = (Int32)(reserveCount / MINUTE);
		reserveCount -= minute * MINUTE;
		second = (Int32)(reserveCount / SECOND);
	}
	public static void GetTime(ViTime64 count, out Int32 minute, out Int32 second)
	{
		ViTime64 reserveCount = count + 50;
		minute = (Int32)(reserveCount / MINUTE);
		reserveCount -= minute * MINUTE;
		second = (Int32)(reserveCount / SECOND);
	}
	public static void GetTime(ViTime64 count, out Int32 second)
	{
		ViTime64 reserveCount = count + 50;
		second = (Int32)(reserveCount / SECOND);
	}
}

public static class ViTimerInstance
{
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static ViTime64 Time { get { return _timer.Time; } }
	public static ViTime64 Time1970 { get { return _iLocalAccumulateTime + _startTime1970; } }
	public static double LocalTime { get { return _fLocalAccumulateTime; } }
	public static ViTimer Timer { get { return _timer; } }
	public static bool IsRunning { get { return _startAccumulateTime != ERROR_TIME; } }

	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	static public void Start(ViTime64 time1970, ViTime64 accumulateTime, UInt32 span, UInt32 rollSize0, UInt32 rollSize1)
	{
		_fLocalAccumulateTime = 0.0;
		_iLocalAccumulateTime = 0;
		_startAccumulateTime = accumulateTime;
		_startTime1970 = time1970;
		_timer.Start(accumulateTime, span, rollSize0, rollSize1);
	}
	static public void End()
	{
		_timer.End();
		_timer = null;
	}
	public static void SetTime(ViTime64 accumulateTime)
	{
		_iLocalAccumulateTime = accumulateTime - _startAccumulateTime;
		_fLocalAccumulateTime = _iLocalAccumulateTime * 0.01;
	}
	public static void Update(float deltaTime)
	{
		if (_startAccumulateTime != ERROR_TIME)
		{
			ViDebuger.AssertError(_timer);
			_fLocalAccumulateTime += deltaTime;
			_iLocalAccumulateTime = (ViTime64)(_fLocalAccumulateTime * 100);
			ViTime64 uiAccumulateTime = _startAccumulateTime + _iLocalAccumulateTime;
			_timer.Update(uiAccumulateTime);
		}
	}
	public static void Update(ViTime64 deltaTime)
	{
		if (_startAccumulateTime != ERROR_TIME)
		{
			ViDebuger.AssertError(_timer);
			_iLocalAccumulateTime += deltaTime;
			_fLocalAccumulateTime = _iLocalAccumulateTime * 0.01;
			ViTime64 uiAccumulateTime = _startAccumulateTime + _iLocalAccumulateTime;
			_timer.Update(uiAccumulateTime);
		}
	}
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	static public void SetTime(ViTimeNode1 node, Int32 deltaTime, ViTimeNode1.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + deltaTime);
		_timer.Add(node);
	}
	static public void SetTime(ViTimeNode1 node, UInt32 deltaTime, ViTimeNode1.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + deltaTime);
		_timer.Add(node);
	}
	static public void SetTime(ViTimeNode1 node, Int64 deltaTime, ViTimeNode1.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + (ViTime64)(deltaTime));
		_timer.Add(node);
	}
	static public void SetTime(ViTimeNode1 node, UInt64 deltaTime, ViTimeNode1.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + (ViTime64)(deltaTime ));
		_timer.Add(node);
	}
	static public void SetTime(ViTimeNode1 node, float deltaTime, ViTimeNode1.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + (ViTime64)(deltaTime * 100));
		_timer.Add(node);
	}
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	static public void SetTime<T>(ViTimeNodeEx1<T> node, Int32 deltaTime, ViTimeNodeEx1<T>.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + deltaTime);
		_timer.Add(node);
	}
	static public void SetTime<T>(ViTimeNodeEx1<T> node, UInt32 deltaTime, ViTimeNodeEx1<T>.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + deltaTime);
		_timer.Add(node);
	}
	static public void SetTime<T>(ViTimeNodeEx1<T> node, Int64 deltaTime, ViTimeNodeEx1<T>.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + deltaTime);
		_timer.Add(node);
	}
	static public void SetTime<T>(ViTimeNodeEx1<T> node, UInt64 deltaTime, ViTimeNodeEx1<T>.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + (Int64)deltaTime);
		_timer.Add(node);
	}
	static public void SetTime<T>(ViTimeNodeEx1<T> node, float deltaTime, ViTimeNodeEx1<T>.Callback dele)
	{
		node._SetDelegate(dele);
		node.SetTime(_timer.Time + (ViTime64)(deltaTime * 100));
		_timer.Add(node);
	}
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void SetFreq(ViTimeNodeInterface node, float oldFreq, float newFreq)
	{
		if (node.IsAttach() == false)//! 如果回调已经发生过了, 则无法进行重新设置
		{
			return;
		}
		ViTime64 currentTime = _timer.Time;
		ViTime64 delta = node.Time - currentTime;
		ViTime64 deltaTimeOldMod = (delta > 0) ? delta : 0;
		ViTime64 deltaTime = (ViTime64)(deltaTimeOldMod * oldFreq);
		ViTime64 deltaTimeNewMod = (ViTime64)(deltaTime / newFreq);
		node.SetTime(_timer.Time + deltaTimeNewMod);
		_timer.Add(node);
	}
	public static void Modify(ViTimeNodeInterface node, ViTime64 deltaTime)
	{
		if (node.IsAttach() == false)//! 如果回调已经发生过了, 则无法进行重新设置
		{
			return;
		}
		if (node.Time > -deltaTime)
		{
			node.SetTime(node.Time + deltaTime);
		}
		else
		{
			node.SetTime(0);
		}
		_timer.Add(node);
	}

	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static ViTimer _timer = new ViTimer();
	private static ViTime64 _startAccumulateTime = ERROR_TIME;
	private static ViTime64 _startTime1970;
	private static double _fLocalAccumulateTime;
	private static ViTime64 _iLocalAccumulateTime;
	private const ViTime64 ERROR_TIME = -1;

}
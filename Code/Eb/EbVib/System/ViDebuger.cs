using System;
using System.Diagnostics;

public enum ViLogLevel
{
	OK,
	CRIT_OK,
	WARNING,
	ERROR,
	TOTAL,
}

public static class ViDebuger
{
	public delegate void OnPrintStringCallback(string msg);
	public static OnPrintStringCallback NoteCallback { get; set; }
	public static OnPrintStringCallback CritOKCallback { get; set; }
	public static OnPrintStringCallback ErrorCallback { get; set; }
	public static OnPrintStringCallback WarningCallback { get; set; }
	public static ViLogLevel LogLevel = ViLogLevel.OK;

	//+---------------------------------------------------------------------------------------------------------
	public static void AssertError(bool bCondition)
	{
		if (bCondition == false)
		{
			_Error("<ERROR>");
		}
	}
	public static void AssertError(bool bCondition, string log)
	{
		if (bCondition == false)
		{
			_Error(log);
		}
	}
	public static void AssertWarning(bool bCondition)
	{
		if (bCondition == false)
		{
			_Warning("<WARNING>");
		}
	}
	public static void AssertWarning(bool bCondition, string log)
	{
		if (bCondition == false)
		{
			_Warning(log);
		}
	}
	public static void AssertError<T>(T ptr) where T : class
	{
		if (ptr == null)
		{
			_Error("<ERROR>");
		}
	}
	public static void AssertError<T>(T ptr, string log)
	{
		if (ptr == null)
		{
			_Error(log);
		}
	}
	public static void AssertWarning<T>(T ptr) where T : class
	{
		if (ptr == null)
		{
			_Warning("<WARNING>");
		}
	}
	public static void AssertWarning<T>(T ptr, string log)
	{
		if (ptr == null)
		{
			_Warning(log);
		}
	}
	//+---------------------------------------------------------------------------------------------------------
	public static void Error(string log)
	{
		_Error(log);
	}
	public static void Warning(string log)
	{
		_Warning(log);
	}
	public static void CritOK(string log)
	{
		//Debug.Print(log);
		if (LogLevel <= ViLogLevel.CRIT_OK)
		{
			if (CritOKCallback != null)
			{
				CritOKCallback(log);
			}
		}
	}
	public static void Note(string log)
	{
		//Debug.Print(log);
		if (LogLevel <= ViLogLevel.OK)
		{
			if (NoteCallback != null)
			{
				NoteCallback(log);
			}
		}
	}
	//+---------------------------------------------------------------------------------------------------------
	static void _Error(string log)
	{
		//Debug.Assert(false, log);
		if (LogLevel <= ViLogLevel.ERROR)
		{
			if (ErrorCallback != null)
			{
				ErrorCallback(log);
			}
		}
	}
	static void _Warning(string log)
	{
		//Trace.WriteLine(false, log);
		if (LogLevel <= ViLogLevel.WARNING)
		{
			if (WarningCallback != null)
			{
				WarningCallback(log);
			}
		}
	}
}
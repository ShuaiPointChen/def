using System;
using System.Text;
using System.Collections.Generic;




//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogInterface
{
	public delegate void OnPrintValueCallback(ViLogLevel logType, ViTupleInterface tuple);
	public delegate void OnPrintStringCallback(ViLogLevel logType, string msg);
	public static OnPrintValueCallback PrintValueCallback { get; set; }
	public static OnPrintStringCallback PrintStringCallback { get; set; }
	//
	static UInt32 Register(string name)
	{
		UInt32 value = 0;
		if (_logDictionary.TryGetValue(name, out value))
		{

		}
		else
		{
			value = (UInt32)_logDictionary.Count + 1;
			_logDictionary.Add(name, value);
		}
		return value;
	}
	static Dictionary<string, UInt32> _logDictionary = new Dictionary<string, UInt32>();
	//
	public ViLogInterface(ViLogLevel type, string description)
	{
		_type = type;
		_name = description;
		_description = description.Split(new char[] { '&' }, StringSplitOptions.None);
		_idx = Register(_name);
	}
	public ViLogInterface(string description)
		: this(ViLogLevel.OK, description)
	{

	}
	//
	public void Print(ViTupleInterface tuple)
	{
		if (_type < ViDebuger.LogLevel)
		{
			return;
		}
		StringBuilder logStr = new StringBuilder(1024, 1024);
		UInt32 idx = 0;
		for (; idx < (UInt32)Description.Length; ++idx)
		{
			logStr.Append(Description[idx]);
			Object value = tuple.Value(idx);
			if (value != null)
			{
				logStr.Append("(");
				logStr.Append(tuple.Value(idx));
				logStr.Append(")");
			}
			else
			{
				//Console.Write("(参数不足)");
			}
		}
		for (; idx < tuple.Size; ++idx)
		{
			Object value = tuple.Value(idx);
			ViDebuger.AssertError(value);
			logStr.Append("(");
			logStr.Append(tuple.Value(idx));
			logStr.Append(")");
		}
		logStr.AppendLine();
		Print(logStr.ToString(), tuple);
	}
	public void Print(string msg, ViTupleInterface tuple)
	{
		if (_type < ViDebuger.LogLevel)
		{
			return;
		}
		Console.WriteLine(msg);
		//System.Diagnostics.Debug.Write(msg);
		//System.Diagnostics.Trace.Write(msg);
		if (PrintValueCallback != null)
		{
			PrintValueCallback(_type, tuple);
		}
		if (PrintStringCallback != null)
		{
			PrintStringCallback(_type, msg);
		}
	}
	//
	public UInt32 Idx { get { return _idx; } }
	public ViLogLevel Type { get { return _type; } }
	public string Name { get { return _name; } }
	public string[] Description { get { return _description; } }
	//
	UInt32 _idx;
	ViLogLevel _type;
	string _name;
	string[] _description;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note()
	{
		ViTuple tuple = new ViTuple();
		Print(Name, tuple);
	}
	public void Note<T0>(T0 param0)
	{
		ViTuple<T0> tuple = new ViTuple<T0>();
		tuple.Value0 = param0;
		Print(tuple);
	}
	public void Note<T0, T1>(T0 param0, T1 param1)
	{
		ViTuple<T0, T1> tuple = new ViTuple<T0, T1>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		Print(tuple);
	}
	public void Note<T0, T1, T2>(T0 param0, T1 param1, T2 param2)
	{
		ViTuple<T0, T1, T2> tuple = new ViTuple<T0, T1, T2>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		Print(tuple);
	}
	public void Note<T0, T1, T2, T3>(T0 param0, T1 param1, T2 param2, T3 param3)
	{
		ViTuple<T0, T1, T2, T3> tuple = new ViTuple<T0, T1, T2, T3>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		tuple.Value3 = param3;
		Print(tuple);
	}
	public void Note<T0, T1, T2, T3, T4>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4)
	{
		ViTuple<T0, T1, T2, T3, T4> tuple = new ViTuple<T0, T1, T2, T3, T4>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		tuple.Value3 = param3;
		tuple.Value4 = param4;
		Print(tuple);
	}

}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx<T0> : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note(T0 param0)
	{
		ViTuple<T0> tuple = new ViTuple<T0>();
		tuple.Value0 = param0;
		Print(tuple);
	}
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx<T0, T1> : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note(T0 param0, T1 param1)
	{
		ViTuple<T0, T1> tuple = new ViTuple<T0, T1>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		Print(tuple);
	}
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx<T0, T1, T2> : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note(T0 param0, T1 param1, T2 param2)
	{
		ViTuple<T0, T1, T2> tuple = new ViTuple<T0, T1, T2>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		Print(tuple);
	}
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx<T0, T1, T2, T3> : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note(T0 param0, T1 param1, T2 param2, T3 param3)
	{
		ViTuple<T0, T1, T2, T3> tuple = new ViTuple<T0, T1, T2, T3>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		tuple.Value3 = param3;
		Print(tuple);
	}
}
//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViLogEx<T0, T1, T2, T3, T4> : ViLogInterface
{
	public ViLogEx(ViLogLevel type, string description) : base(type, description) { }
	public ViLogEx(string description) : base(description) { }
	//
	public void Note(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4)
	{
		ViTuple<T0, T1, T2, T3, T4> tuple = new ViTuple<T0, T1, T2, T3, T4>();
		tuple.Value0 = param0;
		tuple.Value1 = param1;
		tuple.Value2 = param2;
		tuple.Value3 = param3;
		tuple.Value4 = param4;
		Print(tuple);
	}
}

public class Demo_LogEx
{
#pragma warning disable 0219
	public static ViLogEx _log0 = new ViLogEx("&+&=&");
	public static ViLogEx<int, int, int> _log1 = new ViLogEx<int, int, int>("&-&=&");

	public static void Test()
	{
		_log0.Note(1, 2, 3);
		_log1.Note(1, 2, -1);
	}
#pragma warning restore 0219
}



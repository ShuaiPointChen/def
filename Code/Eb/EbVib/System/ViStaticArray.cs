using System;
using System.Collections.Generic;


public class ViStaticArray<T>
	where T : new()
{
	public T this[int index]
	{
		get { ViDebuger.AssertError(index < _values.Length); return _values[index]; }
		set { ViDebuger.AssertError(index < _values.Length); _values[index] = value; }
	}
	public int Length { get { return _values.Length; } }
	public T[] Array { get { return _values; } }

	static ViStaticArray()
	{
		ViArrayParser.Register(typeof(ViStaticArray<T>), GetLength);
		ViArrayParser.Register(typeof(ViStaticArray<T>), GetObject);
		ViArrayParser.Register(typeof(ViStaticArray<T>), SetObject);
	}
	public ViStaticArray(UInt32 size)
	{
		_values = new T[size];
		for (int idx = 0; idx < size; ++idx)
		{
			_values[idx] = new T();
		}
	}
	internal static int GetLength(object obj)
	{
		if (obj is ViStaticArray<T>)
		{
			ViStaticArray<T> array = (ViStaticArray<T>)obj;
			return array.Length;			
		}
		return 0;
	}
	internal static object GetObject(object obj, int index)
	{
		if (obj is ViStaticArray<T>)
		{
			ViStaticArray<T> array = (ViStaticArray<T>)obj;
			if (index >= array.Length)
			{
				return default(T);
			}
			return array[index];
		}
		return default(T);
	}
	internal static bool SetObject(object obj, int index, object newValue)
	{
		if (obj is ViStaticArray<T>)
		{
			ViStaticArray<T> array = (ViStaticArray<T>)obj;
			if (index < array.Length)
			{
				array[index] = (T)newValue;
				return true;
			}
		}
		return false;
	}
	T[] _values;
}

public static class ViArrayParser
{
	public delegate int LengthParser(object obj);
	public delegate object GetObjectParser(object obj, int index);
	public delegate bool SetObjectParser(object obj, int index, object newValue);

	public static void Register(Type type, LengthParser parse)
	{
		_lenParseList[type] = parse;
	}
	public static void Register(Type type, GetObjectParser parse)
	{
		_getObjParseList[type] = parse;
	}
	public static void Register(Type type, SetObjectParser parse)
	{
		_setObjParseList[type] = parse;
	}
	public static int Length(object obj)
	{
		LengthParser parse;
		if (_lenParseList.TryGetValue(obj.GetType(), out parse))
		{
			ViDebuger.AssertError(parse != null);
			return parse(obj);
		}
		else
		{
			ViDebuger.Error("");
			return 0;
		}
	}

	public static object Object(object obj, int index)
	{
		GetObjectParser parse;
		if (_getObjParseList.TryGetValue(obj.GetType(), out parse))
		{
			ViDebuger.AssertError(parse != null);
			return parse(obj, index);
		}
		else
		{
			ViDebuger.Error("");
			return null;
		}
	}
	public static bool SetObject(object obj, int index, object newValue)
	{
		SetObjectParser parse;
		if (_setObjParseList.TryGetValue(obj.GetType(), out parse))
		{
			ViDebuger.AssertError(parse != null);
			return parse(obj, index, newValue);
		}
		else
		{
			ViDebuger.Error("");
			return false;
		}
	}
	static Dictionary<Type, LengthParser> _lenParseList = new Dictionary<Type, LengthParser>();
	static Dictionary<Type, GetObjectParser> _getObjParseList = new Dictionary<Type, GetObjectParser>();
	static Dictionary<Type, SetObjectParser> _setObjParseList = new Dictionary<Type, SetObjectParser>();

}
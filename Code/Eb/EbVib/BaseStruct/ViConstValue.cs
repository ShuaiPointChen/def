using System;
using System.Collections.Generic;

public class ViConstValue<T>
{
	public ViConstValue(string key, T defaultValue)
	{
		_node.Data = defaultValue;
		ViConstValueList<T>.Attach(key, _node);
	}
	public static implicit operator T(ViConstValue<T> data)
	{
		return data.Value;
	}
	public T Value{get{return _node.Data;}}
	ViDoubleLinkNode2<T> _node = new ViDoubleLinkNode2<T>();
}
//
internal class ValueList<T>
{
	public T _value;
	public bool _init = false;
	public ViDoubleLink2<T> _list = new ViDoubleLink2<T>();
}
//
public static class ViConstValueList<T>
{
	public static void AddValue(string key, T value)
	{
		ValueList<T> list;
		if (_values.TryGetValue(key, out list))
		{
			list._value = value;
			list._init = true;
			list._list.SetValue(value);
		}
		else
		{
			list = new ValueList<T>();
			list._value = value;
			list._init = true;
			list._list.SetValue(value);
			_values.Add(key, list);
		}
	}
	internal static void Attach(string key, ViDoubleLinkNode2<T> node)
	{
		ValueList<T> list;
		if (_values.TryGetValue(key, out list))
		{
			if (list._init)
			{
				node.Data = list._value;
			}
			list._list.PushBack(node);
		}
		else
		{
			list = new ValueList<T>();
			list._list.PushBack(node);
			_values.Add(key, list);
		}
	}
	private static Dictionary<string, ValueList<T>> _values = new Dictionary<string, ValueList<T>>();
}

public class Demo_ConstValue
{
#pragma warning disable 0219
	static ViConstValue<int> ScriptConstValue = new ViConstValue<int>("INT", 1);
	//
	static void Func()
	{
		int value = ScriptConstValue;
	}
	//
	public static void Test()
	{
		Func();
		ViConstValueList<int>.AddValue("INT", 100);
		Func();
	}
#pragma warning restore 0219
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDoubleLinkNode3<T>
{
	public ViDoubleLinkNode3()
	{

	}
	public ViDoubleLinkNode3(T data)
	{
		Data = data;
	}
	internal void _Detach()
	{
		if (_pre != null)
		{
			_pre._next = _next;
			_next._pre = _pre;
			_pre = null;
			_next = null;
		}
	}
	public T Data;
	//
	internal ViDoubleLinkNode3<T> _pre;
	internal ViDoubleLinkNode3<T> _next;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViDoubleLink3<T> : IEnumerable<T>
{
	//+-----------------------------------------------------------------------------------------------------------------------------
	public static void Next(ref ViDoubleLinkNode3<T> node)
	{
		//ViDebuger.AssertError(node != null && node._next != null);
		node = node._next;
	}
	public static void Pre(ref ViDoubleLinkNode3<T> node)
	{
		//ViDebuger.AssertError(node != null && node._pre != null);
		node = node._pre;
	}
	public static void PushAfter(ViDoubleLinkNode3<T> before, T value)
	{
		ViDoubleLinkNode3<T> node = new ViDoubleLinkNode3<T>(value);
		_PushAfter(before, node);
	}
	public static void PushBefore(ViDoubleLinkNode3<T> after, T value)
	{
		ViDoubleLinkNode3<T> node = new ViDoubleLinkNode3<T>(value);
		_PushBefore(after, node);
	}
	//public static void PushAfter(ViDoubleLinkNode3<T> before, ViDoubleLink3<T> list)
	//{
	//    if (before.IsAttach() == false)
	//    {
	//        return;
	//    }
	//    if (list.IsEmpty())
	//    {
	//        return;
	//    }
	//    _PushAfter(before, list);
	//}
	//public static void PushBefore(ViDoubleLinkNode3<T> after, ViDoubleLink3<T> list)
	//{
	//    if (after.IsAttach() == false)
	//    {
	//        return;
	//    }
	//    if (list.IsEmpty())
	//    {
	//        return;
	//    }
	//    _PushBefore(after, list);
	//}
	//+-----------------------------------------------------------------------------------------------------------------------------
	public ViDoubleLink3()
	{
		_Init();
	}
	public bool IsEmpty()
	{
		return _root._next == _root;
	}
	public bool IsNotEmpty()
	{
		return _root._next != _root;
	}
	public bool IsEnd(ViDoubleLinkNode3<T> node)
	{
		return node == _root;
	}
	public ViDoubleLinkNode3<T> GetHead()
	{
		return _root._next;
	}
	public ViDoubleLinkNode3<T> GetTail()
	{
		return _root._pre;
	}
	public void PushBack(T value)
	{
		ViDoubleLinkNode3<T> node = new ViDoubleLinkNode3<T>(value);
		++_count;
		_PushBefore(_root, node);
	}
	public void PushFront(T value)
	{
		ViDoubleLinkNode3<T> node = new ViDoubleLinkNode3<T>(value);
		++_count;
		_PushAfter(_root, node);
	}
	public T GetFront(T defaultValue)
	{
		if (IsEmpty())
		{
			return defaultValue;
		}
		else
		{
			return _root._next.Data;
		}
	}
	public T GetBack(T defaultValue)
	{
		if (IsEmpty())
		{
			return defaultValue;
		}
		else
		{
			return _root._pre.Data;
		}
	}
	public void PopFront()
	{
		if (!IsEmpty())
		{
			--_count;
			_root._next._Detach();
		}
	}
	public void PopBack()
	{
		if (!IsEmpty())
		{
			--_count;
			_root._pre._Detach();
		}
	}
	public bool Has(T value)
	{
		EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		ViDoubleLinkNode3<T> next = _root._next;
		while (next != _root)
		{
			if (comparer.Equals(next.Data, value))
			{
				return true;
			}
			next = next._next;
		}
		return false;
	}
	public void Remove(ViDoubleLinkNode3<T> node)
	{
		_IsChild(node);
		node._Detach();
		--_count;
	}
	//public void PushBack(ViDoubleLink3<T> list)
	//{
	//    _PushBefore(_root, list);
	//}
	//public void PushFront(ViDoubleLink3<T> list)
	//{
	//    _PushAfter(_root, list);
	//}
	public void SetValue(T value)
	{
		ViDoubleLinkNode3<T> next = _root._next;
		while (next != _root)
		{
			next.Data = value;
			next = next._next;
		}
	}
	public void Clear()
	{
		ViDoubleLinkNode3<T> next = _root._next;
		while (next != _root)
		{
			ViDoubleLinkNode3<T> nextCopy = next._next;
			next._pre = null;
			next._next = null;
			next = nextCopy;
		}
		_Init();
	}
	public UInt32 Count { get { return _count; } }
	//+-----------------------------------------------------------------------------------------------------------------------------
	static void _PushAfter(ViDoubleLinkNode3<T> before, ViDoubleLinkNode3<T> node)
	{
		ViDoubleLinkNode3<T> next = before._next;
		ViDebuger.AssertError(next);
		_Link(before, node);
		_Link(node, next);
	}
	static void _PushBefore(ViDoubleLinkNode3<T> after, ViDoubleLinkNode3<T> node)
	{
		ViDoubleLinkNode3<T> pre = after._pre;
		ViDebuger.AssertError(pre);
		_Link(pre, node);
		_Link(node, after);
	}
	static void _PushAfter(ViDoubleLinkNode3<T> before, ViDoubleLink3<T> list)
	{
		if (list.IsEmpty())
		{
			return;
		}
		ViDoubleLinkNode3<T> first = list._root._next;
		ViDoubleLinkNode3<T> back = list._root._pre;
		ViDoubleLinkNode3<T> next = before._next;
		_Link(before, first);
		_Link(back, next);
		list._Init();
	}
	static void _PushBefore(ViDoubleLinkNode3<T> after, ViDoubleLink3<T> list)
	{
		if (list.IsEmpty())
		{
			return;
		}
		ViDoubleLinkNode3<T> first = list._root._next;
		ViDoubleLinkNode3<T> back = list._root._pre;
		ViDoubleLinkNode3<T> pre = after._pre;
		_Link(pre, first);
		_Link(back, after);
		list._Init();
	}
	static void _Link(ViDoubleLinkNode3<T> pre, ViDoubleLinkNode3<T> next)
	{
		pre._next = next;
		next._pre = pre;
	}
	private void _Init()
	{
		_Link(_root, _root);
		_count = 0;
	}
	private bool _IsChild(ViDoubleLinkNode3<T> node)
	{
		ViDoubleLinkNode3<T> next = _root._next;
		while (next != _root)
		{
			if (node == next)
			{
				return true;
			}
			next = next._next;
		}
		return false;
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	private ViDoubleLinkNode3<T> _root = new ViDoubleLinkNode3<T>();
	private UInt32 _count;

	///<foreach>
	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	internal struct Enumerator : IEnumerator<T>, IEnumerator
	{
		private ViDoubleLinkNode3<T> _root;
		private ViDoubleLinkNode3<T> _node;
		private T _current;
		internal Enumerator(ViDoubleLink3<T> list)
		{
			_root = list._root;
			_node = list._root._next;
			_current = default(T);
		}
		T IEnumerator<T>.Current { get { return _current; } }
		object IEnumerator.Current { get { return _current; } }
		public bool MoveNext()
		{
			if (_node != _root)
			{
				_current = _node.Data;
				_node = _node._next;
				return true;
			}
			return false;
		}
		public void Reset() { _node = _root._next; }
		public void Dispose() { }
	}
}

public class Demo_ViDoubleLink3
{
#pragma warning disable 0219
	public static void Test()
	{
		ViDoubleLink3<int> list = new ViDoubleLink3<int>();
		list.PushBack(1);
		list.PushBack(3);

		{///<正向迭代>
			ViDoubleLinkNode3<int> iter = list.GetHead();
			while (!list.IsEnd(iter))
			{
				int value = iter.Data;
				ViDoubleLink3<int>.Next(ref iter);
				///<使用>
				Console.WriteLine(value);
				///</使用>
			}
		}
		{///<反向迭代>
			ViDoubleLinkNode3<int> iter = list.GetTail();
			while (!list.IsEnd(iter))
			{
				int value = iter.Data;
				ViDoubleLink3<int>.Pre(ref iter);
				///<使用>
				Console.WriteLine(value);
				///</使用>
			}
		}

		foreach (int value in list)
		{
			Console.WriteLine(value);
		}
	}
#pragma warning restore 0219
}
public class Performance_ViDoubleLink3
{
#pragma warning disable 0219
	public static void Test0()
	{
		Console.WriteLine("Performance_ViDoubleLink3.Test0");
		ViDoubleLink3<UInt32> list = new ViDoubleLink3<UInt32>();
		UInt32 cnt = 10000000;
		UInt32 round = 100;
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < cnt; ++idx)
		{
			list.PushBack(idx);
		}
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < round; ++idx)
		{
			ViDoubleLinkNode3<UInt32> iter = list.GetHead();
			while (!list.IsEnd(iter))
			{
				UInt32 value = iter.Data;
				ViDoubleLink3<UInt32>.Next(ref iter);
				///<使用>
				///</使用>
			}
		}
		Console.WriteLine(DateTime.Now);
	}
	public static void Test1()
	{
		Console.WriteLine("Performance_ViDoubleLink3.Test1");
		ViDoubleLink3<UInt32> list = new ViDoubleLink3<UInt32>();
		UInt32 cnt = 10000000;
		UInt32 round = 100;
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < cnt; ++idx)
		{
			list.PushBack(idx);
		}
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < round; ++idx)
		{
			foreach (UInt32 value in list)
			{

			}
		}
		Console.WriteLine(DateTime.Now);
	}

	public static void Test2()
	{
		Console.WriteLine("Performance_ViDoubleLink3.Test2");
		LinkedList<UInt32> list = new LinkedList<UInt32>();
		UInt32 cnt = 10000000;
		UInt32 round = 100;
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < cnt; ++idx)
		{
			list.AddLast(idx);
		}
		Console.WriteLine(DateTime.Now);
		for (UInt32 idx = 0; idx < round; ++idx)
		{
			foreach (UInt32 value in list)
			{

			}
		}
		Console.WriteLine(DateTime.Now);
	}
#pragma warning restore 0219
}
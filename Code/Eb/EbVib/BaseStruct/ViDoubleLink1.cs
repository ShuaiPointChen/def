using System;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViDoubleLinkNode1<T>
{
	public bool IsAttach()
	{
		return (_pre != null);
	}
	public void Detach()
	{
		if (_pre != null)
		{
			_pre._next = _next;
			_next._pre = _pre;
			_pre = null;
			_next = null;
		}
	}
	internal ViDoubleLinkNode1<T> _pre;
	internal ViDoubleLinkNode1<T> _next;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViDoubleLink1<T>
{
	//+-----------------------------------------------------------------------------------------------------------------------------
	public static void Next(ref ViDoubleLinkNode1<T> node)
	{
		ViDebuger.AssertError(node != null && node._next != null);
		node = node._next;
	}
	public static void Pre(ref ViDoubleLinkNode1<T> node)
	{
		ViDebuger.AssertError(node != null && node._pre != null);
		node = node._pre;
	}
	public static void PushAfter(ViDoubleLinkNode1<T> before, ViDoubleLinkNode1<T> node)
	{
		node.Detach();
		_PushAfter(before, node);
	}
	public static void PushBefore(ViDoubleLinkNode1<T> after, ViDoubleLinkNode1<T> node)
	{
		node.Detach();
		_PushBefore(after, node);
	}
	public static void PushAfter(ViDoubleLinkNode1<T> before, ViDoubleLink1<T> list)
	{
		if (before.IsAttach() == false)
		{
			return;
		}
		if (list.IsEmpty())
		{
			return;
		}
		_PushAfter(before, list);
	}
	public static void PushBefore(ViDoubleLinkNode1<T> after, ViDoubleLink1<T> list)
	{
		if (after.IsAttach() == false)
		{
			return;
		}
		if (list.IsEmpty())
		{
			return;
		}
		_PushBefore(after, list);
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	public ViDoubleLink1()
	{
		_Init();
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	public bool IsEmpty()
	{
		return _root._next == _root;
	}
	public bool IsNotEmpty()
	{
		return _root._next != _root;
	}
	public bool IsEnd(ViDoubleLinkNode1<T> node)
	{
		return node == _root;
	}
	public ViDoubleLinkNode1<T> GetHead()
	{
		return _root._next;
	}
	public ViDoubleLinkNode1<T> GetTail()
	{
		return _root._pre;
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	public void PushBack(ViDoubleLinkNode1<T> node)
	{
		node.Detach();
		_PushBefore(_root, node);
	}
	public void PushFront(ViDoubleLinkNode1<T> node)
	{
		node.Detach();
		_PushAfter(_root, node);
	}
	public void PushBack(ViDoubleLink1<T> list)
	{
		_PushBefore(_root, list);
	}
	public void PushFront(ViDoubleLink1<T> list)
	{
		_PushAfter(_root, list);
	}
	public void Clear()
	{
		ViDoubleLinkNode1<T> next = _root._next;
		while (next != _root)
		{
			ViDoubleLinkNode1<T> nextCopy = next._next;
			next._pre = null;
			next._next = null;
			next = nextCopy;
		}
		_Init();
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	static void _PushAfter(ViDoubleLinkNode1<T> before, ViDoubleLinkNode1<T> node)
	{
		ViDoubleLinkNode1<T> next = before._next;
		ViDebuger.AssertError(next);
		_Link(before, node);
		_Link(node, next);
	}
	static void _PushBefore(ViDoubleLinkNode1<T> after, ViDoubleLinkNode1<T> node)
	{
		ViDoubleLinkNode1<T> pre = after._pre;
		ViDebuger.AssertError(pre);
		_Link(pre, node);
		_Link(node, after);
	}
	static void _PushAfter(ViDoubleLinkNode1<T> before, ViDoubleLink1<T> list)
	{
		if (list.IsEmpty())
		{
			return;
		}
		ViDoubleLinkNode1<T> first = list._root._next;
		ViDoubleLinkNode1<T> back = list._root._pre;
		ViDoubleLinkNode1<T> next = before._next;
		_Link(before, first);
		_Link(back, next);
		list._Init();
	}
	static void _PushBefore(ViDoubleLinkNode1<T> after, ViDoubleLink1<T> list)
	{
		if (list.IsEmpty())
		{
			return;
		}
		ViDoubleLinkNode1<T> first = list._root._next;
		ViDoubleLinkNode1<T> back = list._root._pre;
		ViDoubleLinkNode1<T> pre = after._pre;
		_Link(pre, first);
		_Link(back, after);
		list._Init();
	}
	static void _Link(ViDoubleLinkNode1<T> pre, ViDoubleLinkNode1<T> next)
	{
		pre._next = next;
		next._pre = pre;
	}
	private void _Init()
	{
		_Link(_root, _root);
	}
	//+-----------------------------------------------------------------------------------------------------------------------------
	internal ViDoubleLinkNode1<T> _root = new ViDoubleLinkNode1<T>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class Demo_ViDoubleLink1
{
#pragma warning disable 0219
	public static void Test()
	{
		ViDoubleLink1<int> list = new ViDoubleLink1<int>();
		{
			ViDoubleLinkNode1<int> node1 = new ViDoubleLinkNode1<int>();
			ViDoubleLinkNode1<int> node2 = new ViDoubleLinkNode1<int>();
			list.PushBack(node1);
			list.PushBack(node2);

			{///<正向迭代>
				ViDoubleLinkNode1<int> iter = list.GetHead();
				while (!list.IsEnd(iter))
				{
					ViDoubleLink1<int>.Next(ref iter);
					///<使用>
					///</使用>
				}
			}
			{///<反向迭代>
				ViDoubleLinkNode1<int> iter = list.GetTail();
				while (!list.IsEnd(iter))
				{
					ViDoubleLink1<int>.Pre(ref iter);
					///<使用>
					///</使用>
				}
			}
		}
	}
#pragma warning restore 0219
}
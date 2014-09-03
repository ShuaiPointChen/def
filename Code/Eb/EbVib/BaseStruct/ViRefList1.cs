using System;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViRefNode1<T>
{
	public ViRefNode1() { }

	public bool IsAttach()
	{
		return (_list != null);
	}
	public bool IsAttach(ViRefList1<T> list)
	{
		return (list == _list);
	}
	public void Detach()
	{
		if (_list != null)
		{
			_list._Detach(this);
			_list = null;
		}
	}
	internal ViRefNode1<T> _pre;
	internal ViRefNode1<T> _next;
	internal ViRefList1<T> _list;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViRefList1<T>
{
	public static void Next(ref ViRefNode1<T> node)
	{
		ViDebuger.AssertError(node != null && node._next != null);
		node = node._next;
	}
	//+--------------------------------------------------------------------------------------------------------------
	public ViRefList1()
	{
		_root._list = this;
		_Init();
	}
	//+--------------------------------------------------------------------------------------------------------------
	public bool IsEmpty()
	{
		return _root._next == _root;
	}
	public UInt32 Size { get { return _cnt; } }
	public ViRefNode1<T> GetHead()
	{
		return _root._next;
	}
	public ViRefNode1<T> GetTail()
	{
		return _root._pre;
	}
	public ViRefNode1<T> CurrentNode
	{
		get { return _next; }
	}
	public void Next()
	{
		Next(ref _next);
	}
	public void BeginIterator()
	{
		_next = _root;
		Next(ref _next);
	}
	public void EndIterator()
	{
		_next = _root;
	}
	public bool IsEnd(ViRefNode1<T> node)
	{
		return node == _root;
	}
	public bool IsEnd() { return _next == _root; }
	public void PushBack(ViRefNode1<T> node)
	{
		node.Detach();
		++_cnt;
		node._list = this;
		_PushBefore(_root, node);
	}
	public void PushFront(ViRefNode1<T> node)
	{
		node.Detach();
		++_cnt;
		node._list = this;
		_PushAfter(_root, node);
	}
	public void PushBack(ViRefList1<T> list)
	{
		if (list == this)
		{
			return;
		}
		PushBefore(_root, list);
	}
	public void PushFront(ViRefList1<T> list)
	{
		if (list == this)
		{
			return;
		}
		PushAfter(_root, list);
	}
	public void PushAfter(ViRefNode1<T> before, ViRefNode1<T> node)
	{
		node.Detach();
		++_cnt;
		node._list = this;
		_PushAfter(before, node);
	}
	public void PushBefore(ViRefNode1<T> after, ViRefNode1<T> node)
	{
		node.Detach();
		++_cnt;
		node._list = this;
		_PushBefore(after, node);
	}
	public void PushAfter(ViRefNode1<T> before, ViRefList1<T> list)
	{
		if (list.Size == 0)
		{
			return;
		}
		if (before.IsAttach(list))
		{
			return;
		}
		if (before.IsAttach() == false)
		{
			return;
		}
		ViDebuger.AssertError(before._list);
		ViRefList1<T> receiveList = before._list;
		ViRefNode1<T> iter = list._root._next;
		while (iter != list._root)
		{
			iter._list = receiveList;
			iter = iter._next;
		}
		ViDebuger.AssertError(receiveList != list);
		ViRefNode1<T> first = list._root._next;
		ViRefNode1<T> back = list._root._pre;
		ViRefNode1<T> next = before._next;
		_Link(before, first);
		_Link(back, next);
		receiveList._cnt += list.Size;
		list._Init();
	}
	public void PushBefore(ViRefNode1<T> after, ViRefList1<T> list)
	{
		if (list.Size == 0)
		{
			return;
		}
		if (after.IsAttach(list))
		{
			return;
		}
		if (after.IsAttach() == false)
		{
			return;
		}
		ViDebuger.AssertError(after._list);
		ViRefList1<T> receiveList = after._list;
		ViRefNode1<T> iter = list._root._next;
		while (iter != list._root)
		{
			iter._list = receiveList;
			iter = iter._next;
		}
		ViDebuger.AssertError(receiveList != list);
		ViRefNode1<T> first = list._root._next;
		ViRefNode1<T> back = list._root._pre;
		ViRefNode1<T> pre = after._pre;
		_Link(pre, first);
		_Link(back, after);
		receiveList._cnt += list.Size;
		list._Init();
	}
	public void Clear()
	{
		ViRefNode1<T> next = _root._next;
		while (next != _root)
		{
			ViRefNode1<T> nextCopy = next._next;
			next._pre = null;
			next._next = null;
			next._list = null;
			next = nextCopy;
		}
		_Init();
	}
	internal void _Detach(ViRefNode1<T> node)
	{
		ViDebuger.AssertError(node._list == this);
		if (node == _next)//! 如果是正在迭代的点, 则自动将Next移到下个点
		{
			Next(ref _next);
		}
		_Link(node._pre, node._next);
		node._pre = null;
		node._next = null;
		--_cnt;
	}
	//+--------------------------------------------------------------------------------------------------------------
	static void _PushAfter(ViRefNode1<T> before, ViRefNode1<T> node)
	{
		ViRefNode1<T> next = before._next;
		ViDebuger.AssertError(next);
		_Link(before, node);
		_Link(node, next);
	}
	static void _PushBefore(ViRefNode1<T> after, ViRefNode1<T> node)
	{
		ViRefNode1<T> pre = after._pre;
		ViDebuger.AssertError(pre);
		_Link(pre, node);
		_Link(node, after);
	}
	static void _Link(ViRefNode1<T> pre, ViRefNode1<T> next)
	{
		pre._next = next;
		next._pre = pre;
	}
	private void _Init()
	{
		_Link(_root, _root);
		_next = _root;
		_cnt = 0;
	}
	//+--------------------------------------------------------------------------------------------------------------
	internal ViRefNode1<T> _root = new ViRefNode1<T>();
	internal UInt32 _cnt;
	internal ViRefNode1<T> _next;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class Demo_RefList1
{
#pragma warning disable 0219
	public static void Test()
	{
		ViRefList1<int> list = new ViRefList1<int>();
		ViRefNode1<int> node0 = new ViRefNode1<int>();
		ViRefNode1<int> node1 = new ViRefNode1<int>();
		ViRefNode1<int> node2 = new ViRefNode1<int>();
		list.PushBack(node0);
		list.PushBack(node1);
		list.PushBack(node2);

		list.BeginIterator();
		while (!list.IsEnd())
		{
			ViRefNode1<int> node = list.CurrentNode;
			list.Next();
			///<使用>
			///</使用>
		}
		list.Clear();
	}
#pragma warning restore 0219
}
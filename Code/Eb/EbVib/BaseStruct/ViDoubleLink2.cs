using System;

public class ViDoubleLinkNode2<T>
{
    //-------------------------------------------------------------------------
    public T Data;
    internal ViDoubleLinkNode2<T> _pre;
    internal ViDoubleLinkNode2<T> _next;

    //-------------------------------------------------------------------------
    public ViDoubleLinkNode2()
    {
    }

    //-------------------------------------------------------------------------
    public ViDoubleLinkNode2(T data)
    {
        Data = data;
    }

    //-------------------------------------------------------------------------
    public bool IsAttach()
    {
        return (_pre != null);
    }

    //-------------------------------------------------------------------------
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
}

public class ViDoubleLink2<T>
{
    //-------------------------------------------------------------------------
    internal ViDoubleLinkNode2<T> _root = new ViDoubleLinkNode2<T>();

    //-------------------------------------------------------------------------
    public static void Next(ref ViDoubleLinkNode2<T> node)
    {
        ViDebuger.AssertError(node != null && node._next != null);
        node = node._next;
    }

    //-------------------------------------------------------------------------
    public static void Pre(ref ViDoubleLinkNode2<T> node)
    {
        ViDebuger.AssertError(node != null && node._pre != null);
        node = node._pre;
    }

    //-------------------------------------------------------------------------
    public static void PushAfter(ViDoubleLinkNode2<T> before, ViDoubleLinkNode2<T> node)
    {
        node.Detach();
        _PushAfter(before, node);
    }

    //-------------------------------------------------------------------------
    public static void PushBefore(ViDoubleLinkNode2<T> after, ViDoubleLinkNode2<T> node)
    {
        node.Detach();
        _PushBefore(after, node);
    }

    //-------------------------------------------------------------------------
    public static void PushAfter(ViDoubleLinkNode2<T> before, ViDoubleLink2<T> list)
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

    //-------------------------------------------------------------------------
    public static void PushBefore(ViDoubleLinkNode2<T> after, ViDoubleLink2<T> list)
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

    //-------------------------------------------------------------------------
    public ViDoubleLink2()
    {
        _Init();
    }

    //-------------------------------------------------------------------------
    public bool IsEmpty()
    {
        return _root._next == _root;
    }

    //-------------------------------------------------------------------------
    public bool IsNotEmpty()
    {
        return _root._next != _root;
    }

    //-------------------------------------------------------------------------
    public bool IsEnd(ViDoubleLinkNode2<T> node)
    {
        return node == _root;
    }

    //-------------------------------------------------------------------------
    public ViDoubleLinkNode2<T> GetHead()
    {
        return _root._next;
    }

    //-------------------------------------------------------------------------
    public ViDoubleLinkNode2<T> GetTail()
    {
        return _root._pre;
    }

    //-------------------------------------------------------------------------
    public void PushBack(ViDoubleLinkNode2<T> node)
    {
        node.Detach();
        _PushBefore(_root, node);
    }

    //-------------------------------------------------------------------------
    public void PushFront(ViDoubleLinkNode2<T> node)
    {
        node.Detach();
        _PushAfter(_root, node);
    }

    //-------------------------------------------------------------------------
    public void PushBack(ViDoubleLink2<T> list)
    {
        _PushBefore(_root, list);
    }

    //-------------------------------------------------------------------------
    public void PushFront(ViDoubleLink2<T> list)
    {
        _PushAfter(_root, list);
    }

    //-------------------------------------------------------------------------
    public void SetValue(T value)
    {
        ViDoubleLinkNode2<T> next = _root._next;
        while (next != _root)
        {
            next.Data = value;
            next = next._next;
        }
    }

    //-------------------------------------------------------------------------
    public void Clear()
    {
        ViDoubleLinkNode2<T> next = _root._next;
        while (next != _root)
        {
            ViDoubleLinkNode2<T> nextCopy = next._next;
            next._pre = null;
            next._next = null;
            next = nextCopy;
        }
        _Init();
    }

    //-------------------------------------------------------------------------
    public void ClearAndClearContent()
    {
        ViDoubleLinkNode2<T> next = _root._next;
        while (next != _root)
        {
            ViDoubleLinkNode2<T> nextCopy = next._next;
            next._pre = null;
            next._next = null;
            next.Data = default(T);
            next = nextCopy;
        }
        _Init();
    }

    //-------------------------------------------------------------------------
    static void _PushAfter(ViDoubleLinkNode2<T> before, ViDoubleLinkNode2<T> node)
    {
        ViDoubleLinkNode2<T> next = before._next;
        ViDebuger.AssertError(next);
        _Link(before, node);
        _Link(node, next);
    }

    //-------------------------------------------------------------------------
    static void _PushBefore(ViDoubleLinkNode2<T> after, ViDoubleLinkNode2<T> node)
    {
        ViDoubleLinkNode2<T> pre = after._pre;
        ViDebuger.AssertError(pre);
        _Link(pre, node);
        _Link(node, after);
    }

    //-------------------------------------------------------------------------
    static void _PushAfter(ViDoubleLinkNode2<T> before, ViDoubleLink2<T> list)
    {
        if (list.IsEmpty())
        {
            return;
        }
        ViDoubleLinkNode2<T> first = list._root._next;
        ViDoubleLinkNode2<T> back = list._root._pre;
        ViDoubleLinkNode2<T> next = before._next;
        _Link(before, first);
        _Link(back, next);
        list._Init();
    }

    //-------------------------------------------------------------------------
    static void _PushBefore(ViDoubleLinkNode2<T> after, ViDoubleLink2<T> list)
    {
        if (list.IsEmpty())
        {
            return;
        }
        ViDoubleLinkNode2<T> first = list._root._next;
        ViDoubleLinkNode2<T> back = list._root._pre;
        ViDoubleLinkNode2<T> pre = after._pre;
        _Link(pre, first);
        _Link(back, after);
        list._Init();
    }

    //-------------------------------------------------------------------------
    static void _Link(ViDoubleLinkNode2<T> pre, ViDoubleLinkNode2<T> next)
    {
        pre._next = next;
        next._pre = pre;
    }

    //-------------------------------------------------------------------------
    private void _Init()
    {
        _Link(_root, _root);
    }
}

public class Demo_ViDoubleLink2
{
#pragma warning disable 0219
    public static void Test()
    {
        ViDoubleLink2<int> list = new ViDoubleLink2<int>();
        {
            ViDoubleLinkNode2<int> node1 = new ViDoubleLinkNode2<int>();
            node1.Data = 1;
            ViDoubleLinkNode2<int> node2 = new ViDoubleLinkNode2<int>();
            node2.Data = 2;
            list.PushBack(node1);
            list.PushBack(node2);

            {//<正向迭代>
                ViDoubleLinkNode2<int> iter = list.GetHead();
                while (!list.IsEnd(iter))
                {
                    int value = iter.Data;
                    ViDoubleLink2<int>.Next(ref iter);
                }
            }

            {//<反向迭代>
                ViDoubleLinkNode2<int> iter = list.GetTail();
                while (!list.IsEnd(iter))
                {
                    int value = iter.Data;
                    ViDoubleLink2<int>.Pre(ref iter);
                }
            }
        }
    }
#pragma warning restore 0219
}

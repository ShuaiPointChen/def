using System;
using System.Collections.Generic;

public class ViHeapNode
{
	public float Key;
}

internal class ViMinHeap<T> where T : ViHeapNode, new()
{
    //-------------------------------------------------------------------------
    UInt32 _size;
    T[] Datas;
	public bool IsEmpty { get { return (_size == 0); } }
	public UInt32 Size { get { return _size; } }

    //-------------------------------------------------------------------------
	public ViMinHeap(UInt32 size)
	{
		Datas = new T[size];
	}

    //-------------------------------------------------------------------------
	public void Push(T kT)
	{
		if (_size < Datas.Length - 1)
		{
			++_size;
			Datas[_size] = kT;
			UInt32 pos = _size;
			while (pos > 1)
			{
				UInt32 parent = _GetParent(pos);
				if (Datas[parent].Key > Datas[pos].Key)
				{
					_Swap(ref Datas[pos], ref Datas[parent]);
					pos = parent;
				}
				else
				{
					break;
				}
			}
		}
	}

    //-------------------------------------------------------------------------
	public void FastPush(T kT)
	{
		ViDebuger.AssertError(_size < Datas.Length - 1);
		++_size;
		Datas[_size] = kT;
		UInt32 pos = _size;
		while (pos > 1)
		{
			UInt32 parent = _GetParent(pos);
			if (Datas[parent].Key > Datas[pos].Key)
			{
				_Swap(ref Datas[pos], ref Datas[parent]);
				pos = parent;
			}
			else
			{
				break;
			}
		}
	}

    //-------------------------------------------------------------------------
	public T Pop()
	{
		if (_size > 0)
		{
			_Swap(ref Datas[1], ref Datas[_size]);
			--_size;
			_Siftdown(1);
			return Datas[_size + 1];
		}
		else
		{
			return null;
		}
	}

    //-------------------------------------------------------------------------
	public void Clear()
	{
		_size = 0;
	}

    //-------------------------------------------------------------------------
	UInt32 _GetLeft(UInt32 idx) { return idx << 1; }//k*2

    //-------------------------------------------------------------------------
	UInt32 _GetRight(UInt32 idx) { return (idx << 1) + 1; }//k*2+1

    //-------------------------------------------------------------------------
	UInt32 _GetParent(UInt32 idx) { return (idx >> 1); }//k/2

    //-------------------------------------------------------------------------
	static void _Swap(ref T one, ref T other) { T tmp = one; one = other; other = tmp; }

    //-------------------------------------------------------------------------
	void _Siftdown(UInt32 pos)
	{
		UInt32 posMod = pos;
		UInt32 left = _GetLeft(posMod);
		while (left <= _size)
		{
			UInt32 t = left;//int t= pos<<1; 
			UInt32 right = _GetRight(posMod);
			if (right <= _size && Datas[right].Key < Datas[t].Key)
			{
				t = right;
			}
			if (Datas[t].Key < Datas[posMod].Key)
			{
				_Swap(ref Datas[t], ref Datas[posMod]);
				posMod = t;
				left = _GetLeft(posMod);
			}
			else
			{
				break;
			}
		}
	}
}

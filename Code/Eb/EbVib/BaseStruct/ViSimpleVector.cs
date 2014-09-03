using System;


public class ViSimpleVector<T>
		where T : class, new()
{
	public void Clear()
	{
		for (UInt32 idx = 0; idx < _size; ++idx)
		{
			_array[idx] = null;
		}
		_array = null;
		_size = 0;
	}
	public void Resize(UInt32 size)
	{
		if (size == 0) return;
		if (_size != 0) return;//! 不能被初始化两次,否则就去用Vector
		ViDebuger.AssertError(_array == null);
		_array = new T[size];
		_size = size;
		for (UInt32 idx = 0; idx < _size; ++idx)
		{
			_array[idx] = new T();
		}
	}
	public T Get(UInt32 idx)
	{
		if (idx < _size)
		{
			return _array[idx];
		}
		else
			return default(T);
	}
	//
	public UInt32 Size { get { return _size; } }
	//
	private T[] _array;
	private UInt32 _size;
}


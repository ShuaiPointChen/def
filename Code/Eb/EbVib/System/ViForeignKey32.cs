using System;


public struct ViForeignKey32<T>
	where T : ViSealedData, new()
{
	public Int32 Value { get { return _value; } }

	public ViForeignKey32(Int32 value)
	{
		_value = value;
	}
	public static implicit operator Int32(ViForeignKey32<T> data)
	{
		return data.Value;
	}
	public static implicit operator UInt32(ViForeignKey32<T> data)
	{
		return (UInt32)data.Value;
	}
	public T Data { get { return ViSealedDB<T>.Data(_value); } }
	public T PData
	{
		get
		{
			if (_value == 0)
			{
				return null;
			}
			else
			{
				return ViSealedDB<T>.GetData(_value);
			}
		}
	}

	public static implicit operator T(ViForeignKey32<T> data)
	{
		return data.PData;
	}

	Int32 _value;
}
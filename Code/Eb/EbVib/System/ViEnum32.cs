using System;
using System.Collections.Generic;

public struct ViEnum32<T>
{
	public Int32 Value { get { return _value; } }

	public ViEnum32(Int32 value)
	{
		_value = value;
	}
	public static implicit operator Int32(ViEnum32<T> data)
	{
		return data.Value;
	}
	public static implicit operator UInt32(ViEnum32<T> data)
	{
		return (UInt32)data.Value;
	}
	Int32 _value;
}

public enum BoolValue
{
	FALSE,
	TRUE,
}

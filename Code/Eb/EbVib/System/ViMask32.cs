using System;

public static class ViMask32
{
	public static bool HasAny(Int32 value, Int32 mask)
	{
		return ((value & mask) != 0);
	}
	public static bool HasAll(Int32 value, Int32 mask)
	{
		return ((value & mask) == mask);
	}
	public static void Add(ref Int32 value, Int32 mask)
	{
		value |= mask;
	}
	public static void Del(ref Int32 value, Int32 mask)
	{
		value &= ~mask;
	}
	public static Int32 Value(Int32 value, Int32 mask)
	{
		return (value & mask);
	}
	public static bool Enter(Int32 oldValue, Int32 iNewValue, Int32 mask)
	{
		return (HasAny(oldValue, mask) == false) && (HasAny(iNewValue, mask) == true);
	}
	public static bool Exit(Int32 oldValue, Int32 iNewValue, Int32 mask)
	{
		return (HasAny(oldValue, mask) == true) && (HasAny(iNewValue, mask) == false);
	}
	public static bool HasAny(UInt32 value, UInt32 mask)
	{
		return ((value & mask) != 0);
	}
	public static bool HasAll(UInt32 value, UInt32 mask)
	{
		return ((value & mask) == mask);
	}
	public static void Add(ref UInt32 value, UInt32 mask)
	{
		value |= mask;
	}
	public static void Del(ref UInt32 value, UInt32 mask)
	{
		value &= ~mask;
	}
	public static UInt32 Value(UInt32 value, UInt32 mask)
	{
		return (value & mask);
	}
	public static bool Enter(UInt32 oldValue, UInt32 iNewValue, UInt32 mask)
	{
		return (HasAny(oldValue, mask) == false) && (HasAny(iNewValue, mask) == true);
	}
	public static bool Exit(UInt32 oldValue, UInt32 iNewValue, UInt32 mask)
	{
		return (HasAny(oldValue, mask) == true) && (HasAny(iNewValue, mask) == false);
	}
	public static Int32 Mask(Int32 value)
	{
		Int32 mask = 1;
		mask = mask << (int)value;
		return mask;
	}
	public static UInt32 Mask(UInt32 value)
	{
		Int32 mask = 1;
		mask = mask << (int)value;
		return (UInt32)mask;
	}
}

public struct ViMask32<T>
{
	public Int32 Value { get { return _value; } }

	public ViMask32(Int32 value)
	{
		_value = value;
	}
	public bool HasAny(Int32 mask)
	{
		return ViMask32.HasAny(_value, mask);
	}
	public bool HasAll(Int32 mask)
	{
		return ViMask32.HasAll(_value, mask);
	}
	public void Add(Int32 mask)
	{
		ViMask32.Add(ref _value, mask);
	}
	public void Del(Int32 mask)
	{
		ViMask32.Del(ref _value, mask);
	}
	public static implicit operator Int32(ViMask32<T> data)
	{
		return data.Value;
	}
	public static implicit operator UInt32(ViMask32<T> data)
	{
		return (UInt32)data.Value;
	}

	Int32 _value;
}

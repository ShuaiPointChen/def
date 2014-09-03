using System;
using System.Collections.Generic;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public interface ViTupleInterface
{
	Object Value(UInt32 idx);
	UInt32 Size { get; }
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple : ViTupleInterface
{
	public Object Value(UInt32 idx)
	{
		return null;
	}
	public UInt32 Size { get { return 0; } }
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0> : ViTuple, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			default: return null;
		}
	}
	public T0 Value0 { get { return _value0; } set { _value0 = value; } }
	public new UInt32 Size { get { return 1; } }
	//
	public T0 _value0;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1> : ViTuple<T0>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			default: return null;
		}
	}
	public T1 Value1 { get { return _value1; } set { _value1 = value; } }
	public new UInt32 Size { get { return 2; } }
	//
	public T1 _value1;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1, T2> : ViTuple<T0, T1>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			case 2: return _value2;
			default: return null;
		}
	}
	public T2 Value2 { get { return _value2; } set { _value2 = value; } }
	public new UInt32 Size { get { return 3; } }
	//
	public T2 _value2;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1, T2, T3> : ViTuple<T0, T1, T2>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			case 2: return _value2;
			case 3: return _value3;
			default: return null;
		}
	}
	public T3 Value3 { get { return _value3; } set { _value3 = value; } }
	public new UInt32 Size { get { return 4; } }
	//
	public T3 _value3;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1, T2, T3, T4> : ViTuple<T0, T1, T2, T3>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			case 2: return _value2;
			case 3: return _value3;
			case 4: return _value4;
			default: return null;
		}
	}
	public T4 Value4 { get { return _value4; } set { _value4 = value; } }
	public new UInt32 Size { get { return 5; } }
	//
	public T4 _value4;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1, T2, T3, T4, T5> : ViTuple<T0, T1, T2, T3, T4>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			case 2: return _value2;
			case 3: return _value3;
			case 4: return _value4;
			case 5: return _value5;
			default: return null;
		}
	}
	public T5 Value5 { get { return _value5; } set { _value5 = value; } }
	public new UInt32 Size { get { return 6; } }
	//
	public T5 _value5;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViTuple<T0, T1, T2, T3, T4, T5, T6> : ViTuple<T0, T1, T2, T3, T4, T5>, ViTupleInterface
{
	public new Object Value(UInt32 idx)
	{
		switch (idx)
		{
			case 0: return _value0;
			case 1: return _value1;
			case 2: return _value2;
			case 3: return _value3;
			case 4: return _value4;
			case 5: return _value5;
			case 6: return _value6;
			default: return null;
		}
	}
	public T6 Value6 { get { return _value6; } set { _value6 = value; } }
	public new UInt32 Size { get { return 7; } }
	//
	public T6 _value6;
}

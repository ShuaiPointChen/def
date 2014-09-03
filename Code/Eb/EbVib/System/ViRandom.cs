
using System;
using System.Collections.Generic;

public static class ViRandom
{
	public static UInt32 Seed { set { _seed = value; } }

	public static bool Value(float prob)
	{
		MutateSeed();
		if (prob <= 0.0f)
		{
			return false;
		}
		if (prob >= 1.0f)
		{
			return true;
		}
		UInt32 range = (UInt32)(0XFFFFFFFF * prob);
		return range > _seed;
	}

	public static Int32 Probability(List<Int32> rangeList)
	{
		Int32 sup = 0;
		foreach (Int32 range in rangeList)
		{
			sup += range;
		}
		Int32 value = Value(0, sup);
		Int32 idx = 0;
		foreach (Int32 range in rangeList)
		{
			value -= range;
			if (value < 0)
			{
				return idx;
			}
			++idx;
		}
		return 0;
	}
	static bool Probability(List<UInt32> rangeList, out Int32 idx)
	{
		Int32 sup = 0;
		foreach (Int32 range in rangeList)
		{
			sup += range;
		}
		idx = 0;
		Int32 value = Value(0, sup);
		foreach (Int32 range in rangeList)
		{
			value -= range;
			if (value < 0)
			{
				return true;
			}
			++idx;
		}
		return false;
	}
	public static void GetValues(Int32 range, Int32 count, List<Int32> values)// 取值范围[0, range)
	{
		if (range <= count)
		{
			for (Int32 idx = 0; idx < range; ++idx)
			{
				values.Add(idx);
			}
		}
		else
		{
			for (Int32 idx = 0; idx < count; ++idx)
			{
				Int32 currentRange = range - idx;
				Int32 value = _GetFreeValue(values, Value(0, currentRange));
				values.Add(value);
			}
		}
	}
	public static Int32 Value(Int32 min, Int32 max)// 取值范围[min, max)
	{
		MutateSeed();
		if (min >= max)
		{
			return min;
		}
		UInt32 span = (UInt32)(max - min);
		Int32 value = (Int32)(_seed % span) + min;
		return value;
	}

	static void MutateSeed()
	{
		_seed = (_seed * 196314165) + 907633515;
	}

	static Int32 _GetFreeValue(List<Int32> values, Int32 idx)
	{
		Int32 value = 0;
		while (true)
		{
			if (!values.Contains(value))
			{
				if (idx == 0)
				{
					return value;
				}
				--idx;
			}
			++value;
		}
	}

	static UInt32 _seed = 0;
}

public static class Demo_Random
{
	public static void Test()
	{
		int range = 10000;
		int[] counts = new int[range];
		for (int idx = 0; idx < range*10; ++idx)
		{
			Int32 randomValue = ViRandom.Value(0, range);
			++counts[randomValue];
		}
		for (int idx = 0; idx < range; ++idx)
		{
			//ViDebuger.Note("" + idx + ">>" + counts[idx]);
		}
	}
}
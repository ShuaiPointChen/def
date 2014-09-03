using System;
using System.Collections;
using System.Collections.Generic;

using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViArrayIdx = System.Int32;

public static class ViSystemSerialize
{
	public static void Append(this ViOStream OS, List<Int8> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (Int8 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<Int8> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int8>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int8 value;
			IS.Read(out value);
			list.Add(value);
		}
	}

	public static void Append(this ViOStream OS, List<UInt8> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (UInt8 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<UInt8> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt8>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt8 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<Int16> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (Int16 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<Int16> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int16>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int16 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<UInt16> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (UInt16 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<UInt16> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt16>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt16 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<Int32> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (Int32 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<Int32> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int32>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int32 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<UInt32> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (UInt32 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<UInt32> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt32>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt32 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<Int64> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (Int64 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<Int64> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int64>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int64 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<UInt64> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (UInt64 value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<UInt64> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt64>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt64 value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<float> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (float value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<float> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<float>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			float value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<double> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (double value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<double> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<double>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			double value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static void Append(this ViOStream OS, List<string> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (string value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out List<string> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<string>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			string value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static bool Read(this ViStringIStream IS, out List<Int8> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int8>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int8 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<UInt8> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt8>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt8 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<Int16> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int16>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int16 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<UInt16> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt16>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt16 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<Int32> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int32>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int32 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<UInt32> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt32>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt32 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<Int64> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<Int64>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			Int64 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<UInt64> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<UInt64>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			UInt64 value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<float> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<float>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			float value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<double> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<double>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			double value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<string> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<string>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			string value;
			if (IS.Read(out value) == false) { return false; }
			list.Add(value);
		}
		return true;
	}
}


public static class ViStringSerialize
{
	public static void Read(this string strValue, out bool value)
	{
		value = Convert.ToBoolean(strValue);
	}
	public static void Read(this string strValue, out Int8 value)
	{
		value = Convert.ToSByte(strValue);
	}
	public static void Read(this string strValue, out UInt8 value)
	{
		value = Convert.ToByte(strValue);
	}
	public static void Read(this string strValue, out Int16 value)
	{
		value = Convert.ToInt16(strValue);
	}
	public static void Read(this string strValue, out UInt16 value)
	{
		value = Convert.ToUInt16(strValue);
	}
	public static void Read(this string strValue, out Int32 value)
	{
		value = Convert.ToInt32(strValue);
	}
	public static void Read(this string strValue, out UInt32 value)
	{
		value = Convert.ToUInt32(strValue);
	}
	public static void Read(this string strValue, out Int64 value)
	{
		value = Convert.ToInt64(strValue);
	}
	public static void Read(this string strValue, out UInt64 value)
	{
		value = Convert.ToUInt64(strValue);
	}
	public static void Read(this string strValue, out float value)
	{
		value = Convert.ToSingle(strValue);
	}
	public static void Read(this string strValue, out double value)
	{
		value = Convert.ToDouble(strValue);
	}
	public static void PrintTo(this Int8 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<Int8> list, ref string strValue)
	{
		foreach (Int8 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this UInt8 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<UInt8> list, ref string strValue)
	{
		foreach (UInt8 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this Int16 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<Int16> list, ref string strValue)
	{
		foreach (Int16 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this UInt16 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<UInt16> list, ref string strValue)
	{
		foreach (UInt16 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this Int32 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<Int32> list, ref string strValue)
	{
		foreach (Int32 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this UInt32 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<UInt32> list, ref string strValue)
	{
		foreach (UInt32 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this Int64 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<Int64> list, ref string strValue)
	{
		foreach (Int64 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this UInt64 value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<UInt64> list, ref string strValue)
	{
		foreach (UInt64 value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this float value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<float> list, ref string strValue)
	{
		foreach (float value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this double value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<double> list, ref string strValue)
	{
		foreach (double value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
	public static void PrintTo(this string value, ref string strValue)
	{
		strValue += value;
	}
	public static void PrintTo(this List<string> list, ref string strValue)
	{
		foreach (string value in list)
		{
			strValue += value;
			strValue += ",";
		}
		strValue += ")";
	}
}
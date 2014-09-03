using System;


public class ViMiscInt32
{
	public string _Name { get { return name; } }
	public Int32 _Value { get { return value; } }

	public string name = string.Empty;
	public Int32 value = 0;
}

public static class ViMiscInt32Assisstant
{
	public static Int32 Value(this ViStaticArray<ViMiscInt32> datas, string key, Int32 defaultValue)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				return datas[idx]._Value;
			}
		}
		return defaultValue;
	}
	public static Int32 Value(this ViStaticArray<ViMiscInt32> datas, string key)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				return datas[idx]._Value;
			}
		}
		return 0;
	}

	public static bool GetValue(this ViStaticArray<ViMiscInt32> datas, string key, ref Int32 value)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				value = datas[idx]._Value;
				return true;
			}
		}
		return false;
	}
}

public class ViMiscString
{
	public string _Name { get { return name; } }
	public string _Value { get { return value; } }

	public string name = string.Empty;
	public string value = string.Empty;
}

public static class ViMiscStringAssisstant
{
	public static string Value(this ViStaticArray<ViMiscString> datas, string key, string defaultValue)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				return datas[idx]._Value;
			}
		}
		return defaultValue;
	}
	public static string Value(this ViStaticArray<ViMiscString> datas, string key)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				return datas[idx]._Value;
			}
		}
		return string.Empty;
	}

	public static bool GetValue(this ViStaticArray<ViMiscString> datas, string key, ref string value)
	{
		for (int idx = 0; idx < datas.Length; ++idx)
		{
			if (String.Compare(datas[idx]._Name, key, true) == 0)
			{
				value = datas[idx]._Value;
				return true;
			}
		}
		return false;
	}
}

public struct ViVector3Struct
{
	public static implicit operator ViVector3(ViVector3Struct data)
	{
		return new ViVector3(data.X * 0.01f, data.Y * 0.01f, data.Z * 0.01f);
	}
	public Int32 X;
	public Int32 Y;
	public Int32 Z;
}
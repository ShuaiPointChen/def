using System;

using Int8 = System.SByte;
using UInt8 = System.Byte;

public static class ViBitConverter
{
	public static readonly bool IsLittleEndian = true;
	
	public static byte[] GetBytes(bool value)
	{
		byte byteValue = 1;
		if(value)
		{
			byteValue = 1;
		}
		else
		{
			byteValue = 0;
		}
		return new byte[] { byteValue };
	}

	public static byte[] GetBytes(Int8 value)
	{
		byte[] buffer = new byte[1];
		buffer[0] = (byte)value;
		return buffer;
	}

	public static byte[] GetBytes(UInt8 value)
	{
		byte[] buffer = new byte[1];
		buffer[0] = value;
		return buffer;
	}
	public static byte[] GetBytes(Int16 value)
	{
		byte[] buffer = new byte[2];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		return buffer;
	}
	public static byte[] GetBytes(UInt16 value)
	{
		byte[] buffer = new byte[2];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		return buffer;
	}
	public static byte[] GetBytes(Int32 value)
	{
		byte[] buffer = new byte[4];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		buffer[2] = (byte)(value >> 16);
		buffer[3] = (byte)(value >> 24);
		return buffer;
	}
	public static byte[] GetBytes(UInt32 value)
	{
		byte[] buffer = new byte[4];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		buffer[2] = (byte)(value >> 16);
		buffer[3] = (byte)(value >> 24);
		return buffer;
	}
	public static byte[] GetBytes(Int64 value)
	{
		byte[] buffer = new byte[8];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		buffer[2] = (byte)(value >> 16);
		buffer[3] = (byte)(value >> 24);
		buffer[4] = (byte)(value >> 32);
		buffer[5] = (byte)(value >> 40);
		buffer[6] = (byte)(value >> 48);
		buffer[7] = (byte)(value >> 56);
		return buffer;
	}
	public static byte[] GetBytes(UInt64 value)
	{
		byte[] buffer = new byte[8];
		buffer[0] = (byte)value;
		buffer[1] = (byte)(value >> 8);
		buffer[2] = (byte)(value >> 16);
		buffer[3] = (byte)(value >> 24);
		buffer[4] = (byte)(value >> 32);
		buffer[5] = (byte)(value >> 40);
		buffer[6] = (byte)(value >> 48);
		buffer[7] = (byte)(value >> 56);
		return buffer;
	}
	public static byte[] GetBytes(float value)
	{
		return GetBytes((int)(value * 100));
	}
	public static byte[] GetBytes(double value)
	{
		return GetBytes((long)(value * 100));
	}


	public static bool ToBool(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 1 > value.Length)
		{
			return false;
		}
		if (value[0] == 0) { return false; }
		else{ return true; }
	}
	public static Int8 ToInt8(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 1 > value.Length) 
		{
			return (Int8)0;
		}
		return (Int8)value[offset + 0];
	}
	public static UInt8 ToUInt8(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 1 > value.Length)
		{
			return (UInt8)0;
		}
		return (UInt8)value[offset + 0];
	}
	public static Int16 ToInt16(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 2 > value.Length)
		{
			return (Int16)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)));
		return (Int16)num;
	}
	public static UInt16 ToUInt16(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 2 > value.Length)
		{
			return (UInt16)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)));
		return (UInt16)num;
	}
	public static Int32 ToInt32(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 4 > value.Length)
		{
			return (Int32)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)) | (value[offset + 2] << 16)) | (value[offset + 3] << 24);
		return num;
	}
	public static UInt32 ToUInt32(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 4 > value.Length)
		{
			return (UInt32)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)) | (value[offset + 2] << 16)) | (value[offset + 3] << 24);
		return (UInt32)num;
	}
	public static Int64 ToInt64(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 8 > value.Length)
		{
			return (Int64)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)) | (value[offset + 2] << 16)) | (value[offset + 3] << 24);
		int num2 = ((value[offset + 4] | (value[offset + 5] << 8)) | (value[offset + 6] << 16)) | (value[offset + 7] << 24);
		UInt64 num3 = (UInt64)((UInt32)num);
		UInt64 num4 = (UInt64)((UInt32)num2);
		UInt64 num5 = num3 | (num4 << 32);
		return (Int64)num5;
	}
	public static UInt64 ToUInt64(byte[] value, int offset)
	{
		ViDebuger.AssertError(value);
		if (offset < 0 || offset + 8 > value.Length)
		{
			return (Int64)0;
		}
		int num = ((value[offset + 0] | (value[offset + 1] << 8)) | (value[offset + 2] << 16)) | (value[offset + 3] << 24);
		int num2 = ((value[offset + 4] | (value[offset + 5] << 8)) | (value[offset + 6] << 16)) | (value[offset + 7] << 24);
		UInt64 num3 = (UInt64)((UInt32)num);
		UInt64 num4 = (UInt64)((UInt32)num2);
		UInt64 num5 = num3 | (num4 << 32);
		return num5;
	}
	public static float ToFloat(byte[] value, int offset)
	{
		return ToInt32(value, offset) * 0.01f;
	}
	public static double ToDouble(byte[] value, int offset)
	{
		return ToInt64(value, offset) * 0.01;
	}
	public static string ToString(byte[] value)
	{
		if (value == null)
		{
			return string.Empty;
		}
		return ToString(value, 0, value.Length);
	}

	public static string ToString(byte[] value, int startIndex)
	{
		if (value == null)
		{
			return string.Empty;
		}
		return ToString(value, startIndex, value.Length - startIndex);
	}

	public static string ToString(byte[] value, int startIndex, int length)
	{
		ViDebuger.AssertError(value);
		if (startIndex < 0 || length < 0 || startIndex + length > value.Length)
		{
			return string.Empty;
		}
		int len16 = length / 2;
		char[] chArray = new char[len16 + 1];
		for (int idx = 0; idx < len16; ++idx)
		{
			int upper = (int)value[startIndex + 2 * idx + 1] << 8;
			int lower = (int)value[startIndex + 2 * idx];
			chArray[idx] = (char)(upper + lower);
		}
		chArray[len16] = (char)0;
		return new string(chArray, 0, len16);
	}
}

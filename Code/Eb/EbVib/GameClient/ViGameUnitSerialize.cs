using System;
using System.Collections;
using System.Collections.Generic;
using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViEntityID = System.UInt64;
using ViEntityTypeID = System.Byte;
using ViString = System.String;
using ViArrayIdx = System.Int32;
//
public static class ViGameUnitSerialize
{
	public static void Append(this ViOStream OS, ViGameUnit value)
	{
		ViEntitySerialize.Append(OS, value);
	}
	public static void Append(this ViOStream OS, List<ViGameUnit> list)
	{
		ViEntitySerialize.Append(OS, list);
	}
	public static void Read(this ViIStream IS, out ViGameUnit value)
	{
		ViEntitySerialize.Read(IS, out value);
	}
	public static void Read(this ViIStream IS, out List<ViGameUnit> list)
	{
		ViEntitySerialize.Read(IS, out list);
	}
	public static bool Read(this ViStringIStream IS, out ViGameUnit value)
	{
		return ViEntitySerialize.Read(IS, out value);
	}
	public static bool Read(this ViStringIStream IS, out List<ViGameUnit> list)
	{
		return ViEntitySerialize.Read(IS, out list);
	}
	public static void PrintTo(this ViGameUnit value, ref string strValue)
	{
		if (value != null)
		{
			strValue += value.Name;
		}
	}
	public static void PrintTo(this List<ViGameUnit> list, ref string strValue)
	{
		strValue += "(";
		foreach (ViGameUnit value in list)
		{
			if (value != null)
			{
				strValue += value.Name;
				strValue += ",";
			}
		}
		strValue += ")";
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;

public class ViStreamReader
{

	public static readonly BindingFlags BindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static bool Read<T>(ViStringIStream IS, out T obj)
	where T : class,  new()
	{
		obj = new T();
		return Read(IS, obj);
	}

	static bool Read(ViStringIStream IS, object obj)
	{
		FieldInfo[] fieldList = ViSealedDataAssisstant.GetFeilds(obj.GetType());
		foreach (FieldInfo field in fieldList)
		{
			if (field.FieldType.Equals(typeof(Int32)))
			{
				if (ReadInt32Field(IS, field, ref obj) == false)
				{
					return false;
				}
			}
			else if (field.FieldType.Equals(typeof(String)))
			{
				if (ReadStringField(IS, field, ref obj) == false)
				{
					return false;
				}
			}
			else if (field.FieldType.Name.StartsWith("ViMask32"))
			{
				if (ReadStructInt32Field(IS, field, ref obj) == false)
				{
					return false;
				}
			}
			else if (field.FieldType.Name.StartsWith("ViEnum32"))
			{
				if (ReadStructInt32Field(IS, field, ref obj) == false)
				{
					return false;
				}
			}
			else if (field.FieldType.Name.StartsWith("ViForeignKey32"))
			{
				if (ReadStructInt32Field(IS, field, ref obj) == false)
				{
					return false;
				}
			}
			else if (field.FieldType.Name.StartsWith("ViStaticArray"))
			{
				object fieldObject = field.GetValue(obj);
				int len = ViArrayParser.Length(fieldObject);
				for (int idx = 0; idx < len; ++idx)
				{
					object elementObject = ViArrayParser.Object(fieldObject, idx);
					if (Read(IS, elementObject) == false)
					{
						return false;
					}
					field.SetValue(obj, fieldObject);
				}
			}
			else
			{
				object fieldObject = field.GetValue(obj);
				if (Read(IS, fieldObject) == false)
				{
					return false;
				}
				field.SetValue(obj, fieldObject);
			}
		}
		return true;
	}

	static bool ReadStringField(ViStringIStream IS, FieldInfo field, ref object data)
	{
		string value;
		if (IS.Read(out value))
		{
			field.SetValue(data, value);
			return true;
		}
		else
		{
			return false;
		}
	}
	static bool ReadInt32Field(ViStringIStream IS, FieldInfo field, ref object data)
	{
		Int32 value;
		if (IS.Read(out value))
		{
			field.SetValue(data, value);
			return true;
		}
		else
		{
			return false;
		}
	}
	static bool ReadStructInt32Field(ViStringIStream IS, FieldInfo field, ref object data)
	{
		object obj = field.GetValue(data);
		FieldInfo[] fieldList = obj.GetType().GetFields(BindingFlag);
		ViDebuger.AssertError(fieldList.Length == 1);
		Int32 value;
		if (IS.Read(out value))
		{
			fieldList[0].SetValue(obj, value);
			field.SetValue(data, obj);
			return true;
		}
		else
		{
			return false;
		}
	}
}
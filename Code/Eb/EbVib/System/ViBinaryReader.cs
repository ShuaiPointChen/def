using System;
using System.Collections.Generic;
using System.Reflection;

public class ViBinaryReader
{
	public static readonly BindingFlags BindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static readonly Type ViStaticArrayType = typeof(ViStaticArray<object>).GetGenericTypeDefinition();
	public static readonly Type ViEnumType = typeof(ViEnum32<object>).GetGenericTypeDefinition();
	public static readonly Type ViMaskType = typeof(ViMask32<object>).GetGenericTypeDefinition();
	public static readonly Type ViForeignKeyType = typeof(ViForeignKey32<ViSealedData>).GetGenericTypeDefinition();

	public static bool Read<T>(ViIStream IS, out T obj)
		where T : class, new()
	{
		obj = new T();
		return Read(IS, obj);
	}
	public static bool ReadSealedData<T>(ViIStream IS, out T obj)
		where T : ViSealedData, new()
	{
		obj = new T();
		return Read(IS, obj);
	}

	static bool Read(ViIStream IS, object obj)
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
			else
			{
				if (field.FieldType.IsGenericType)
				{
					field.FieldType.GetGenericTypeDefinition();
					if (field.FieldType.GetGenericTypeDefinition() == ViStaticArrayType)
					{
						if (ReadArray(IS, field, ref obj) == false)
						{
							return false;
						}
					}
					else
					{
						if (ReadStructInt32Field(IS, field, ref obj) == false)
						{
							return false;
						}
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
		}
		return true;
	}

	static bool ReadArray(ViIStream IS, FieldInfo field, ref object data)
	{
		object fieldObject = field.GetValue(data);
		int len = ViArrayParser.Length(fieldObject);
		for (int idx = 0; idx < len; ++idx)
		{
			object elementObject = ViArrayParser.Object(fieldObject, idx);
			if (elementObject.GetType().Equals(typeof(Int32)))
			{
				Int32 value;
				if (IS.Read(out value) == false)
				{
					return false;
				}
				ViArrayParser.SetObject(fieldObject, idx, value);
			}
			else if (elementObject.GetType().Equals(typeof(String)))
			{
				string value;
				if (IS.Read(out value) == false)
				{
					return false;
				}
				ViArrayParser.SetObject(fieldObject, idx, elementObject);
			}
			else
			{
				if (Read(IS, elementObject) == false)
				{
					return false;
				}
				ViArrayParser.SetObject(fieldObject, idx, elementObject);
			}
		}
		field.SetValue(data, fieldObject);
		return true;
	}
	static bool ReadStringField(ViIStream IS, FieldInfo field, ref object data)
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
	static bool ReadInt32Field(ViIStream IS, FieldInfo field, ref object data)
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
	static bool ReadStructInt32Field(ViIStream IS, FieldInfo field, ref object data)
	{
		object obj = field.GetValue(data);
		FieldInfo[] fieldList = obj.GetType().GetFields(BindingFlag);
		//ViDebuger.AssertError(fieldList.Length == 1);
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
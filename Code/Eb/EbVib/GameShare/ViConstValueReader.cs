using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ViConstValueReader
{
	public static void Load(string fileName)
	{
		XmlDocument xml = new XmlDocument();
		xml.Load(fileName);
		Load(xml["Root"]);
	}

	public static void Load(XmlNode root)
	{
		LoadBool("BoolData", root);
		LoadBoolList("BoolListData", root);
		LoadInt32("Int32Data", root);
		LoadInt32List("Int32ListData", root);
		LoadFloat("FloatData", root);
		LoadFloatList("FloatListData", root);
		LoadString("StringData", root);
		LoadStringList("StringListData", root);
		LoadVector3("Vector3Data", root);
		LoadVector3List("Vector3ListData", root);
	}

	static void LoadBool(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlElement valueElement = node["Value"];
		if (nameElement != null && valueElement != null)
		{
			bool value;
			valueElement.InnerText.Read(out value);
			Log("ConstValue<bool>[" + nameElement.InnerText + "] = " + value);
			ViConstValueList<bool>.AddValue(nameElement.InnerText, value);
		}
	}
	static void LoadReefBool(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadBool(element);
		}
	}
	static void LoadBool(string name, XmlNode node)
	{
		LoadReefBool(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefBool(name, element);
		}
	}

	//+-----------------------------------------------------------------------------
	static void LoadBoolList(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlNodeList elementList = node.SelectNodes("Value");
		List<bool> valueList = new List<bool>(elementList.Count);
		foreach (XmlNode element in elementList)
		{
			bool value;
			element.InnerText.Read(out value);
			valueList.Add(value);
		}
		Log("ConstValue<BoolList>[" + nameElement.InnerText + "] = " + valueList);
		ViConstValueList<List<bool>>.AddValue(nameElement.InnerText, valueList);
	}
	static void LoadReefBoolList(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadBoolList(element);
		}
	}
	static void LoadBoolList(string name, XmlNode node)
	{
		LoadReefBoolList(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefBoolList(name, element);
		}
	}


	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	static void LoadInt32(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlElement valueElement = node["Value"];
		if (nameElement != null && valueElement != null)
		{
			Int32 value;
			valueElement.InnerText.Read(out value);
			Log("ConstValue<Int32>[" + nameElement.InnerText + "] = " + value);
			ViConstValueList<Int32>.AddValue(nameElement.InnerText, value);
		}
	}
	static void LoadReefInt32(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadInt32(element);
		}
	}
	static void LoadInt32(string name, XmlNode node)
	{
		LoadReefInt32(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefInt32(name, element);
		}
	}

	//+-----------------------------------------------------------------------------
	static void LoadInt32List(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlNodeList elementList = node.SelectNodes("Value");
		List<Int32> valueList = new List<Int32>(elementList.Count);
		foreach (XmlNode element in elementList)
		{
			Int32 value;
			element.InnerText.Read(out value);
			valueList.Add(value);
		}
		Log("ConstValue<Int32List>[" + nameElement.InnerText + "] = " + valueList);
		ViConstValueList<List<Int32>>.AddValue(nameElement.InnerText, valueList);
	}
	static void LoadReefInt32List(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadInt32List(element);
		}
	}
	static void LoadInt32List(string name, XmlNode node)
	{
		LoadReefInt32List(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefInt32List(name, element);
		}
	}

	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	static void LoadFloat(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlElement valueElement = node["Value"];
		if (nameElement != null && valueElement != null)
		{
			float value;
			valueElement.InnerText.Read(out value);
			Log("ConstValue<Float>[" + nameElement.InnerText + "] = " + value);
			ViConstValueList<float>.AddValue(nameElement.InnerText, value);
		}
	}
	static void LoadReefFloat(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadFloat(element);
		}
	}
	static void LoadFloat(string name, XmlNode node)
	{
		LoadReefFloat(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefFloat(name, element);
		}
	}

	//+-----------------------------------------------------------------------------
	static void LoadFloatList(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlNodeList elementList = node.SelectNodes("Value");
		List<float> valueList = new List<float>(elementList.Count);
		foreach (XmlNode element in elementList)
		{
			float value;
			element.InnerText.Read(out value);
			valueList.Add(value);
		}
		Log("ConstValue<FloatList>[" + nameElement.InnerText + "] = " + valueList);
		ViConstValueList<List<float>>.AddValue(nameElement.InnerText, valueList);
	}
	static void LoadReefFloatList(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadFloatList(element);
		}
	}
	static void LoadFloatList(string name, XmlNode node)
	{
		LoadReefFloatList(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefFloatList(name, element);
		}
	}
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	static void LoadString(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlElement valueElement = node["Value"];
		if (nameElement != null && valueElement != null)
		{
			Log("ConstValue<String>[" + nameElement.InnerText + "] = " + valueElement.InnerText);
			ViConstValueList<string>.AddValue(nameElement.InnerText, valueElement.InnerText);
		}
	}
	static void LoadReefString(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadString(element);
		}
	}
	static void LoadString(string name, XmlNode node)
	{
		LoadReefString(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefString(name, element);
		}
	}

	//+-----------------------------------------------------------------------------
	static void LoadStringList(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlNodeList elementList = node.SelectNodes("Value");
		List<string> valueList = new List<string>(elementList.Count);
		foreach (XmlNode element in elementList)
		{
			valueList.Add(element.InnerText);
		}
		Log("ConstValue<StringList>[" + nameElement.InnerText + "] = " + valueList);
		ViConstValueList<List<string>>.AddValue(nameElement.InnerText, valueList);
	}
	static void LoadReefStringList(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadStringList(element);
		}
	}
	static void LoadStringList(string name, XmlNode node)
	{
		LoadReefStringList(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefStringList(name, element);
		}
	}


	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	static void LoadVector3(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlElement valueElement = node["Value"];
		if (nameElement != null && valueElement != null)
		{
			ViVector3 value;
			valueElement.InnerText.Read(out value);
			Log("ConstValue<Vector3>[" + nameElement.InnerText + "] = " + value);
			ViConstValueList<ViVector3>.AddValue(nameElement.InnerText, value);
		}
	}
	static void LoadReefVector3(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadVector3(element);
		}
	}
	static void LoadVector3(string name, XmlNode node)
	{
		LoadReefVector3(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefVector3(name, element);
		}
	}

	//+-----------------------------------------------------------------------------
	static void LoadVector3List(XmlNode node)
	{
		XmlElement nameElement = node["Name"];
		XmlNodeList elementList = node.SelectNodes("Value");
		List<ViVector3> valueList = new List<ViVector3>(elementList.Count);
		foreach (XmlNode element in elementList)
		{
			ViVector3 value;
			element.InnerText.Read(out value);
			valueList.Add(value);
		}
		Log("ConstValue<Vector3List>[" + nameElement.InnerText + "] = " + valueList);
		ViConstValueList<List<ViVector3>>.AddValue(nameElement.InnerText, valueList);
	}
	static void LoadReefVector3List(string name, XmlNode node)
	{
		foreach (XmlNode element in node.SelectNodes(name))
		{
			LoadVector3List(element);
		}
	}
	static void LoadVector3List(string name, XmlNode node)
	{
		LoadReefVector3List(name, node);
		foreach (XmlNode element in node.ChildNodes)
		{
			LoadReefVector3List(name, element);
		}
	}

	static void Log(string log)
	{
		ViDebuger.Note(log);
	}
}
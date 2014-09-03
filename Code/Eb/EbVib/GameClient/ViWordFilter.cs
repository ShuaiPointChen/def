using System;
using System.Collections.Generic;

public class ViWordFilter
{
	public void AddWord(string src, string dest)
	{
		ViDebuger.AssertError(src.Length == dest.Length);
		int len = src.Length;
		ViDebuger.AssertError(len < _dictionaryList.Count);
		Dictionary<string, string> dict = _dictionaryList[len];
		if (!dict.ContainsKey(src))
		{
			dict.Add(src, dest);
		}
	}

	public void SetMaxLen(int len)
	{
		_dictionaryList.Capacity = len + 1;
		for (int iter = 0; iter < len + 1; ++iter)
		{
			_dictionaryList.Add(new Dictionary<string, string>());
		}
	}

	public string Filter(string value)
	{
		int strLen = value.Length;
		for (int leftIter = 0; leftIter < strLen; ++leftIter)
		{
			int maxRight = Math.Min(leftIter + _dictionaryList.Count-1, strLen);
			int rightIter = leftIter + 1;
			while (rightIter <= maxRight)
			{
				string subString = value.Substring(leftIter, rightIter - leftIter);
				string replaceString;
				if (Filter(subString, out replaceString))
				{
					value = value.Replace(subString, replaceString);
					leftIter = rightIter - 1;
					break;
				}
				else
				{
					++rightIter;
				}
			}
		}
		return value;
	}

	public bool IsFiltered(string value)
	{
		int strLen = value.Length;
		for (int leftIter = 0; leftIter < strLen; ++leftIter)
		{
			int maxRight = Math.Min(leftIter + _dictionaryList.Count - 1, strLen);
			int rightIter = leftIter + 1;
			while (rightIter <= maxRight)
			{
				string subString = value.Substring(leftIter, rightIter - leftIter);
				string replaceString;
				if (Filter(subString, out replaceString))
				{
					return true;
				}
				else
				{
					++rightIter;
				}
			}
		}
		return false;
	}

	bool Filter(string src, out string dest)
	{
		int len = src.Length;
		ViDebuger.AssertError(len < _dictionaryList.Count);
		if (_dictionaryList[len].TryGetValue(src, out dest))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	List<Dictionary<string, string>> _dictionaryList = new List<Dictionary<string, string>>();

}
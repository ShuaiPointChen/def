using System;
using System.Collections.Generic;


public class ViEntityCommandInvoker
{
	public delegate bool DeleExec(ViEntity entity, string name, List<string> paramList);
	public static void Register(DeleExec dele)
	{
		ViDebuger.AssertError(dele);
		_execFuncList.Add(dele);
	}
	public static bool Exec(ViEntity entity, string name, List<string> paramList)
	{
		foreach (DeleExec exec in _execFuncList)
		{
			ViDebuger.AssertError(exec);
			if (exec(entity, name, paramList) == true)
			{
				return true;
			}
		}
		return false;
	}
	static List<DeleExec> _execFuncList = new List<DeleExec>();
}

public class ViEntityCommandInvoker<TEntity>
{
	public delegate bool DeleExec(TEntity entity, List<string> paramList);

	public static bool Exec(TEntity entity, string name, List<string> paramList)
	{
		List<DeleExec> list;
		string lowName = name.ToLower();
		if (_execFuncList.TryGetValue(lowName, out list))
		{
			foreach (DeleExec dele in list)
			{
				if (dele(entity, paramList))
				{
					return true;
				}
			}
		}
		return false;
	}

	protected static void AddFunc(string name, DeleExec dele)
	{
		List<DeleExec> list;
		string lowName = name.ToLower();
		if (_execFuncList.TryGetValue(lowName, out list))
		{
			list.Add(dele);
		}
		else
		{
			list = new List<DeleExec>();
			list.Add(dele);
			_execFuncList.Add(lowName, list);
		}
	}

	static Dictionary<string, List<DeleExec>> _execFuncList = new Dictionary<string, List<DeleExec>>();
}
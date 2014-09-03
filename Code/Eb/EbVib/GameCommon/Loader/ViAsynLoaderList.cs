using System;
using System.Collections.Generic;

public class ViAsynLoaderList
{
	public delegate void OnCompleted(ViAsynLoaderList loaderList);

	public OnCompleted DeleOnCompleted { get { return _deleOnCompleted; } set { _deleOnCompleted = value; } }
	public bool IsLoaded
	{
		get
		{
			foreach (ViAsynLoader iterLoader in _list)
			{
				if (iterLoader.IsLoaded == false)
				{
					return false;
				}
			}
			return true;
		}
	}
	public void Add(ViAsynLoader loader)
	{
		_list.Add(loader);
		loader.Attach(this.OnLoaded);
	}
	public void Del(ViAsynLoader loader)
	{
		foreach (ViAsynLoader iterLoader in _list)
		{
			if (iterLoader == loader)
			{
				loader.Detach();
				_list.Remove(iterLoader);
				break;
			}
		}
	}
	public void Clear()
	{
		foreach (ViAsynLoader loader in _list)
		{
			loader.Detach();
		}
		_list.Clear();
		_deleOnCompleted = null;
	}

	void OnLoaded(ViAsynLoader loader)
	{
		//ViDebuger.Note("ViAsynLoaderList.OnLoaded");
		foreach (ViAsynLoader iterLoader in _list)
		{
			if (iterLoader.IsLoaded == false)
			{
				return;
			}
		}
		OnCompleted dele = _deleOnCompleted;
		_deleOnCompleted = null;
		if (dele != null)
		{
			dele(this);
		}
	}

	OnCompleted _deleOnCompleted;
	List<ViAsynLoader> _list = new List<ViAsynLoader>();

}
using System;

public abstract class ViAsynLoader
{
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void UpdateAll()
	{
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViRefNode2<ViAsynLoader> node = _list.CurrentNode;
			ViAsynLoader loader = node.Data;
			ViDebuger.AssertError<ViAsynLoader>(loader, "ViAsynLoader.UpdateAll() loader is null");
			_list.Next();
			loader.TryLoad();
			if (loader.IsLoaded)
			{
				OnCompleted dele = loader.DeleOnCompleted;
				loader.Detach();
				loader.OnLoaded();
				if (dele != null)
				{
					dele(loader);
				}
			}
		}
	}
	public static void ClearAll()
	{
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViRefNode2<ViAsynLoader> node = _list.CurrentNode;
			ViAsynLoader loader = node.Data;
			ViDebuger.AssertError<ViAsynLoader>(loader, "ViAsynLoader.ClearAll() loader is null");
			_list.Next();
			loader.Detach();
		}
		_list.Clear();
	}

	static ViRefList2<ViAsynLoader> _list = new ViRefList2<ViAsynLoader>();

	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	public delegate void OnCompleted(ViAsynLoader loader);
	public OnCompleted DeleOnCompleted { get { return _deleOnCompleted; } }
	public virtual bool IsLoaded { get { return true; } }
	public bool IsAttach { get { return _callbackNode.IsAttach(); } }

	public virtual void OnLoaded() { }
	public virtual void TryLoad() { }

	public void Detach()
	{
		_callbackNode.Detach();
		_callbackNode.Data = null;
		_deleOnCompleted = null;
	}
	public void Attach()
	{
		_callbackNode.Data = this;
		_list.PushBack(_callbackNode);
	}
	public void Attach(OnCompleted dele)
	{
		_deleOnCompleted = dele;
		_callbackNode.Data = this;
		_list.PushBack(_callbackNode);
	}
	OnCompleted _deleOnCompleted;
	ViRefNode2<ViAsynLoader> _callbackNode = new ViRefNode2<ViAsynLoader>();
}

using System;
using System.Collections.Generic;

using ViOperatorIdx = System.Byte;
using ViArrayIdx = System.Int32;
using UInt8 = System.Byte;



public interface ViReceiveDataSetNodeNodeWatcher<TReceiveKey>
	where TReceiveKey : ViReceiveDataKeyInterface
{
	void OnStart(TReceiveKey key,ViEntity entity);
	void OnUpdate(TReceiveKey key, ViEntity entity);
	void OnEnd(TReceiveKey key, ViEntity entity);
}

public class ViReceiveDataSetNode<TReceiveKey>
	where TReceiveKey : ViReceiveDataKeyInterface
{
	public ViReceiveDataSetNodeNodeWatcher<TReceiveKey> Watcher;
}

public class ViReceiveDataSet<TReceiveKey, TProteKey> : ViReceiveDataNode
	where TReceiveKey : ViReceiveDataKeyInterface, new()
{
	public static readonly UInt16 END_SLOT = 0XFFFF;

	public delegate ViReceiveDataSetNodeNodeWatcher<TReceiveKey> WatcherCreator();
	public static WatcherCreator Creator;

	public ViEventList<TReceiveKey> UpdateArrayCallbackList { get { return _updateArrayCallbackList; } }
	public Dictionary<TReceiveKey, ViReceiveDataSetNode<TReceiveKey>> Array { get { return _array; } }
	//
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (!MatchChannel(channel))
		{
			return;
		}
		ViOperatorIdx op;
		IS.Read(out op);
		switch ((ViDataArrayOperator)op)
		{
			case ViDataArrayOperator.INSERT:
				{
					TReceiveKey key = new TReceiveKey();
					key.Read(IS);
					ViDebuger.AssertWarning(!_array.ContainsKey(key));
					//
					ViReceiveDataSetNode<TReceiveKey> newNode = new ViReceiveDataSetNode<TReceiveKey>();
					AttachWatcher(key, newNode, entity);
					_array[key] = newNode;
					OnUpdateInvoke(null);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.INSERT, key);
				}
				break;
			case ViDataArrayOperator.DEL:
				{
					TReceiveKey key = new TReceiveKey();
					key.Read(IS);
					ViDebuger.AssertWarning(_array.ContainsKey(key));
					//
					ViReceiveDataSetNode<TReceiveKey> node = _array[key];
					_array.Remove(key);
					OnUpdateInvoke(null);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.DEL, key);
					DetachWatcher(key, node, entity);
				}
				break;
			case ViDataArrayOperator.CLEAR:
				foreach (KeyValuePair<TReceiveKey, ViReceiveDataSetNode<TReceiveKey>> pair in _array)
				{
					ViReceiveDataSetNode<TReceiveKey> node = pair.Value;
					DetachWatcher(pair.Key, node, entity);
				}
				_array.Clear();
				OnUpdateInvoke(null);
				_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.CLEAR, default(TReceiveKey));
				break;
			default:
				break;
		}
	}
	public new void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (!MatchChannel(channel))
		{
			return;
		}
		ViArrayIdx size;
		IS.Read(out size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TReceiveKey key = new TReceiveKey();
			key.Read(IS);
			ViReceiveDataSetNode<TReceiveKey> newNode = new ViReceiveDataSetNode<TReceiveKey>();
			AttachWatcher(key, newNode, entity);
			_array[key] = newNode;
		}
	}
	public new void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		if (!MatchChannel(channelMask))
		{
			return;
		}
		ViArrayIdx size;
		IS.Read(out size);
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TReceiveKey key = new TReceiveKey();
			key.Read(IS);
			ViReceiveDataSetNode<TReceiveKey> newNode = new ViReceiveDataSetNode<TReceiveKey>();
			AttachWatcher(key, newNode, entity);
			_array[key] = newNode;
		}
	}
	public override void End(ViEntity entity)
	{
		foreach (KeyValuePair<TReceiveKey, ViReceiveDataSetNode<TReceiveKey>> pair in _array)
		{
			ViReceiveDataSetNode<TReceiveKey> node = pair.Value;
			if (node.Watcher != null)
			{
				node.Watcher.OnEnd(pair.Key, entity);
			}
		}
		_array.Clear();
		DetachAllCallback();
		base.End(entity);
	}

	public override void Clear()
	{
		_array.Clear();
		DetachAllCallback();
		base.Clear();
	}

	//
	void DetachAllCallback()
	{
		_updateArrayCallbackList.Clear();
	}

	void AttachWatcher(TReceiveKey key, ViReceiveDataSetNode<TReceiveKey> node, ViEntity entity)
	{
		if (Creator != null)
		{
			ViReceiveDataSetNodeNodeWatcher<TReceiveKey> watcher = Creator();
			node.Watcher = watcher;
			if (watcher != null)
			{
				watcher.OnStart(key, entity);
			}
		}
	}

	void DetachWatcher(TReceiveKey key, ViReceiveDataSetNode<TReceiveKey> node, ViEntity entity)
	{
		if (node.Watcher != null)
		{
			node.Watcher.OnEnd(key, entity);
			node.Watcher = null;
		}
	}

	Dictionary<TReceiveKey, ViReceiveDataSetNode<TReceiveKey>> _array = new Dictionary<TReceiveKey, ViReceiveDataSetNode<TReceiveKey>>();
	ViEventList<TReceiveKey> _updateArrayCallbackList = new ViEventList<TReceiveKey>();
}

public static class ViReceiveDataSetSerialize
{
	public static void Append<TReceiveKey, TProteKey>(this ViOStream OS, ViReceiveDataSet<TReceiveKey, TProteKey> value)
		where TReceiveKey : ViReceiveDataKeyInterface, new()
	{
		ViDebuger.Error("ViReceiveDataSetSerialize未实现代码");
		//value.Update();
	}
}
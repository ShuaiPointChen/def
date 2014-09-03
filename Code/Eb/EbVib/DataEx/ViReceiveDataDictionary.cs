using System;
using System.Collections.Generic;

using ViOperatorIdx = System.Byte;
using ViArrayIdx = System.Int32;
using UInt8 = System.Byte;


public interface ViReceiveDataDictionaryNodeNodeWatcher<TReceiveKey, TReceiveData>
	where TReceiveKey : ViReceiveDataKeyInterface
	where TReceiveData : ViReceiveDataNode
{
	void OnStart(TReceiveKey key, TReceiveData property, ViEntity entity);
	void OnUpdate(TReceiveKey key, TReceiveData property, ViEntity entity);
	void OnEnd(TReceiveKey key, TReceiveData property, ViEntity entity);
}

public class ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>
	where TReceiveKey : ViReceiveDataKeyInterface
	where TReceiveData : ViReceiveDataNode
{
	public TReceiveData Property;
	public ViReceiveDataDictionaryNodeNodeWatcher<TReceiveKey, TReceiveData> Watcher;
}

public class ViReceiveDataDictionary<TReceiveKey, TProteKey, TReceiveData, TProtoData> : ViReceiveDataNode
	where TReceiveKey : ViReceiveDataKeyInterface, new()
	where TReceiveData : ViReceiveDataNode, new()
{
	public static readonly UInt16 END_SLOT = 0XFFFF;

	public delegate ViReceiveDataDictionaryNodeNodeWatcher<TReceiveKey, TReceiveData> WatcherCreator();
	public static WatcherCreator Creator;

	public ViEventList<TReceiveKey, TReceiveData> UpdateArrayCallbackList { get { return _updateArrayCallbackList; } }
	public Dictionary<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>> Array { get { return _array; } }
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
					TReceiveData property = new TReceiveData();
					property.StartByArray();
					property.Start(channel, IS, entity);
					property.Parent = this;
					ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> newNode = new ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>();
					newNode.Property = property;
					AttachWatcher(key, newNode, entity);
					_array[key] = newNode;
					OnUpdateInvoke(null);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.INSERT, key, property);
				}
				break;
			case ViDataArrayOperator.DEL:
				{
					TReceiveKey key = new TReceiveKey();
					key.Read(IS);
					ViDebuger.AssertWarning(_array.ContainsKey(key));
					//
					ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node = _array[key];
					_array.Remove(key);
					OnUpdateInvoke(node.Property);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.DEL, key, node.Property);
					DetachWatcher(key, node, entity);
					node.Property.End(entity);
					node.Property.Clear();
				}
				break;
			case ViDataArrayOperator.MOD:
				{
					TReceiveKey key = new TReceiveKey();
					key.Read(IS);
					ViDebuger.AssertWarning(_array.ContainsKey(key));
					ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node = _array[key];
					UInt16 slot;
					while (IS.Read(out slot) && slot != END_SLOT)
					{
						node.Property.OnUpdate(slot, channel, IS, entity);
					}
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.MOD, key, node.Property);
					if (node.Watcher != null)
					{
						node.Watcher.OnUpdate(key, node.Property, entity);
					}
				}
				break;
			case ViDataArrayOperator.CLEAR:
				foreach (KeyValuePair<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>> pair in _array)
				{
					ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node = pair.Value;
					DetachWatcher(pair.Key, node, entity);
					node.Property.End(entity);
					node.Property.Clear();
				}
				_array.Clear();
				OnUpdateInvoke(null);
				_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.CLEAR, default(TReceiveKey), null);
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
			TReceiveData property = new TReceiveData();
			property.StartByArray();
			property.Start(channel, IS, entity);
			property.Parent = this;
			ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> newNode = new ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>();
			newNode.Property = property;
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
			TReceiveData property = new TReceiveData();
			property.StartByArray();
			property.Start(channelMask, IS, entity);
			property.Parent = this;
			ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> newNode = new ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>();
			newNode.Property = property;
			AttachWatcher(key, newNode, entity);
			_array[key] = newNode;
		}
	}
	public override void End(ViEntity entity)
	{
		foreach (KeyValuePair<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>> pair in _array)
		{
			ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node = pair.Value;
			if (node.Watcher != null)
			{
				node.Watcher.OnEnd(pair.Key, node.Property, entity);
			}
			node.Property.End(entity);
			node.Property.Clear();
		}
		_array.Clear();
		DetachAllCallback();
		base.End(entity);
	}

	public override void Clear()
	{
		foreach (KeyValuePair<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>> pair in _array)
		{
			ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node = pair.Value;
			node.Property.Clear();
		}
		_array.Clear();
		DetachAllCallback();
		base.Clear();
	}

	//
	void DetachAllCallback()
	{
		_updateArrayCallbackList.Clear();
	}

	void AttachWatcher(TReceiveKey key, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node, ViEntity entity)
	{
		if (Creator != null)
		{
			ViReceiveDataDictionaryNodeNodeWatcher<TReceiveKey, TReceiveData> watcher = Creator();
			node.Watcher = watcher;
			if (watcher != null)
			{
				watcher.OnStart(key, node.Property, entity);
			}
		}
	}

	void DetachWatcher(TReceiveKey key, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData> node, ViEntity entity)
	{
		if (node.Watcher != null)
		{
			node.Watcher.OnEnd(key, node.Property, entity);
			node.Watcher = null;
		}
	}

	Dictionary<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>> _array = new Dictionary<TReceiveKey, ViReceiveDataDictionaryNode<TReceiveKey, TReceiveData>>();
	ViEventList<TReceiveKey, TReceiveData> _updateArrayCallbackList = new ViEventList<TReceiveKey, TReceiveData>();
}

public static class ViReceiveDataDictionarySerialize
{
	public static void Append<TReceiveKey, TProteKey, TReceiveData, TProtoData>(this ViOStream OS, ViReceiveDataDictionary<TReceiveKey, TProteKey, TReceiveData, TProtoData> value)
		where TReceiveKey : ViReceiveDataKeyInterface, new()
		where TReceiveData : ViReceiveDataNode, new()
	{
		ViDebuger.Error("ViReceiveDataDictionarySerialize未实现代码");
		//value.Update();
	}
}
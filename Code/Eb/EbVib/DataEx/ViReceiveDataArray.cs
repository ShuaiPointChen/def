using System;
using System.Collections;
using System.Collections.Generic;
using ViOperatorIdx = System.Byte;
using ViArrayIdx = System.Int32;
using UInt8 = System.Byte;


public interface ViReceiveDataArrayNodeWatcher<TReceiveData>
	where TReceiveData : ViReceiveDataNode
{
	void OnStart(TReceiveData property, ViEntity entity);
	void OnUpdate(TReceiveData property, ViEntity entity);
	void OnEnd(TReceiveData property, ViEntity entity);
}

public class ViReceiveDataArrayNode<TReceiveData>
	where TReceiveData : ViReceiveDataNode
{
	public TReceiveData Property;
	public ViReceiveDataArrayNodeWatcher<TReceiveData> Watcher;
}

public class ViReceiveDataArray<TReceiveData, TProto> : ViReceiveDataNode
	where TReceiveData : ViReceiveDataNode, new()
{
	public static readonly UInt16 END_SLOT = 0XFFFF;

	public delegate ViReceiveDataArrayNodeWatcher<TReceiveData> WatcherCreator();
	public static WatcherCreator Creator;

	public ViEventList<TReceiveData> UpdateArrayCallbackList { get { return _updateArrayCallbackList; } }
	public int Count { get { return _array.Count; } }
	public List<ViReceiveDataArrayNode<TReceiveData>> Array { get { return _array; } }
	public ViReceiveDataArrayNode<TReceiveData> this[int index] { get { return Array[index]; } }
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
			case ViDataArrayOperator.ADD_BACK:
				{
					TReceiveData property = new TReceiveData();
					property.StartByArray();
					property.Start(channel, IS, entity);
					property.Parent = this;
					ViReceiveDataArrayNode<TReceiveData> newNode = new ViReceiveDataArrayNode<TReceiveData>();
					newNode.Property = property;
					AttachWatcher(newNode, entity);
					_array.Add(newNode);
					OnUpdateInvoke(null);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.ADD_BACK, property);
				}
				break;
			case ViDataArrayOperator.ADD_FRONT:
				{
					TReceiveData property = new TReceiveData();
					property.StartByArray();
					property.Start(channel, IS, entity);
					property.Parent = this;
					ViReceiveDataArrayNode<TReceiveData> newNode = new ViReceiveDataArrayNode<TReceiveData>();
					newNode.Property = property;
					AttachWatcher(newNode, entity);
					_array.Insert(0, newNode);
					OnUpdateInvoke(null);
					_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.ADD_FRONT, property);
				}
				break;
			case ViDataArrayOperator.INSERT:
				{
					ViArrayIdx idx;
					IS.Read(out idx);
					ViDebuger.AssertWarning(idx <= _array.Count);
					if (idx <= _array.Count)
					{
						TReceiveData property = new TReceiveData();
						property.StartByArray();
						property.Start(channel, IS, entity);
						property.Parent = this;
						ViReceiveDataArrayNode<TReceiveData> newNode = new ViReceiveDataArrayNode<TReceiveData>();
						newNode.Property = property;
						AttachWatcher(newNode, entity);
						_array.Insert(idx, newNode);
						OnUpdateInvoke(null);
						_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.INSERT, property);
					}
				}
				break;
			case ViDataArrayOperator.DEL:
				{
					ViArrayIdx idx;
					IS.Read(out idx);
					ViDebuger.AssertWarning(idx < _array.Count);
					if (idx < _array.Count)
					{
						ViReceiveDataArrayNode<TReceiveData> node = _array[idx];
						_array.RemoveAt(idx);
						OnUpdateInvoke(node.Property);
						_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.DEL, node.Property);
						DetachWatcher(node, entity);
						node.Property.End(entity);
						node.Property.Clear();
					}
				}
				break;
			case ViDataArrayOperator.MOD:
				{
					ViArrayIdx idx;
					IS.Read(out idx);
					ViDebuger.AssertWarning(idx < _array.Count);
					if (idx < _array.Count)
					{
						ViReceiveDataArrayNode<TReceiveData> node = _array[idx];
						UInt16 slot;
						while (IS.Read(out slot) && slot != END_SLOT)
						{
							node.Property.OnUpdate(slot, channel, IS, entity);
						}
						_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.MOD, node.Property);
						if (node.Watcher != null)
						{
							node.Watcher.OnUpdate(node.Property, entity);
						}
					}
				}
				break;
			case ViDataArrayOperator.CLEAR:
				foreach (ViReceiveDataArrayNode<TReceiveData> node in _array)
				{
					DetachWatcher(node, entity);
					node.Property.End(entity);
					node.Property.Clear();
				}
				_array.Clear();
				OnUpdateInvoke(null);
				_updateArrayCallbackList.Invoke((UInt32)ViDataArrayOperator.CLEAR, null);
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
		_array.Capacity = (int)size;
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TReceiveData property = new TReceiveData();
			property.StartByArray();
			property.Start(channel, IS, entity);
			property.Parent = this;
			ViReceiveDataArrayNode<TReceiveData> newNode = new ViReceiveDataArrayNode<TReceiveData>();
			newNode.Property = property;
			AttachWatcher(newNode, entity);
			_array.Add(newNode);
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
		_array.Capacity = (int)size;
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TReceiveData property = new TReceiveData();
			property.StartByArray();
			property.Start(channelMask, IS, entity);
			property.Parent = this;
			ViReceiveDataArrayNode<TReceiveData> newNode = new ViReceiveDataArrayNode<TReceiveData>();
			newNode.Property = property;
			AttachWatcher(newNode, entity);
			_array.Add(newNode);
		}
	}
	public override void End(ViEntity entity)
	{
		foreach (ViReceiveDataArrayNode<TReceiveData> node in _array)
		{
			if (node.Watcher != null)
			{
				node.Watcher.OnEnd(node.Property, entity);
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
		foreach (ViReceiveDataArrayNode<TReceiveData> node in _array)
		{
			node.Property.Clear();
		}
		_array.Clear();
		DetachAllCallback();
		base.Clear();
	}

	void DetachAllCallback()
	{
		_updateArrayCallbackList.Clear();
	}

	void AttachWatcher(ViReceiveDataArrayNode<TReceiveData> node, ViEntity entity)
	{
		if (Creator != null)
		{
			ViReceiveDataArrayNodeWatcher<TReceiveData> watcher = Creator();
			node.Watcher = watcher;
			if (watcher != null)
			{
				watcher.OnStart(node.Property, entity);
			}
		}
	}

	void DetachWatcher(ViReceiveDataArrayNode<TReceiveData> node, ViEntity entity)
	{
		if (node.Watcher != null)
		{
			node.Watcher.OnEnd(node.Property, entity);
			node.Watcher = null;
		}
	}

	List<ViReceiveDataArrayNode<TReceiveData>> _array = new List<ViReceiveDataArrayNode<TReceiveData>>();
	ViEventList<TReceiveData> _updateArrayCallbackList = new ViEventList<TReceiveData>();
}

public static class ViReceiveDataArraySerialize
{
	public static void Append<T, L>(this ViOStream OS, ViReceiveDataArray<T, L> value)
	where T : ViReceiveDataNode, new()
	{
		ViDebuger.Error("ViReceiveDataArraySerialize未实现代码");
		//value.Update();
	}
}
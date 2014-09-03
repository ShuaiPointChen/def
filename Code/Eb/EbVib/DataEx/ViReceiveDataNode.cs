using System;
using System.Collections;
using System.Collections.Generic;

using UInt8 = System.Byte;
using Index = System.UInt16;

public class ViReceiveProperty
{
	public static readonly int SIZE = 0;

	public delegate void DeleIndexPropertyUpdate(Index idx, ViReceiveDataInt32 data, Int32 oldValue);
	public DeleIndexPropertyUpdate OnIndexPropertyUpdate;

	public void ReservePropertySize(int size)
	{
		_updateSlots.Capacity = size;
	}
	public Index RegisterNode(ViReceiveDataNode node)
	{
		_updateSlots.Add(node);
		return (Index)(_updateSlots.Count - 1);
	}
	public void OnUpdate(Index slot, UInt8 channel, ViIStream IS, ViEntity entity)
	{
		ViDebuger.AssertError(slot < _updateSlots.Count);
		ViReceiveDataNode data = _updateSlots[slot];
		ViDebuger.AssertError(data);
		if (OnIndexPropertyUpdate != null)
		{
			Index idx = 0;
			if (_slotToIndex.TryGetValue(slot, out idx))
			{
				ViReceiveDataInt32 dataInt32 = data as ViReceiveDataInt32;
				Int32 oldValue = dataInt32.Value;
				data.OnUpdate(channel, IS, entity);
				OnIndexPropertyUpdate(idx, dataInt32, oldValue);
			}
			else
			{
				data.OnUpdate(channel, IS, entity);
			}
		}
		else
		{
			data.OnUpdate(channel, IS, entity);
		}
	}
	public void StartProperty(UInt16 channelMask, ViIStream IS, ViEntity entity) { }
	public void OnPropertyUpdate(UInt8 channel, ViIStream IS, ViEntity entity) { }
	public void EndProperty(ViEntity entity)
	{
		OnIndexPropertyUpdate = null;
	}
	public virtual void Clear()
	{
		OnIndexPropertyUpdate = null;
		_updateSlots.Clear();
		_updateSlots = null;
	}
	public void ReserveIdxPropertySize(int size)
	{
		_indexPropertys.Capacity = size;
	}
	public void AddIdxProperty(ViReceiveDataInt32 data)
	{
		_indexPropertys.Add(data);
		_slotToIndex[data.SlotIdx] = (Index)(_indexPropertys.Count - 1);
	}
	public ViReceiveDataInt32 GetIdxProperty(Index idx)
	{
		if (idx >= _indexPropertys.Count)
		{
			return null;
		}
		else
		{
			return _indexPropertys[idx];
		}
	}

	Dictionary<Index, Index> _slotToIndex = new Dictionary<Index, Index>();
	List<ViReceiveDataInt32> _indexPropertys = new List<ViReceiveDataInt32>();
	List<ViReceiveDataNode> _updateSlots = new List<ViReceiveDataNode>();
}


public enum ViReceiveDataNodeEventID
{
	START,
	UPDATE,
	DESTROY,
}

public enum ViDataArrayOperator
{
	ADD_BACK,
	ADD_FRONT,
	INSERT,
	MOD,
	DEL,
	CLEAR,
}

public class ViReceiveDataNode : ViReceiveProperty
{
	public ViReceiveDataNode Parent { get { return _parent; } set { _parent = value; } }
	public ViEventList<ViReceiveDataNode, object> CallbackList { get { return _updateCallbackList; } }
	public Index SlotIdx { get { return _slot; } }
	public bool MatchChannel(UInt8 channel)
	{
		UInt16 mask = (UInt16)(1 << (int)channel);
		return (_channelMask & mask) != 0;
	}
	public bool MatchChannel(UInt16 channelMask)
	{
		return (channelMask & _channelMask) != 0;
	}
	//
	public virtual void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		ViDebuger.Warning("Must be Override");
	}
	public virtual void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		ViDebuger.Warning("Must be Override");
	}
	public virtual void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{

	}
	public virtual void End(ViEntity entity)
	{
		_updateCallbackList.Clear();
		EndProperty(entity);
	}
	public override void Clear()
	{
		_updateCallbackList.Clear();
		_parent = null;
		base.Clear();
	}
	public void RegisterTo(UInt16 channelMask, ViReceiveProperty property)
	{
		_channelMask = channelMask;
		_slot = property.RegisterNode(this);
	}
	public virtual void StartByArray() { }
	protected void OnUpdateInvoke(object oldValue)
	{
		_updateCallbackList.Invoke((UInt32)ViReceiveDataNodeEventID.UPDATE, this, oldValue);
		if (_parent != null)
		{
			_parent.OnUpdateFromChildren(this, oldValue);
		}
	}

	void OnUpdateFromChildren(ViReceiveDataNode node, object oldValue)
	{
		_updateCallbackList.Invoke((UInt32)ViReceiveDataNodeEventID.UPDATE, node, oldValue);
		if (_parent != null)
		{
			_parent.OnUpdateFromChildren(node, oldValue);
		}
	}

	Index _slot;
	UInt16 _channelMask;
	ViReceiveDataNode _parent;
	ViEventList<ViReceiveDataNode, object> _updateCallbackList = new ViEventList<ViReceiveDataNode, object>();
}
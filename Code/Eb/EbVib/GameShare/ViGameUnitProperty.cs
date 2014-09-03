using System;
using System.Collections;
using System.Collections.Generic;
using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViEntityID = System.UInt64;
using ViString = System.String;
using ViArrayIdx = System.Int32;
public class ViGameUnitReceiveProperty : ViReceiveProperty
{
	public static readonly new int SIZE = 5;
	public ViReceiveDataUInt32 ActionState = new ViReceiveDataUInt32();
	public ViReceiveDataUInt32 MoveState = new ViReceiveDataUInt32();
	public ViReceiveDataUInt32 AuraState = new ViReceiveDataUInt32();
	public ViReceiveDataArray<ReceiveDataVisualAuraProperty, VisualAuraProperty> VisualAuraPropertyList = new ViReceiveDataArray<ReceiveDataVisualAuraProperty, VisualAuraProperty>();
	public ViReceiveDataArray<ReceiveDataLogicAuraProperty, LogicAuraProperty> LogicAuraPropertyList = new ViReceiveDataArray<ReceiveDataLogicAuraProperty, LogicAuraProperty>();
	public ViGameUnitReceiveProperty()
	{
		ReservePropertySize(SIZE);
		ReserveIdxPropertySize(0);
		ActionState.RegisterTo((UInt16)((1 << (int)ViGameLogicChannel.ALL_CLIENT)), this);
		MoveState.RegisterTo((UInt16)((1 << (int)ViGameLogicChannel.ALL_CLIENT)), this);
		AuraState.RegisterTo((UInt16)((1 << (int)ViGameLogicChannel.ALL_CLIENT)), this);
		VisualAuraPropertyList.RegisterTo((UInt16)((1 << (int)ViGameLogicChannel.ALL_CLIENT)), this);
		LogicAuraPropertyList.RegisterTo((UInt16)((1 << (int)ViGameLogicChannel.DB)), this);
	}
	public new void OnPropertyUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		UInt16 slot;
		IS.Read(out slot);
		OnUpdate(slot, channel, IS, entity);
	}
	public new void StartProperty(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		base.StartProperty(channelMask, IS, entity);
		ActionState.Start(channelMask, IS, entity);
		MoveState.Start(channelMask, IS, entity);
		AuraState.Start(channelMask, IS, entity);
		VisualAuraPropertyList.Start(channelMask, IS, entity);
		LogicAuraPropertyList.Start(channelMask, IS, entity);
	}
	public new void EndProperty(ViEntity entity)
	{
		base.EndProperty(entity);
		ActionState.End(entity);
		MoveState.End(entity);
		AuraState.End(entity);
		VisualAuraPropertyList.End(entity);
		LogicAuraPropertyList.End(entity);
	}
	public override void Clear()
	{
		base.Clear();
		ActionState.Clear();
		MoveState.Clear();
		AuraState.Clear();
		VisualAuraPropertyList.Clear();
		LogicAuraPropertyList.Clear();
	}
}

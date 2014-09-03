using System;



public struct ViAnimStruct
{
	public bool IsEmpty { get { return (res == null || res == string.Empty); } }
	public Int32 scale;
	public string res;
}


public class ViCameraShakeStruct
{
	public bool IsEmpty { get { return ((Duration == 0) || (Amplitude == 0)); } }
	//
	public Int32 Duration;
	public Int32 Amplitude;
	public Int32 Range;
	public Int32 Probability;
	public Int32 Reserve1;
	public Int32 Reserve2;
}

public struct ViAttachExpressStruct
{
	public bool IsEmpty { get { return (res == null || res == string.Empty); } }
	public float Scale { get { return (scale == 0) ? 1.0f : scale * 0.01f; } }
	public Int32 delayTime;
	public Int32 duration;
	public Int32 fadeTime;
	public Int32 fadeType;
	public Int32 scale;
	public string res;
	public Int32 attachPos;
	public Int32 attachType;
}

public enum ViTravelEndExpressDirection
{
	INHERIT,
	RESERVE,
}

public class ViTravelExpressStruct
{
	public bool IsEmpty { get { return (res == null || res == string.Empty); } }
	public Int32 srcPos;
	public Int32 destPos;
	public Int32 gravity;
	public string res;
	public Int32 reserveTime;
	public string reserveRes;
	public ViEnum32<ViTravelEndExpressDirection> reserveDirection;
	public ViCameraShakeStruct hitShake = new ViCameraShakeStruct();
}

public class ViAvatarDurationVisualStruct : ViSealedData
{
	static readonly UInt32 MISC_VALUE_MAX = 10;

	public Int32 reserve0;
	public Int32 type;
	public Int32 duration;
	public ViStaticArray<ViMiscInt32> miscValue = new ViStaticArray<ViMiscInt32>(MISC_VALUE_MAX);
}
public class ViVisualAuraStruct : ViSealedData
{
	public struct DurationEffect 
	{
		public ViForeignKey32<ViAvatarDurationVisualStruct> effect;
	}
	static readonly UInt32 EXPRESS_MAX = 3;
	static readonly UInt32 DURATION_VISUAL_MAX = 3;
	
	public ViAnimStruct anim = new ViAnimStruct();

	public string description = string.Empty;
	public string icon = string.Empty;

	public Int32 weight;
	public Int32 displayerType;

	public ViStaticArray<ViAttachExpressStruct> express = new ViStaticArray<ViAttachExpressStruct>(EXPRESS_MAX);
	public ViStaticArray<DurationEffect> durationVisual = new ViStaticArray<DurationEffect>(DURATION_VISUAL_MAX);
}

public class ViVisualHitEffectStruct : ViSealedData
{
	static readonly UInt32 ANIM_MAX = 3;
	static readonly UInt32 EXPRESS_MAX = 5;

	public Int32 reserve0;
	public ViStaticArray<ViAnimStruct> anims = new ViStaticArray<ViAnimStruct>(ANIM_MAX);
	public ViStaticArray<ViAttachExpressStruct> express = new ViStaticArray<ViAttachExpressStruct>(EXPRESS_MAX);
	public ViCameraShakeStruct hitShake = new ViCameraShakeStruct();
}

public class ViVisualDriveStruct : ViSealedData
{
	public ViAnimStruct anim;
	public ViForeignKey32<ViVisualAuraStruct> auraVisual;
	public ViForeignKey32<ViVisualHitEffectStruct> hiteffectVisual;

	//
	public Int32 reserve0;
	public Int32 reserve1;
	public Int32 reserve2;
	public Int32 reserve3;
	public Int32 reserve4;
	public Int32 reserve5;
}


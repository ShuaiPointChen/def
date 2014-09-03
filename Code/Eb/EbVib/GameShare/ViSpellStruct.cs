using System;


public class ViSpellValueStruct
{
	public static readonly UInt32 REF_VALUE_MAX = 2;
	public ViUnitValueRange value;
	public ViStaticArray<ViUnitRefValue> casterRefValue = new ViStaticArray<ViUnitRefValue>(REF_VALUE_MAX);
	public ViStaticArray<ViUnitRefValue> targetRefValue = new ViStaticArray<ViUnitRefValue>(REF_VALUE_MAX);
}
public struct ViSpellCostStruct
{
	public ViUnitValue value;
	public ViUnitRefValue refValue;
}

public class ViTargetSelectStruct
{
	public static readonly UInt32 CONDITION_MAX = 3;

	public ViMask32<ViSpellSelectRalationMask> casterMask;
	public ViAreaRangeStruct casterEffectRange;
	public ViMask32<ViSpellSelectRalationMask> targetMask;
	public ViAreaRangeStruct targetEffectRange;
	public Int32 count;
	public Int32 probability;
	public ViStaticArray<ViUnitValueRange> condition = new ViStaticArray<ViUnitValueRange>(CONDITION_MAX);
	public ViForeignKey32<ViStateConditionStruct> state;
}

public class ViHitEffectStruct
{
	public static readonly UInt32 MISC_VALUE_MAX = 5;

	public bool IsEmpty { get { return (template == (Int32)ViHitEffectTemplate.NONE); } }
	public Int32 MiscValue(string key, Int32 defaultValue) { return miscValue.Value(key, defaultValue); }
	public Int32 MiscValue(string key) { return miscValue.Value(key); }
	public bool IsArea { get { return template == (Int32)ViHitEffectTemplate.AREA; } }

	public Int32 template;
	public Int32 type;
	public Int32 Reserve_0;
	public ViEnum32<ViEffectSign> effectSign;

	public Int32 delay;

	public Int32 visual;

	public ViEnum32<ViEffectSelectorIdx> selectorChannel;
	public Int32 createProbabilityChannel;
	public Int32 createProbability;

	public ViSpellValueStruct valueSet = new ViSpellValueStruct();

	public ViStaticArray<ViMiscInt32> miscValue = new ViStaticArray<ViMiscInt32>(MISC_VALUE_MAX);
}

public class ViAuraStruct
{
	public static readonly UInt32 VALUE_MAX = 4;
	public static readonly UInt32 CONDITION_MAX = 3;
	public static readonly UInt32 END_EVENT_MAX = 3;
	public static readonly UInt32 MISC_VALUE_MAX = 5;

	//static bool IsNotEmpty(ValueRangeArray& kCondition);
	public bool IsEmpty { get { return (template == (Int32)ViAuraTemplate.NONE) && (ScriptAura.Length == 0); } }
	public Int32 MiscValue(string key, Int32 defaultValue) { return miscValue.Value(key, defaultValue); }
	public Int32 MiscValue(string key) { return miscValue.Value(key); }
	public bool IsArea { get { return template == (Int32)ViAuraTemplate.AREA; } }

	public Int32 template;
	public Int32 type;
	public string ScriptAura;
	public ViEnum32<ViEffectSign> effectSign;

	public Int32 visual;

	///<创建>
	public ViEnum32<ViEffectSelectorIdx> selectorChannel;
	public Int32 createChannel;
	public Int32 createProbability;

	///<创建>
	public Int32 closeEffectChannel;
	public Int32 effectChannel;
	public Int32 effectChannelWeight;
	public ViEnum32<ViAuraAttachType> attachType;
	public Int32 maxAuras;

	///<作用时间>
	public ViEnum32<ViAccumulateTimeType> AccumulateTimeType;
	public Int32 delay;
	public Int32 tickCnt;
	public Int32 amplitude;

	///<作用/结束条件>
	public ViStaticArray<ViUnitValueRange> effectCondition = new ViStaticArray<ViUnitValueRange>(CONDITION_MAX);
	public ViForeignKey32<ViStateConditionStruct> effectState;

	public ViStaticArray<ViUnitValueRange> endCondition = new ViStaticArray<ViUnitValueRange>(CONDITION_MAX);
	public ViForeignKey32<ViStateConditionStruct> endState;

	public ViStaticArray<Int32> endEvents = new ViStaticArray<Int32>(END_EVENT_MAX);

	public Int32 actionStateMask;
	public Int32 auraStateMask;

	public ViStaticArray<ViSpellValueStruct> valueSet = new ViStaticArray<ViSpellValueStruct>(VALUE_MAX);

	public ViStaticArray<ViMiscInt32> miscValue = new ViStaticArray<ViMiscInt32>(MISC_VALUE_MAX);
}

public struct ViSpellTravelStruct
{
	public ViEnum32<ViSpellTravelType> type;
	public Int32 speed;
	public Int32 range;//! 飞行范围
}

//+-----------------------------------------------------------------------------
public class ViSpellProcStruct
{
	public static readonly UInt32 COST_MAX = 3;
	public static readonly UInt32 CONDITION_MAX = 3;
	public static readonly UInt32 MISC_VALUE_MAX = 5;

	public Int32 MiscValue(string key, Int32 defaultValue) { return miscValue.Value(key, defaultValue); }
	public Int32 MiscValue(string key) { return miscValue.Value(key); }

	public string ScriptFlow;
	public ViMask32<ViUnitSocietyMask> focusTargetMask;
	public Int32 facingFlag;
	public Int32 addStateMask;
	public Int32 delStateMask;
	public Int32 cutDurationMask;

    public ViEnum32<ViSpellLogType> LogType;
	public Int32 reserve_1;
	public Int32 reserve_2;
	public Int32 reserve_3;
	public Int32 reserve_4;

	public ViStaticArray<ViUnitValueRange> casterCondition = new ViStaticArray<ViUnitValueRange>(CONDITION_MAX);
	public ViForeignKey32<ViStateConditionStruct> casterState;

	public ViStaticArray<ViUnitValueRange> focusCondition = new ViStaticArray<ViUnitValueRange>(CONDITION_MAX);
	public ViForeignKey32<ViStateConditionStruct> focusState;

	public Int32 coolChannel;
	public Int32 coolDuration;

	public Int32 prepareTime;
	public Int32 castTime;
	public Int32 castCnt;
	public Int32 castEndTime;
	public Int32 actCoolTime;

	public Int32 rangeInf;
	public Int32 rangeSup;

	public ViSpellTravelStruct traveller;

	public ViStaticArray<ViSpellCostStruct> cost = new ViStaticArray<ViSpellCostStruct>(COST_MAX);

	public ViStaticArray<ViMiscInt32> miscValue = new ViStaticArray<ViMiscInt32>(MISC_VALUE_MAX);
}

public class ViSpellStruct : ViSealedData
{
	static readonly UInt32 SELECT_MAX = 3;
	static readonly UInt32 EFFECT_MAX = 3;
	
	public Int32 reqLevel;

	public ViSpellProcStruct proc = new ViSpellProcStruct();

	public ViStaticArray<ViTargetSelectStruct> targetSelect = new ViStaticArray<ViTargetSelectStruct>(SELECT_MAX);
	public ViStaticArray<ViHitEffectStruct> htEffectInfos = new ViStaticArray<ViHitEffectStruct>(EFFECT_MAX);
	public ViStaticArray<ViAuraStruct> auraInfos = new ViStaticArray<ViAuraStruct>(EFFECT_MAX);

}

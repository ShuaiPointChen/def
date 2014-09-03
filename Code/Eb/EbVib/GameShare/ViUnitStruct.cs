using System;


public struct ViUnitValue
{
	public bool IsEmpty() { return (type == 0); }
	public Int32 type;
	public Int32 value;
}

public struct ViValueRange
{
	public Int32 Value { get { return ViRandom.Value(Inf, Sup + 1); } }
	public bool IsIn(Int32 value) { return (Inf <= value && value <= Sup); }
	public Int32 Inf;
	public Int32 Sup;
}

public struct ViUnitValueRange
{
	public bool IsEmpty() { return (type == 0); }
	public Int32 type;
	public ViValueRange value;
}

public struct ViUnitRefValue
{
	public bool IsEmpty() { return (type == 0 || percValue == 0); }
	public Int32 type;
	public Int32 percValue;
}

public class ViStateConditionStruct : ViSealedData
{
	public bool IsEmpty() { return (reqAuraState | notAuraState | reqActionState | notActionState | reqMoveState | notMoveState) == 0; }

	public Int32 reqAuraState;
	public Int32 notAuraState;
	public Int32 reqActionState;
	public Int32 notActionState;
	public Int32 reqMoveState;
	public Int32 notMoveState;
}


public struct ViAreaRangeStruct
{
	public enum GameAreaType
	{
		ROUND,//! 圆形
		RECT,//! 矩形
		SECTOR,//! 扇形
	}
	public Int32 type;								// 范围类型
	public Int32 minRange;						// 最小距离(圆)最小半径:米(矩形)横:米(扇形)弧度
	public Int32 maxRange;						// 最大距离(圆)最大半径:米(矩形)竖:米(扇形)半径
}
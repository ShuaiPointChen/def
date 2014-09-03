using System;



public class ViValueMapping : ViRefNode1<ViValueMapping>
{
	public static bool GetValue(Int32 deltaValue, ViRefList1<ViValueMapping> list, ref Int32 detalValue, ref UInt32 idx)
	{
		if (list.Size == 0)
		{
			return false;
		}
		ViValueMapping node = list.GetHead() as ViValueMapping;
		ViDebuger.AssertError(node);
		idx = node.DestIdx;
		detalValue = node.Value(deltaValue);
		return true;
	}
	//
	public UInt32 DestIdx { get { return _destIdx; } }
	public float Scale { get { return _scale; } }
	//
	public Int32 Value(Int32 deltaValue)
	{
		return (Int32)(deltaValue * _scale);
	}
	public Int32 RevertValue(Int32 deltaValue)
	{
		ViDebuger.AssertError(_scale != 0.0f);
		return (Int32)(deltaValue / _scale);
	}
	//
	void Init(UInt32 destIdx, float scale)
	{
		_scale = scale;
		if (_scale == 0.0f)
		{
			_scale = 1.0f;
		}
		_destIdx = destIdx;
	}
	//
	UInt32 _destIdx;
	float _scale = 1.0f;
}
using System;

public struct ViAngle
{

	public static readonly float INF = -ViMathDefine.PI;
	public static readonly float SUP = ViMathDefine.PI;

	public ViAngle(ViAngle rAngle)
	{
		_value = rAngle.Value;
	}
	public ViAngle(float angle)
	{
		_value = angle;
		Normalize();
	}
	public float Value { get { return _value; } set { _value = value; Normalize(); } }
	public void SetValue(float value) { _value = value; Normalize(); }
	//
	public static void Normalize(ref float angle)
	{
		if (angle >= ViMathDefine.PI)
		{
			if (angle > ViMathDefine.PI_X3)
			{
				angle = angle - (Int32)(angle / ViMathDefine.PI_X2) * ViMathDefine.PI_X2;
			}
			else
			{
				angle = angle - ViMathDefine.PI_X2;
			}
		}
		else if (angle < -ViMathDefine.PI)
		{
			if (angle < -ViMathDefine.PI_X3)
			{
				angle = angle - (Int32)(angle / ViMathDefine.PI_X2 - 1) * ViMathDefine.PI_X2;
			}
			else
			{
				angle = angle + ViMathDefine.PI_X2;
			}
		}
	}
	public static float Diff(ViAngle from, ViAngle to)
	{
		float fDiff = from.Value - to.Value;
		if (fDiff < -ViMathDefine.PI)
		{
			return fDiff + ViMathDefine.PI_X2;
		}
		else if (fDiff >= ViMathDefine.PI)
		{
			return fDiff - ViMathDefine.PI_X2;
		}
		else
		{
			return fDiff;
		}
	}
	public static float SameSignAngle(ViAngle angle, ViAngle record)
	{
		if (angle.Value > record.Value + ViMathDefine.PI)
		{
			return angle.Value - ViMathDefine.PI_X2;
		}
		else if (angle.Value < record.Value - ViMathDefine.PI)
		{
			return angle.Value + ViMathDefine.PI_X2;
		}
		else
		{
			return angle.Value;
		}
	}

	public static ViAngle operator +(ViAngle kAngle1, ViAngle kAngle2)
	{
		return new ViAngle(kAngle1.Value + kAngle2.Value);
	}
	public static ViAngle operator -(ViAngle kAngle1, ViAngle kAngle2)
	{
		return new ViAngle(kAngle1.Value - kAngle2.Value);
	}
	public bool IsBetween(ViAngle kLeft, ViAngle kRight)
	{
		if (kLeft.Value <= kRight.Value)
		{
			return (kLeft.Value <= Value && Value <= kRight.Value);
		}
		else
		{
			return (kLeft.Value <= Value || Value <= kRight.Value);
		}
	}
	public void Lerp(ViAngle a, ViAngle b, float t)
	{
		_value = a.Value * t + SameSignAngle(a, b) * (1 - t);
		Normalize();
	}
	void Normalize()
	{
		Normalize(ref _value);
	}

	//
	private float _value;
}

public class Demo_Angle
{
#pragma warning disable 0219
	public static void Test()
	{

		ViAngle angle0 = new ViAngle(ViMathDefine.PI * 0.5f);
		ViAngle angle1 = new ViAngle(-ViMathDefine.PI);

		float fDiff = ViAngle.Diff(angle0, angle1);//-pi/ 2
		float fSameSignAngle = ViAngle.SameSignAngle(angle0, angle1);//- 1.5pi
		bool bIsIn = angle1.IsBetween(angle1, angle0);//true

		ViAngle angle2 = new ViAngle(8.0f);
		ViAngle angle3 = new ViAngle(12.0f);

		float fDiff1 = ViAngle.Diff(angle2, angle3);
		float fSameSignAngle1 = ViAngle.SameSignAngle(angle2, angle3);
		bool bIsIn1 = angle1.IsBetween(angle2, angle3);

		ViAngle angle4 = new ViAngle(3.0f);
		ViAngle angle5 = new ViAngle(-3.0f);
		ViAngle angle6 = new ViAngle(-3.1f);
		ViAngle angle7 = new ViAngle(1.0f);
		bool bIsIn2 = angle6.IsBetween(angle4, angle5);
		ViDebuger.AssertError(bIsIn2 == true);
		bool bIsIn3 = angle6.IsBetween(angle5, angle4);
		ViDebuger.AssertError(bIsIn3 == false);
		bool bIsIn4 = angle7.IsBetween(angle4, angle5);
		ViDebuger.AssertError(bIsIn4 == false);
		bool bIsIn5 = angle7.IsBetween(angle5, angle4);
		ViDebuger.AssertError(bIsIn5 == true);
	}
#pragma warning restore 0219
}

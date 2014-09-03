using System;

using Int8 = System.SByte;
using UInt8 = System.Byte;

public static class ViMathDefine
{
	public static readonly float PI = 3.141593f;
	public static readonly float PI_X2 = PI * 2;
	public static readonly float PI_X3 = PI * 3;
	public static readonly float PI_X4 = PI * 4;
	public static readonly float PI_HALF = PI * 0.5f;
	public static readonly float Infinity = float.PositiveInfinity;
	public static readonly float NegativeInfinity = float.NegativeInfinity;
	public static readonly float Deg2Rad = 0.01745329f;
	public static readonly float Rad2Deg = 57.29578f;
	public static readonly float Epsilon = float.Epsilon;
	public static float Sin(float f)
	{
		return (float)Math.Sin((double)f);
	}

	public static float Cos(float f)
	{
		return (float)Math.Cos((double)f);
	}

	public static float Tan(float f)
	{
		return (float)Math.Tan((double)f);
	}

	public static float Asin(float f)
	{
		return (float)Math.Asin((double)f);
	}

	public static float Acos(float f)
	{
		return (float)Math.Acos((double)f);
	}

	public static float Atan(float f)
	{
		return (float)Math.Atan((double)f);
	}

	public static float Atan2(float y, float x)
	{
		return (float)Math.Atan2((double)y, (double)x);
	}

	public static float Sqrt(float f)
	{
		return (float)Math.Sqrt((double)f);
	}

	public static Int32 IntInf(float value)
	{
		if (value >= 0)
		{
			return (Int32)value;
		}
		else
		{
			return (Int32)(value - 0.999f);
		}
	}
	public static Int32 IntSup(float value) 
	{
		if (value >= 0)
		{
			return (Int32)(value + 0.999f);
		}
		else
		{
			return (Int32)value;
		}
	}
	public static Int32 IntNear(float value)
	{
		return (Int32)(value + 0.5f);
	}

	public static float Abs(float value)
	{
		return Math.Abs(value);
	}
	public static Int8 Abs(Int8 value)
	{
		return Math.Abs(value);
	}
	public static Int16 Abs(Int16 value)
	{
		return Math.Abs(value);
	}
	public static Int32 Abs(Int32 value)
	{
		return Math.Abs(value);
	}
	public static Int64 Abs(Int64 value)
	{
		return Math.Abs(value);
	}
	public static float Min(float a, float b)
	{
		return ((a >= b) ? b : a);
	}
	public static Int8 Min(Int8 a, Int8 b)
	{
		return ((a >= b) ? b : a);
	}
	public static UInt8 Min(UInt8 a, UInt8 b)
	{
		return ((a >= b) ? b : a);
	}
	public static Int16 Min(Int16 a, Int16 b)
	{
		return ((a >= b) ? b : a);
	}
	public static UInt16 Min(UInt16 a, UInt16 b)
	{
		return ((a >= b) ? b : a);
	}
	public static Int32 Min(Int32 a, Int32 b)
	{
		return ((a >= b) ? b : a);
	}
	public static UInt32 Min(UInt32 a, UInt32 b)
	{
		return ((a >= b) ? b : a);
	}
	public static Int64 Min(Int64 a, Int64 b)
	{
		return ((a >= b) ? b : a);
	}
	public static UInt64 Min(UInt64 a, UInt64 b)
	{
		return ((a >= b) ? b : a);
	}
	public static float Min(params float[] values)
	{
		int length = values.Length;
		if (length == 0)
		{
			return 0f;
		}
		float num2 = values[0];
		for (int i = 1; i < length; i++)
		{
			if (values[i] < num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}
	public static int Min(params int[] values)
	{
		int length = values.Length;
		if (length == 0)
		{
			return 0;
		}
		int num2 = values[0];
		for (int i = 1; i < length; i++)
		{
			if (values[i] < num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static float Max(float a, float b)
	{
		return ((a <= b) ? b : a);
	}
	public static Int8 Max(Int8 a, Int8 b)
	{
		return ((a <= b) ? b : a);
	}
	public static UInt8 Max(UInt8 a, UInt8 b)
	{
		return ((a <= b) ? b : a);
	}
	public static Int16 Max(Int16 a, Int16 b)
	{
		return ((a <= b) ? b : a);
	}
	public static UInt16 Max(UInt16 a, UInt16 b)
	{
		return ((a <= b) ? b : a);
	}
	public static Int32 Max(Int32 a, Int32 b)
	{
		return ((a <= b) ? b : a);
	}
	public static UInt32 Max(UInt32 a, UInt32 b)
	{
		return ((a <= b) ? b : a);
	}
	public static Int64 Max(Int64 a, Int64 b)
	{
		return ((a <= b) ? b : a);
	}
	public static UInt64 Max(UInt64 a, UInt64 b)
	{
		return ((a <= b) ? b : a);
	}
	public static float Max(params float[] values)
	{
		int length = values.Length;
		if (length == 0)
		{
			return 0f;
		}
		float num2 = values[0];
		for (int i = 1; i < length; i++)
		{
			if (values[i] > num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}
	public static int Max(params int[] values)
	{
		int length = values.Length;
		if (length == 0)
		{
			return 0;
		}
		int num2 = values[0];
		for (int i = 1; i < length; i++)
		{
			if (values[i] > num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static float Pow(float f, float p)
	{
		return (float)Math.Pow((double)f, (double)p);
	}

	public static float Exp(float power)
	{
		return (float)Math.Exp((double)power);
	}

	public static float Log(float f, float p)
	{
		return (float)Math.Log((double)f, (double)p);
	}

	public static float Log(float f)
	{
		return (float)Math.Log((double)f);
	}

	public static float Log10(float f)
	{
		return (float)Math.Log10((double)f);
	}

	public static float Ceil(float f)
	{
		return (float)Math.Ceiling((double)f);
	}

	public static float Floor(float f)
	{
		return (float)Math.Floor((double)f);
	}

	public static float Round(float f)
	{
		return (float)Math.Round((double)f);
	}

	public static int CeilToInt(float f)
	{
		return (int)Math.Ceiling((double)f);
	}

	public static int FloorToInt(float f)
	{
		return (int)Math.Floor((double)f);
	}

	public static int RoundToInt(float f)
	{
		return (int)Math.Round((double)f);
	}

	public static float Sign(float f)
	{
		return ((f < 0f) ? -1f : 1f);
	}
	public static float Radius2Degree(float radius)
	{
		return 180.0f - radius * Rad2Deg;
	}
	public static float Clamp(float value, float min, float max)
	{
		if (value < min)
		{
			value = min;
			return value;
		}
		if (value > max)
		{
			value = max;
		}
		return value;
	}

	public static int Clamp(int value, int min, int max)
	{
		if (value < min)
		{
			value = min;
			return value;
		}
		if (value > max)
		{
			value = max;
		}
		return value;
	}

	public static float Clamp01(float value)
	{
		if (value < 0f)
		{
			return 0f;
		}
		if (value > 1f)
		{
			return 1f;
		}
		return value;
	}

	public static float Lerp(float from, float to, float t)
	{
		return (from + ((to - from) * Clamp01(t)));
	}

	public static float Wrap(float val, float low, float high)// 取值范围[low, high)
	{
		float ret = (val);
		float rang = (high - low);

		while (ret >= high)
		{
			ret -= rang;
		}
		while (ret < low)
		{
			ret += rang;
		}
		return ret;
	}

	public static int Wrap(int val, int low, int high)// 取值范围[low, high)
	{
		int ret = (val);
		int rang = (high - low);

		while (ret >= high)
		{
			ret -= rang;
		}
		while (ret < low)
		{
			ret += rang;
		}
		return ret;
	}
	public static float MoveTowards(float current, float target, float maxDelta)
	{
		if (Abs((float)(target - current)) <= maxDelta)
		{
			return target;
		}
		return (current + (Sign(target - current) * maxDelta));
	}

	//public static float MoveTowardsAngle(float current, float target, float maxDelta)
	//{
	//    target = current + DeltaAngle(current, target);
	//    return MoveTowards(current, target, maxDelta);
	//}

	public static float SmoothStep(float from, float to, float t)
	{
		t = Clamp01(t);
		t = (((-2f * t) * t) * t) + ((3f * t) * t);
		return ((to * t) + (from * (1f - t)));
	}

	public static float Gamma(float value, float absmax, float gamma)
	{
		bool flag = false;
		if (value < 0f)
		{
			flag = true;
		}
		float num = Abs(value);
		if (num > absmax)
		{
			return (!flag ? num : -num);
		}
		float num2 = Pow(num / absmax, gamma) * absmax;
		return (!flag ? num2 : -num2);
	}

	public static bool Approximately(float a, float b)
	{
		return (Abs((float)(b - a)) < Max((float)(1E-06f * Max(Abs(a), Abs(b))), (float)1.121039E-44f));
	}

	//public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
	//{
	//    float deltaTime = Time.deltaTime;
	//    return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	//}

	//public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
	//{
	//    float deltaTime = Time.deltaTime;
	//    float positiveInfinity = float.PositiveInfinity;
	//    return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
	//}

	//public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
	//{
	//    smoothTime = Max(0.0001f, smoothTime);
	//    float num = 2f / smoothTime;
	//    float num2 = num * deltaTime;
	//    float num3 = 1f / (((1f + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
	//    float num4 = current - target;
	//    float num5 = target;
	//    float max = maxSpeed * smoothTime;
	//    num4 = Clamp(num4, -max, max);
	//    target = current - num4;
	//    float num7 = (currentVelocity + (num * num4)) * deltaTime;
	//    currentVelocity = (currentVelocity - (num * num7)) * num3;
	//    float num8 = target + ((num4 + num7) * num3);
	//    if (((num5 - current) > 0f) == (num8 > num5))
	//    {
	//        num8 = num5;
	//        currentVelocity = (num8 - num5) / deltaTime;
	//    }
	//    return num8;
	//}

	//public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
	//{
	//    float deltaTime = Time.deltaTime;
	//    return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	//}

	//public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
	//{
	//    float deltaTime = Time.deltaTime;
	//    float positiveInfinity = float.PositiveInfinity;
	//    return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
	//}

	//public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
	//{
	//    target = current + DeltaAngle(current, target);
	//    return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	//}

	public static float Repeat(float t, float length)
	{
		return (t - (Floor(t / length) * length));
	}

	public static float PingPong(float t, float length)
	{
		t = Repeat(t, length * 2f);
		return (length - Abs((float)(t - length)));
	}

	public static float InverseLerp(float from, float to, float value)
	{
		if (from < to)
		{
			if (value < from)
			{
				return 0f;
			}
			if (value > to)
			{
				return 1f;
			}
			value -= from;
			value /= to - from;
			return value;
		}
		if (from <= to)
		{
			return 0f;
		}
		if (value < to)
		{
			return 1f;
		}
		if (value > from)
		{
			return 0f;
		}
		return (1f - ((value - to) / (from - to)));
	}
}

public class ViMath2D
{
	public static readonly float FRONT_X = 0.0f;
	public static readonly float FRONT_Y = -1.0f;

	public static float Length(float x, float y)
	{
		return (float)Math.Sqrt(x * x + y * y);
	}

	public static float Length2(float x, float y)
	{
		return (x * x + y * y);
	}
	public static float Length(float fSrcX, float fSrcY, float fDesX, float fDesY)
	{
		float deltaX = fDesX - fSrcX;
		float deltaY = fDesY - fSrcY;
		return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
	}

	public static float Length2(float fSrcX, float fSrcY, float fDesX, float fDesY)
	{
		float deltaX = fDesX - fSrcX;
		float deltaY = fDesY - fSrcY;
		return (deltaX * deltaX + deltaY * deltaY);
	}

	public static float GetAngle(float fX, float fY)
	{
		float angle = (float)Math.Atan2(fX, -fY);
		return angle;
	}
	public static void CaculateAngle(float fAngle, ref float fX, ref float fY)
	{
		fY = -(float)Math.Cos(fAngle);
		fX = (float)Math.Sin(fAngle);
	}
	public static float GetRotateAngle(float fSrcX, float fSrcy, float fDesX, float fDesY)
	{
		//if(abs(fSrcX) <= 0.001f && abs(fSrcy) <= 0.001f 
		//	|| abs(fDesX) <= 0.001f && abs(fDesY) <= 0.001f)
		//{
		//	return 0.0f;
		//}
		float fDesAngle = GetAngle(fDesX, fDesY);
		float fSrcAngle = GetAngle(fSrcX, fSrcy);
		float fRotateAngle = fDesAngle - fSrcAngle;
		// 映射到(-M_PI, M_PI)区间上来
		if (fRotateAngle > ViMathDefine.PI)
			fRotateAngle -= ViMathDefine.PI_X2;
		else if (fRotateAngle < -ViMathDefine.PI)
			fRotateAngle += ViMathDefine.PI_X2;
		return fRotateAngle;
	}

	public static void Rotate(float fSrcX, float fSrcy, float fRotateAngle, ref float fDesX, ref float fDesY)
	{
		//! 逆时针旋转
		float fSin = (float)Math.Sin(fRotateAngle);
		float fCon = (float)Math.Cos(fRotateAngle);
		//! 顺时针旋转
		//float fSin = sin(-fRotateAngle);
		//float fCon = cos(-fRotateAngle);
		fDesX = fCon * fSrcX - fSin * fSrcy;
		fDesY = fSin * fSrcX + fCon * fSrcy;
	}

	public static void Rotate(ref float fX, ref float fY, float fRotateAngle)
	{
		float fSin = (float)Math.Sin(fRotateAngle);
		float fCon = (float)Math.Cos(fRotateAngle);
		//! 顺时针旋转
		//float fSin = sin(-fRotateAngle);
		//float fCon = cos(-fRotateAngle);
		float fDesX = fCon * fX - fSin * fY;
		float fDesY = fSin * fX + fCon * fY;
		fX = fDesX;
		fY = fDesY;
	}

	public static void RotateRight90(ref float x, ref float y)
	{
		float temp = x;
		x = y;
		y = -temp;
	}
	public static void RotateLeft90(ref float x, ref float y)
	{
		float temp = x;
		x = -y;
		y = temp;
	}
	public static int GetSide(float fromX, float fromY, float toX, float toY, float x, float y)
	{
		float s = (fromX - x) * (toY - y) - (fromY - y) * (toX - x);
		if (s == 0)
		{
			return 0;
		}
		else if (s < 0)//! 右侧
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}
}


public class ViMath3D
{
	public static void Convert(ViVector3 diretion, float roll, out ViVector3 horizDir, out ViVector3 normal)
	{
		horizDir = diretion;
		horizDir.z = 0.0f;
		horizDir.Normalize();
		diretion.Normalize();
		ViVector3 rotateAxis = ViVector3.Cross(diretion, ViVector3.UNIT_Z);
		rotateAxis.Normalize();
		ViQuaternion verticalRotateQuat = ViQuaternion.FromAxisAngle(rotateAxis, ViMathDefine.PI_HALF);
		normal = verticalRotateQuat * diretion;
		ViQuaternion rollRotateQuat = ViQuaternion.FromAxisAngle(diretion, roll);
		normal = rollRotateQuat * normal;
	}

	//public static void Convert(ViVector3 horizDir, ViVector3 normal, out ViVector3 diretion, out float roll)
	//{
	//    normal.Normalize();
	//    ViVector3 rotateAxis = ViVector3.Cross(horizDir, ViVector3.UNIT_Z);
	//    if (rotateAxis == normal)
	//    {
	//        diretion = horizDir;
	//        roll = 0.0f;
	//    }
	//    diretion = ViVector3.Cross(rotateAxis, normal);
	//    if (ViVector3.Angle(horizDir, diretion) < ViMathDefine.PI_HALF)
	//    {

	//    }
	//    else
	//    {
	//        diretion = -diretion;
	//        rotateAxis = -rotateAxis;
	//    }
	//    ViQuaternion verticalRotateQuat = ViQuaternion.FromAxisAngle(rotateAxis, ViMathDefine.PI_HALF);
	//    ViVector3 normal1 = verticalRotateQuat * diretion;
	//    ViVector3 normalRotAxisDir = ViVector3.Cross(normal1, normal);
	//    if (ViVector3.Angle(horizDir, diretion) < ViMathDefine.PI_HALF)
	//    {
	//        roll = ViVector3.Angle(normal1, normal);
	//    }
	//    else
	//    {
	//        roll = -ViVector3.Angle(normal1, normal);
	//    }
		
	//}
}
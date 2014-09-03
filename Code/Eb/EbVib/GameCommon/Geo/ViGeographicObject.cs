using System;



public interface ViGeographicInterface
{
	ViVector3 Position { get; set; }
	float BodyRadius { get;}
	float Yaw { get; set; }

	float Aim(ViVector3 pos);
	float Aim(ViGeographicObject obj);
	void TurnAngle(float deltaAngle);
	float GetAngleFromLocation(ViVector3 pos);
	float GetAngleFromDirection(float deltaX, float deltaY);
	bool IsInFront(ViVector3 pos);
	float GetDistance(ViVector3 kPos);
	float GetDistance(ViGeographicObject obj);
	ViVector3 GetIntersectionPosByDist(ViVector3 toPos, float distance);
	ViVector3 GetIntersectionPosByPerc(ViVector3 toPos, float perc);
	ViVector3 GetRoundPos(float angle, float distance);
}



public abstract class ViGeographicObject : ViRefObj, ViGeographicInterface
{
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static readonly ViVector3 FRONT = new ViVector3(ViMath2D.FRONT_X, ViMath2D.FRONT_Y, 0);
	public static readonly ViVector3 VERTICAL = new ViVector3(0, 0, 1);

	public static float GetDirection(float x, float y)
	{
		return ViMath2D.GetRotateAngle(ViMath2D.FRONT_X, ViMath2D.FRONT_Y, x, y);
	}
	public static void GetRotate(float angle, ref float x, ref float y)
	{
		ViMath2D.Rotate(ViMath2D.FRONT_X, ViMath2D.FRONT_Y, angle, ref x, ref y);
	}
	public static float GetDistance(ViVector3 pos1, ViVector3 pos2)
	{
		return GetHorizontalDistance(pos1, pos2);
	}
	public static float GetDistance2(ViVector3 pos1, ViVector3 pos2)
	{
		return GetHorizontalDistance2(pos1, pos2);
	}

	public static float GetHorizontalDistance(ViVector3 pos1, ViVector3 pos2)
	{
		float deltaX = pos1.x - pos2.x;
		float deltaY = pos1.y - pos2.y;
		return ViMathDefine.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
	}
	public static float GetHorizontalDistance2(ViVector3 pos1, ViVector3 pos2)
	{
		float deltaX = pos1.x - pos2.x;
		float deltaY = pos1.y - pos2.y;
		return ((deltaX * deltaX) + (deltaY * deltaY));
	}
	//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

	public abstract ViVector3 Position { get; set; }
	public abstract float BodyRadius { get; }
	public abstract float Yaw { get; set; }

	public float Aim(ViVector3 pos)
	{
		float angle = GetAngleFromLocation(pos);
		TurnAngle(angle);
		return angle;
	}
	public float Aim(ViGeographicObject obj)
	{
		if (obj == null)
		{
			return 0.0f;
		}
		else if (obj != this)
		{
			return Aim(obj.Position);
		}
		else
		{
			return 0.0f;
		}
	}
	public void TurnAngle(float deltaAngle)
	{
		Yaw = (Yaw + deltaAngle);
	}

	public float GetAngleFromLocation(ViVector3 pos)
	{
		float deltaX = pos.x - Position.x;
		float deltaY = pos.y - Position.y;
		return GetAngleFromDirection(deltaX, deltaY);
	}
	public float GetAngleFromDirection(float deltaX, float deltaY)
	{
		if (Math.Abs(deltaX) < 0.1f && Math.Abs(deltaY) < 0.1f)
		{
			return 0.0f;
		}
		float deltaAngle = GetDirection(deltaX, deltaY) - Yaw;
		ViAngle.Normalize(ref deltaAngle);
		return deltaAngle;
	}
	public bool IsInFront(ViVector3 pos)
	{
		float angle = GetAngleFromLocation(pos);
		if (-ViMathDefine.PI_HALF < angle && angle < ViMathDefine.PI_HALF)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public float GetDistance(ViVector3 pos)
	{
		float diff = GetHorizontalDistance(pos, Position);
		return ViMathDefine.Max(0.0f, diff - BodyRadius);
	}
	public float GetDistance(ViGeographicObject obj)
	{
		if (obj == null)
		{
			return 1.0f;
		}
		float diff = GetHorizontalDistance(Position, obj.Position);
		return ViMathDefine.Max(0.0f, diff - BodyRadius - obj.BodyRadius);
	}
	public ViVector3 GetIntersectionPosByDist(ViVector3 toPos, float distance)	//! fDistance = 0, reach this, fDistance < 0 , penetrate this
	{
		return GetIntersectionPosByDist(Position, toPos, distance);
	}
	public static ViVector3 GetIntersectionPosByDist(ViVector3 rootPos, ViVector3 toPos, float distance)//! fDistance = 0, reach kRootPos, fDistance< 0 , penetrate this
	{
		float fSpan = GetHorizontalDistance(rootPos, toPos);
		if (fSpan != 0.0f)
		{
			float fPerc = distance / fSpan;
			float fDeltaX = toPos.x - rootPos.x;
			float fDeltaY = toPos.y - rootPos.y;
			ViVector3 kDest = new ViVector3(rootPos.x + fDeltaX * fPerc, rootPos.y + fDeltaY * fPerc, rootPos.z);
			float diff = GetHorizontalDistance(rootPos, kDest);
			ViDebuger.AssertError(Math.Abs(diff - Math.Abs(distance)) < 0.1f);
			return new ViVector3(rootPos.x + fDeltaX * fPerc, rootPos.y + fDeltaY * fPerc, rootPos.z);
		}
		else
		{
			return rootPos;
		}
	}
	public ViVector3 GetIntersectionPosByPerc(ViVector3 toPos, float perc)	//! fPerc = 0  Reach this, fPerc = 1 Reach From, fPerc < 0 behind self
	{
		float fDeltaX = toPos.x - Position.x;
		float fDeltaY = toPos.y - Position.y;
		return new ViVector3(Position.x + fDeltaX * perc, Position.y + fDeltaY * perc, Position.z);
	}
	public ViVector3 GetRoundPos(float angle, float distance)
	{
		float x = 0.0f;
		float y = 0.0f;
		GetRotate(Yaw + angle, ref x, ref y);
		x *= distance;
		y *= distance;
		return new ViVector3(x, y, 0) + Position;
	}
}
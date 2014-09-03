using System.Diagnostics;
using System;

public class ViDirectionCursor
{
	public ViVector3 Direction { get { return _curDiretion; } set { _curDiretion = value; } }
	public float RotSpd { get { return _rotSpd; } set { _rotSpd = ViMathDefine.Max(1.0f, Math.Abs(value)); } }

	public bool Update(float deltaTime, ViVector3 newDir)
	{
		if (_curDiretion == newDir)
		{
			return false;
		}
		float angleDiff = ViVector3.Angle(newDir, _curDiretion);
		float rotAngle = _rotSpd * deltaTime;
		if (angleDiff <= rotAngle)
		{
			_curDiretion = newDir;
		}
		else
		{
			ViVector3 rotateAxis = ViVector3.Cross(_curDiretion, newDir);
			rotateAxis.Normalize();
			ViQuaternion rotateQuat = ViQuaternion.FromAxisAngle(rotateAxis, rotAngle);
			_curDiretion = rotateQuat * _curDiretion;
		}
		return true;
	}

	ViVector3 _curDiretion;
	float _rotSpd = 5.0f;
}


public static class Demo_Direction
{
	public static void Test()
	{
		ViDirectionCursor cursor = new ViDirectionCursor();
		cursor.Direction = new ViVector3(0, 0, 1);
		ViVector3 newDir = new ViVector3(-1, 0, 1);
		newDir.Normalize();
		while (cursor.Update(0.03f, newDir) == true)
		{
			//Debug.Print("Direction >> " + cursor.Direction);
		}
	}
}
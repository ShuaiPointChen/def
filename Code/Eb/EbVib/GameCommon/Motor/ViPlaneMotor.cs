
using System;

using System.Diagnostics;

public class ViPlaneMotor : ViMotorInterface
{
	public override float Roll { get { return _lateralAngle * _lateralSign; } }

	public void SetDir(ViVector3 dir, float roll)
	{
		_direction = dir;
		_lateralAngle = roll;
		_rollSpd = 0.3f;
	}

	public override void _Update(float deltaTime, ViVector3 target)
	{
		ViVector3 diffDir = target - Translate;
		diffDir.Normalize();
		if (_direction == diffDir)
		{
			_velocity = _direction * _speed;
		}
		else
		{
			ViVector3 rotateAxis = ViVector3.Cross(_direction, diffDir);
			rotateAxis.Normalize();
			const float STABLE = 0.0001f;
			// 计算公式与变量定义
			// V 线速度 
			// W 角速度 
			// A 侧向加速度
			// R 运动半径
			// W = V/R;
			// A = (V*V)/R = W*W*R = V*W;
			float angleDiff = ViVector3.Angle(diffDir, _direction);
			float destW = 4.0f * Math.Abs((angleDiff + STABLE) / (_duration + STABLE));
			float destA = destW * Speed;
			float destLateralAngle = (float)Math.Atan2(destA, _gravity);
			//
			_rollSpd = 3.0f * (destLateralAngle - _lateralAngle + STABLE) / (_duration + STABLE);
			if (destLateralAngle > _lateralAngle)
			{
				_lateralAngle = ViMathDefine.MoveTowards(_lateralAngle, destLateralAngle, _rollSpd * deltaTime);
			}
			else
			{
				_lateralAngle = destLateralAngle;
			}
			float currentA = (float)Math.Tan(_lateralAngle) * _gravity;
			float currentW = currentA / Speed;
			float deltaAngle = currentW * deltaTime;
			//
			ViQuaternion rotateQuat = ViQuaternion.FromAxisAngle(rotateAxis, deltaAngle);
			ViVector3 newDir = rotateQuat * _direction;
			newDir.Normalize();
			_velocity = (newDir + _direction) * _speed * 0.5f;
			if (ViVector3.Dot(ViVector3.Cross(_direction, newDir), ViVector3.Cross(newDir, diffDir)) < 0.0f)// 插值抖动
			{
				_lateralSign = 0.0f;
				_direction = diffDir;
			}
			else
			{
				_direction = newDir;
				_lateralSign = (rotateAxis.z > 0.0f) ? 1.0f : -1.0f;
			}
		}
	}

	float _gravity = 9.8f;
	float _lateralSign = 1.0f;
	float _lateralAngle;
	float _rollSpd = 0.3f;
}

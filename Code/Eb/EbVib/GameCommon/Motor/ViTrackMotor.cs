using System;


public class ViTrackMotor : ViMotorInterface
{
	public override void _Update(float deltaTime, ViVector3 target)
	{
		ViVector3 dir = target - Translate;
		dir.Normalize();
		_velocity = dir * _speed;
		_direction = dir;
	}
}

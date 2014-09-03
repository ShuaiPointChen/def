using System;

public class ViGravityMotor : ViMotorInterface
{
	public static readonly float GRAVITY = 9.8f;
	public float Gravity { get { return m_GravityAcc; } set { m_GravityAcc = ViMathDefine.Clamp(value, 0.1f * GRAVITY, 5.0f * GRAVITY); } }

	public override void Start(float duration)
	{
		ViDebuger.AssertWarning(Target);
		if (Target == null)
		{
			Target = new ViSimpleProvider<ViVector3>();
		}
		_velocity = ViVector3.ZERO;
		_duration = ViMathDefine.Max(0.01f, duration);
		ViVector3 targetPos = Target.Value;
		float distanceH = ViMath2D.Length(targetPos.x, targetPos.y, Translate.x, Translate.y);
		float distanceV = targetPos.z - Translate.z;
		float time = distanceV / m_GravityAcc / _duration;
		float preDeltaTime = _duration * 0.5f + time;
		float aftDeltaTime = _duration * 0.5f - time;
		_velocity.z = preDeltaTime * m_GravityAcc;		
	}

	public override void _Update(float deltaTime, ViVector3 target)
	{
		ViDebuger.AssertWarning(deltaTime > 0.0f);
		ViDebuger.AssertWarning(_speed > 0.0f);
		//
		//
		ViVector3 targetPos = Target.Value;
		float distanceH = ViMath2D.Length(targetPos.x, targetPos.y, Translate.x, Translate.y);
		float distanceV = targetPos.z - Translate.z;
		float time = distanceH / _speed;
		m_GravityAcc = -2.0f * (distanceV / (time * time) - _velocity.z / time);
		m_GravityAcc = ViMathDefine.Clamp(m_GravityAcc, -GRAVITY, 5.0f * GRAVITY);
		ViVector3 kDir = targetPos - Translate;
		kDir.z = 0.0f;
		kDir.Normalize();
		_velocity.x = kDir.x * _speed;
		_velocity.y = kDir.y * _speed;
		_velocity.z -= m_GravityAcc * deltaTime;
		_direction = _velocity;
		_direction.Normalize();
	}

	float m_GravityAcc = GRAVITY;
}
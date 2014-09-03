using System;

public class ViCameraDistance
{
	public float Accelerate { get { return _accelerate; } set { _accelerate = Math.Max(0.0f, value); } }
	public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = Math.Max(0.0f, value); } }
	public float Distance { get { return _currentDistance; } }
	public bool IsFiltering { get { return (_currentSpeed != 0.0f); } }
	public void Update(float desireDistance, float delatTime)
	{
		float diff = desireDistance - _currentDistance;
		if (diff == 0.0f)
		{

		}
		else if (diff > 0.0f)
		{
			float newSpd = _currentSpeed + _accelerate * delatTime;
			float maxSpd = ViMathDefine.Sqrt(diff * _accelerate * 2.0f);
			_currentSpeed = ViMathDefine.Min(ViMathDefine.Min(newSpd, maxSpd), _maxSpeed);
			float delatDist = _currentSpeed * delatTime;
			_currentDistance += delatDist;
			if (_currentDistance >= desireDistance)
			{
				_currentDistance = desireDistance;
				_currentSpeed = 0.0f;
			}
		}
		else
		{
			float newSpd = _currentSpeed - _accelerate * delatTime;
			float maxSpd = -ViMathDefine.Sqrt(-diff * _accelerate * 2.0f);
			_currentSpeed = ViMathDefine.Max(ViMathDefine.Max(newSpd, maxSpd), -_maxSpeed);
			float delatDist = _currentSpeed * delatTime;
			_currentDistance += delatDist;
			if (_currentDistance <= desireDistance)
			{
				_currentDistance = desireDistance;
				_currentSpeed = 0.0f;
			}
		}
	}
	public void StopAt(float fDistance)
	{
		_currentDistance = fDistance;
		_currentSpeed = 0.0f;
	}

	float _accelerate = 2.0f;
	float _currentSpeed = 0.0f;
	float _maxSpeed = 4.0f;
	float _currentDistance = 0.0f;
}

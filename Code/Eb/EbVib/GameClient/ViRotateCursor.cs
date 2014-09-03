using System;

public class ViRotateCursor
{

	public float Yaw { get { return _cursorYaw; } }
	public float FeetDirection { get { return _feetDirection; } }
	public float BodyDirection { get { return _bodyDirection; } }
	public bool Active { get { return _active; } set { _active = value; } }
	public float AllowAngle { get { return _allowAngle; } set { _allowAngle = value; } }
	public float AccordSpd { get { return _accordSpd; } set { _accordSpd = value; } }
	public ViProvider<ViAngle> DestDirectionProvider { set { _destDirectionProvider = value; } }
	public Int32 Sign { get { return _accordSign; } }

	public delegate void Callback();
	public Callback CallbackStart;
	public Callback CallbackUpdate;
	public Callback CallbackEnd;

	public void ForceAccord()
	{
		if (_cursorYaw == 0)
		{
			return;
		}
		if (_accordSign == 0)
		{
			if (_cursorYaw > 0.0f)
			{
				_accordSign = 1;
			}
			else
			{
				_accordSign = -1;
			}
			OnRoteStart();
		}
	}
	public bool Tick(float fDeltaTime)
	{
		if (_destDirectionProvider == null)
		{
			return false;
		}
		bool result = false;
		float yaw = _destDirectionProvider.Value.Value;
		float feetDirection = _feetDirection;
		float oldCursorYaw = _cursorYaw;
		_cursorYaw = yaw - feetDirection;
		ViAngle.Normalize(ref _cursorYaw);
		Int32 fomated = _FomatByMaxAngle(ref feetDirection, yaw);
		if (oldCursorYaw != _cursorYaw)
		{
			OnRoteUpdated();
		}
		if (fomated != 0)
		{
			if (_accordSign == 0)
			{
				_accordSign = fomated;
				OnRoteStart();
			}
			result = true;
		}

		if (_accordSign != 0)
		{
			float frameDeltaYaw = _accordSpd * fDeltaTime;
			if (_cursorYaw > frameDeltaYaw)
			{
				feetDirection += frameDeltaYaw;
				ViAngle.Normalize(ref feetDirection);
				result = true;
			}
			else if (_cursorYaw < -frameDeltaYaw)
			{
				feetDirection -= frameDeltaYaw;
				ViAngle.Normalize(ref feetDirection);
				result = true;
			}
			else
			{
				feetDirection = yaw;
				OnRoteEnd();
				_accordSign = 0;
			}
		}

		_feetDirection = feetDirection;
		_bodyDirection = (feetDirection + yaw) * 0.5f;

		return result;
	}
	public void FastAccord()
	{
		if (_destDirectionProvider != null)
		{
			float direction = _destDirectionProvider.Value.Value;
			_feetDirection = direction;
			_bodyDirection = direction;
		}
	}
	void OnRoteStart() { if (CallbackStart != null) { CallbackStart(); } }
	void OnRoteUpdated() { if (CallbackUpdate != null) { CallbackUpdate(); } }
	void OnRoteEnd() { if (CallbackEnd != null) { CallbackEnd(); } }

	Int32 _FomatByMaxAngle(ref float feetDirection, float bodyYaw)
	{
		if (_cursorYaw > _allowAngle)
		{
			feetDirection = bodyYaw - _allowAngle;
			_cursorYaw = _allowAngle;
			ViAngle.Normalize(ref feetDirection);
			return 1;
		}
		else if (_cursorYaw < -_allowAngle)
		{
			feetDirection = bodyYaw + _allowAngle;
			_cursorYaw = -_allowAngle;
			ViAngle.Normalize(ref feetDirection);
			return -1;
		}
		return 0;
	}

	public void Clear()
	{
		CallbackStart = null;
		CallbackUpdate = null;
		CallbackEnd = null;
	}

	float _accordSpd = 3.0f;
	Int32 _accordSign;
	float _allowAngle = 0.0f;
	float _cursorYaw;
	bool _active;

	float _bodyDirection;
	float _feetDirection;
	ViProvider<ViAngle> _destDirectionProvider;
}
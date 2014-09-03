using System;

public class ViMotorInterface
{
	public float Duration { get { return _duration; } }
	public float Speed { get { return _speed; } }
	public float Distance { get { return _distance; } }

	public ViVector3 Direction { get { return _direction; } }
	public virtual float Roll { get { return 0.0f; } }
	public bool IsEnd { get { return (_duration < 0.01f); } }
	public ViVector3 Velocity { get { return _velocity; } }
	public bool EndForceUpdate { get { return _endForceUpdate; } set { _endForceUpdate = value; } }
	public float IterSpan { set { _iterSpan = value; } }
	public ViVector3 Translate
	{
		get { return _translate; }
		set { _translate = value; }
	}
	public ViProvider<ViVector3> Target
	{
		get { ViDebuger.AssertError(_target); return _target; }
		set { ViDebuger.AssertError(value); _target = value; }
	}

	public virtual void Start(float duration)
	{
		_duration = duration;
		_velocity = ViVector3.ZERO;
		ViDebuger.AssertError(_target);
	}
	public bool Update(float deltaTime)
	{
		ViVector3 moved;
		return Update(deltaTime, out moved);
	}
	public bool Update(float deltaTime, out ViVector3 moved)
	{
		moved = ViVector3.ZERO;
		while (deltaTime > _iterSpan)
		{
			deltaTime -= _iterSpan;
			ViVector3 iterMoved;
			bool result = _Update(_iterSpan, out iterMoved);
			moved += iterMoved;
			if (result == false)
			{
				return false;
			}
		}
		if (deltaTime > 0)
		{
			ViVector3 iterMoved;
			bool result = _Update(deltaTime, out iterMoved);
			moved += iterMoved;
			if (result == false)
			{
				return false;
			}
		}
		return true;
	}
	bool _Update(float deltaTime, out ViVector3 moved)
	{
		ViDebuger.AssertError(_target);
		if (IsEnd)
		{
			moved = ViVector3.ZERO;
			return false;
		}
		ViVector3 target = _target.Value;
		ViVector3 diff = target - _translate;
		_distance = diff.Length;
		const float STABLE = 0.01f;
		_speed = (_distance + STABLE) / (_duration + STABLE);
		if (_speed > 1)
		{
			_Update(deltaTime, target);
			moved = _velocity * deltaTime;
			_translate += moved;
		}
		else
		{
			moved = ViVector3.ZERO;
		}
		_duration -= deltaTime;
		if (_duration <= 0.01f)
		{
			if (_endForceUpdate)
			{
				moved = target - _translate;
				_translate = target;
			}
			return false;
		}
		else
		{
			return true;
		}
	}

	public virtual void _Update(float deltaTime, ViVector3 target) { }

	protected ViProvider<ViVector3> _target;
	protected float _duration;
	protected float _speed;
	protected float _distance;
	protected bool _endForceUpdate = true;

	protected ViVector3 _direction;
	protected ViVector3 _translate;
	protected ViVector3 _velocity;

	float _iterSpan = 0.1f;
}
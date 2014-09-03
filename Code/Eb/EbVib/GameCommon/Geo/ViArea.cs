using System;


public class ViArea
{
	public virtual bool InArea(ViVector3 pos) { return false; }
	public virtual bool InArea(ViVector3 pos, float range) { return false; }

	public float Range { get { return _range; } }
	public ViVector3 Center { get { return _center; } set { _center = value; } }

	protected ViVector3 _center;
	protected float _range;
}

public class ViRoundArea : ViArea
{
	public void Init(float radius)
	{
		_radius2 = radius * radius;
		_range = radius;
	}
	public override bool InArea(ViVector3 pos)
	{
		return (ViMath2D.Length2(_center.x, _center.y, pos.x, pos.y) < _radius2);
	}
	public override bool InArea(ViVector3 pos, float range)
	{
		return (ViMath2D.Length2(_center.x, _center.y, pos.x, pos.y) < (_radius2 + range * range));
	}
	float _radius2;
}

public class ViSectorArea : ViArea
{
	public void Init(float radius, float leftAngle, float rightAngle)
	{
		_leftAngle = leftAngle;
		ViAngle.Normalize(ref _leftAngle);
		_rightAngle = rightAngle;
		ViAngle.Normalize(ref _rightAngle);
		if (_leftAngle > _rightAngle)
		{
			_rightAngle += ViMathDefine.PI_X2;
		}
		_range = radius;
		_radius2 = radius * radius;
		ViDebuger.AssertWarning(_leftAngle <= _rightAngle);
	}
	public void SetDir(float dir)
	{
		_dirLeftAngle = _leftAngle + dir;
		_dirRightAngle = _rightAngle + dir;
		if (_dirLeftAngle > _dirRightAngle)
		{
			_dirRightAngle += ViMathDefine.PI_X2;
		}
		ViDebuger.AssertWarning(_leftAngle <= _rightAngle);
	}

	public override bool InArea(ViVector3 pos)
	{
		ViDebuger.AssertWarning(_dirLeftAngle <= _dirRightAngle);
		float deltaX = pos.x - _center.x;
		float deltaY = pos.y - _center.y;
		float dir = ViMath2D.GetAngle(deltaX, deltaY);
		return (ViMath2D.Length2(_center.x, _center.y, pos.x, pos.y) < _radius2) && (_dirLeftAngle <= dir && dir < _dirRightAngle);

	}
	public override bool InArea(ViVector3 pos, float range)
	{
		ViDebuger.AssertWarning(_dirLeftAngle <= _dirRightAngle);
		float deltaX = pos.x - _center.x;
		float deltaY = pos.y - _center.y;
		float dir = ViMath2D.GetAngle(deltaX, deltaY);
		return (ViMath2D.Length2(_center.x, _center.y, pos.x, pos.y) < (_radius2 + range * range)) && (_dirLeftAngle <= dir && dir < _dirRightAngle);
	}


	float _leftAngle;
	float _rightAngle;

	float _dirLeftAngle;
	float _dirRightAngle;

	float _radius2;
}


public struct ViRotRect
{
	public float Len;
	public float HalfWidth;
	public ViVector3 Dir { get { return _dir; } set { _dir = value; _dir.Normalize(); } }
	//
	public bool In(ViVector3 center, ViVector3 pos)
	{
		ViVector3 delta = pos - center;
		float len = ViVector3.Dot(delta, _dir);
		if (len < 0.0f || len > Len)
		{
			return false;
		}
		ViVector3 prjLen = _dir * len;
		ViVector3 prjWidth = delta - prjLen;
		float halfWidth2 = prjWidth.sqrMagnitude;
		if (halfWidth2 > HalfWidth * HalfWidth)
		{
			return false;
		}
		return true;
	}
	public bool In(ViVector3 center, ViVector3 pos, float range)
	{
		ViVector3 delta = pos - center;
		float len = ViVector3.Dot(delta, _dir);
		if (len < 0.0f || len > (Len + range))
		{
			return false;
		}
		ViVector3 prjLen = _dir * len;
		ViVector3 prjWidth = delta - prjLen;
		float halfWidth2 = prjWidth.sqrMagnitude;
		if (halfWidth2 > (HalfWidth + range) * (HalfWidth + range))
		{
			return false;
		}
		return true;
	}
	//
	ViVector3 _dir;
}


public class ViRotRectArea : ViArea
{
	public void SetDir(ViVector3 dir)
	{
		_rotRect.Dir = dir;
	}
	public void SetDir(float dir)
	{
		ViVector3 kDir = ViVector3.ZERO;
		ViGeographicObject.GetRotate(dir, ref kDir.x, ref kDir.y);
		_rotRect.Dir = kDir;
	}
	public void Init(float front, float halfWidth)
	{
		_rotRect.Len = front;
		_rotRect.HalfWidth = halfWidth;
		_range = ViMath2D.Length(front, halfWidth);
	}
	public override bool InArea(ViVector3 pos)
	{
		return _rotRect.In(_center, pos);
	}
	public override bool InArea(ViVector3 pos, float range)
	{
		return _rotRect.In(_center, pos, range);
	}
	//
	ViRotRect _rotRect;
}
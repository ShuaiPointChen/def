using System;
using System.Collections;
using System.Collections.Generic;


public abstract class ViRoundObjects<TEntity>
	where TEntity : ViRefObj, ViGeographicInterface
{
	public delegate bool DELE_IsInRange(TEntity kObj, ViVector3 center);
	public delegate bool DELE_IsStateMatch(TEntity kObj);
	public DELE_IsInRange _deleIsInRange;
	public DELE_IsStateMatch _deleIsStateMatch;

	UInt32 Max { get; set; }
	//
	public void Clear()
	{
		_objs.Clear();
	}
	//
	public TEntity Next(float dir, ViVector3 center, Queue<TEntity> objs)
	{
		if (Max == 0)
		{
			return null;
		}
		_OnDirCenterUpdated(dir, center);
		_Check(center);
		TEntity obj = GetNearst(objs, center);
		if (obj == null && !_objs.IsEmpty())
		{
			obj = _objs.GetFront(null).Obj;
			_objs.PopFront();
		}
		if (obj != null)
		{
			if (_objs.Count == Max)
			{
				_objs.PopFront();
			}
			_objs.PushBack(new ViRefPtr<TEntity>(obj));
		}
		return obj;
	}
	TEntity Prev()
	{
		if (_objs.Count == 0)
		{
			return null;
		}
		TEntity pkObj = _objs.GetBack(null).Obj;
		_objs.PopBack();
		return pkObj;
	}

	bool _Has(TEntity kObj)
	{
		foreach (ViRefPtr<TEntity> ptr in _objs)
		{
			if (ptr.Obj == kObj)
			{
				return true;
			}
		}
		return false;
	}
	void _Check(ViVector3 center)
	{
		ViDebuger.AssertError(_deleIsInRange);
		ViDoubleLinkNode3<ViRefPtr<TEntity>> iter = _objs.GetHead();
		while (!_objs.IsEnd(iter))
		{
			TEntity obj = iter.Data.Obj;
			ViDoubleLink3<ViRefPtr<TEntity>>.Next(ref iter);
			if (!_deleIsInRange(obj, center))
			{
				_objs.Remove(iter);
			}
		}
	}
	void _EraseFarst(ViVector3 center)
	{
		if (_objs.Count == 0)
		{
			return;
		}
		float fMaxDist = 0.0f;
		ViDoubleLinkNode3<ViRefPtr<TEntity>> iter = _objs.GetHead();
		ViDoubleLinkNode3<ViRefPtr<TEntity>> iterFar = iter;
		while (!_objs.IsEnd(iter))
		{
			TEntity obj = iter.Data.Obj;
			ViDoubleLink3<ViRefPtr<TEntity>>.Next(ref iter);
			float fDist = obj.GetDistance(center);
			if (fMaxDist <= fDist)
			{
				fMaxDist = fDist;
				iterFar = iter;
			}
		}
		_objs.Remove(iterFar);
	}
	TEntity GetNearst(Queue<TEntity> objs, ViVector3 center)
	{
		TEntity pkNearst = null;
		float fMinDist = 100.0f;
		ViDebuger.AssertError(_deleIsInRange);
		ViDebuger.AssertError(_deleIsStateMatch);
		foreach (TEntity obj in objs)
		{
			ViDebuger.AssertError(obj);
			if (_Has(obj))
				continue;
			if (!_deleIsInRange(obj, center))
				continue;
			if (!_deleIsStateMatch(obj))
				continue;

			float fDist = obj.GetDistance(center);
			if (fMinDist > fDist)
			{
				fMinDist = fDist;
				pkNearst = obj;
			}
		}
		return pkNearst;
	}

	public abstract void _OnDirCenterUpdated(float fDir, ViVector3 center);

	ViDoubleLink3<ViRefPtr<TEntity>> _objs = new ViDoubleLink3<ViRefPtr<TEntity>>();

}
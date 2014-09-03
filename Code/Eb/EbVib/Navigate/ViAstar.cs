using System;
using System.Collections.Generic;

public enum ViAstarStepState
{
	NONE,
	OPEN,
	CLOSE,
}

public struct ViAstarRoundStep
{
	public ViAstarStep node;
	public float cost;
}

public struct ViAStarPos
{
	public int x;
	public int y;
}

public class ViAstarStep : ViHeapNode
{
    //-------------------------------------------------------------------------
    List<ViAstarRoundStep> _roundNode = new List<ViAstarRoundStep>();
    ViAstarStep _parent;
	public ViAstarStep Parent { get { return _parent; } set { _parent = value; } }
	public List<ViAstarRoundStep> RoundSteps { get { return _roundNode; } }
	public bool IsOpen { get { return (State == ViAstarStepState.OPEN); } }
	public bool IsClose { get { return (State == ViAstarStepState.CLOSE); } }
	public ViAstarStepState State = ViAstarStepState.NONE;
	public float G;
	public float H;
	public ViDoubleLinkNode2<ViAstarStep> AttachNode = new ViDoubleLinkNode2<ViAstarStep>();
	public ViAStarPos Pos;
	public float Cost = 0.0f;

    //-------------------------------------------------------------------------
	public static float Distance(ViAstarStep from, ViAstarStep to)
	{
		float deltaX = from.Pos.x - to.Pos.x;
		float deltaY = from.Pos.y - to.Pos.y;
		return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
	}

    //-------------------------------------------------------------------------
	public ViAstarStep()
	{
		AttachNode.Data = this;
	}

    //-------------------------------------------------------------------------
	public void Clear()
	{
		G = 0;
		_parent = null;
		State = ViAstarStepState.NONE;
	}
}

public class ViAstarDestChecker
{
    //-------------------------------------------------------------------------
	public virtual bool IsDest(ViAstarStep kStep)
	{
		return true;
	}
}

public class ViAstar<TDestChecker> where TDestChecker : ViAstarDestChecker
{
    //-------------------------------------------------------------------------
    UInt32 _stepCnt;
    UInt32 _maxStepCnt = 10000;
    ViAstarStep _startStep;
    ViAstarStep _destStep;
    ViAstarStep _currentStep;
    ViAstarStep _bestStep;
    ViMinHeap<ViAstarStep> _openHeap;
    ViDoubleLink2<ViAstarStep> _openList = new ViDoubleLink2<ViAstarStep>();
    ViDoubleLink2<ViAstarStep> _closeList = new ViDoubleLink2<ViAstarStep>();
	public UInt32 StepCnt { get { return _stepCnt; } }
	public UInt32 MaxStepCnt { get { return _maxStepCnt; } set { _maxStepCnt = value; } }
	public TDestChecker DestChecker;

    //-------------------------------------------------------------------------
	public ViAstar(UInt32 heapSize)
	{
		_openHeap = new ViMinHeap<ViAstarStep>(heapSize);
	}

    //-------------------------------------------------------------------------
	public bool Search(ViAstarStep src, ViAstarStep dest)
	{
		ViDebuger.AssertWarning(src.Cost == 0.0f);
		src.G = 0;
		src.H = ViAstarStep.Distance(src, dest);
		src.Key = src.G + src.H;
		//
		_startStep = src;
		_bestStep = src;
		_startStep.Clear();
		_destStep = dest;
		//
		_stepCnt = 0;
		//
		_AddToOpen(src);
		//
		while (_openHeap.Size > 0)
		{
			_currentStep = _openHeap.Pop();
			ViDebuger.AssertError(_currentStep);
			if (DestChecker.IsDest(_currentStep))
			{
				_bestStep = _currentStep;
				return true;
			}
			if (_stepCnt > _maxStepCnt)
			{
				return false;
			}
			if (_currentStep.H < _bestStep.H)
			{
				_bestStep = _currentStep;
			}
			++_stepCnt;
			ViDebuger.AssertError(_currentStep.IsOpen);
			_NewStep(_currentStep);
			_currentStep.AttachNode.Detach();
			_AddToClose(_currentStep);
		}
		return false;
	}

    //-------------------------------------------------------------------------
	public void MakeRoute(List<ViAstarStep> steps)
	{
		int dir = 1 + (1 << 2);
		ViAstarStep next = _bestStep;
		if (next == null)
		{
			return;
		}
		while (true)
		{
			ViAstarStep fromStep = next;
			next = next.Parent;
			if (next != null)
			{
				ViAstarStep toStep = next;
				int deltaX = toStep.Pos.x - fromStep.Pos.x + 1;
				int deltaY = (toStep.Pos.y - fromStep.Pos.y + 1) << 2;
				int delta = deltaY + deltaX;
				if (delta != dir)
				{
					steps.Insert(0, fromStep);
					dir = delta;
				}
			}
			else
			{
				steps.Insert(0, fromStep);
				break;
			}
		}
	}

    //-------------------------------------------------------------------------
	public void Reset()
	{
		ViDoubleLinkNode2<ViAstarStep> iter = _openList.GetHead();
		while (!_openList.IsEnd(iter))
		{
			iter.Data.Clear();
			ViDoubleLink2<ViAstarStep>.Next(ref iter);
		}
		_openList.Clear();
		//
		iter = _closeList.GetHead();
		while (!_closeList.IsEnd(iter))
		{
			iter.Data.Clear();
			ViDoubleLink2<ViAstarStep>.Next(ref iter);
		}
		_closeList.Clear();
		//
		_openHeap.Clear();
	}

    //-------------------------------------------------------------------------
	void _NewStep(ViAstarStep step)
	{
		for (Int32 idx = 0; idx < step.RoundSteps.Count; ++idx)
		{
			ViAstarRoundStep roundStep = step.RoundSteps[idx];
			ViDebuger.AssertError(roundStep.node);
			ViAstarStep childStep = roundStep.node;
			if (childStep.IsClose)
			{
				continue;
			}
			float newG = roundStep.cost + step.G;
			if (childStep.IsOpen)
			{
				if (childStep.G > newG)
				{
					childStep.G = newG;
					childStep.Key = childStep.G + childStep.H;
					childStep.Parent = step;
				}
			}
			else
			{
				childStep.G = newG;
				childStep.H = ViAstarStep.Distance(childStep, _destStep);
				childStep.Key = childStep.G + childStep.H;
				childStep.Parent = step;
				_AddToOpen(childStep);
			}
		}
	}

    //-------------------------------------------------------------------------
	void _AddToOpen(ViAstarStep step)
	{
		_openList.PushBack(step.AttachNode);
		step.State = ViAstarStepState.OPEN;
		_openHeap.Push(step);
	}

    //-------------------------------------------------------------------------
	void _AddToClose(ViAstarStep step)
	{
		step.State = ViAstarStepState.CLOSE;
		_closeList.PushBack(step.AttachNode);
	}
}
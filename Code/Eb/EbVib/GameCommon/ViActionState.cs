using System;
using System.Collections.Generic;

public class ViActionState
{
	public virtual Int32 Weight { get { return 0; } }
	public bool Active { get { return _active; } }
	public virtual UInt32 EndMask { get { return UInt32.MaxValue; } }

	public virtual bool Reset() { return false; }

	public virtual void OnEnd() { }

	public virtual void OnActive() { }
	public virtual void OnDeactive() { }

	public void SetActive(bool active) { _active = active; }

	bool _active = false;
}


public class ViActionStateList<TState> where TState : ViActionState
{
	public TState ActiveState
	{
		get
		{
			foreach (TState state in _list)
			{
				if (state.Active)
				{
					return state;
				}
			}
			return null;
		}
	}

	public void Start(TState state)
	{
		if (_list.Contains(state))
		{
			return;
		}
		for (int idx = 0; idx < _list.Count; ++idx )
		{
			TState iterState = _list[idx];
			if (state.Weight >= iterState.Weight)
			{
				_list.Insert(idx, state);
				return;
			}
		}
		_list.Add(state);
	}
	
	public bool End(UInt32 mask)
	{
		bool result = false;
		TState activeState = ActiveState;
		if (activeState != null && ViMask32.HasAny(activeState.EndMask, mask))
		{
			activeState.OnDeactive();
			activeState.SetActive(false);
			result = true;
		}
		//
		for (int idx = 0; idx < _list.Count; )
		{
			TState iterState = _list[idx];
			if (ViMask32.HasAny(iterState.EndMask, mask))
			{
				iterState.OnEnd();
				_list.RemoveAt(idx);
			}
			else
			{
				++idx;
			}
		}
		//
		if (result)
		{
			Reset();
		}
		return result;
	}
	
	public bool End()
	{
		bool result = false;
		TState activeState = ActiveState;
		if (activeState != null)
		{
			activeState.OnDeactive();
			activeState.SetActive(false);
			result = true;
		}
		//
		_asynResetNode.Detach();
		//
		foreach (TState iterState in _list)
		{
			iterState.OnEnd();
		}
		_list.Clear();
		return result;
	}

	public bool End(TState state)
	{
		for (int idx = 0; idx < _list.Count; ++idx)
		{
			TState iterState = _list[idx];
			if (Object.ReferenceEquals(state, iterState))
			{
				if (state.Active)
				{
					state.OnDeactive();
					state.SetActive(false);
					Reset();
				}
				state.OnEnd();
				_list.RemoveAt(idx);
				return true;
			}
		}
		return false;
	}

	public void Reset()
	{
		_asynResetNode.AsynExec(this._OnResetExec);
	}

	void _OnResetExec()
	{
		TState oldActiveState = ActiveState;
		foreach (TState iterState in _list)
		{
			if (iterState.Reset())
			{
				if (!System.Object.ReferenceEquals(iterState, oldActiveState))
				{
					if (oldActiveState != null)
					{
						oldActiveState.OnDeactive();
						oldActiveState.SetActive(false);
					}
					ViDebuger.AssertError(iterState.Active == false);
					iterState.SetActive(true);
					iterState.OnActive();
				}
				break;
			}
		}
	}
	ViFramEndCallback0 _asynResetNode = new ViFramEndCallback0();

	List<TState> _list = new List<TState>();
}
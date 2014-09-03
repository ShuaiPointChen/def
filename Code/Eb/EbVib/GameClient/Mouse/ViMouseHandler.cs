using System;



public abstract class ViMouseHandlerInterface : ViDoubleLinkNode1<ViMouseHandlerInterface>
{
	public abstract bool AcceptMove { get; }
	public abstract bool Exclusive { get; }

	public virtual void End() { Detach(); }
	public virtual void OnPressed() { }
	public virtual void OnReleased() { }
	public virtual void OnMoved() { }
	public virtual void Reset() { }
}

public class ViMouseController
{
	public bool AcceptMove
	{
		get
		{
			ViMouseHandlerInterface handler = _GetTop();
			if (handler != null)
			{
				return handler.AcceptMove;
			}
			else
			{
				return false;
			}
		}
	}
	public bool Exclusive
	{
		get
		{
			ViMouseHandlerInterface handler = _GetTop();
			if (handler != null)
			{
				return handler.Exclusive;
			}
			else
			{
				return false;
			}
		}
	}
	public void AttachFront(ViMouseHandlerInterface handler)
	{
		_handlerList.PushFront(handler);
	}
	public void AttachBack(ViMouseHandlerInterface handler)
	{
		_handlerList.PushBack(handler);
	}
	public void OnPressed()
	{
		ViMouseHandlerInterface handler = _GetTop();
		if (handler != null)
		{
			handler.OnPressed();
		}
	}
	public void OnReleased()
	{
		ViMouseHandlerInterface handler = _GetTop();
		if (handler != null)
		{
			handler.OnReleased();
		}
	}
	public void OnMoved()
	{
		ViMouseHandlerInterface handler = _GetTop();
		if (handler != null)
		{
			handler.OnMoved();
		}
	}
	public void Clear()
	{
		_handlerList.Clear();
	}
	public void ClearPress()
	{
		ViMouseHandlerInterface handler = _GetTop();
		if (handler != null)
		{
			handler.Reset();
		}
	}
	ViMouseHandlerInterface _GetTop()
	{
		if (_handlerList.IsNotEmpty())
		{
			return _handlerList.GetHead() as ViMouseHandlerInterface;
		}
		else
			return null;
	}

	ViDoubleLink1<ViMouseHandlerInterface> _handlerList = new ViDoubleLink1<ViMouseHandlerInterface>();
}
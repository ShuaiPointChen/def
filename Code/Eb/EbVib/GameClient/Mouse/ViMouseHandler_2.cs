using System;



public class ViMouseHandler_2: ViMouseHandlerInterface
{

	public delegate void Callback();

	enum State
	{
		NONE,
		MOVING,
	}

	public override bool AcceptMove { get { return IsPressing; } }
	public override bool Exclusive { get { return IsPressing; } }
	public bool IsMoving { get { return (_state == State.MOVING); } }
	public bool IsPressing { get { return (_state != State.NONE); } }
	
	public Callback _callbackOnMoveStart;
	public Callback _callbackOnMoving;
	public Callback _callbackOnMoveEnd;

	public override void End()
	{
		base.End();
		_callbackOnMoveStart = null;
		_callbackOnMoving = null;
		_callbackOnMoveEnd = null;
	}

	public override void Reset()
	{
		if (_state == State.MOVING)
		{
			_state = State.NONE;
			_OnMoveEnd();
		}
	}

	public override void OnPressed()
	{
		if (_state == State.NONE)
		{
			_state = State.MOVING;
			_OnMoveStart();
		}
	}
	public override void OnReleased()
	{
		if (_state == State.MOVING)
		{
			_state = State.NONE;
			_OnMoveEnd();
		}
	}
	public override void OnMoved()
	{
		if (_state == State.MOVING)
		{
			_OnMoving();
		}
	}
	
	void _OnMoveStart() { if (_callbackOnMoveStart != null) { _callbackOnMoveStart(); } }
	void _OnMoving() { if (_callbackOnMoving != null) { _callbackOnMoving(); } }
	void _OnMoveEnd() { if (_callbackOnMoveEnd != null) { _callbackOnMoveEnd(); } }

	State _state;
}
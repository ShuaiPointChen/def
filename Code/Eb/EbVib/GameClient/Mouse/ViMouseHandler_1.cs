using System;

public class ViMouseHandler_1 : ViMouseHandlerInterface
{
	public delegate void Callback();

	enum State
	{
		NONE,
		PRESSED,
		MOVING_START,
		MOVING,
	}

	public override bool AcceptMove { get { return IsPressing; } }
	public override bool Exclusive { get { return IsPressing; } }
	public bool IsMoving { get { return (_state == State.MOVING); } }
	public bool IsPressing { get { return (_state != State.NONE); } }
	public bool IsPressed { get { return (_state == State.PRESSED); } }
	public float EnterMovingStateTime { get { return _enterMovingStateTime; } set { _enterMovingStateTime = value; } }
	public float IgnoreMovingStateTime { get { return _ignoreMovingStateTime; } set { _ignoreMovingStateTime = value; } }
	public Callback _callbackOnMoveStart;
	public Callback _callbackOnMoving;
	public Callback _callbackOnMoveEnd;
	public Callback _callbackOnReleased;

	public override void Reset()
	{
		if (_state == State.MOVING)
		{
			_OnMoveEnd();
		}
		_pressedEndNode.Detach();
		_allowMoveStartNode.Detach();
		_state = State.NONE;
	}
	public override void End()
	{
		base.End();
		_callbackOnMoveStart = null;
		_callbackOnMoving = null;
		_callbackOnMoveEnd = null;
		_callbackOnReleased = null;
		_pressedEndNode.Detach();
		_allowMoveStartNode.Detach();
	}

	public override void OnPressed()
	{
		ViDebuger.AssertWarning(_state == State.NONE);
		_state = State.PRESSED;
		ViTimerInstance.SetTime(_allowMoveStartNode, IgnoreMovingStateTime, this.OnAllowMoveStartTime);
		ViTimerInstance.SetTime(_pressedEndNode, EnterMovingStateTime, this.OnMoveTime);
	}
	public override void OnReleased()
	{
		if (_state == State.PRESSED)
		{
			_OnReleased();
		}
		if (_state == State.MOVING)
		{
			_OnMoveEnd();
		}
		_state = State.NONE;
		_pressedEndNode.Detach();
		_pressedEndNode.Detach();
	}
	public override void OnMoved()
	{
		if (_state == State.PRESSED)
		{
			if (!_allowMoveStartNode.IsAttach())
			{
				_state = State.MOVING;
				_pressedEndNode.Detach();
				_OnMoveStart();
			}
		}
		else if (_state == State.MOVING)
		{
			_OnMoving();
		}
	}
	void OnMoveTime(ViTimeNodeInterface node)
	{
		ViDebuger.AssertWarning(_state == State.PRESSED);
		_state = State.MOVING;
		_OnMoveStart();
	}

	void OnAllowMoveStartTime(ViTimeNodeInterface node)
	{

	}
	void _OnMoveStart() { if (_callbackOnMoveStart != null) { _callbackOnMoveStart(); } }
	void _OnMoving() { if (_callbackOnMoving != null) { _callbackOnMoving(); } }
	void _OnMoveEnd() { if (_callbackOnMoveEnd != null) { _callbackOnMoveEnd(); } }
	void _OnReleased() { if (_callbackOnReleased != null) { _callbackOnReleased(); } }

	ViTimeNode1 _pressedEndNode = new ViTimeNode1();
	ViTimeNode1 _allowMoveStartNode = new ViTimeNode1();

	State _state;
	float _enterMovingStateTime = 0.3f;
	float _ignoreMovingStateTime = 0.06f;

}

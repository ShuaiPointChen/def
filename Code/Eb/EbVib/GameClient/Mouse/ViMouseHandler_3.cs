using System;


public class ViMouseHandler_3 : ViMouseHandlerInterface
{

	public delegate void Callback();
	enum State
	{
		NONE,
		PRESSING,
	}

	public override bool AcceptMove { get { return IsPressing; } }
	public override bool Exclusive { get { return IsPressing; } }
	public bool IsPressing { get { return (_state != State.NONE); } }

	public Callback _callbackOnPressed;
	public Callback _callbackOnReleased;

	public override void End()
	{
		base.End();
		_callbackOnPressed = null;
		_callbackOnReleased = null;
	}


	public override void Reset()
	{
		if (_state == State.PRESSING)
		{
			_OnReleased();
		}
		_state = State.NONE;
	}

	public override void OnPressed()
	{
		if (_state == State.NONE)
		{
			_state = State.PRESSING;
			_OnPressed();
		}
	}
	public override void OnReleased()
	{
		if (_state == State.PRESSING)
		{
			_state = State.NONE;
			_OnReleased();
		}
	}
	public override void OnMoved()
	{

	}

	void _OnPressed() { if (_callbackOnPressed != null) { _callbackOnPressed(); } }
	void _OnReleased() { if (_callbackOnReleased != null) { _callbackOnReleased(); } }

	State _state;
}
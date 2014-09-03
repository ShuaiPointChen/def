using System;

public interface ViNotifierInterface
{
	void Notify();
}

class ViAsynNotifier<TNotifier>
	where TNotifier : ViNotifierInterface, new()
{
	public ViAsynNotifier()
	{
		_notifier = new TNotifier();
		_asynCallback = new ViFramEndCallback0();
	}
	void AsynNotify()
	{
		_asynCallback.AsynExec(_notifier.Notify);
	}
	void DetachAsyn()
	{
		_asynCallback.Detach();
	}

	public TNotifier Notifier { get { return _notifier; } }
	private TNotifier _notifier;
	private ViFramEndCallback0 _asynCallback;
}

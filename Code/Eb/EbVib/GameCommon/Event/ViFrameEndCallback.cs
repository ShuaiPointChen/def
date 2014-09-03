using System;


public abstract class ViFramEndCallbackInterface : ViDoubleLinkNode1<ViFramEndCallbackInterface>
{
	public static void Update()
	{
		while (_callbackList.IsNotEmpty())
		{
			ViFramEndCallback0 callback = _callbackList.GetHead() as ViFramEndCallback0;
			ViDebuger.AssertError(callback);
			callback._OnExec();
		}
	}
	//
	internal abstract void _OnExec();
	protected static ViDoubleLink1<ViFramEndCallbackInterface> _callbackList = new ViDoubleLink1<ViFramEndCallbackInterface>();
}

public class ViFramEndCallback0 : ViFramEndCallbackInterface
{
	public delegate void Callback();
	public new bool IsAttach()
	{
		return base.IsAttach();
	}
	public new void Detach()
	{
		_delegate = null;
		base.Detach();
	}
	public void AsynExec(Callback dele)
	{
		_delegate = dele;
		_callbackList.PushBack(this);
	}
	internal override void _OnExec()
	{
		base.Detach();
		ViDebuger.AssertError(_delegate);
		Callback tempDele = _delegate;
		_delegate = null;
		tempDele();
	}
	//
	private Callback _delegate;
}

public class ViFramEndCallback1<TParam0> : ViFramEndCallbackInterface
{
	public delegate void Callback(TParam0 param0);
	public new bool IsAttach()
	{
		return base.IsAttach();
	}
	public new void Detach()
	{
		_delegate = null;
		_param0 = default(TParam0);
		base.Detach();
	}
	public void AsynExec(Callback dele, TParam0 param0)
	{
		_delegate = dele;
		_param0 = param0;
		_callbackList.PushBack(this);
	}
	internal override void _OnExec()
	{
		base.Detach();
		ViDebuger.AssertError(_delegate);
		Callback tempDele = _delegate;
		TParam0 tempParam0 = _param0;
		_delegate = null;
		_param0 = default(TParam0);
		tempDele(tempParam0);
	}
	//
	private TParam0 _param0;
	private Callback _delegate;
}
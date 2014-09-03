using System;


public abstract class ViProvider<T>
		where T : struct
{
	public abstract T Value { get; }
}

public class ViSimpleProvider<T> : ViProvider<T>
		where T : struct
{
	public ViSimpleProvider() { }
	public ViSimpleProvider(T value) { _value = value; }
	public override T Value { get { return _value; } }
	public void SetValue(T value) { _value = value; }
	T _value;
}
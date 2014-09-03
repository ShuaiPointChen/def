using System;
using System.Collections.Generic;

using UInt8 = System.Byte;

public class ViReceiveDataVector3 : ViReceiveDataNode
{
	public ViVector3 Value { get { return _value; } }
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			ViVector3 oldValue = _value;
			IS.Read(out _value);
			OnUpdateInvoke(oldValue);
		}
	}
	public new void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			IS.Read(out _value);
		}
	}
	public new void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channelMask))
		{
			IS.Read(out _value);
		}
	}
	public static bool operator ==(ViReceiveDataVector3 lhs, ViReceiveDataVector3 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataVector3 lhs, ViReceiveDataVector3 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataVector3 lhs, ViVector3 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataVector3 lhs, ViVector3 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator ViVector3(ViReceiveDataVector3 data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataVector3))
		{
			return false;
		}
		ViReceiveDataVector3 data = (ViReceiveDataVector3)other;
		return _value.Equals(data.Value);
	}
	ViVector3 _value;
}
public static class ViReceiveDataVector3Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataVector3 value)
	{
		OS.Append(value);
	}
}
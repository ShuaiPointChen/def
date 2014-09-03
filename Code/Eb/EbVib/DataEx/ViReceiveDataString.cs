using System;

using UInt8 = System.Byte;

public class ViReceiveDataString : ViReceiveDataNode
{
	public string Value { get { return _value; } }
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			string oldValue = _value;
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
	public static bool operator ==(ViReceiveDataString lhs, ViReceiveDataString rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataString lhs, ViReceiveDataString rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataString lhs, string rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataString lhs, string rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator string(ViReceiveDataString data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataString))
		{
			return false;
		}
		ViReceiveDataString data = (ViReceiveDataString)other;
		return _value.Equals(data.Value);
	}
	string _value = string.Empty;
}
public static class ViReceiveDataStringSerialize
{
	public static void Append(this ViOStream OS, ViReceiveDataString value)
	{
		OS.Append(value);
	}
}
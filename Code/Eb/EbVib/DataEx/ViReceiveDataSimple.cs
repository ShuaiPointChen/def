using System;

using Int8 = System.SByte;
using UInt8 = System.Byte;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataInt8 : ViReceiveDataNode
{
	public Int8 Value { get { return _value; } }
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			Int8 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataInt8 lhs, ViReceiveDataInt8 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataInt8 lhs, ViReceiveDataInt8 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataInt8 lhs, Int8 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataInt8 lhs, Int8 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int8(ViReceiveDataInt8 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataInt8 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataInt8))
		{
			return false;
		}
		ViReceiveDataInt8 data = (ViReceiveDataInt8)other;
		return _value.Equals(data.Value);
	}
	Int8 _value;
}

public static class ViReceiveDataInt8Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataInt8 value)
	{
		OS.Append(value);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataUInt8 : ViReceiveDataNode
{
	public UInt8 Value { get { return _value; } }
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			UInt8 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataUInt8 lhs, ViReceiveDataUInt8 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataUInt8 lhs, ViReceiveDataUInt8 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataUInt8 lhs, UInt8 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataUInt8 lhs, UInt8 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt8(ViReceiveDataUInt8 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataUInt8 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataUInt8))
		{
			return false;
		}
		ViReceiveDataUInt8 data = (ViReceiveDataUInt8)other;
		return _value.Equals(data.Value);
	}
	UInt8 _value;
}

public static class ViReceiveDataUInt8Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataUInt8 value)
	{
		OS.Append(value);
	}
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataInt16 : ViReceiveDataNode
{
	public Int16 Value { get { return _value; } }
	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			Int16 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataInt16 lhs, ViReceiveDataInt16 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataInt16 lhs, ViReceiveDataInt16 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataInt16 lhs, Int16 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataInt16 lhs, Int16 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int16(ViReceiveDataInt16 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataInt16 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataInt16))
		{
			return false;
		}
		ViReceiveDataInt16 data = (ViReceiveDataInt16)other;
		return _value.Equals(data.Value);
	}
	Int16 _value;
}

public static class ViReceiveDataInt16Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataInt16 value)
	{
		OS.Append(value);
	}
}



//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataUInt16 : ViReceiveDataNode
{
	public UInt16 Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			UInt16 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataUInt16 lhs, ViReceiveDataUInt16 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataUInt16 lhs, ViReceiveDataUInt16 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataUInt16 lhs, UInt16 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataUInt16 lhs, UInt16 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt16(ViReceiveDataUInt16 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataUInt16 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataUInt16))
		{
			return false;
		}
		ViReceiveDataUInt16 data = (ViReceiveDataUInt16)other;
		return _value.Equals(data.Value);
	}
	UInt16 _value;
}

public static class ViReceiveDataUInt16Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataUInt16 value)
	{
		OS.Append(value);
	}
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataInt32 : ViReceiveDataNode
{
	public Int32 Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			Int32 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataInt32 lhs, ViReceiveDataInt32 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataInt32 lhs, ViReceiveDataInt32 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataInt32 lhs, Int32 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataInt32 lhs, Int32 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int32(ViReceiveDataInt32 data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataInt32))
		{
			return false;
		}
		ViReceiveDataInt32 data = (ViReceiveDataInt32)other;
		return _value.Equals(data.Value);
	}
	Int32 _value;
}

public static class ViReceiveDataInt32Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataInt32 value)
	{
		OS.Append(value);
	}
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataUInt32 : ViReceiveDataNode
{
	public UInt32 Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			UInt32 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataUInt32 lhs, ViReceiveDataUInt32 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataUInt32 lhs, ViReceiveDataUInt32 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataUInt32 lhs, UInt32 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataUInt32 lhs, UInt32 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt32(ViReceiveDataUInt32 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataUInt32 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataUInt32))
		{
			return false;
		}
		ViReceiveDataUInt32 data = (ViReceiveDataUInt32)other;
		return _value.Equals(data.Value);
	}
	UInt32 _value;
}

public static class ViReceiveDataUInt32Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataUInt32 value)
	{
		OS.Append(value);
	}
}


//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataInt64 : ViReceiveDataNode
{
	public Int64 Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			Int64 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataInt64 lhs, ViReceiveDataInt64 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataInt64 lhs, ViReceiveDataInt64 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataInt64 lhs, Int64 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataInt64 lhs, Int64 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int64(ViReceiveDataInt64 data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataInt64))
		{
			return false;
		}
		ViReceiveDataInt64 data = (ViReceiveDataInt64)other;
		return _value.Equals(data.Value);
	}
	Int64 _value;
}

public static class ViReceiveDataInt64Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataInt64 value)
	{
		OS.Append(value);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataUInt64 : ViReceiveDataNode
{
	public UInt64 Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			UInt64 oldValue = _value;
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
	public static bool operator ==(ViReceiveDataUInt64 lhs, ViReceiveDataUInt64 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataUInt64 lhs, ViReceiveDataUInt64 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataUInt64 lhs, UInt64 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataUInt64 lhs, UInt64 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt64(ViReceiveDataUInt64 data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataUInt64))
		{
			return false;
		}
		ViReceiveDataUInt64 data = (ViReceiveDataUInt64)other;
		return _value.Equals(data.Value);
	}
	UInt64 _value;
}

public static class ViReceiveDataUInt64Serialize
{
	public static void Append(this ViOStream OS, ViReceiveDataUInt64 value)
	{
		OS.Append(value);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataFloat : ViReceiveDataNode
{
	public float Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			float oldValue = _value;
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
	public static bool operator ==(ViReceiveDataFloat lhs, ViReceiveDataFloat rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataFloat lhs, ViReceiveDataFloat rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataFloat lhs, float rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataFloat lhs, float rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator float(ViReceiveDataFloat data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataFloat))
		{
			return false;
		}
		ViReceiveDataFloat data = (ViReceiveDataFloat)other;
		return _value.Equals(data.Value);
	}
	float _value;
}

public static class ViReceiveDataFloatSerialize
{
	public static void Append(this ViOStream OS, ViReceiveDataFloat value)
	{
		OS.Append(value);
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViReceiveDataDouble : ViReceiveDataNode
{
	public double Value { get { return _value; } }

	public override void OnUpdate(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		if (MatchChannel(channel))
		{
			double oldValue = _value;
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
	public static bool operator ==(ViReceiveDataDouble lhs, ViReceiveDataDouble rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataDouble lhs, ViReceiveDataDouble rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataDouble lhs, double rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataDouble lhs, double rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator double(ViReceiveDataDouble data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataDouble))
		{
			return false;
		}
		ViReceiveDataDouble data = (ViReceiveDataDouble)other;
		return _value.Equals(data.Value);
	}
	double _value;
}

public static class ViReceiveDataDoubleSerialize
{
	public static void Append(this ViOStream OS, ViReceiveDataDouble value)
	{
		OS.Append(value);
	}
}
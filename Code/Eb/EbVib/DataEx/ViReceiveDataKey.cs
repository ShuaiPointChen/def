using System;
using System.Collections.Generic;

using Int8 = System.SByte;
using UInt8 = System.Byte;

public interface ViReceiveDataKeyInterface
{
	void Read(ViIStream IS);
}

public class ViReceiveDataKeyInt8 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyInt8() { }
	public ViReceiveDataKeyInt8(Int8 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public Int8 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyInt8 lhs, ViReceiveDataKeyInt8 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyInt8 lhs, ViReceiveDataKeyInt8 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyInt8 lhs, Int8 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyInt8 lhs, Int8 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int8(ViReceiveDataKeyInt8 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyInt8 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyInt8))
		{
			return false;
		}
		ViReceiveDataKeyInt8 data = (ViReceiveDataKeyInt8)other;
		return _value.Equals(data.Value);
	}
	Int8 _value;
}

public class ViReceiveDataKeyUInt8 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyUInt8() { }
	public ViReceiveDataKeyUInt8(UInt8 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public UInt8 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyUInt8 lhs, ViReceiveDataKeyUInt8 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyUInt8 lhs, ViReceiveDataKeyUInt8 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyUInt8 lhs, UInt8 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyUInt8 lhs, UInt8 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt8(ViReceiveDataKeyUInt8 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyUInt8 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyUInt8))
		{
			return false;
		}
		ViReceiveDataKeyUInt8 data = (ViReceiveDataKeyUInt8)other;
		return _value.Equals(data.Value);
	}
	UInt8 _value;
}


public class ViReceiveDataKeyInt16 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyInt16() { }
	public ViReceiveDataKeyInt16(Int16 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public Int16 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyInt16 lhs, ViReceiveDataKeyInt16 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyInt16 lhs, ViReceiveDataKeyInt16 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyInt16 lhs, Int16 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyInt16 lhs, Int16 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int16(ViReceiveDataKeyInt16 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyInt16 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyInt16))
		{
			return false;
		}
		ViReceiveDataKeyInt16 data = (ViReceiveDataKeyInt16)other;
		return _value.Equals(data.Value);
	}
	Int16 _value;
}


public class ViReceiveDataKeyUInt16 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyUInt16() { }
	public ViReceiveDataKeyUInt16(UInt16 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public UInt16 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyUInt16 lhs, ViReceiveDataKeyUInt16 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyUInt16 lhs, ViReceiveDataKeyUInt16 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyUInt16 lhs, UInt16 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyUInt16 lhs, UInt16 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt16(ViReceiveDataKeyUInt16 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyUInt16 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyUInt16))
		{
			return false;
		}
		ViReceiveDataKeyUInt16 data = (ViReceiveDataKeyUInt16)other;
		return _value.Equals(data.Value);
	}
	UInt16 _value;
}


public class ViReceiveDataKeyInt32 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyInt32() { }
	public ViReceiveDataKeyInt32(Int32 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public Int32 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyInt32 lhs, ViReceiveDataKeyInt32 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyInt32 lhs, ViReceiveDataKeyInt32 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyInt32 lhs, Int32 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyInt32 lhs, Int32 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int32(ViReceiveDataKeyInt32 data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyInt32))
		{
			return false;
		}
		ViReceiveDataKeyInt32 data = (ViReceiveDataKeyInt32)other;
		return _value.Equals(data.Value);
	}
	Int32 _value;
}


public class ViReceiveDataKeyUInt32 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyUInt32() { }
	public ViReceiveDataKeyUInt32(UInt32 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public UInt32 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyUInt32 lhs, ViReceiveDataKeyUInt32 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyUInt32 lhs, ViReceiveDataKeyUInt32 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyUInt32 lhs, UInt32 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyUInt32 lhs, UInt32 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt32(ViReceiveDataKeyUInt32 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyUInt32 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyUInt32))
		{
			return false;
		}
		ViReceiveDataKeyUInt32 data = (ViReceiveDataKeyUInt32)other;
		return _value.Equals(data.Value);
	}
	UInt32 _value;
}


public class ViReceiveDataKeyInt64 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyInt64() { }
	public ViReceiveDataKeyInt64(Int64 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public Int64 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyInt64 lhs, ViReceiveDataKeyInt64 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyInt64 lhs, ViReceiveDataKeyInt64 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyInt64 lhs, Int64 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyInt64 lhs, Int64 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator Int64(ViReceiveDataKeyInt64 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyInt64 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyInt64))
		{
			return false;
		}
		ViReceiveDataKeyInt64 data = (ViReceiveDataKeyInt64)other;
		return _value.Equals(data.Value);
	}
	Int64 _value;
}


public class ViReceiveDataKeyUInt64 : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyUInt64() { }
	public ViReceiveDataKeyUInt64(UInt64 value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public UInt64 Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyUInt64 lhs, ViReceiveDataKeyUInt64 rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyUInt64 lhs, ViReceiveDataKeyUInt64 rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyUInt64 lhs, UInt64 rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyUInt64 lhs, UInt64 rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator UInt64(ViReceiveDataKeyUInt64 data)
	{
		return data.Value;
	}
	public static implicit operator int(ViReceiveDataKeyUInt64 data)
	{
		return (int)data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyUInt64))
		{
			return false;
		}
		ViReceiveDataKeyUInt64 data = (ViReceiveDataKeyUInt64)other;
		return _value.Equals(data.Value);
	}
	UInt64 _value;
}


public class ViReceiveDataKeyString : ViReceiveDataKeyInterface
{
	public ViReceiveDataKeyString() { }
	public ViReceiveDataKeyString(string value) { _value = value; }

	public void Read(ViIStream IS)
	{
		IS.Read(out _value);
	}
	public string Value { get { return _value; } }
	public static bool operator ==(ViReceiveDataKeyString lhs, ViReceiveDataKeyString rhs)
	{
		return (lhs.Value == rhs.Value);
	}
	public static bool operator !=(ViReceiveDataKeyString lhs, ViReceiveDataKeyString rhs)
	{
		return (lhs.Value != rhs.Value);
	}
	public static bool operator ==(ViReceiveDataKeyString lhs, String rhs)
	{
		return (lhs.Value == rhs);
	}
	public static bool operator !=(ViReceiveDataKeyString lhs, String rhs)
	{
		return (lhs.Value != rhs);
	}
	public static implicit operator string(ViReceiveDataKeyString data)
	{
		return data.Value;
	}
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}
	public override bool Equals(object other)
	{
		if (!(other is ViReceiveDataKeyString))
		{
			return false;
		}
		ViReceiveDataKeyString data = (ViReceiveDataKeyString)other;
		return _value.Equals(data.Value);
	}
	string _value;
}

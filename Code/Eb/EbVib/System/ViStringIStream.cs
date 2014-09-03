using System;
using System.Collections;
using System.Collections.Generic;

using Int8 = System.SByte;
using UInt8 = System.Byte;

public class ViStringIStream
{
	public int RemainLength { get { return _buffer.Count - _offset; } }

	public void Init(List<string> buffer)
	{
		_buffer = buffer;
		_offset = 0;
	}
	public void ReWind() { _offset = 0; }

	public bool Read(out Int8 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (Int8)0;
			return false;
		}
		try
		{
			value = Convert.ToSByte(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (Int8)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out UInt8 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (UInt8)0;
			return false;
		}
		try
		{
			value = Convert.ToByte(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (UInt8)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out Int16 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (Int16)0;
			return false;
		}
		try
		{
			value = Convert.ToInt16(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (Int16)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out UInt16 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (UInt16)0;
			return false;
		}
		try
		{
			value = Convert.ToUInt16(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (UInt16)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out Int32 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (Int32)0;
			return false;
		}
		try
		{
			value = Convert.ToInt32(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (Int32)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out UInt32 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (UInt32)0;
			return false;
		}
		 try
		 {
			 value = Convert.ToUInt32(_buffer[_offset]);
        }
		 catch (FormatException e)
		 {
			 ViDebuger.Warning("ViStringIStream.Read Error" + e);
			 value = 0;
			 return false;
		 }
		++_offset;
		return true;
	}
	public bool Read(out Int64 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (Int64)0;
			return false;
		}
		try
		{
			value = Convert.ToInt64(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (Int64)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out UInt64 value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (UInt64)0;
			return false;
		}
		try
		{
			value = Convert.ToUInt64(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (UInt64)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out float value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (float)0;
			return false;
		}
		try
		{
			value = Convert.ToSingle(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (float)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out double value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = (double)0;
			return false;
		}
		try
		{
			value = Convert.ToDouble(_buffer[_offset]);
		}
		catch (FormatException e)
		{
			ViDebuger.Warning("ViStringIStream.Read Error" + e);
			value = (double)0;
			return false;
		}
		++_offset;
		return true;
	}
	public bool Read(out string value)
	{
		ViDebuger.AssertError(_buffer);
		if (_offset >= _buffer.Count)
		{
			value = string.Empty;
			return false;
		}
		value = _buffer[_offset];
		++_offset;
		return true;
	}
	List<string> _buffer;
	int _offset;
}

using System;

using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViArrayIdx = System.Int32;


public class ViIStream
{
	public byte[] Buffer { get { return _buffer; } }
	public int RemainLength { get { return _len - _offset; } }
	public readonly static int ArraySizeSize = 4;


	public void Init(byte[] buffer, int len)
	{
		this._buffer = buffer;
		_offset = 0;
		_len = len;
		//ViDebuger.AssertError(buffer.Length >= len);
	}
	public void ReWind() { _offset = 0; }

	public bool Read(out Int8 value)
	{
		if (_offset + 1 > _len)
		{
			ViDebuger.Warning("Read Fail Int8");
			value = (Int8)0;
			return false;
		}
		value = ViBitConverter.ToInt8(_buffer, _offset);
		_Print("Int8", value);
		_offset += 1;
		return true;
	}
	public bool Read(out UInt8 value)
	{
		if (_offset + 1 > _len)
		{
			ViDebuger.Warning("Read Fail UInt8");
			value = (UInt8)0;
			return false;
		}
		value = _buffer[_offset];
		_Print("UInt8", value);
		_offset += 1;
		return true;
	}
	public bool Read(out Int16 value)
	{
		if (_offset + 2 > _len)
		{
			ViDebuger.Warning("Read Fail Int16");
			value = (Int16)0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToInt16(_buffer, _offset);
		_Print("Int16", value);
		_offset += 2;
		return true;
	}
	public bool Read(out UInt16 value)
	{
		if (_offset + 2 > _len)
		{
			ViDebuger.Warning("Read Fail UInt16");
			value = (UInt16)0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToUInt16(_buffer, _offset);
		_Print("UInt16", value);
		_offset += 2;
		return true;
	}
	public bool Read(out Int32 value)
	{
		if (_offset + 4 > _len)
		{
			ViDebuger.Warning("Read Fail Int32");
			value = 0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToInt32(_buffer, _offset);
		_Print("Int32", value);
		_offset += 4;
		return true;
	}
	public bool Read(out UInt32 value)
	{
		if (_offset + 4 > _len)
		{
			ViDebuger.Warning("Read Fail UInt32");
			value = 0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToUInt32(_buffer, _offset);
		_Print("UInt32", value);
		_offset += 4;
		return true;
	}
	public bool Read(out Int64 value)
	{
		if (_offset + 8 > _len)
		{
			ViDebuger.Warning("Read Fail Int64");
			value = (Int64)0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToInt64(_buffer, _offset);
		_Print("Int64", value);
		_offset += 8;
		return true;
	}
	public bool Read(out UInt64 value)
	{
		if (_offset + 8 > _len)
		{
			ViDebuger.Warning("Read Fail UInt64");
			value = (UInt64)0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToUInt64(_buffer, _offset);
		_Print("UInt64", value);
		_offset += 8;
		return true;
	}
	public bool Read(out float value)
	{
		if (_offset + 4 > _len)
		{
			ViDebuger.Warning("Read Fail float");
			value = 0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToFloat(_buffer, _offset);
		_Print("float", value);
		_offset += 4;
		return true;
	}
	public bool Read(out double value)
	{
		if (_offset + 8 > _len)
		{
			ViDebuger.Warning("Read Fail double");
			value = 0;
			_offset = _len;
			return false;
		}
		value = ViBitConverter.ToDouble(_buffer, _offset);
		_Print("double", value);
		_offset += 8;
		return true;
	}
	public bool Read(out string value)
	{
		if (_offset + 1 > _len)
		{
			ViDebuger.Warning("Read Fail string");
			value = string.Empty;
			_offset = _len;
			return false;
		}
		Int32 len = ViBitConverter.ToUInt8(_buffer, _offset);
		_offset += 1;
		if (len == 255)
		{
			if (_offset + 2 > _len)
			{
				ViDebuger.Warning("Read Fail string");
				value = string.Empty;
				_offset = _len;
				return false;
			}
			len = ViBitConverter.ToUInt16(_buffer, _offset);
			_offset += 2;
		}
		value = ViBitConverter.ToString(_buffer, _offset, len);
		_Print("string", value);
		_offset += len;
		return true;
	}
	void _Print<T>(string type, T value)
	{
		//ViDebuger.Note("Read [" + _offset + "] <" + type + "> " + value);
	}
	protected byte[] _buffer;
	protected int _offset;
	protected int _len;
}

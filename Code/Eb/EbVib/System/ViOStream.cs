using System;

using Int8 = System.SByte;
using UInt8 = System.Byte;

public class ViOStream
{
	public byte[] Cache { get { return _buffer; } }
	public int Length { get { return _offset; } }

	public ViOStream()
		: this(1024)
	{

	}
	public ViOStream(int initialSize)
	{
		this._buffer = new byte[initialSize];
	}
	public void Reset() { _offset = 0; }

	public void Append(byte[] value)
	{
		this.Append(value, 0, value.Length);
	}
	public void Append(Int8 value)
	{
		_memCache[0] = (byte)value;
		this.Append(_memCache, 0, 1);
	}
	public void Append(UInt8 value)
	{
		this.EnsureBuffer(1);
		this._buffer[this._offset++] = value;
	}
	public void Append(Int16 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		this.Append(_memCache, 0, 2); 
	}
	public void Append(UInt16 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		this.Append(_memCache, 0, 2);
	}
	public void Append(Int32 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		_memCache[2] = (byte)(value >> 16);
		_memCache[3] = (byte)(value >> 24);
		this.Append(_memCache, 0, 4);
	}
	public void Append(UInt32 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		_memCache[2] = (byte)(value >> 16);
		_memCache[3] = (byte)(value >> 24);
		this.Append(_memCache, 0, 4);
	}
	public void Append(Int64 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		_memCache[2] = (byte)(value >> 16);
		_memCache[3] = (byte)(value >> 24);
		_memCache[4] = (byte)(value >> 32);
		_memCache[5] = (byte)(value >> 40);
		_memCache[6] = (byte)(value >> 48);
		_memCache[7] = (byte)(value >> 56);
		this.Append(_memCache, 0, 8);
	}
	public void Append(UInt64 value)
	{
		_memCache[0] = (byte)value;
		_memCache[1] = (byte)(value >> 8);
		_memCache[2] = (byte)(value >> 16);
		_memCache[3] = (byte)(value >> 24);
		_memCache[4] = (byte)(value >> 32);
		_memCache[5] = (byte)(value >> 40);
		_memCache[6] = (byte)(value >> 48);
		_memCache[7] = (byte)(value >> 56);
		this.Append(_memCache, 0, 8);
	}
	public void Append(float value)
	{
		this.Append((Int32)(value*100));
	}
	public void Append(double value)
	{
		this.Append((Int64)(value * 100));
	}

	public void Append(string value)
	{
		value = value.Trim();
		Int32 size = value.Length*2;
		if (size < 255)
		{
			Append((UInt8)size);
		}
		else
		{
			Append((UInt8)255);
			Append((UInt16)size);
		}
		this.EnsureBuffer(size);
		for (int i = 0; i < value.Length; i++)
		{
			char ch = value[i];
			//if (ch > '\x00ff')
			//{
			//    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
			//}
			this._buffer[this._offset + 2*i] = (byte)ch;
			this._buffer[this._offset + 2*i + 1] = (byte)(ch >> 8);
		}
		this._offset += value.Length*2;
	}
	public void Append(byte[] value, int offset, int count)
	{
		this.EnsureBuffer(count);
		System.Buffer.BlockCopy(value, offset, this._buffer, this._offset, count);
		this._offset += count;
	}

	protected void EnsureBuffer(int count)
	{
		if (count > (this._buffer.Length - this._offset))
		{
			byte[] dst = new byte[((this._buffer.Length * 2) > (this._buffer.Length + count)) ? (this._buffer.Length * 2) : (this._buffer.Length + count)];
			System.Buffer.BlockCopy(this._buffer, 0, dst, 0, this._offset);
			this._buffer = dst;
		}
	}

	public void _SetValue(int offset, byte[] value, int cnt)
	{
		ViDebuger.AssertError(offset + cnt < _offset && cnt <= value.Length);
		for (int i = 0; i < cnt; i++)
		{
			_buffer[offset + i] = value[i];
		}
	}

	protected byte[] _memCache = new byte[8];

	protected byte[] _buffer;
	protected int _offset;
}
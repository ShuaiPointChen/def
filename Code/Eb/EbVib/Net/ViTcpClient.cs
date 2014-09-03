using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

using UInt8 = System.Byte;

public class ViTcpCLientReceiver
{
	public ViTcpCLientReceiver(int maxSize)
	{
		Datas = new byte[maxSize];
	}
	public void SetCompleteSize(UInt16 flag, UInt32 size)
	{
		_flag = flag;
		_size = (int)(size);
	}

	public void Read(NetworkStream netStream)
	{
		int reserveSize = _size - _receiveSize;
		int len = netStream.Read(Datas, _receiveSize, reserveSize);
		_receiveSize += len;
	}
	public void Clear()
	{
		_size = 0;
		_receiveSize = 0;
	}

	public int Size { get { return _size; } }
	public UInt16 Flag { get { return _flag; } }
	public bool IsComplete { get { return _receiveSize == _size; } }

	public byte[] Data { get { return Datas; } }

	byte[] Datas;
	int _size = 0;
	int _receiveSize = 0;
	UInt16 _flag;

}

public class ViTcpClient
{

	public TcpClient TCP { get { return _client; } }

	public delegate void DeleDisconnected();
	public DeleDisconnected DisconnectedCallback;

	public bool Connected { get { return _connected; } }

	public ViTcpClient()
	{
		UInt16 index = 0;
		UInt32 size = 0;
		_indexBytes = ViBitConverter.GetBytes(index);
		_lengthBytes = ViBitConverter.GetBytes(size);
		_receiveHead = new byte[RECEIVE_HEAD_SIZE];

		_OS.Append(_indexBytes);
		_OS.Append(_lengthBytes);

		_receiver = new ViTcpCLientReceiver(RECEIVE_BYTES_SIZE);
	}
	public void Connect(string ip, int port)
	{
		_client.Connect(ip, port);
	}
	public void BeginConnect(string ip, int port, AsyncCallback requestCallback, object state)
	{
		_client.BeginConnect(ip, port, requestCallback, state);
	}
	public void Close()
	{
		_client.Close();
	}

	public ViOStream OS { get { return _OS; } }

	public void Update()
	{
		try
		{
			NetworkStream stream = _client.GetStream();
			_lengthBytes = ViBitConverter.GetBytes(_OS.Length - SEND_HEAD_SIZE);
			_OS._SetValue(SEND_SIZE_OFFSET, _lengthBytes, _lengthBytes.Length);
			stream.Write(_OS.Cache, 0, _OS.Length);
			_OS.Reset();
			_OS.Append(_indexBytes);
			_OS.Append(_lengthBytes);
		}
		catch (System.ObjectDisposedException ex)
		{
			SetConnectState(false);
		}
		catch (Exception ex)
		{
			SetConnectState(false);
		}
	}

	public bool GetIStream(out UInt16 flag, out ViIStream IS)
	{
		try
		{
			return UpdateStream(out flag, out IS);
		}
		catch (System.ObjectDisposedException ex)
		{
			flag = 0;
			IS = null;
			SetConnectState(false);
			return false;
		}
		catch (System.Exception ex)
		{
			ViDebuger.Note(ex.ToString());
			flag = 0;
			IS = null;
			SetConnectState(false);
			return false;
		}
	}

	bool UpdateStream(out UInt16 flag, out ViIStream IS)
	{
		flag = 0;
		IS = null;
		NetworkStream stream = _client.GetStream();
		if (_receiver.Size == 0)
		{
			if (_client.Available < RECEIVE_HEAD_SIZE)
			{
				return false;
			}
			SetConnectState(true);
			int bufferSize = stream.Read(_receiveHead, 0, RECEIVE_HEAD_SIZE);
			UInt16 flag_ = ViBitConverter.ToUInt16(_receiveHead, 0);
			UInt32 size = ViBitConverter.ToUInt32(_receiveHead, RECEIVE_SIZE_OFFSET);
			_receiver.SetCompleteSize(flag_, size);
		}
		_receiver.Read(stream);
		if (_receiver.IsComplete == false)
		{
			return false;
		}
		_IS.Init(_receiver.Data, _receiver.Size);
		_receiver.Clear();
		IS = _IS;
		return true;
	}

	void SetConnectState(bool connected)
	{
		if (connected == false && _connected)
		{
			if (DisconnectedCallback != null)
			{
				DisconnectedCallback();
			}
		}
		_connected = connected;
	}


	ViOStream _OS = new ViOStream(RECEIVE_BYTES_SIZE);
	ViIStream _IS = new ViIStream();
	TcpClient _client = new TcpClient();
	byte[] _indexBytes;
	byte[] _lengthBytes;
	static readonly int SEND_SIZE_OFFSET = 2;
	static readonly int SEND_HEAD_SIZE = 6;

	static readonly int RECEIVE_SIZE_OFFSET = 2;
	static readonly int RECEIVE_HEAD_SIZE = 6;
	static readonly int RECEIVE_BYTES_SIZE = 1024 * 1024;

	bool _connected = false;
	byte[] _receiveHead;
	ViTcpCLientReceiver _receiver;
}
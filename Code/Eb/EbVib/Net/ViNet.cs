using System;

using UInt8 = System.Byte;
using System.Net.Sockets;

public class ViNet
{

	public ViOStream OS { get { return _client.OS; } }
	public TcpClient TCP { get { return _client.TCP; } }
	public ViTcpClient Client { get { return _client; } }

	public void BeginConnect(string ip, int port, AsyncCallback requestCallback, object state)
	{
		_client.BeginConnect(ip, port, requestCallback, state);
	}
	public void Connect(string ip, int port)
	{
		if (!_client.TCP.Connected)
		{
			_client.Connect(ip, port);
		}
	}
	public void Close()
	{
		_client.Close();
	}
	public void Update()
	{
		_client.Update();
	}
	public bool GetIStream(out UInt16 flag, out ViIStream IS)
	{
		return _client.GetIStream(out flag, out IS);
	}
	
	ViTcpClient _client = new ViTcpClient();
}

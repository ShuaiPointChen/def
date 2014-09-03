using System;


public class ViRPCCallback<T>
{
	public UInt32 ID { get { return _CBID; } set { _CBID = value; } }

	public void Exception(ViRPCEntity entity)
	{
		if(_CBID == 0)
		{
			ViDebuger.Warning("ViRPCCallback: Exception Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 funcIdx = (UInt16)ViRPCMessage.EXEC_EXCEPTION;
		oStream.Append(funcIdx);
		oStream.Append(_CBID);
		entity.RPC.SendMessage();
		_CBID = 0;
	}
	UInt32 _CBID = 0;
}

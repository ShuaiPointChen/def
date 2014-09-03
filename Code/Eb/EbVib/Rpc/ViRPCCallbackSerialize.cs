using System;

using UInt8 = System.Byte;
using Int8 = System.SByte;

public static class ViRPCCallbackSerialize
{
	public static bool Read<T>(this ViIStream iStream, out ViRPCCallback<T> callback)
	{
		callback = new ViRPCCallback<T>();
		UInt32 CBID;
		if (!iStream.Read(out CBID))
		{
			return false;
		}
		callback.ID = CBID;
		return true;
	}
	public static bool Read<T>(this ViStringIStream iStream, out ViRPCCallback<T> callback)
	{
		ViDebuger.Warning("ViRPCCallback: not stringlize");
		callback = null;
		return false;
	}
	//
	public static void Invoke(this ViRPCCallback<string> callback, ViRPCEntity entity, string value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<string>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<float> callback, ViRPCEntity entity, float value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<float>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<Int8> callback, ViRPCEntity entity, Int8 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<Int8>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<UInt8> callback, ViRPCEntity entity, UInt8 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<UInt8>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<Int16> callback, ViRPCEntity entity, Int16 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<Int16>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<UInt16> callback, ViRPCEntity entity, UInt16 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<UInt16>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<Int32> callback, ViRPCEntity entity, Int32 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<Int32>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
	public static void Invoke(this ViRPCCallback<UInt32> callback, ViRPCEntity entity, UInt32 value)
	{
		if (callback.ID == 0)
		{
			ViDebuger.Warning("ViRPCCallback<UInt32>: Invoke Invalid");
			return;
		}
		ViOStream oStream = entity.RPC.OS;
		UInt16 uiFuncIdx = (UInt16)ViRPCMessage.EXEC_RESULT;
		oStream.Append(uiFuncIdx);
		oStream.Append(callback.ID);
		oStream.Append(value);
		entity.RPC.SendMessage();
		callback.ID = 0;
	}
}
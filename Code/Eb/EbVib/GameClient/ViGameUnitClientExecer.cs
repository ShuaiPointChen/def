using System;
using System.Collections;
using System.Collections.Generic;
using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViEntityID = System.UInt64;
using ViString = System.String;
using ViArrayIdx = System.Int32;
public class ViGameUnitClientExecer : ViRPCExecer
{
	public override ViEntityID ID() { ViDebuger.AssertError(_entity); return _entity.ID; }
	public override ViRPCEntity Entity { get { return _entity; } }
	protected void SetEntity(ViGameUnit entity)
	{
		_entity = entity;
	}
	public override void OnMessage(UInt16 funcIdx, ViIStream IS)
	{
		switch((ViGameUnitClientMethod)funcIdx)
		{
			default:
				base.OnMessage(funcIdx, IS);
				break;
		}
	}
	protected ViGameUnit _entity;
}

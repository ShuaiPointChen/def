using System;
using System.Collections;
using System.Collections.Generic;
using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViEntityID = System.UInt64;
using ViString = System.String;
using ViArrayIdx = System.Int32;
public struct LogicAuraValueArray
{
	public Int32 Element0;
	public Int32 Element1;
	public Int32 Element2;
	public Int32 Element3;
	public Int32 this[int index]
	{
		get
		{
			switch(index)
			{
				case 0:
					return Element0;
				case 1:
					return Element1;
				case 2:
					return Element2;
				case 3:
					return Element3;
			}
			ViDebuger.Error("");
			return Element0;
		}
	}
}
public static class LogicAuraValueArraySerialize
{
	public static void Append(this ViOStream OS, LogicAuraValueArray value)
	{
		OS.Append(value.Element0);
		OS.Append(value.Element1);
		OS.Append(value.Element2);
		OS.Append(value.Element3);
	}
	public static void Append(this ViOStream OS, List<LogicAuraValueArray> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (LogicAuraValueArray value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out LogicAuraValueArray value)
	{
		IS.Read(out value.Element0);
		IS.Read(out value.Element1);
		IS.Read(out value.Element2);
		IS.Read(out value.Element3);
	}
	public static void Read(this ViIStream IS, out List<LogicAuraValueArray> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<LogicAuraValueArray>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraValueArray value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static bool Read(this ViStringIStream IS, out LogicAuraValueArray value)
	{
		value = default(LogicAuraValueArray);
		if(IS.Read(out value.Element0) == false){return false;}
		if(IS.Read(out value.Element1) == false){return false;}
		if(IS.Read(out value.Element2) == false){return false;}
		if(IS.Read(out value.Element3) == false){return false;}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<LogicAuraValueArray> list)
	{
		list = null;
		ViArrayIdx size;
		if(IS.Read(out size) == false){return false;}
		list = new List<LogicAuraValueArray>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraValueArray value;
			if(IS.Read(out value) == false){return false;}
			list.Add(value);
		}
		return true;
	}
}
public class ReceiveDataLogicAuraValueArray: ViReceiveDataNode
{
	public static readonly new int SIZE = 4;
	public static readonly int Count = 4;
	public ViReceiveDataInt32 Element0 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element1 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element2 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element3 = new ViReceiveDataInt32();
	public override void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		Element0.Start(channel, IS, entity);
		Element0.Parent = this;
		Element1.Start(channel, IS, entity);
		Element1.Parent = this;
		Element2.Start(channel, IS, entity);
		Element2.Parent = this;
		Element3.Start(channel, IS, entity);
		Element3.Parent = this;
	}
	public override void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		Element0.Start(channelMask, IS, entity);
		Element0.Parent = this;
		Element1.Start(channelMask, IS, entity);
		Element1.Parent = this;
		Element2.Start(channelMask, IS, entity);
		Element2.Parent = this;
		Element3.Start(channelMask, IS, entity);
		Element3.Parent = this;
	}
	public override void End(ViEntity entity)
	{
		Element0.End(entity);
		Element1.End(entity);
		Element2.End(entity);
		Element3.End(entity);
		base.End(entity);
	}
	public override void Clear()
	{
		Element0.Clear();
		Element1.Clear();
		Element2.Clear();
		Element3.Clear();
		base.Clear();
	}
	public new void RegisterTo(UInt16 channelMask, ViReceiveProperty property)
	{
		Element0.RegisterTo(channelMask, property);
		Element1.RegisterTo(channelMask, property);
		Element2.RegisterTo(channelMask, property);
		Element3.RegisterTo(channelMask, property);
	}
	public override void StartByArray()
	{
		RegisterTo(ViConstValueDefine.MAX_UINT16, this);
	}
	public static implicit operator LogicAuraValueArray(ReceiveDataLogicAuraValueArray data)
	{
		LogicAuraValueArray value = new LogicAuraValueArray();
		value.Element0 = data.Element0;
		value.Element1 = data.Element1;
		value.Element2 = data.Element2;
		value.Element3 = data.Element3;
		return value;
	}
	public ViReceiveDataInt32 this[int index]
	{
		get
		{
			switch(index)
			{
				case 0:
					return Element0;
				case 1:
					return Element1;
				case 2:
					return Element2;
				case 3:
					return Element3;
			}
			ViDebuger.Error("");
			return Element0;
		}
	}
}
public struct LogicAuraCastorValueArray
{
	public Int32 Element0;
	public Int32 Element1;
	public Int32 Element2;
	public Int32 Element3;
	public Int32 this[int index]
	{
		get
		{
			switch(index)
			{
				case 0:
					return Element0;
				case 1:
					return Element1;
				case 2:
					return Element2;
				case 3:
					return Element3;
			}
			ViDebuger.Error("");
			return Element0;
		}
	}
}
public static class LogicAuraCastorValueArraySerialize
{
	public static void Append(this ViOStream OS, LogicAuraCastorValueArray value)
	{
		OS.Append(value.Element0);
		OS.Append(value.Element1);
		OS.Append(value.Element2);
		OS.Append(value.Element3);
	}
	public static void Append(this ViOStream OS, List<LogicAuraCastorValueArray> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (LogicAuraCastorValueArray value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out LogicAuraCastorValueArray value)
	{
		IS.Read(out value.Element0);
		IS.Read(out value.Element1);
		IS.Read(out value.Element2);
		IS.Read(out value.Element3);
	}
	public static void Read(this ViIStream IS, out List<LogicAuraCastorValueArray> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<LogicAuraCastorValueArray>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraCastorValueArray value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static bool Read(this ViStringIStream IS, out LogicAuraCastorValueArray value)
	{
		value = default(LogicAuraCastorValueArray);
		if(IS.Read(out value.Element0) == false){return false;}
		if(IS.Read(out value.Element1) == false){return false;}
		if(IS.Read(out value.Element2) == false){return false;}
		if(IS.Read(out value.Element3) == false){return false;}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<LogicAuraCastorValueArray> list)
	{
		list = null;
		ViArrayIdx size;
		if(IS.Read(out size) == false){return false;}
		list = new List<LogicAuraCastorValueArray>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraCastorValueArray value;
			if(IS.Read(out value) == false){return false;}
			list.Add(value);
		}
		return true;
	}
}
public class ReceiveDataLogicAuraCastorValueArray: ViReceiveDataNode
{
	public static readonly new int SIZE = 4;
	public static readonly int Count = 4;
	public ViReceiveDataInt32 Element0 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element1 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element2 = new ViReceiveDataInt32();
	public ViReceiveDataInt32 Element3 = new ViReceiveDataInt32();
	public override void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		Element0.Start(channel, IS, entity);
		Element0.Parent = this;
		Element1.Start(channel, IS, entity);
		Element1.Parent = this;
		Element2.Start(channel, IS, entity);
		Element2.Parent = this;
		Element3.Start(channel, IS, entity);
		Element3.Parent = this;
	}
	public override void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		Element0.Start(channelMask, IS, entity);
		Element0.Parent = this;
		Element1.Start(channelMask, IS, entity);
		Element1.Parent = this;
		Element2.Start(channelMask, IS, entity);
		Element2.Parent = this;
		Element3.Start(channelMask, IS, entity);
		Element3.Parent = this;
	}
	public override void End(ViEntity entity)
	{
		Element0.End(entity);
		Element1.End(entity);
		Element2.End(entity);
		Element3.End(entity);
		base.End(entity);
	}
	public override void Clear()
	{
		Element0.Clear();
		Element1.Clear();
		Element2.Clear();
		Element3.Clear();
		base.Clear();
	}
	public new void RegisterTo(UInt16 channelMask, ViReceiveProperty property)
	{
		Element0.RegisterTo(channelMask, property);
		Element1.RegisterTo(channelMask, property);
		Element2.RegisterTo(channelMask, property);
		Element3.RegisterTo(channelMask, property);
	}
	public override void StartByArray()
	{
		RegisterTo(ViConstValueDefine.MAX_UINT16, this);
	}
	public static implicit operator LogicAuraCastorValueArray(ReceiveDataLogicAuraCastorValueArray data)
	{
		LogicAuraCastorValueArray value = new LogicAuraCastorValueArray();
		value.Element0 = data.Element0;
		value.Element1 = data.Element1;
		value.Element2 = data.Element2;
		value.Element3 = data.Element3;
		return value;
	}
	public ViReceiveDataInt32 this[int index]
	{
		get
		{
			switch(index)
			{
				case 0:
					return Element0;
				case 1:
					return Element1;
				case 2:
					return Element2;
				case 3:
					return Element3;
			}
			ViDebuger.Error("");
			return Element0;
		}
	}
}
public struct VisualAuraProperty
{
	public UInt32 SpellID;
	public UInt8 EffectIdx;
	public Int64 EndTime;
}
public static class VisualAuraPropertySerialize
{
	public static void Append(this ViOStream OS, VisualAuraProperty value)
	{
		OS.Append(value.SpellID);
		OS.Append(value.EffectIdx);
		OS.Append(value.EndTime);
	}
	public static void Append(this ViOStream OS, List<VisualAuraProperty> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (VisualAuraProperty value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out VisualAuraProperty value)
	{
		IS.Read(out value.SpellID);
		IS.Read(out value.EffectIdx);
		IS.Read(out value.EndTime);
	}
	public static void Read(this ViIStream IS, out List<VisualAuraProperty> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<VisualAuraProperty>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			VisualAuraProperty value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static bool Read(this ViStringIStream IS, out VisualAuraProperty value)
	{
		value = default(VisualAuraProperty);
		if(IS.Read(out value.SpellID) == false){return false;}
		if(IS.Read(out value.EffectIdx) == false){return false;}
		if(IS.Read(out value.EndTime) == false){return false;}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<VisualAuraProperty> list)
	{
		list = null;
		ViArrayIdx size;
		if(IS.Read(out size) == false){return false;}
		list = new List<VisualAuraProperty>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			VisualAuraProperty value;
			if(IS.Read(out value) == false){return false;}
			list.Add(value);
		}
		return true;
	}
}
public class ReceiveDataVisualAuraProperty: ViReceiveDataNode
{
	public static readonly new int SIZE = 3;
	public ViReceiveDataUInt32 SpellID = new ViReceiveDataUInt32();
	public ViReceiveDataUInt8 EffectIdx = new ViReceiveDataUInt8();
	public ViReceiveDataInt64 EndTime = new ViReceiveDataInt64();
	public override void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		SpellID.Start(channel, IS, entity);
		SpellID.Parent = this;
		EffectIdx.Start(channel, IS, entity);
		EffectIdx.Parent = this;
		EndTime.Start(channel, IS, entity);
		EndTime.Parent = this;
	}
	public override void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		SpellID.Start(channelMask, IS, entity);
		SpellID.Parent = this;
		EffectIdx.Start(channelMask, IS, entity);
		EffectIdx.Parent = this;
		EndTime.Start(channelMask, IS, entity);
		EndTime.Parent = this;
	}
	public override void End(ViEntity entity)
	{
		SpellID.End(entity);
		EffectIdx.End(entity);
		EndTime.End(entity);
		base.End(entity);
	}
	public override void Clear()
	{
		SpellID.Clear();
		EffectIdx.Clear();
		EndTime.Clear();
		base.Clear();
	}
	public new void RegisterTo(UInt16 channelMask, ViReceiveProperty property)
	{
		SpellID.RegisterTo(channelMask, property);
		EffectIdx.RegisterTo(channelMask, property);
		EndTime.RegisterTo(channelMask, property);
	}
	public override void StartByArray()
	{
		RegisterTo(ViConstValueDefine.MAX_UINT16, this);
	}
	public static implicit operator VisualAuraProperty(ReceiveDataVisualAuraProperty data)
	{
		VisualAuraProperty value = new VisualAuraProperty();
		value.SpellID = data.SpellID;
		value.EffectIdx = data.EffectIdx;
		value.EndTime = data.EndTime;
		return value;
	}
}
public struct LogicAuraProperty
{
	public UInt32 SpellID;
	public UInt8 EffectIdx;
	public Int64 EndTime;
	public LogicAuraCastorValueArray CastorValue;
	public LogicAuraValueArray Value;
}
public static class LogicAuraPropertySerialize
{
	public static void Append(this ViOStream OS, LogicAuraProperty value)
	{
		OS.Append(value.SpellID);
		OS.Append(value.EffectIdx);
		OS.Append(value.EndTime);
		OS.Append(value.CastorValue);
		OS.Append(value.Value);
	}
	public static void Append(this ViOStream OS, List<LogicAuraProperty> list)
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (LogicAuraProperty value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read(this ViIStream IS, out LogicAuraProperty value)
	{
		IS.Read(out value.SpellID);
		IS.Read(out value.EffectIdx);
		IS.Read(out value.EndTime);
		IS.Read(out value.CastorValue);
		IS.Read(out value.Value);
	}
	public static void Read(this ViIStream IS, out List<LogicAuraProperty> list)
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<LogicAuraProperty>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraProperty value;
			IS.Read(out value);
			list.Add(value);
		}
	}
	public static bool Read(this ViStringIStream IS, out LogicAuraProperty value)
	{
		value = default(LogicAuraProperty);
		if(IS.Read(out value.SpellID) == false){return false;}
		if(IS.Read(out value.EffectIdx) == false){return false;}
		if(IS.Read(out value.EndTime) == false){return false;}
		if(IS.Read(out value.CastorValue) == false){return false;}
		if(IS.Read(out value.Value) == false){return false;}
		return true;
	}
	public static bool Read(this ViStringIStream IS, out List<LogicAuraProperty> list)
	{
		list = null;
		ViArrayIdx size;
		if(IS.Read(out size) == false){return false;}
		list = new List<LogicAuraProperty>((int)size);
		for (ViArrayIdx idx = 0; idx < size; ++idx )
		{
			LogicAuraProperty value;
			if(IS.Read(out value) == false){return false;}
			list.Add(value);
		}
		return true;
	}
}
public class ReceiveDataLogicAuraProperty: ViReceiveDataNode
{
	public static readonly new int SIZE = 11;
	public ViReceiveDataUInt32 SpellID = new ViReceiveDataUInt32();
	public ViReceiveDataUInt8 EffectIdx = new ViReceiveDataUInt8();
	public ViReceiveDataInt64 EndTime = new ViReceiveDataInt64();
	public ReceiveDataLogicAuraCastorValueArray CastorValue = new ReceiveDataLogicAuraCastorValueArray();
	public ReceiveDataLogicAuraValueArray Value = new ReceiveDataLogicAuraValueArray();
	public override void Start(UInt8 channel, ViIStream IS, ViEntity entity)
	{
		SpellID.Start(channel, IS, entity);
		SpellID.Parent = this;
		EffectIdx.Start(channel, IS, entity);
		EffectIdx.Parent = this;
		EndTime.Start(channel, IS, entity);
		EndTime.Parent = this;
		CastorValue.Start(channel, IS, entity);
		CastorValue.Parent = this;
		Value.Start(channel, IS, entity);
		Value.Parent = this;
	}
	public override void Start(UInt16 channelMask, ViIStream IS, ViEntity entity)
	{
		SpellID.Start(channelMask, IS, entity);
		SpellID.Parent = this;
		EffectIdx.Start(channelMask, IS, entity);
		EffectIdx.Parent = this;
		EndTime.Start(channelMask, IS, entity);
		EndTime.Parent = this;
		CastorValue.Start(channelMask, IS, entity);
		CastorValue.Parent = this;
		Value.Start(channelMask, IS, entity);
		Value.Parent = this;
	}
	public override void End(ViEntity entity)
	{
		SpellID.End(entity);
		EffectIdx.End(entity);
		EndTime.End(entity);
		CastorValue.End(entity);
		Value.End(entity);
		base.End(entity);
	}
	public override void Clear()
	{
		SpellID.Clear();
		EffectIdx.Clear();
		EndTime.Clear();
		CastorValue.Clear();
		Value.Clear();
		base.Clear();
	}
	public new void RegisterTo(UInt16 channelMask, ViReceiveProperty property)
	{
		SpellID.RegisterTo(channelMask, property);
		EffectIdx.RegisterTo(channelMask, property);
		EndTime.RegisterTo(channelMask, property);
		CastorValue.RegisterTo(channelMask, property);
		Value.RegisterTo(channelMask, property);
	}
	public override void StartByArray()
	{
		RegisterTo(ViConstValueDefine.MAX_UINT16, this);
	}
	public static implicit operator LogicAuraProperty(ReceiveDataLogicAuraProperty data)
	{
		LogicAuraProperty value = new LogicAuraProperty();
		value.SpellID = data.SpellID;
		value.EffectIdx = data.EffectIdx;
		value.EndTime = data.EndTime;
		value.CastorValue = data.CastorValue;
		value.Value = data.Value;
		return value;
	}
}

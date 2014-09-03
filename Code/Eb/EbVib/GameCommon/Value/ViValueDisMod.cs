using System;


public abstract class ViValueDisMod : ViRefNode1<ViValueDisMod>
{
	public static Int32 Mod(Int32 iDamage, ViRefList1<ViValueDisMod> kList)
	{
		Int32 iReserveDamage = iDamage;
		kList.BeginIterator();
		while (!kList.IsEnd())
		{
			ViValueDisMod kModifier = kList.CurrentNode as ViValueDisMod;
			kList.Next();
			ViDebuger.AssertError(kModifier);
			iReserveDamage = kModifier.Mod(iReserveDamage);
			if (iReserveDamage == 0)
			{
				kList.EndIterator();
				break;
			}
		}
		return iReserveDamage;
	}
	//
	public ViEventList EndCallback { get { return _endCallback; } }
	public abstract Int32 Mod(Int32 iDmg);
	//
	protected void _OnEnd()
	{
		base.Detach();
		_endCallback.Invoke(0);
	}
	//
	ViEventList _endCallback = new ViEventList();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViValueDisModEx_0 : ViValueDisMod
{
	public Int32 DisValue { get { return _disValue; } }
	//
	public override Int32 Mod(Int32 iDmg)
	{
		if (iDmg < _disValue)
		{
			_disValue -= iDmg;
			return 0;
		}
		else
		{
			Int32 iReserveDmg = iDmg - _disValue;
			_disValue = 0;
			_OnEnd();
			return iReserveDmg;
		}
	}
	public void SetDisValue(Int32 iValue)
	{
		_disValue = iValue;
	}
	//
	Int32 _disValue;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class Demo_ValueDisMod
{
#pragma warning disable 0219
	public static void Test()
	{
		ViValueDisModEx_0 disMod0 = new ViValueDisModEx_0();
		disMod0.SetDisValue(100);
		ViValueDisModEx_0 disMod1 = new ViValueDisModEx_0();
		disMod1.SetDisValue(100);
		ViRefList1<ViValueDisMod> disModList = new ViRefList1<ViValueDisMod>();
		disModList.PushBack(disMod0);
		disModList.PushBack(disMod1);
		Int32 iReserve = 0;
		iReserve = ViValueDisMod.Mod(45, disModList);
		iReserve = ViValueDisMod.Mod(45, disModList);
		iReserve = ViValueDisMod.Mod(45, disModList);
		iReserve = ViValueDisMod.Mod(45, disModList);
		iReserve = ViValueDisMod.Mod(45, disModList);
	}
#pragma warning restore 0219
}
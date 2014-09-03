using System;


//+------------------------------------------------------------------------------------------------------------------------------------------------
public class ViValueInherit : ViRefNode1<ViValueInherit>
{
	public void Update(Int32 oldValue, Int32 newValue)
	{
		if (IsAttach())
		{
			Int32 oldInheritValue = _inheritValue;
			_srcValue = newValue;
			_inheritValue = (Int32)(_perc * _srcValue);
			_OnUpdated(oldInheritValue, _inheritValue);
		}
	}
	public void Init(float fPerc, Int32 srcValue, bool bUpdate)
	{
		if (IsAttach())
		{
			return;
		}
		_srcValue = srcValue;
		_perc = fPerc;
		_inheritValue = (Int32)(_perc * _srcValue);
		_OnAttach(bUpdate);
	}
	public void SetPerc(float fPerc)
	{
		_perc = fPerc;
		if (IsAttach())
		{
			_OnDetach(true);
			_inheritValue = (Int32)(_perc * _srcValue);
			_OnAttach(true);
		}
	}
	public void Detach(bool bUpdate)
	{
		if (IsAttach())
		{
			base.Detach();
			_OnDetach(bUpdate);
			_perc = 0.0f;
		}
	}
	//
	public Int32 Value { get { return _inheritValue; } }
	//
	public virtual void _OnUpdated(Int32 oldInheritValue, Int32 newInheritValue) { }
	public virtual void _OnAttach(bool bUpdate) { }
	public virtual void _OnDetach(bool bUpdate) { }
	//
	private Int32 _inheritValue;
	private float _perc;
	private Int32 _srcValue;
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViValueInheritList
{
	public void OnUpdate(Int32 kOld, Int32 kNew)
	{
		_list.EndIterator();
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViValueInherit pkInherit = _list.CurrentNode as ViValueInherit;
			_list.Next();
			ViDebuger.AssertError(pkInherit);
			pkInherit.Update(kOld, kNew);
		}
	}
	public void AttachBack(ViValueInherit kNode)
	{
		if (!kNode.IsAttach())
		{
			_list.PushBack(kNode);
		}
	}
	public void DetachAll(bool bUpdate)
	{
		_list.EndIterator();
		_list.BeginIterator();
		while (!_list.IsEnd())
		{
			ViValueInherit pkInherit = _list.CurrentNode as ViValueInherit;
			_list.Next();
			ViDebuger.AssertError(pkInherit);
			pkInherit.Detach(bUpdate);
		}
	}
	//
	private ViRefList1<ViValueInherit> _list = new ViRefList1<ViValueInherit>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class Demo_ValueInherit
{
#pragma warning disable 0219
	static void Test()
	{
		Int32 srcValue = 100;
		ViValueInheritList kList = new ViValueInheritList();
		ViValueInherit kValueInherit = new ViValueInherit();
		kValueInherit.Init(1.0f, srcValue, true);
		kList.AttachBack(kValueInherit);
		kList.OnUpdate(200, srcValue);
		srcValue = 200;
		kList.OnUpdate(300, srcValue);
		srcValue = 300;
		kValueInherit.Detach(true);
	}
#pragma warning restore 0219
}
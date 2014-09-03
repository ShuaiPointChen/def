using System;

public class ViStateConditionCallback
{
	public static void Update(ViGameUnit entity, ViRefList2<ViStateConditionCallback> list)
	{
		list.BeginIterator();
		while (!list.IsEnd())
		{
			ViStateConditionCallback obj = list.CurrentNode.Data;
			list.Next();
			ViDebuger.AssertError(obj);
			obj._Update(entity);
		}
	}

	public delegate void Callback();

	public Callback DeleMatch;
	public Callback DeleUnMatch;
	public bool IsMatch { get { return _isMatch; } }
	public ViStateConditionStruct Condition { get { return _condition; } }
	public ViRefNode2<ViStateConditionCallback> AttachNode { get { return _attachNode; } }

	public void Start(ViGameUnit entity, ViStateConditionStruct condition)
	{
		_condition = condition;
		_isMatch = entity.IsMatch(_condition);
		_attachNode.Data = this;
	}
	public void Notify()
	{
		if (IsMatch)
		{
			if (DeleMatch != null) { DeleMatch(); }
		}
		else
		{
			if (DeleUnMatch != null) { DeleUnMatch(); }
		}
	}
	public void End()
	{
		_attachNode.Data = null;
		_attachNode.Detach();
	}


	void _Update(ViGameUnit entity)
	{
		if (IsMatch)
		{
			if (!entity.IsMatch(_condition))
			{
				_isMatch = false;
				if (DeleUnMatch != null) { DeleUnMatch(); }
			}
		}
		else
		{
			if (entity.IsMatch(_condition))
			{
				_isMatch = true;
				if (DeleMatch != null) { DeleMatch(); }
			}
		}
	}

	ViRefNode2<ViStateConditionCallback> _attachNode = new ViRefNode2<ViStateConditionCallback>();

	ViStateConditionStruct _condition;
	bool _isMatch;
}

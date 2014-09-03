using System;



public class ViRefObj
{
	public void _ClearReference()
	{
		_List.ClearAndClearContent();
	}
	public void _AddReference(ViDoubleLinkNode2<ViRefObj> node)
	{
		_List.PushBack(node);
	}
	ViDoubleLink2<ViRefObj> _List = new ViDoubleLink2<ViRefObj>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViRefPtr<T> : ViDoubleLinkNode2<ViRefObj>
	where T : ViRefObj
{
	public T Obj
	{
		get { return base.Data as T; }
		set { _SetObj(value); }
	}
	//
	public ViRefPtr()
	{

	}
	public ViRefPtr(T obj)
	{
		if (obj != null)
		{
			base.Data = obj;
			obj._AddReference(this);
		}
	}
	public ViRefPtr(ViRefPtr<T> ptr)
	{
		_SetObj(ptr.Obj);
	}
	public void Clear()
	{
		if (base.Data != null)
		{
			base.Detach();
			base.Data = null;
		}
	}
	private void _SetObj(T obj)
	{
		if (base.Data == obj)
		{
			return;
		}
		if (base.Data != null)
		{
			base.Detach();
			base.Data = null;
		}
		if (obj != null)
		{
			base.Data = obj;
			obj._AddReference(this);
		}
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class ViDynamicRefPtr : ViDoubleLinkNode2<ViRefObj>
{
	public ViDynamicRefPtr()
	{

	}
	public ViDynamicRefPtr(ViRefObj obj)
	{
		if (obj != null)
		{
			base.Data = obj;
			obj._AddReference(this);
		}
	}
	public ViDynamicRefPtr(ViDynamicRefPtr ptr)
	{
		_SetObj(ptr.Obj);
	}
	public ViRefObj Obj
	{
		get { return base.Data; }
		set { _SetObj(value); }
	}
	public void Clear()
	{
		if (base.Data != null)
		{
			base.Detach();
			base.Data = null;
		}
	}
	private void _SetObj(ViRefObj obj)
	{
		if (base.Data == obj)
		{
			return;
		}
		if (base.Data != null)
		{
			base.Detach();
			base.Data = null;
		}
		if (obj != null)
		{
			base.Data = obj;
			obj._AddReference(this);
		}
	}
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------

public class Demo_RefPtr
{
#pragma warning disable 0219
	class Entity : ViRefObj
	{

	}
	public static void Test()
	{
		Entity obj = new Entity();
		ViRefPtr<Entity> ptrObj = new ViRefPtr<Entity>();
		ptrObj.Obj = obj;
		obj._ClearReference();
		obj = null;
	}
#pragma warning restore 0219
}
using System;
using System.Collections.Generic;



public abstract class ViAvatarDurationVisualInterface<TAvatar>
{
	public abstract void OnActive(TAvatar avatar);
	public abstract void OnDeactive(TAvatar avatar);
	public abstract void OnStart(TAvatar avatar);
	public abstract void OnEnd(TAvatar avatar);
	//
	public ViAvatarDurationVisualInterface()
	{
		_priorityNode.Data = this;
		_attachNode.Data = this;
	}
	public void Init(ViAvatarDurationVisualStruct info, UInt32 weight)
	{
		_info = info;
		_weight = weight;
	}
	public bool IsAttach()
	{
		return _priorityNode.IsAttach();
	}
	public Int32 Type { get { return _info.type; } }
	public UInt32 Weight { get { return _weight; } }
	public ViAvatarDurationVisualStruct Info { get { return _info; } }
	//
	UInt32 _weight;
	ViAvatarDurationVisualStruct _info;
	internal ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> _priorityNode = new ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>>();
	internal ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> _attachNode = new ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>>();
}

public class ViAvatarDurationVisualController<TAvatar>
{
	public void Active(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> kEffect)
	{
		if (m_kPriorityList.IsEmpty())
		{
			m_kPriorityList.PushBack(kEffect._priorityNode);
			kEffect.OnActive(avatar);
			_OnUpdated(avatar, null);
		}
		else
		{
			ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> iter = m_kPriorityList.GetHead();
			ViAvatarDurationVisualInterface<TAvatar> pkOldTop = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(pkOldTop);
			while (!m_kPriorityList.IsEnd(iter))
			{
				ViAvatarDurationVisualInterface<TAvatar> pkEffect = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
				ViDebuger.AssertError(pkEffect);
				if (kEffect.Weight > pkEffect.Weight)
				{
					break;
				}
				ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.Next(ref iter);
			}
			ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.PushBefore(iter, kEffect._priorityNode);
			if (kEffect._priorityNode == m_kPriorityList.GetHead())
			{
				pkOldTop.OnDeactive(avatar);
				kEffect.OnActive(avatar);
				_OnUpdated(avatar, pkOldTop);
			}
		}
	}
	public void Deactive(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> kEffect)
	{
		if (kEffect._priorityNode.IsAttach() == false)
		{
			return;
		}
		if (m_kPriorityList.IsEmpty())
		{
			return;
		}
		if (kEffect._priorityNode == m_kPriorityList.GetHead())
		{
			kEffect._priorityNode.Detach();
			kEffect.OnDeactive(avatar);
			if (!m_kPriorityList.IsEmpty())
			{
				ViAvatarDurationVisualInterface<TAvatar> pNewTop = m_kPriorityList.GetHead().Data as ViAvatarDurationVisualInterface<TAvatar>;
				ViDebuger.AssertError(pNewTop);
				pNewTop.OnActive(avatar);
			}
			_OnUpdated(avatar, kEffect);
		}
		else
		{
			kEffect._priorityNode.Detach();
		}

	}
	public void Fresh(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> visual)
	{
		_OnUpdated(avatar, null);
	}

	public ViAvatarDurationVisualInterface<TAvatar> GetTop()
	{
		if (m_kPriorityList.IsEmpty())
		{
			return null;
		}
		else
		{
			ViAvatarDurationVisualInterface<TAvatar> top = m_kPriorityList.GetHead().Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(top);
			return top;
		}
	}
	public void Clear(TAvatar avatar)
	{
		if (m_kPriorityList.IsEmpty())
		{
			return;
		}
		ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> iter = m_kPriorityList.GetHead();
		ViAvatarDurationVisualInterface<TAvatar> top = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
		ViDebuger.AssertError(top);
		top.OnDeactive(avatar);
		while (!m_kPriorityList.IsEnd(iter))
		{
			ViAvatarDurationVisualInterface<TAvatar> pkEffect = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(pkEffect);
			ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.Next(ref iter);
			pkEffect._priorityNode.Detach();
		}
	}

	private void _OnUpdated(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> oldVisual)
	{
		ViAvatarDurationVisualInterface<TAvatar> pkNewTop = GetTop();
		ViAvatarDurationVisualInterface<TAvatar> pkEffect = (oldVisual != null ? oldVisual : pkNewTop);
		if (pkEffect != null)
		{
			//    UInt32 uiType = pkEffect->Type();
			//    FuncUpdate pFunc =  ViVisualDurationEffectCreator::FuncUpdateList<TAvatar>().Get(uiType);
			//    if(pFunc)
			//    {
			//        (*pFunc)(kAvatar, pkOldTop, pkNewTop);
			//    }
		}
	}
	ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>> m_kPriorityList = new ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>();
}

public class ViAvatarDurationVisualControllers<TAvatar>
{
	public ViAvatarDurationVisualController<TAvatar> GetEffectController(UInt32 type)
	{
		ViAvatarDurationVisualController<TAvatar> controller = null;
		if (m_kEffectControllerList.TryGetValue(type, out controller))
		{
			return controller;
		}
		else
		{
			controller = new ViAvatarDurationVisualController<TAvatar>();
			m_kEffectControllerList[type] = controller;
			return controller;
		}
	}
	public void Clear(TAvatar avatar)
	{
		foreach (KeyValuePair<UInt32, ViAvatarDurationVisualController<TAvatar>> pair in m_kEffectControllerList)
		{
			pair.Value.Clear(avatar);
		}
		m_kEffectControllerList.Clear();
	}
	Dictionary<UInt32, ViAvatarDurationVisualController<TAvatar>> m_kEffectControllerList = new Dictionary<UInt32, ViAvatarDurationVisualController<TAvatar>>();
}

public class ViAvatarDurationVisualOwnList<TAvatar>
{
	public delegate ViAvatarDurationVisualController<TAvatar> Dele_GetEffectController(TAvatar avatar, Int32 type);
	public delegate ViAvatarDurationVisualInterface<TAvatar> Dele_CerateDurationVisual(Int32 type);

	public static Dele_GetEffectController _deleGetEffectController;
	public static Dele_CerateDurationVisual _deleCerateDurationVisual;

	public void Add(TAvatar avatar, ViAvatarDurationVisualStruct kInfo, UInt32 weight)
	{
		ViDebuger.AssertError(_deleCerateDurationVisual);
		ViAvatarDurationVisualInterface<TAvatar> pkEffect = _deleCerateDurationVisual(kInfo.type);
		if (pkEffect != null)
		{
			m_kEffectList.PushBack(pkEffect._attachNode);
			pkEffect.Init(kInfo, weight);
			pkEffect.OnStart(avatar);
			_Active(avatar, pkEffect);
		}
	}
	public void Clear(TAvatar avatar)
	{
		ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> iter = m_kEffectList.GetHead();
		while (!m_kEffectList.IsEnd(iter))
		{
			ViAvatarDurationVisualInterface<TAvatar> pkEffect = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(pkEffect);
			ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.Next(ref iter);
			_Deactive(avatar, pkEffect);
			pkEffect.OnEnd(avatar);
			pkEffect._attachNode.Detach();
		}
	}
	public void Load(ViAvatarDurationVisualStruct kInfo, UInt32 weight)
	{
		ViDebuger.AssertError(_deleCerateDurationVisual);
		ViAvatarDurationVisualInterface<TAvatar> pkEffect = _deleCerateDurationVisual(kInfo.type);
		if (pkEffect != null)
		{
			m_kEffectList.PushBack(pkEffect._attachNode);
			pkEffect.Init(kInfo, weight);
		}
	}
	public void Active(TAvatar avatar)
	{
		ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> iter = m_kEffectList.GetHead();
		while (!m_kEffectList.IsEnd(iter))
		{
			ViAvatarDurationVisualInterface<TAvatar> pkEffect = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(pkEffect);
			ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.Next(ref iter);
			_Active(avatar, pkEffect);
		}
	}
	public void Deactive(TAvatar avatar)
	{
		ViDoubleLinkNode2<ViAvatarDurationVisualInterface<TAvatar>> iter = m_kEffectList.GetHead();
		while (!m_kEffectList.IsEnd(iter))
		{
			ViAvatarDurationVisualInterface<TAvatar> pkEffect = iter.Data as ViAvatarDurationVisualInterface<TAvatar>;
			ViDebuger.AssertError(pkEffect);
			ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>.Next(ref iter);
			_Deactive(avatar, pkEffect);
		}
	}
	void _Active(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> kEffect)
	{
		ViDebuger.AssertError(_deleGetEffectController);
		ViAvatarDurationVisualController<TAvatar> pkController = _deleGetEffectController(avatar, kEffect.Type);
		if (pkController != null)
		{
			pkController.Active(avatar, kEffect);
		}
	}
	void _Deactive(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> kEffect)
	{
		ViDebuger.AssertError(_deleGetEffectController);
		ViAvatarDurationVisualController<TAvatar> pkController = _deleGetEffectController(avatar, kEffect.Type);
		if (pkController != null)
		{
			pkController.Deactive(avatar, kEffect);
		}
	}
	void _Fresh(TAvatar avatar, ViAvatarDurationVisualInterface<TAvatar> kEffect)
	{
		ViDebuger.AssertError(_deleGetEffectController);
		ViAvatarDurationVisualController<TAvatar> pkController = _deleGetEffectController(avatar, kEffect.Type);
		if (pkController != null)
		{
			pkController.Fresh(avatar, kEffect);
		}
	}

	ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>> m_kEffectList = new ViDoubleLink2<ViAvatarDurationVisualInterface<TAvatar>>();
}
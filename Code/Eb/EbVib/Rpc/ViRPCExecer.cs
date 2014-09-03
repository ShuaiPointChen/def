using System;
using System.Collections.Generic;

using Int8 = System.SByte;
using UInt8 = System.Byte;
using ViEntityID = System.UInt64;
using ViEntityTypeID = System.Byte;

public class ViRPCExecer
{
	public virtual ViEntityID ID() { return 0; }
	public virtual ViRPCEntity Entity { get { return null; } }

	public virtual void End(ViEntityManager entityManager) { }
	public virtual void Start(ViEntityID ID, ViEntityManager entityManager, UInt16 channelMask, ViIStream IS) { }
	public virtual void OnPropertyUpdateStart(UInt8 channel) { }
	public virtual void OnPropertyUpdate(UInt8 channel, ViIStream IS) { }
	public virtual void OnPropertyUpdateEnd(UInt8 channel) { }
	public virtual void OnMessage(UInt16 funcIdx, ViIStream IS) { }
}

public class ViEntityType
{
	static public ViEntityTypeID Type(ViEntityID ID)
	{
		return (ViEntityTypeID)(ID >> 56);
	}
	public static void Register(ViEntityTypeID typeID, CreateRPCExecer creator)
	{
		ViEntityType type = new ViEntityType();
		type._creator = creator;
		ViEntityCreator.RegisterCreator(typeID, type);
	}

	public delegate ViRPCExecer CreateRPCExecer();

	public ViRPCExecer Create()
	{
		ViDebuger.AssertError(_creator);
		return _creator();
	}

	CreateRPCExecer _creator;

}

public static class ViEntityCreator
{
	public static void RegisterCreator(ViEntityTypeID typeID, ViEntityType type)
	{
		_execerCreatorList[typeID] = type;
	}
	public static Dictionary<ViEntityTypeID, ViEntityType> List { get { return _execerCreatorList; } }

	static Dictionary<ViEntityTypeID, ViEntityType> _execerCreatorList = new Dictionary<ViEntityTypeID, ViEntityType>();
}


public enum ViRPCMessage
{
	INF = 60000,
	CONNECT_START,
	CONNECT_END,
	EXEC_ACK,
	EXEC_RESULT,
	EXEC_EXCEPTION,
	LOGIN,
	LOGIN_RESULT,
	SUP,
}

//public enum ViRPCEntityChannel
//{
//    DB,
//    OWN_CLIENT,
//    ALL_CLIENT,
//}

public enum ViRPCEntityMessage
{
	INF = ViRPCMessage.SUP,
	GAME_START = INF,
	GAME_TIME_UPDATE,
	CREATE_SELF,
	ENTITY_EMERGE,
	ENTITY_VANISH,
	ENTITY_LIST_EMERGE,
	ENTITY_LIST_VANISH,
	ENTITY_PROPERTY_UPDATE_CHANNEL_INF,
	ENTITY_PROPERTY_UPDATE_CHANNEL_SUP = ENTITY_PROPERTY_UPDATE_CHANNEL_INF + 15,
	ENTITY_GM,
	ENTITY_MESSAGE,
	ENTITY_DB_REQUEST_LOAD_ID,
	ENTITY_DB_REQUEST_LOAD_NAME,
	ENTITY_DB_RESULT_LOAD_SUCESS,
	ENTITY_DB_RESULT_LOAD_EXCEPTION,
	ENTITY_DB_REQUEST_LOAD_CHANNEL_ID,
	ENTITY_DB_REQUEST_LOAD_CHANNEL_NAME,
	ENTITY_DB_RESULT_LOAD_CHANNEL_SUCESS,
	ENTITY_DB_RESULT_LOAD_CHANNEL_EXCEPTION,
	ENTITY_DB_REQUEST_FIND_BY_ID,
	ENTITY_DB_REQUEST_FIND_BY_NAME,
	ENTITY_DB_RESULT_FIND_TRUE,
	ENTITY_DB_RESULT_FIND_FALSE,
	ENTITY_DB_REQUEST_ADD,
	ENTITY_DB_RESULT_ADD_SUCCESS,
	ENTITY_DB_RESULT_ADD_EXCEPTION,
	ENTITY_DB_REQUEST_ADD_NO_ESULT,
	ENTITY_DB_EXEC_SAVE,
	ENTITY_DB_EXEC_SAVE_CHANNEL,
	ENTITY_DB_PROPERTY_START,
	ENTITY_DB_PROPERTY_UPDATE,
	ENTITY_DB_PROPERTY_END,
	ENTITY_DB_REQUEST_LOAD_NAME_AND_ID,
	ENTITY_DB_RESULT_LOAD_NAME_AND_ID,
}

public class ViRPCExecerManager
{
	public delegate void OnSystemMessage(UInt16 funcID, ViIStream IS);
	public delegate void DeleLoginResult(UInt8 result);
	public delegate void OnSelfCreated(ViRPCExecer execer);
	public delegate void OnSelfMessage(UInt16 msgID, UInt64 types, ViIStream IS);
	public delegate void OnEntityEnter(ViRPCExecer execer);
	public delegate void OnEntityLeave(ViRPCExecer execer);
	public delegate void DeleGameStart(Int64 time1970, Int64 timeAccumulate);
	public delegate void DeleGameTime(Int64 timeAccumulate);
	public OnSystemMessage SystemMessageExecer;
	public DeleLoginResult OnLoginResult;
	public OnSelfCreated OnSelfCreatedExecer;
	public OnSelfMessage OnSelfMessageExecer;
	public OnEntityEnter OnEntityEnterExecer;
	public OnEntityLeave OnEntityLeaveExecer;
	public DeleGameStart OnGameStartExecer;
	public DeleGameTime OnGameTimeExecer;

	public static readonly UInt16 SYSTEM_MSG_INF = 60000;
	public UInt16 SELF_PROPERTY_MASK = 0;
	public UInt16 OTHER_PROPERTY_MASK = 0;

	public ViEntityManager EntityManager { get { return _entityManager; } }
	public bool OtherEntityShow { get { return _otherEntityShow; } }

	public ViRPCExecerManager(bool otherEntityShow)
	{
		_otherEntityShow = otherEntityShow;
	}

	public void OnMessage(UInt16 flag, ViIStream IS)
	{
		UInt16 funcID;
		IS.Read(out funcID);

		if (funcID >= SYSTEM_MSG_INF)
		{
			if (ViRPCEntityMessage.ENTITY_PROPERTY_UPDATE_CHANNEL_INF <= (ViRPCEntityMessage)funcID && (ViRPCEntityMessage)funcID <= ViRPCEntityMessage.ENTITY_PROPERTY_UPDATE_CHANNEL_SUP)
			{
				UInt8 channel = (UInt8)(funcID - (UInt16)ViRPCEntityMessage.ENTITY_PROPERTY_UPDATE_CHANNEL_INF);
				OnEntityPropertyUpdate(channel, IS);
				return;
			}
			switch (funcID)
			{
				case (UInt16)ViRPCEntityMessage.CREATE_SELF:
					OnSelfEntity(funcID, IS);
					break;
				case (UInt16)ViRPCEntityMessage.ENTITY_EMERGE:
					OnEntityEmerge(IS);
					break;
				case (UInt16)ViRPCEntityMessage.ENTITY_VANISH:
					OnEntityVanish(IS);
					break;
				case (UInt16)ViRPCEntityMessage.ENTITY_LIST_EMERGE:
					OnEntityListEmerge(IS);
					break;
				case (UInt16)ViRPCEntityMessage.ENTITY_LIST_VANISH:
					OnEntityListVanish(IS);
					break;
				case (UInt16)ViRPCEntityMessage.ENTITY_MESSAGE:
					OnEntityMessage(IS);
					break;
				case (UInt16)ViRPCEntityMessage.GAME_START:
					OnGameStart(IS);
					break;
				case (UInt16)ViRPCEntityMessage.GAME_TIME_UPDATE:
					OnGameTime(IS);
					break;
				case (UInt16)ViRPCMessage.EXEC_ACK:
					OnExecACK(IS);
					break;
				case (UInt16)ViRPCMessage.EXEC_RESULT:

					break;
				case (UInt16)ViRPCMessage.LOGIN_RESULT:
					_OnLoginResult(IS);
					break;
				default:
					ViDebuger.AssertError(SystemMessageExecer != null);
					SystemMessageExecer(funcID, IS);
					break;
			}
		}
		else
		{
			ViEntityID entityID;
			IS.Read(out entityID);
			OnEntityExec(entityID, funcID, IS);
		}
	}
	public void Add(ViRPCExecer execer)
	{
		ViDebuger.AssertError(execer);
		ViDebuger.AssertError(_execerList.ContainsKey(execer.ID()));
		_execerList[execer.ID()] = execer;
	}
	public void Del(ViRPCExecer execer)
	{
		_execerList.Remove(execer.ID());
	}

	public void Start(ViNet tcp)
	{
		_net = tcp;
		_entityManager.Start();
	}
	public void End()
	{
		_net = null;
		foreach (KeyValuePair<ViEntityID, ViRPCExecer> pair in _execerList)
		{
			pair.Value.End(_entityManager);
		}
		_execerList.Clear();
		_entityManager.End();
	}
	public void OnEntityExec(ViEntityID entityID, UInt16 funcID, ViIStream IS)
	{
		ViRPCExecer execer;
		if (_execerList.TryGetValue(entityID, out execer))
		{
			ViEntitySerialize.EntityNameger = _entityManager;
			execer.OnMessage(funcID, IS);
		}
		else
		{
			if (OtherEntityShow)
			{
				ViDebuger.Warning("ViRPCExecerManager发现无效Execer");
			}
		}
	}

	public void OnEntityListEmerge(ViIStream IS)
	{
		if (!OtherEntityShow)
		{
			return;
		}
		List<ViEntityID> entities;
		IS.Read(out entities);
		foreach (ViEntityID entityID in entities)
		{
			ViEntityTypeID typeID = ViEntityType.Type(entityID);
			ViEntityType type;
			if (ViEntityCreator.List.TryGetValue(typeID, out type))
			{
				ViRPCExecer execer = type.Create();
				_execerList[entityID] = execer;
				UInt16 channelMask = OTHER_PROPERTY_MASK;
				execer.Start(entityID, EntityManager, channelMask, IS);
				execer.Entity.RPC.Net = _net;
				if (OnEntityEnterExecer != null)
				{
					OnEntityEnterExecer(execer);
				}
			}
		}
	}

	public void OnEntityEmerge(ViIStream IS)
	{
		if (!OtherEntityShow)
		{
			return;
		}
		ViEntityID entityID;
		IS.Read(out entityID);
		ViEntityTypeID typeID = ViEntityType.Type(entityID);
		ViEntityType type;
		if (ViEntityCreator.List.TryGetValue(typeID, out type))
		{
			ViRPCExecer execer = type.Create();
			_execerList[entityID] = execer;
			UInt16 channelMask = OTHER_PROPERTY_MASK;
			execer.Start(entityID, EntityManager, channelMask, IS);
			execer.Entity.RPC.Net = _net;
			if (OnEntityEnterExecer != null)
			{
				OnEntityEnterExecer(execer);
			}
		}
	}

	public void OnEntityListVanish(ViIStream IS)
	{
		if (!OtherEntityShow)
		{
			return;
		}
		List<ViEntityID> entities;
		IS.Read(out entities);
		foreach (ViEntityID entityID in entities)
		{
			ViRPCExecer execer;
			if (_execerList.TryGetValue(entityID, out execer))
			{
				if (OnEntityLeaveExecer != null)
				{
					OnEntityLeaveExecer(execer);
				}
				execer.End(EntityManager);
				_execerList.Remove(entityID);
			}
		}
	}

	public void OnEntityVanish(ViIStream IS)
	{
		if (!OtherEntityShow)
		{
			return;
		}
		ViEntityID entityID;
		IS.Read(out entityID);
		ViRPCExecer execer;
		if (_execerList.TryGetValue(entityID, out execer))
		{
			if (OnEntityLeaveExecer != null)
			{
				OnEntityLeaveExecer(execer);
			}
			execer.End(EntityManager);
			_execerList.Remove(entityID);
		}
	}
	public void OnEntityPropertyUpdate(UInt8 channel, ViIStream IS)
	{
		ViEntityID entityID;
		IS.Read(out entityID);
		ViRPCExecer execer;
		if (_execerList.TryGetValue(entityID, out execer))
		{
			execer.OnPropertyUpdateStart(channel);
			while (IS.RemainLength > 0)
			{
				execer.OnPropertyUpdate(channel, IS);
			}
			execer.OnPropertyUpdateEnd(channel);
		}
	}

	public void OnSelfEntity(UInt16 funcID, ViIStream IS)
	{
		ViEntityID entityID;
		IS.Read(out entityID);
		ViEntityTypeID typeID = ViEntityType.Type(entityID);
		ViEntityType type;
		if (ViEntityCreator.List.TryGetValue(typeID, out type))
		{
			ViRPCExecer execer = type.Create();
			_execerList[entityID] = execer;
			UInt16 channelMask = SELF_PROPERTY_MASK;
			execer.Start(entityID, EntityManager, channelMask, IS);
			execer.Entity.RPC.Net = _net;
			ViDebuger.AssertError(OnSelfCreatedExecer != null);
			OnSelfCreatedExecer(execer);
		}
	}

	public void OnEntityMessage(ViIStream IS)
	{
		UInt16 msgID = 0;
		IS.Read(out msgID);
		UInt64 types = 0;
		IS.Read(out types);
		OnSelfMessageExecer(msgID, types, IS);
	}
	public void OnExecACK(ViIStream IS)
	{
		ViEntityID entityID;
		UInt16 funcID;
		IS.Read(out entityID);
		IS.Read(out funcID);
		ViRPCExecer execer;
		if (_execerList.TryGetValue(entityID, out execer))
		{
			execer.Entity.RPC.ACK.Ack(funcID);
		}
	}

	public void _OnLoginResult(ViIStream IS)
	{
		UInt8 result;
		IS.Read(out result);
		if (OnLoginResult != null)
		{
			OnLoginResult(result);
		}
	}

	public void OnGameStart(ViIStream IS)
	{
		Int64 time1970 = 0;
		Int64 timeAccumulate = 0;
		IS.Read(out time1970);
		IS.Read(out timeAccumulate);
		OnGameStartExecer(time1970, timeAccumulate);
	}

	public void OnGameTime(ViIStream IS)
	{
		Int64 timeAccumulate = 0;
		IS.Read(out timeAccumulate);
		OnGameTimeExecer(timeAccumulate);
	}


	ViNet _net;
	Dictionary<ViEntityID, ViRPCExecer> _execerList = new Dictionary<ViEntityID, ViRPCExecer>();
	ViEntityManager _entityManager = new ViEntityManager();
	bool _otherEntityShow;
}


public class ViRPCExecerManagerInstance
{
	public static ViRPCExecerManager Instance { get { return _instance; } }
	static ViRPCExecerManager _instance = new ViRPCExecerManager(true);
}
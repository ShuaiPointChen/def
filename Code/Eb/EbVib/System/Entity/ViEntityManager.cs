using System;
using System.Collections.Generic;

using ViEntityID = System.UInt64;


public class ViEntityManager
{
	public Dictionary<ViEntityID, ViEntity> Entitys { get { return _entityList; } } 
	//
	public void AddEntity(ViEntityID ID, ViEntity entity)
	{
		ViDebuger.AssertError(entity);
		ViDebuger.AssertError(!_entityList.ContainsKey(ID));
		_entityList[ID] = entity;
		entity.PreStart();
		entity.Start();
		entity.AftStart();
	}
	public void DelEntity(ViEntity entity)
	{
		entity.PreEnd();
		entity.End();
		entity.AftEnd();
		_entityList.Remove(entity.ID);
	}

	public TEntity GetEntity<TEntity>(ViEntityID ID) where TEntity : ViEntity
	{
		ViEntity entity;
		if (_entityList.TryGetValue(ID, out entity))
		{
			return (TEntity)entity;
		}
		else
		{
			return default(TEntity);
		}
	}
	public ViEntity GetEntity(ViEntityID ID)
	{
		ViEntity entity;
		if (_entityList.TryGetValue(ID, out entity))
		{
			return entity;
		}
		else
		{
			return null;
		}
	}

	public void Start()
	{

	}
	public void End()
	{
		_entityList.Clear();
	}

	Dictionary<ViEntityID, ViEntity> _entityList = new Dictionary<ViEntityID, ViEntity>();
}
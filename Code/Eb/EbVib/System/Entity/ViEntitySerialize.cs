using System;
using System.Collections.Generic;

using ViEntityID = System.UInt64;
using ViArrayIdx = System.Int32;


public static class ViEntitySerialize
{
	public static ViEntityManager EntityNameger { get { return _entityManager; } set { _entityManager = value; } }
	static ViEntityManager _entityManager;

	public static void Append(this ViOStream OS, ViEntity value)
	{
		OS.Append(value.ID);
	}
	public static void Read(this ViIStream IS, out ViEntity value)
	{
		ViEntityID ID;
		IS.Read(out ID);
		value = EntityNameger.GetEntity(ID);
	}
	public static void Append<TEntity>(ViOStream OS, TEntity value) where TEntity : ViEntity
	{
		if (value != null)
		{
			OS.Append(value.ID);
		}
		else
		{
			ViEntityID id = 0;
			OS.Append(id);
		}
	}
	public static void Append<TEntity>(ViOStream OS, List<TEntity> list) where TEntity : ViEntity
	{
		ViArrayIdx size = (ViArrayIdx)list.Count;
		OS.Append(size);
		foreach (TEntity value in list)
		{
			OS.Append(value);
		}
	}
	public static void Read<TEntity>(ViIStream IS, out TEntity value) where TEntity : ViEntity
	{
		ViEntityID ID;
		IS.Read(out ID);
		value = EntityNameger.GetEntity<TEntity>(ID);
	}
	public static void Read<TEntity>(ViIStream IS, out List<TEntity> list) where TEntity : ViEntity
	{
		ViArrayIdx size;
		IS.Read(out size);
		list = new List<TEntity>();
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TEntity value;
			Read(IS, out value);
			list.Add(value);
		}
	}
	public static bool Read<TEntity>(ViStringIStream IS, out TEntity value) where TEntity : ViEntity
	{
		value = default(TEntity);
		ViEntityID ID;
		if (IS.Read(out ID) == false) { return false; }
		value = EntityNameger.GetEntity<TEntity>(ID);
		return true;
	}
	public static bool Read<TEntity>(ViStringIStream IS, out List<TEntity> list) where TEntity : ViEntity
	{
		list = null;
		ViArrayIdx size;
		if (IS.Read(out size) == false) { return false; }
		list = new List<TEntity>();
		for (ViArrayIdx idx = 0; idx < size; ++idx)
		{
			TEntity value;
			if (Read(IS, out value)) { return false; }
			list.Add(value);
		}
		return true;
	}
}

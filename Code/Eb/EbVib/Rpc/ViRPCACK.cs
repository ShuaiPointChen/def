using System;
using System.Collections.Generic;

using ViEntityID = System.UInt64;

public class ViEntityACK
{
	public delegate void DeleACKCallback();

	public void Add(UInt16 funcID)
	{
		if (!_list.ContainsKey(funcID))
		{
			_list[funcID] = null;
		}
	}
	public void Add(UInt16 funcID, DeleACKCallback callback)
	{
		if (!_list.ContainsKey(funcID))
		{
			_list[funcID] = callback;
		}
	}
	public void Ack(UInt16 funcID)
	{
		DeleACKCallback callback = null;
		if (_list.TryGetValue(funcID, out callback))
		{
			if (callback != null)
			{
				callback();
			}
			_list.Remove(funcID);
		}
	}
	public bool Has(UInt16 funcID)
	{
		return _list.ContainsKey(funcID);
	}


	Dictionary<UInt16, DeleACKCallback> _list = new Dictionary<UInt16, DeleACKCallback>();
}

public class ViRPC
{
	public ViOStream OS { get { return _net.OS; } }
	public ViNet Net { get { return _net; } set { _net = value; } }
	public ViEntityACK ACK { get { return _ACK; } }

	public void SendMessage()
	{
		if (_net != null)
		{
			_net.Update();
		}
	}

	ViEntityACK _ACK = new ViEntityACK();
	ViNet _net;
}


public interface ViRPCEntity: ViEntity
{
	ViRPC RPC { get; }
}


//public class ViRPCACK
//{
//    public delegate void DeleACKCallback();

//    class EntityACKList
//    {
//        public void Add(UInt16 funcID, DeleACKCallback callback)
//        {
//            if (!_list.ContainsKey(funcID))
//            {
//                _list[funcID] = callback;
//            }
//        }
//        public bool Has(UInt16 funcID)
//        {
//            return _list.ContainsKey(funcID);
//        }
//        public void Ack(UInt16 funcID)
//        {
//            DeleACKCallback callback = null;
//            if (_list.TryGetValue(funcID, out callback))
//            {
//                if (callback != null)
//                {
//                    callback();
//                }
//                _list.Remove(funcID);
//            }
//        }

//        Dictionary<UInt16, DeleACKCallback> _list = new Dictionary<UInt16, DeleACKCallback>();
//    }

//    public void Add(ViEntityID entityID, UInt16 funcID)
//    {
//        Add(entityID, funcID, null);
//    }
//    public void Add(ViEntityID entityID, UInt16 funcID, DeleACKCallback callback)
//    {
//        EntityACKList entityAck;
//        if (_list.TryGetValue(entityID, out entityAck))
//        {
//            entityAck.Add(funcID, callback);
//        }
//        else
//        {
//            entityAck = new EntityACKList();
//            entityAck.Add(funcID, callback);
//            _list[entityID] = entityAck;
//        }
//    }
//    public bool Has(ViEntityID entityID, UInt16 funcID)
//    {
//        EntityACKList entityAck;
//        if (_list.TryGetValue(entityID, out entityAck))
//        {
//            return entityAck.Has(funcID);
//        }
//        else
//        {
//            return false;
//        }
//    }
//    public void Ack(ViEntityID entityID, UInt16 funcID)
//    {
//        EntityACKList entityAck;
//        if (_list.TryGetValue(entityID, out entityAck))
//        {
//            entityAck.Ack(funcID);
//        }
//    }

//    Dictionary<ViEntityID, EntityACKList> _list = new Dictionary<ViEntityID, EntityACKList>();
//}
using System;
using System.Collections.Generic;

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViRouteNode
{
	public static readonly UInt32 NULL_EVENT = 0XFFFFFFFF;
	public static bool HasEvent(UInt32 eventId)
	{
		return (eventId != NULL_EVENT);
	}
	public ViVector3 _pos;
	public UInt32 _eventId;
}

public class ViRouteAlloc<T> where T:new()
{
	public ViDoubleLinkNode2<T> New()
	{
		if (_nodes.IsEmpty())
		{
			return new ViDoubleLinkNode2<T>(new T());
		}
		else
		{
			ViDoubleLinkNode2<T> node = _nodes.GetHead();
			node.Detach();
			return node;
		}
	}
	public void Delete(ViDoubleLinkNode2<T> node)
	{
		if (node != null)
		{
			_nodes.PushBack(node);
		}
	}
	public void Delete(ViDoubleLink2<T> nodes)
	{
		_nodes.PushBack(nodes);
	}

	public void Clear()
	{
		_nodes.Clear();
	}

	ViDoubleLink2<T> _nodes = new ViDoubleLink2<T>();
}

//+-------------------------------------------------------------------------------------------------------------------------------------------------------------
public class ViRoute
{
	public static float GetLength(List<ViVector3> posList)
	{
		if (posList.Count <= 1)
		{
			return 0.0f;
		}
		float len = 0.0f;
		ViVector3 prePos = posList[0];
		for (int idx = 1; idx < posList.Count; ++idx )
		{
			len += Distance(posList[idx], prePos);
			prePos = posList[idx];
		}
		return len;
	}
	public static float GetLength(ViVector3 startPos, List<ViVector3> posList)
	{
		float len = 0.0f;
		ViVector3 prePos = startPos;
		for (int idx = 0; idx < posList.Count; ++idx)
		{
			len += Distance(posList[idx], prePos);
			prePos = posList[idx];
		}
		return len;
	}

	public static float Distance(ViVector3 pos1, ViVector3 pos2)
	{
		float deltaX = pos1.x - pos2.x;
		float deltaY = pos1.y - pos2.y;
		return ViMathDefine.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
	}

	public ViVector3 Position { get { return _currentPos; } }
	public ViVector3 Direction { get { return _dir; } }
	public ViEventAsynList EndCallbackList { get { return _endCallbackList; } }
	//
	public void Append(ViVector3 pos)
	{
		ViDoubleLinkNode2<ViRouteNode> node = S_Nodes.New();
		node.Data._eventId = ViRouteNode.NULL_EVENT;
		node.Data._pos = pos;
		_nodes.PushBack(node);
	}
	public void Append(ViVector3 pos, UInt32 eventId)
	{
		ViDoubleLinkNode2<ViRouteNode> node = S_Nodes.New();
		node.Data._eventId = eventId;
		node.Data._pos = pos;
		_nodes.PushBack(node);
	}
	public bool OnTick(float deltaTime, float spd)
	{
		return _UpdateDistance(deltaTime * spd);
	}
	public void Start(ViVector3 pos)
	{
		if (_nodes.IsEmpty())
		{
			_ClearState();
			return;
		}
		_prePos = pos;
		_currentPos = _prePos;
		_nextNode = _LerpToNextNode();
		ViDebuger.AssertError(_nextNode);
	}
	public void Reset()
	{
		_ClearState();
		_ClearNodeList();
		_endCallbackList.Clear();
	}
	public bool GetNextPosition(ref ViVector3 pos)
	{
		if (_nextNode != null)
		{
			pos = _nextNode._pos;
			return true;
		}
		else
		{
			return false;
		}
	}
	public bool GetDestPosition(ref ViVector3 pos)
	{
		if (_nodes.IsEmpty())
		{
			return false;
		}
		else
		{
			pos = _nextNode._pos;
			return true;
		}
	}
	public float Length()
	{
		float len = 0.0f;
		ViVector3 prePos = _currentPos;
		ViDoubleLinkNode2<ViRouteNode> iter = _nodes.GetHead();
		while (!_nodes.IsEnd(iter))
		{
			ViRouteNode value = iter.Data;
			ViDoubleLink2<ViRouteNode>.Next(ref iter);
			//
			len += Distance(value._pos, prePos);
			prePos = value._pos;
		}
		return len;
	}
	public bool IsEnd()
	{
		return (_nextNode == null);
	}
	//
	bool _UpdateDistance(float distance)
	{
		if (_nextNode == null)
		{
			return false;
		}
		ViVector3 nextPos = _nextNode._pos;
		float distRight = Distance(_currentPos, nextPos);
		if (distRight > distance)
		{
			_currentPos += _dir * distance;
		}
		else
		{
			float newDistance = distance - distRight;
			_currentPos = nextPos;
			_prePos = nextPos;
			if (ViRouteNode.HasEvent(_nextNode._eventId))
			{
				_endCallbackList.Invoke(_nextNode._eventId);
			}
			S_Nodes.Delete(_nodes.GetHead());
			_nextNode = _LerpToNextNode();
			if (_nextNode != null)
			{
				_UpdateDistance(newDistance);
			}
			else
			{
				_ClearState();
			}
		}
		return true;
	}
	ViRouteNode _LerpToNextNode()
	{
		if (_nodes.IsNotEmpty())
		{
			ViRouteNode pNode = _nodes.GetHead().Data;
			_dir = pNode._pos - _prePos;
			_dir.z = 0.0f;
			_dir.Normalize();
			return pNode;
		}
		else
		{
			return null;
		}
	}
	void _ClearNodeList()
	{
		S_Nodes.Delete(_nodes);
	}
	void _ClearState()
	{
		_nextNode = null;
		_prePos = _currentPos;
	}
	//
	ViVector3 _prePos;
	ViVector3 _currentPos;
	ViRouteNode _nextNode;
	ViVector3 _dir;
	ViDoubleLink2<ViRouteNode> _nodes = new ViDoubleLink2<ViRouteNode>();
	ViEventAsynList _endCallbackList = new ViEventAsynList();

	static ViRouteAlloc<ViRouteNode> S_Nodes = new ViRouteAlloc<ViRouteNode>();
}

public class Demo_Route
{
#pragma warning disable 0219
	class Listener
	{
		public void OnEnd(UInt32 eventId)
		{

		}
		public ViAsynCallback _endCallbackNode = new ViAsynCallback();
	}

	public static void Test()
	{
		Listener listener = new Listener();
		ViRoute route = new ViRoute();
		route.Append(new ViVector3(10, 0, 0));
		route.Append(new ViVector3(20, 0, 0));
		route.Append(new ViVector3(10, 0, 0), 1);
		route.Start(new ViVector3(0, 0, 0));
		float len = route.Length();
		route.EndCallbackList.Attach(listener._endCallbackNode, listener.OnEnd);
		float time = 0.0f;
		while (time < 10.0f)
		{
			route.OnTick(0.3f, 10.0f);
			time += 0.3f;
			ViAsynCallback.Update();
		}
	}
#pragma warning restore 0219
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ViPriorityNode<T>
{
	public ViPriorityNode(Int32 weight, T value)
	{
		Weight = weight;
		Value = value;
	}
	public T Value;
	public Int32 Weight;
}

public class ViPriorityValue<T>
{
	public ViPriorityNode<T> TopNode
	{
		get
		{
			if (_nodes.Count > 0)
			{
				return _nodes[0];
			}
			else
			{
				return null;
			}
		}
	}
	public static implicit operator T(ViPriorityValue<T> data)
	{
		return data.TopNode.Value;
	}

	public void Clear()
	{
		_nodes.Clear();
	}


	public bool Detach(ViPriorityNode<T> node)
	{
		int where = 0;
		for (; where < _nodes.Count; ++where)
		{
			if (Object.ReferenceEquals(node, _nodes[where]))
			{
				_nodes.RemoveAt(where);
				return (where == 0);
			}
		}
		return false;
	}

	public bool Attach(ViPriorityNode<T> node)
	{
		ViPriorityNode<T> oldTop = TopNode;
		Detach(node);
		_Attach(node);
		return !(Object.ReferenceEquals(oldTop, node));
	}

	bool _Attach(ViPriorityNode<T> node)
	{
		int where = 0;
		for (; where < _nodes.Count; ++where)
		{
			if (node.Weight >= _nodes[where].Weight)
			{
				break;
			}
		}
		_nodes.Insert(where, node);
		return (where == 0);
	}


	List<ViPriorityNode<T>> _nodes = new List<ViPriorityNode<T>>();

}

public class Demo_PriorityValue
{
	//class PriorityNode : ViPriorityNode
	//{
	//    public string Name;
	//}

	//public static void Test()
	//{
	//    PriorityNode node0 = new PriorityNode();
	//    node0.Name = "node0";
	//    node0.Weight = 0;
	//    //
	//    PriorityNode node1 = new PriorityNode();
	//    node1.Name = "node1";
	//    node1.Weight = 1;
	//    //
	//    PriorityNode node2 = new PriorityNode();
	//    node2.Name = "node2";
	//    node2.Weight = 2;
	//    //
	//    ViPriorityValue<PriorityNode> value = new ViPriorityValue<PriorityNode>();
	//    value.Attach(node0);
	//    Debug.Print("Top node: " + value.Top.Name);
	//    value.Attach(node2);
	//    Debug.Print("Top node: " + value.Top.Name);
	//    value.Attach(node1);
	//    Debug.Print("Top node: " + value.Top.Name);
	//}

}
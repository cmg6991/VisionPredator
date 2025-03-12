using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 노드 선택
/// </summary>
public class SelectorNode : BTNode
{
    private List<BTNode> children = new List<BTNode>();
    private int currentChild = 0;

    public void AddChild(BTNode node)
    {
        children.Add(node);
    }

    public override NodeState Execute()
    {
        while (currentChild < children.Count)
        {
            NodeState result = children[currentChild].Execute();
            if (result == NodeState.Running)
            {
                return NodeState.Running;
            }
            if (result == NodeState.Success)
            {
                Reset();
                return NodeState.Success;
            }
            currentChild++;
        }

        Reset();
        return NodeState.Failure;
    }

    public override void Reset()
    {
        currentChild = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}

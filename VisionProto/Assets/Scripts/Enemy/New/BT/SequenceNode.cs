using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시퀸스 노드
/// 
/// 자식 노드를 순차적으로 실행
/// </summary>
public class SequenceNode : BTNode
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
            //Debug.Log($"Child {currentChild} result: {result}");

            if (result == NodeState.Running)
            {
                return NodeState.Running;
            }
            if (result == NodeState.Failure)
            {
                Reset();
                return NodeState.Failure;
            }
            currentChild++;
        }

        Reset();
        return NodeState.Success;
    }

    public override void Reset()
    {
        //Debug.Log("SequenceNode Reset");
        currentChild = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}

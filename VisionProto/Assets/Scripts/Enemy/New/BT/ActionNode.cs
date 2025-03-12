using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 행동 노드
/// 
/// 특정 노드를 실행
/// </summary>
public class ActionNode : BTNode
{
    private Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Execute()
    {
        return action();
    }

    public override void Reset()
    {
        
    }
}

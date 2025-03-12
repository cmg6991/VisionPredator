using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : BTNode
{
    private System.Func<bool> condition;

    public ConditionNode(System.Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Execute()
    {
        return condition() ? NodeState.Success : NodeState.Failure;
    }
}
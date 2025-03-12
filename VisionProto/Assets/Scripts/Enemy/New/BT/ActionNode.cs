using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ൿ ���
/// 
/// Ư�� ��带 ����
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

using UnityEngine;
using static BTNode;

public class TestBT : MonoBehaviour
{
    private BTNode behaviorTree;

    private void Start()
    {
        var sequenceNode = new SequenceNode();
        sequenceNode.AddChild(new ActionNode(IdleAction));
        sequenceNode.AddChild(new ActionNode(Action1));
        sequenceNode.AddChild(new ActionNode(Action2));

        var attackSequence = new SequenceNode();
        attackSequence.AddChild(new ActionNode(AttackAction));
        attackSequence.AddChild(new ActionNode(Action3));

        var singSequence = new SequenceNode();
        singSequence.AddChild(new ActionNode(SingAction));
        singSequence.AddChild(new ActionNode(Action4));

        behaviorTree = new SelectorNode();
        ((SelectorNode)behaviorTree).AddChild(sequenceNode);
        ((SelectorNode)behaviorTree).AddChild(attackSequence);
        ((SelectorNode)behaviorTree).AddChild(singSequence);
    }

    private void Update()
    {
        NodeState result = behaviorTree.Execute();
        Debug.Log("Behavior Tree Result: " + result);
    }

    private NodeState IdleAction()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Action 1 ����");
            return NodeState.Success;
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("����!");
            return NodeState.Failure;
        }
        Debug.Log("Idle������...������ ���!");
        return NodeState.Running;
    }

    private NodeState Action1()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Action 2 ����");
            return NodeState.Success;
        }
        Debug.Log("�� �ȳ�...");
        return NodeState.Running;
    }

    private NodeState Action2()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Idle�� ���ư���");
            return NodeState.Success;
        }
        Debug.Log("�� �ܲ�...");
        return NodeState.Running;
    }

    private NodeState AttackAction()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Attack ����");
            return NodeState.Success;
        }
        Debug.Log("��ȹ�ڸ� ã����");
        return NodeState.Running;
    }

    private NodeState Action3()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Attack �Ϸ�");
            return NodeState.Success;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("����!");
            return NodeState.Failure;
        }
        Debug.Log("��ȹ�ڸ� �д���");
        return NodeState.Running;
    }

    private NodeState SingAction()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("sing ����");
            return NodeState.Success;
        }
        Debug.Log("��Ҹ� �غ���...");
        return NodeState.Running;
    }

    private NodeState Action4()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("sing �Ϸ�");
            return NodeState.Success;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("����!");
            return NodeState.Failure;
        }
        Debug.Log("�뷡�� �θ��� ��");
        return NodeState.Running;
    }
}

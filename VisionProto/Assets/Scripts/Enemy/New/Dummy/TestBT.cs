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
            Debug.Log("Action 1 진입");
            return NodeState.Success;
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("실패!");
            return NodeState.Failure;
        }
        Debug.Log("Idle상태중...리나쿤 기상!");
        return NodeState.Running;
    }

    private NodeState Action1()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Action 2 진입");
            return NodeState.Success;
        }
        Debug.Log("밥 냠냠...");
        return NodeState.Running;
    }

    private NodeState Action2()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Idle로 돌아가기");
            return NodeState.Success;
        }
        Debug.Log("물 꿀꺽...");
        return NodeState.Running;
    }

    private NodeState AttackAction()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Attack 시작");
            return NodeState.Success;
        }
        Debug.Log("기획자를 찾는중");
        return NodeState.Running;
    }

    private NodeState Action3()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Attack 완료");
            return NodeState.Success;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("실패!");
            return NodeState.Failure;
        }
        Debug.Log("기획자를 패는중");
        return NodeState.Running;
    }

    private NodeState SingAction()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("sing 시작");
            return NodeState.Success;
        }
        Debug.Log("목소리 준비중...");
        return NodeState.Running;
    }

    private NodeState Action4()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("sing 완료");
            return NodeState.Success;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("실패!");
            return NodeState.Failure;
        }
        Debug.Log("노래를 부르는 중");
        return NodeState.Running;
    }
}

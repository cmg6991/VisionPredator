using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BoseBaseEnemy;
using static BTNode;


public class BoseEnemy : BoseBaseEnemy
{


    // Start is called before the first frame update
    protected override void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();

        base.Start();
    }

    protected override void InitializeBehaviorTree()
    {
        var idle = new SequenceNode();
        idle.AddChild(new ActionNode(() => actions[BossActionType.Idle].Execute()));

        //// Grog ���� ��� (�� ���� ����ǵ��� ����)
        //var grogSequence = new SequenceNode();
        //grogSequence.AddChild(new ConditionNode(() => !grogExecuted.Value)); // grogExecuted�� false�� ���� ����
        //grogSequence.AddChild(new ActionNode(() =>
        //{
        //    actions[BossActionType.Grog].Execute();
        //    //grogExecuted = true; // Grog ���� �� �÷��� ����
        //    return NodeState.Success;
        //}));

        //var grogSequence2 = new SequenceNode();
        //grogSequence2.AddChild(new ConditionNode(() => !grogExecuted2.Value)); // grogExecuted�� false�� ���� ����
        //grogSequence2.AddChild(new ActionNode(() =>
        //{
        //    actions[BossActionType.Grog].Execute();
        //    //grogExecuted = true; // Grog ���� �� �÷��� ����
        //    return NodeState.Success;
        //}));

        var pazeOne = new SequenceNode();
        pazeOne.AddChild(new ActionNode(() => actions[BossActionType.PazeOne].Execute()));
        pazeOne.AddChild(new ActionNode(() => actions[BossActionType.PazeOneClose].Execute()));

        var pazeTwo = new SequenceNode();
        //pazeTwo.AddChild(grogSequence); // Grog ������ ���ǿ� �߰�
        pazeTwo.AddChild(new ActionNode(() => actions[BossActionType.PazeTwo].Execute()));
        pazeTwo.AddChild(new ActionNode(() => actions[BossActionType.PazeTwoClose].Execute()));

        var pazeThree = new SequenceNode();
        //pazeThree.AddChild(grogSequence2); // Grog ������ ���ǿ� �߰�
        pazeThree.AddChild(new ActionNode(() => actions[BossActionType.PazeThree].Execute()));
        pazeThree.AddChild(new ActionNode(() => actions[BossActionType.PazeThreeClose].Execute()));

        baseBehaviorTree = new SelectorNode();
        ((SelectorNode)baseBehaviorTree).AddChild(idle);
        //((SelectorNode)baseBehaviorTree).AddChild(grogSequence);
        //((SelectorNode)baseBehaviorTree).AddChild(grogSequence2);
        ((SelectorNode)baseBehaviorTree).AddChild(pazeOne);
        baseBehaviorTree2 = new SelectorNode();
        ((SelectorNode)baseBehaviorTree2).AddChild(pazeTwo);
        baseBehaviorTree3 = new SelectorNode();
        ((SelectorNode)baseBehaviorTree3).AddChild(pazeThree);
    }
}

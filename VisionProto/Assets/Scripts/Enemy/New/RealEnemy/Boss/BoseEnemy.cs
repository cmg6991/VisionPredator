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

        //// Grog 상태 노드 (한 번만 실행되도록 설정)
        //var grogSequence = new SequenceNode();
        //grogSequence.AddChild(new ConditionNode(() => !grogExecuted.Value)); // grogExecuted가 false일 때만 실행
        //grogSequence.AddChild(new ActionNode(() =>
        //{
        //    actions[BossActionType.Grog].Execute();
        //    //grogExecuted = true; // Grog 실행 후 플래그 설정
        //    return NodeState.Success;
        //}));

        //var grogSequence2 = new SequenceNode();
        //grogSequence2.AddChild(new ConditionNode(() => !grogExecuted2.Value)); // grogExecuted가 false일 때만 실행
        //grogSequence2.AddChild(new ActionNode(() =>
        //{
        //    actions[BossActionType.Grog].Execute();
        //    //grogExecuted = true; // Grog 실행 후 플래그 설정
        //    return NodeState.Success;
        //}));

        var pazeOne = new SequenceNode();
        pazeOne.AddChild(new ActionNode(() => actions[BossActionType.PazeOne].Execute()));
        pazeOne.AddChild(new ActionNode(() => actions[BossActionType.PazeOneClose].Execute()));

        var pazeTwo = new SequenceNode();
        //pazeTwo.AddChild(grogSequence); // Grog 실행을 조건에 추가
        pazeTwo.AddChild(new ActionNode(() => actions[BossActionType.PazeTwo].Execute()));
        pazeTwo.AddChild(new ActionNode(() => actions[BossActionType.PazeTwoClose].Execute()));

        var pazeThree = new SequenceNode();
        //pazeThree.AddChild(grogSequence2); // Grog 실행을 조건에 추가
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PistolEnemy : BaseEnemy
{
    protected override void Start()
    {
        //detectionRadius = 30f; //150
        //detectionAngle = 30f; //160
        //detectionSound = 40f; //100
        //HP.Value = 50;    
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        child = gameObject.transform.GetComponentsInChildren<Transform>();
        Transform[] reyeChildren = child.Where(child => child.name == "Reye").ToArray();
        child = reyeChildren;
        upperBody = gameObject.transform.GetComponentsInChildren<Transform>();
        Transform[] upperChildren = upperBody.Where(child => child.name == "DEF-spine.001").ToArray();
        upperBody = upperChildren;
        enemyType = 0;

        base.Start();
    }

    protected override void InitializeBehaviorTree()
    {
        var idlesequenceNode = new SequenceNode();
        idlesequenceNode.AddChild(new ActionNode(() => actions[EnemyActionType.IdleAction].Execute()));
        idlesequenceNode.AddChild(new ActionNode(() => actions[EnemyActionType.IdleSAction].Execute()));

        var patrolSequence = new SequenceNode();
        patrolSequence.AddChild(new ActionNode(() => actions[EnemyActionType.PatrolAction].Execute()));

        var chaseSequence = new SequenceNode();
        chaseSequence.AddChild(new ActionNode(() => actions[EnemyActionType.ChaseAction].Execute()));

        var hurtSequence = new SequenceNode();
        hurtSequence.AddChild(new ActionNode(() => actions[EnemyActionType.HurtAction].Execute()));

        var dieSequence = new SequenceNode();
        dieSequence.AddChild(new ActionNode(() => actions[EnemyActionType.DieAction].Execute()));

        baseBehaviorTree = new SelectorNode();
        ((SelectorNode)baseBehaviorTree).AddChild(idlesequenceNode);
        ((SelectorNode)baseBehaviorTree).AddChild(patrolSequence);
        ((SelectorNode)baseBehaviorTree).AddChild(chaseSequence);
        //((SelectorNode)baseBehaviorTree).AddChild(hurtSequence);
        //((SelectorNode)baseBehaviorTree).AddChild(dieSequence);
    }

}

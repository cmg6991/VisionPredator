using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DoorOpenState : BaseState
{
    public DoorOpenState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    OpenDoorBType doorBType;

    public override void Enter()
    {
        doorBType = stateMachine.targetGameObject.GetComponent<OpenDoorBType>();
        doorBType.isControl = true;
    }

    public override void Tick()
    {
        if (stateMachine.isTutorial)
        {
            stateMachine.SwitchState(new TutorialIdleState(stateMachine));
        }
        else
            stateMachine.SwitchState(new IdleState(stateMachine));
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {
        stateMachine.targetGameObject = null;
        stateMachine.objectTag = default;
        doorBType.isControl = false;
    }
}

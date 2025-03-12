using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteractionState : BaseState
{
    public TutorialInteractionState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private int grapplingLayer;
    private int grapplingPointLayer;
    private int gunLayer;
    private int outlineLayer;
    private Transform camera;

    public override void Enter()
    {
        stateMachine.ObjectInteraction();
        camera = GameObject.Find("Virtual Camera").GetComponent<Transform>();

        // Gun Layer
        gunLayer = LayerMask.GetMask("Gun");
        outlineLayer = LayerMask.GetMask("Outline");
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        
        if (stateMachine.objectTag == "Door")
            stateMachine.SwitchState(new DoorOpenState(stateMachine));
        else if (stateMachine.objectTag == "Cabinet" || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Button")
            stateMachine.SwitchState(new ObjectOpenState(stateMachine));
        else if (stateMachine.layerMask == gunLayer || stateMachine.layerMask == outlineLayer)
            stateMachine.SwitchState(new PickupState(stateMachine));
        else
            stateMachine.SwitchPreviousState();

    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {

    }
}

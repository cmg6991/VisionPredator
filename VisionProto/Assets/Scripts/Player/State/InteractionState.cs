
using Cinemachine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 상호작용하기 전에 Layer를 체크하는 State다.
/// </summary>
public class InteractionState : BaseState
{
    public InteractionState(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
        if(stateMachine.objectTag == "Door")
            stateMachine.SwitchState(new DoorOpenState(stateMachine)); 
        else if(stateMachine.objectTag == "Cabinet" || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Button")
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

    // Enter -> FixedTick -> Tick
}

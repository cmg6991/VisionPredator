using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialState : BaseState
{
    public TutorialState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.isTutorial = true;
        TutorialManager.Instance.currentState = TutorialStage.VPIdle;
    }
    public override void Tick()
    {
        switch (TutorialManager.Instance.currentState)
        {
            case TutorialStage.VPIdle:
                {
                    VPIdle();
                }
                break;
            case TutorialStage.VPMovement:
                {
                    VPMovement();
                }
                break;
            case TutorialStage.VPJump:
                {
                    VPMovement();
                    VPJump();
                }
                break;


        }
    }
    public override void FixedTick()
    {

    }
    public override void Exit()
    {

    }

    void VPIdle()
    {

    }
    void VPMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            stateMachine.SwitchState(new TutorialMoveState(stateMachine));
            TutorialManager.Instance.NextState();
        }
    }
    void VPJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.SwitchState(new TutorialJumpState(stateMachine));
        }
    }
}

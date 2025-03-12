using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFallState : BaseState
{
    public TutorialFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    float maxFallTime = 1f; // 최대 낙하 시간
    float startTime;

    float stateLockTime = 5f; // 상태 전환 후 잠금 시간
    float lastStateChangeTime;

    public override void Enter()
    {
        stateMachine.input.dashIndex = 1;
        stateMachine.velocity.y = 0f;
        startTime = Time.time;
    }
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (stateMachine.input.isGrounded)
        {
            stateMachine.input.isRay = false;
            stateMachine.SwitchState(new TutorialMoveState(stateMachine));
            stateMachine.input.isJump = false;
        }
        if (stateMachine.isVPState)
        {
            if (TutorialManager.Instance.tutorialSlash && Input.GetKeyDown(KeyCode.LeftControl))
            {
                stateMachine.SwitchState(new TutorialSlashState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.VPDash)
                    TutorialManager.Instance.NextState();
            }
        }
        
    }
    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        ApplyGravity();
        Move();
        if (Time.time - lastStateChangeTime < stateLockTime)
            return;

        if (Time.time - startTime > maxFallTime)
        {
            stateMachine.SwitchState(new TutorialJumpState(stateMachine));
        }
    }
    public override void Exit()
    {

    }
}

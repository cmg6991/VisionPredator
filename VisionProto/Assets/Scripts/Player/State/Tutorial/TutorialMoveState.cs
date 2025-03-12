using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveState : BaseState
{
    private GameObject walkSound;
    private CameraInfomation cameraInformation;
    private bool isMovingSound1;
    bool hasMoved;
    public TutorialMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // Camera
        if (stateMachine.isVPState)
        {
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.vp_Move_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.vp_Move_Frequency;
        }
        else
        {
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Move_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Move_Frequency;
        }
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        stateMachine.capsuleCollider.center = new Vector3(0, 0, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.velocity.y = Physics.gravity.y;

        walkSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Walk_1, stateMachine.transform);
        isMovingSound1 = true;

        stateMachine.isTutorial = true;
        hasMoved = false;
    }
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (walkSound == null)
        {
            if (isMovingSound1)
            {
                walkSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Walk_2, stateMachine.transform);
                isMovingSound1 = false;
            }
            else
            {
                walkSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Walk_1, stateMachine.transform);
                isMovingSound1 = true;
            }
        }
        if (stateMachine.isVPState)
            stateMachine.animator.Movement = 0.5f;
        stateMachine.animator.Speed = 0.5f;


        if(stateMachine.isVPState)
        {
            if (TutorialManager.Instance.tutorialDash && Input.GetKeyDown(KeyCode.LeftShift))
            {
                stateMachine.SwitchState(new TutorialDashState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.VPWeapon)
                    TutorialManager.Instance.NextState();
            }
            if (TutorialManager.Instance.tutorialJump && Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.SwitchState(new TutorialJumpState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.VPMovement)
                    TutorialManager.Instance.NextState();
            }
            if(Input.GetMouseButtonDown(0))
            {
                if (TutorialManager.Instance.currentState == TutorialStage.VPSight)
                    TutorialManager.Instance.NextState();
            }
        }
        if (!stateMachine.isVPState)
        {
            if (TutorialManager.Instance.tutorialRun && Input.GetKey(KeyCode.LeftShift))
            {
                stateMachine.SwitchState(new TutorialRunState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Movement)
                    TutorialManager.Instance.NextState();
            }
            if (TutorialManager.Instance.tutorialHumanJump && Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.SwitchState(new TutorialJumpState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Run)
                    TutorialManager.Instance.NextState();
            }
            if (TutorialManager.Instance.tutorialSlide && Input.GetKey(KeyCode.LeftControl))
            {
                stateMachine.SwitchState(new TutorialSlideState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Sit)
                    TutorialManager.Instance.NextState();
            }
            if (TutorialManager.Instance.tutorialInteraction && Input.GetKeyDown(KeyCode.F))
            {
                stateMachine.ObjectInteraction();

                if (stateMachine.objectTag == "Door" || stateMachine.objectTag == "Cabinet"
                    || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
                {
                    stateMachine.SwitchState(new TutorialInteractionState(stateMachine));
                    if (TutorialManager.Instance.currentState == TutorialStage.HandAttack)
                        TutorialManager.Instance.NextState();
                }
            }
            if (TutorialManager.Instance.isPickUp && Input.GetMouseButtonDown(0))
                if (TutorialManager.Instance.currentState == TutorialStage.Draw)
                    TutorialManager.Instance.NextState();
            if (TutorialManager.Instance.isThrow)
                if (TutorialManager.Instance.currentState == TutorialStage.Shot)
                    TutorialManager.Instance.NextState();
            if (Input.GetMouseButtonDown(0))
            {
                if (TutorialManager.Instance.currentState == TutorialStage.Slide)
                {
                    TutorialManager.Instance.NextState();
                }
            }
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
             !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            stateMachine.SwitchState(new TutorialIdleState(stateMachine));

        if (!stateMachine.input.isGrounded)
            stateMachine.SwitchState(new TutorialFallState(stateMachine));

        if (TutorialManager.Instance.tutorialChange && Input.GetKeyDown(KeyCode.R))
        {
            if (TutorialManager.Instance.currentState == TutorialStage.VPSlash)
                TutorialManager.Instance.NextState();
            if (!stateMachine.coolActive || Time.time >= stateMachine.lastVPStateTime + stateMachine.coolTime)
            {
                stateMachine.SwitchState(new TutorialVPState(stateMachine));
                stateMachine.lastVPStateTime = Time.time;
                stateMachine.coolActive = true;
            }
        }
        if (stateMachine.coolActive && Time.time >= stateMachine.lastVPStateTime + stateMachine.coolTime)
        {
            stateMachine.coolActive = false;
        }
    }
    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        MoveDirection();
        Move();
    }
    public override void Exit()
    {
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRunState : BaseState
{
    public TutorialRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float speed;

    // Grappling Point Layer와 Grappling Layer를 여기다가 둘까 아니면 각자 필요한 곳에 둘까
    private int grapplingLayer;
    private int grapplingPointLayer;

    GameObject runSound;
    private CameraInfomation cameraInformation;
    private bool isRunningSound1;

    public override void Enter()
    {
        grapplingLayer = LayerMask.GetMask("Grappling");
        grapplingPointLayer = LayerMask.GetMask("GrapplingPoint");

        // Camera
        cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
        cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Run_Amplitude;
        cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Run_Frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        stateMachine.velocity.y = Physics.gravity.y;
        speed = stateMachine.moveSpeed;
        stateMachine.moveSpeed += 5f;

        runSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Run_1, stateMachine.transform);
        isRunningSound1 = true;
    }
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (runSound == null)
        {
            if (isRunningSound1)
            {
                runSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Run_1, stateMachine.transform);
                isRunningSound1 = false;
            }
            else
            {
                runSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Run_2, stateMachine.transform);
                isRunningSound1 = true;
            }
        }

        stateMachine.animator.Speed = 1;

        if (!stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.SwitchState(new TutorialJumpState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Run)
                    TutorialManager.Instance.NextState();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
                stateMachine.SwitchState(new TutorialMoveState(stateMachine));
            //if (Input.GetKeyDown(KeyCode.LeftControl))
            //    stateMachine.SwitchState(new SlideState(stateMachine));
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
        if (!stateMachine.input.isGrounded)
            stateMachine.SwitchState(new TutorialFallState(stateMachine));
        if (Input.GetMouseButtonDown(1) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
            if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
            {
                stateMachine.SwitchState(new GrapplingState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Interaction)
                    TutorialManager.Instance.NextState();
            }
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

        stateMachine.moveSpeed = speed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialJumpState : BaseState
{
    public TutorialJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private CameraInfomation cameraInformation;

    public override void Enter()
    {
        if (stateMachine.input.isSit)
        {
            stateMachine.input.isJump = true;
            stateMachine.head.localPosition = stateMachine.sitPos;
        }

        if (stateMachine.isVPState)
        {
            stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.VPJumeForce, stateMachine.velocity.z);
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.vp_Jump_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.vp_Jump_Frequency;
        }
        else
        {
            stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.jumpForce, stateMachine.velocity.z);
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Jump_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Jump_Frequency;
        }
        // Camera
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        //stateMachine.rigid.velocity = new Vector3(stateMachine.rigid.velocity.x, 0f, stateMachine.rigid.velocity.z);
        //stateMachine.rigid.AddForce(Vector3.up * stateMachine.jumpForce*100f, ForceMode.Impulse);
        //stateMachine.rigid.velocity = new Vector3(stateMachine.rigid.velocity.x, stateMachine.jumpForce*100f, stateMachine.rigid.velocity.z);
        stateMachine.input.dashIndex = 0;
        stateMachine.input.isSlopeJump = true;
        stateMachine.input.isRay = true;

        SoundManager.Instance.PlayEffectSound(SFX.Player_Jump, stateMachine.transform);

        stateMachine.isTutorial = true;
    }
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (stateMachine.isVPState)
            stateMachine.animator.Movement = 1f;
        if (stateMachine.isVPState)
        {
            if (TutorialManager.Instance.tutorialSlash && Input.GetKeyDown(KeyCode.LeftControl))
            {
                stateMachine.SwitchState(new TutorialSlashState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.VPDash)
                    TutorialManager.Instance.NextState();
            }
        }
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
        ApplyGravity();
        Move();

        if (stateMachine.velocity.y <= 0f && !stateMachine.input.slashDamage)
        {
            stateMachine.SwitchState(new TutorialFallState(stateMachine));
        }
    }
    public override void Exit()
    {
        if (!stateMachine.input.isSit)
        {
            stateMachine.input.isSit = false;
            stateMachine.head.localPosition = stateMachine.originPos;
        }
        stateMachine.layerMask = 0;
        stateMachine.input.isSlopeJump = false;

        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
    }
}
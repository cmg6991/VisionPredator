using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSlideState : BaseState
{
    Vector3 SlideDirection;
    Vector3 SlideStartPos;
    float slideStartTime;
    bool isExiting;
    private CameraInfomation cameraInformation;

    public TutorialSlideState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.capsuleCollider.center = new Vector3(0, -0.3f, 0);
        stateMachine.capsuleCollider.height = 1.2f;
        SlideDirection = stateMachine.direction.forward.normalized;
        SlideStartPos = stateMachine.transform.position;
        slideStartTime = Time.time;

        stateMachine.originPos = new Vector3(0, 1f, 0);

        isExiting = false;

        // Camera 
        cameraInformation.setting = CameraSetting.Shake;
        cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Slide_Amplitude;
        cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Slide_Frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        SoundManager.Instance.PlayEffectSound(SFX.Player_Slide, stateMachine.transform);
    }

    public override void Tick()
    {

    }

    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        if (stateMachine.input.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(stateMachine.transform.position, Vector3.down, out hit, 2f)) // 경사면 정보 추출
            {
                Vector3 slideDirectionAdjusted = Vector3.ProjectOnPlane(SlideDirection, hit.normal).normalized;

                float t = (Time.time - slideStartTime) / stateMachine.slideDuration;
                if (t <= 1f)
                {
                    float smoothStep = t * t * (3f - 2f * t);
                    stateMachine.velocity = slideDirectionAdjusted * stateMachine.slideSpeed * smoothStep;
                    stateMachine.rigid.useGravity = false;
                }
                else
                    stateMachine.rigid.useGravity = true;
                stateMachine.rigid.velocity = stateMachine.velocity * stateMachine.moveSpeed;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (TutorialManager.Instance.currentState == TutorialStage.Slide)
            {
                TutorialManager.Instance.NextState();
            }
        }

        if (TutorialManager.Instance.isPickUp && Input.GetMouseButtonDown(0))
            if (TutorialManager.Instance.currentState == TutorialStage.Draw)
                TutorialManager.Instance.NextState();

        if (Time.time - slideStartTime >= stateMachine.slideDuration || Vector3.Distance(SlideStartPos, stateMachine.transform.position) > stateMachine.slideDistance)
        {
            isExiting = true;
        }

        if (isExiting)
        {
            stateMachine.head.localPosition = Vector3.Lerp(stateMachine.head.localPosition, stateMachine.originPos, 0.5f);
            if (Vector3.Distance(stateMachine.head.localPosition, stateMachine.originPos) < 0.01f)
            {
                stateMachine.head.localPosition = stateMachine.originPos;
                stateMachine.SwitchState(new TutorialMoveState(stateMachine));
            }
        }
        else
        {
            stateMachine.head.localPosition = Vector3.Lerp(stateMachine.head.localPosition, stateMachine.sitPos, 0.5f);
        }
    }

    public override void Exit()
    {
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        stateMachine.velocity = Vector3.zero;
    }
}

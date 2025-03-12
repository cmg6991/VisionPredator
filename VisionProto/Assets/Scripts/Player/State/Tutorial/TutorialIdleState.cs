using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialIdleState : BaseState
{
    public TutorialIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    Coroutine resetCoroutine;
    CameraInfomation cameraInformation;

    public override void Enter()
    {
        // Camera
        if (stateMachine.isVPState)
        {
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.vp_Idle_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.vp_Idle_Frequency;
        }
        else
        {
            cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Idle_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Idle_Frequency;
        }
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        stateMachine.capsuleCollider.center = new Vector3(0, 0, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.velocity = Vector3.zero;
        stateMachine.rigid.velocity = Vector3.zero;
        stateMachine.originPos = new Vector3(0, 1f, 0);
        stateMachine.head.localPosition = stateMachine.originPos;



        if (resetCoroutine != null)
        {
            stateMachine.StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (stateMachine.input.isSit && resetCoroutine == null)
        {
            resetCoroutine = stateMachine.StartCoroutine(LerpHeadPosition(stateMachine));
        }
    }
    public override void Tick()
    {
        if (stateMachine.isVPState)
            stateMachine.animator.Movement = 0;
        stateMachine.animator.Speed = 0;

        if (stateMachine.isPause)
            return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            stateMachine.SwitchState(new TutorialMoveState(stateMachine));
            if(TutorialManager.Instance.currentState == TutorialStage.VPIdle)
            {
                TutorialManager.Instance.NextState(); // 다음 단계로 진행
            }
            if(TutorialManager.Instance.currentState == TutorialStage.ChangeVP)
            {
                TutorialManager.Instance.NextState();
            }
        }

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
            if (Input.GetMouseButtonDown(0))
            {
                if (TutorialManager.Instance.currentState == TutorialStage.VPSight)
                    TutorialManager.Instance.NextState();
            }
        }

        if (TutorialManager.Instance.currentState == TutorialStage.VPJump)
            TutorialManager.Instance.NextState();

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
            if (TutorialManager.Instance.tutorialSit && Input.GetKey(KeyCode.LeftControl))
            {
                stateMachine.SwitchState(new TutorialSitState(stateMachine));
                if (TutorialManager.Instance.currentState == TutorialStage.Jump)
                    TutorialManager.Instance.NextState();
            }
            if (TutorialManager.Instance.tutorialInteraction && (Input.GetKeyDown(KeyCode.F) || TutorialManager.Instance.isPickUp))
            {
                if(TutorialManager.Instance.isPickUp)
                    if (TutorialManager.Instance.currentState == TutorialStage.HandAttack)
                    {
                        TutorialManager.Instance.NextState();
                        return;
                    }
                stateMachine.ObjectInteraction();

                if (stateMachine.objectTag == "Door" || stateMachine.objectTag == "Cabinet"
                    || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
                {
                    stateMachine.SwitchState(new TutorialInteractionState(stateMachine));
                    if (TutorialManager.Instance.currentState == TutorialStage.HandAttack)
                        TutorialManager.Instance.NextState();
                    if (TutorialManager.Instance.currentState == TutorialStage.Throw)
                        TutorialManager.Instance.NextState();
                }
            }
            if(TutorialManager.Instance.isPickUp && Input.GetMouseButtonDown(0))
                if(TutorialManager.Instance.currentState == TutorialStage.Draw)
                    TutorialManager.Instance.NextState();
            if (TutorialManager.Instance.isThrow)
                if (TutorialManager.Instance.currentState == TutorialStage.Shot)
                    TutorialManager.Instance.NextState();
            if(Input.GetMouseButtonDown(0))
            {
                if (TutorialManager.Instance.currentState == TutorialStage.Slide)
                {
                    TutorialManager.Instance.NextState();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                stateMachine.ObjectInteraction();

                //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)

                if (stateMachine.targetGameObject != null)
                {
                    if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
                    {
                        stateMachine.SwitchState(new GrapplingState(stateMachine));
                        if (TutorialManager.Instance.currentState == TutorialStage.Interaction)
                            TutorialManager.Instance.NextState();
                    }
                }
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

        if (stateMachine.input.explictSit && Input.GetKey(KeyCode.LeftControl))
            stateMachine.SwitchState(new TutorialSitState(stateMachine));
    }

    public override void FixedTick()
    {
        /// Save point와 관련된 공간
        if (stateMachine.isSpawn && !stateMachine.isInitPositionFalse)
        {
            PlayerInformation playerinfo = new PlayerInformation();
            playerinfo = DataManager.Instance.LoadData();
            stateMachine.transform.localPosition = playerinfo.position;
            stateMachine.transform.localRotation = playerinfo.rotation;
            stateMachine.isSpawn = false;
        }
    }

    public override void Exit()
    {
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        if (resetCoroutine != null)
        {
            stateMachine.StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    private IEnumerator LerpHeadPosition(PlayerStateMachine stateMachine)
    {
        Vector3 startPosition = stateMachine.sitPos;
        Vector3 targetPosition = stateMachine.originPos;

        float duration = 0.1f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            stateMachine.head.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        stateMachine.head.localPosition = targetPosition;
        stateMachine.input.isSit = false;
        resetCoroutine = null;
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    Coroutine resetCoroutine;
    CameraInfomation cameraInformation;

    private bool isBossPosition;

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

        stateMachine.StopAllCoroutines();

        if (stateMachine.input.isSit && resetCoroutine == null)
        {
            resetCoroutine = stateMachine.StartCoroutine(LerpHeadPosition(stateMachine));
        }
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;

        if (stateMachine.isVPState)
            stateMachine.animator.Movement = 0;
        stateMachine.animator.Speed = 0;

        if(resetCoroutine == null)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                stateMachine.SwitchState(new MoveState(stateMachine));
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.SwitchState(new JumpState(stateMachine));
        if (!stateMachine.isVPState)
        {
            if (Input.GetKey(KeyCode.LeftControl))
                stateMachine.SwitchState(new SitState(stateMachine));
            if (Input.GetKey(KeyCode.LeftShift))
                stateMachine.SwitchState(new RunState(stateMachine));
        }
        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
        }

        if (Input.GetMouseButtonDown(1) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)

            if (stateMachine.targetGameObject != null)
            {
                if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
                    stateMachine.SwitchState(new GrapplingState(stateMachine));
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            if (stateMachine.objectTag == "Door" || stateMachine.objectTag == "Cabinet"
                || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
            {
                stateMachine.SwitchState(new InteractionState(stateMachine));
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!stateMachine.coolActive || Time.time >= stateMachine.lastVPStateTime + stateMachine.coolTime)
            {
                stateMachine.SwitchState(new VPState(stateMachine));
                stateMachine.lastVPStateTime = Time.time;
                stateMachine.coolActive = true;
            }
        }
        if (stateMachine.coolActive && Time.time >= stateMachine.lastVPStateTime + stateMachine.coolTime)
        {
            stateMachine.coolActive = false;
        }
        if (stateMachine.input.explictSit && Input.GetKey(KeyCode.LeftControl))
        {
            stateMachine.SwitchState(new SitState(stateMachine));
        }
        if(Input.GetKeyDown(KeyCode.F10) || Input.GetKeyDown(KeyCode.F12))
        {
            isBossPosition = true;
        }
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

        if (isBossPosition)
        {
            stateMachine.transform.position = new Vector3(-6.49100018f, -31.2539997f, -1.21200001f);
            isBossPosition = false;
        }
    }

    public override void Exit()
    {
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
        isBossPosition = false;

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

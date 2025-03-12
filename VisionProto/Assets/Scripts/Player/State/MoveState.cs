using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float speed;
    Coroutine resetCoroutine;

    private GameObject walkSound;
    private CameraInfomation cameraInformation;
    private bool isMovingSound1;
    private float playSoundTime;

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
        speed = stateMachine.moveSpeed;
        /*stateMachine.originPos = new Vector3(0, 1f, 0);*/
        stateMachine.head.localPosition = stateMachine.originPos;

        walkSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Walk_1, stateMachine.transform);
        isMovingSound1 = true;
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;

        /// Save point와 관련된 공간
        if (stateMachine.isSpawn && !stateMachine.isInitPositionFalse)
        {
            PlayerInformation playerinfo = new PlayerInformation();
            playerinfo = DataManager.Instance.LoadData();
            stateMachine.transform.localPosition = playerinfo.position;
            stateMachine.transform.localRotation = playerinfo.rotation;
            stateMachine.isSpawn = false;
        }

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
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            stateMachine.SwitchState(new IdleState(stateMachine));
        }
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.SwitchState(new JumpState(stateMachine));
        if (!stateMachine.isVPState)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                stateMachine.SwitchState(new RunState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlideState(stateMachine));
        }
        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
        }
        if (!stateMachine.input.isGrounded)
            stateMachine.SwitchState(new FallState(stateMachine));

        if (Input.GetKeyDown(KeyCode.F) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            if (stateMachine.targetGameObject != null)
            {
                if (stateMachine.targetGameObject.CompareTag("Door") || stateMachine.objectTag == "Cabinet"
                    || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
                {
                    stateMachine.SwitchState(new InteractionState(stateMachine));
                }
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
        if (Input.GetMouseButtonDown(1) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
            if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
                stateMachine.SwitchState(new GrapplingState(stateMachine));
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

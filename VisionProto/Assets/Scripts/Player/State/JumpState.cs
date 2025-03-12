using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JumpState : BaseState
{
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    private CameraInfomation cameraInformation;

    public override void Enter()
    {
        if (stateMachine.input.isSit)
        {
            stateMachine.input.isJump = true;
            stateMachine.head.localPosition = stateMachine.sitPos;
        }
        
        if(stateMachine.isVPState)
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
    }
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (stateMachine.isVPState)
            stateMachine.animator.Movement = 1f;
        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlashState(stateMachine));
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
        // tick에서 계속 검사하는건 좀 그래
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

        /// Save point와 관련된 공간
        if (stateMachine.isSpawn && !stateMachine.isInitPositionFalse)
        {
            PlayerInformation playerinfo = new PlayerInformation();
            playerinfo = DataManager.Instance.LoadData();
            stateMachine.transform.localPosition = playerinfo.position;
            stateMachine.transform.localRotation = playerinfo.rotation;
            stateMachine.isSpawn = false;
        }

        ApplyGravity();
        Move();

        if (stateMachine.velocity.y <= 0f && !stateMachine.input.slashDamage)
        {
              stateMachine.SwitchState(new FallState(stateMachine));
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

//         cameraInformation.amplitude = 0f;
//         cameraInformation.frequency = 0f;
//         EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

    }
}
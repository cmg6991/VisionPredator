using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class FallState : BaseState
{
    public FallState(PlayerStateMachine stateMachine) : base(stateMachine) { }
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
        if (Input.GetKeyDown(KeyCode.F) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            if (stateMachine.targetGameObject.CompareTag("Door") || stateMachine.objectTag == "Cabinet"
                || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
            {
                stateMachine.SwitchState(new InteractionState(stateMachine));
            }
        }

        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlashState(stateMachine));
        }
        if (stateMachine.input.isGrounded)
        {
            stateMachine.input.isRay = false;
            stateMachine.SwitchState(new MoveState(stateMachine));
            stateMachine.input.isJump = false;
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
        if (Time.time - lastStateChangeTime < stateLockTime)
            return;

        if (Time.time - startTime > maxFallTime)
        {
            stateMachine.SwitchState(new JumpState(stateMachine));
        }
    }
    public override void Exit()
    {

    }
}
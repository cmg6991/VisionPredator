using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DashState : BaseState
{
    Vector3 dashDirection;
    Vector3 dashStartPos;
    float dashStartTime;
    bool isExiting;
    Vector3 dashVelocity;
    private CameraInfomation cameraInformation;

    public DashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (stateMachine.isVPState)
        {
            stateMachine.animator.OnDash();
            cameraInformation.setting = CameraSetting.Wobble;
            cameraInformation.amplitude = stateMachine.cameraNoiseSetting.vp_Dash_Amplitude;
            cameraInformation.frequency = stateMachine.cameraNoiseSetting.vp_Dash_Frequency;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
        }

        //stateMachine.hardLock.m_Damping = 0.1f;
        dashDirection = stateMachine.direction.forward.normalized;
        dashStartPos = stateMachine.transform.position;
        dashStartTime = Time.time;
        isExiting = false;
        stateMachine.input.isSit = false;
        stateMachine.capsuleCollider.center = new Vector3(0, 0f, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.head.localPosition = stateMachine.originPos;
        stateMachine.input.dashIndex++;
        stateMachine.input.dashDamage = true;
        stateMachine.DashSphere.SetActive(true);

        EffectManager.Instance.ExecutionEffect(Effect.Dash, stateMachine.transform);
        stateMachine.windEffect.SetFloat("_RadialMask_Value", stateMachine.maskValue);
        SoundManager.Instance.PlayEffectSound(SFX.VP_Dash, stateMachine.transform);
    }

    public override void Tick()
    {
        
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

        if (stateMachine.input.isGrounded || stateMachine.input.dashIndex == 1)
        {
            RaycastHit hit;
            Vector3 dashDirectionAdjusted = dashDirection; // 기본값은 기존 대쉬 방향

            // 지면에 있을 때만 Raycast로 경사면 정보 추출
            if (stateMachine.input.isGrounded && Physics.Raycast(stateMachine.transform.position, Vector3.down, out hit, 1.1f))
            {
                // 경사면의 법선 벡터를 기준으로 대쉬 방향 보정
                dashDirectionAdjusted = Vector3.ProjectOnPlane(dashDirection, hit.normal).normalized;
            }

            float t = (Time.time - dashStartTime) / stateMachine.dashDuration;
            if (t <= 1f)
            {
                float smoothStep = t * t * (3f - 2f * t);
                Vector3 dashVelocity = dashDirectionAdjusted * stateMachine.dashSpeed * smoothStep;

                if (stateMachine.input.isGrounded)
                {
                    // 지면에서 대쉬할 때는 경사면 보정된 방향을 사용
                    stateMachine.velocity = dashVelocity;
                    stateMachine.rigid.useGravity = false;
                }
                else
                {
                    float gravityEffect = -0.5f * t;  // 중력 가속도
                    stateMachine.velocity = new Vector3(dashVelocity.x, gravityEffect + stateMachine.velocity.y, dashVelocity.z);
                    stateMachine.rigid.useGravity = true;
                }

                // 최종적으로 Rigidbody에 속도 적용
                stateMachine.rigid.velocity = stateMachine.velocity * stateMachine.VPSpeed;
            }

            float fadeOutValue = Mathf.Lerp(stateMachine.maskValue, 1f, t); // 대쉬가 끝나갈수록 이펙트 사라짐
            stateMachine.windEffect.SetFloat("_RadialMask_Value", fadeOutValue);
        }

        

        Gravity();

        if (!stateMachine.input.isGrounded && stateMachine.input.dashIndex > 1)
        {
            isExiting = true;
        }

        if (Time.time - dashStartTime >= stateMachine.dashDuration || Vector3.Distance(dashStartPos, stateMachine.transform.position) > stateMachine.dashDistance)
        {
            isExiting = true;
        }

        if (isExiting)
        {
            stateMachine.SwitchState(new MoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        //stateMachine.hardLock.m_Damping = 0;
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        stateMachine.velocity = Vector3.zero;
        stateMachine.input.dashDamage = false;
        stateMachine.DashSphere.SetActive(false);
    }
    protected void Gravity()
    {
        // 평지나 공중에서는 수직으로 중력을 적용
        if (!stateMachine.input.isGrounded)
        {
            stateMachine.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime;
        }
    }
}

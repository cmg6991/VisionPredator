using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SlashState : BaseState
{
    public SlashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private float slashDuration = 0.3f;
    private float slashTime;
    private CameraInfomation cameraInformation;
    /*    private Coroutine dampingCoroutine;*/

    public override void Enter()
    {
        stateMachine.hardLock.m_Damping = 0.1f;
        stateMachine.input.slashDamage = true;
        stateMachine.slideBox.SetActive(true);
        stateMachine.input.isSlash = true;

        slashTime = 0f;
        stateMachine.slashPoint = stateMachine.transform.position;
        stateMachine.velocity.y = -50f; // ������ �ϰ� �ӵ� ����
        
        EffectManager.Instance.ExecutionEffect(Effect.Scanner, stateMachine.groundCheck);
        //EffectManager.Instance.ExecutionEffect(Effect.CutDown, stateMachine.groundCheck);
        SoundManager.Instance.PlayEffectSound(SFX.GrandSlam, stateMachine.transform);

        // Camera
        cameraInformation.setting = CameraSetting.Shake;
        cameraInformation.amplitude = stateMachine.cameraNoiseSetting.vp_Slash_Amplitude;
        cameraInformation.frequency = stateMachine.cameraNoiseSetting.vp_Slash_Frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
        //if (dampingCoroutine != null)
        //{
        //    stateMachine.StopCoroutine(dampingCoroutine);
        //}

        //// ���ο� �ڷ�ƾ ����
        //dampingCoroutine = stateMachine.StartCoroutine(ResetDampingAfterDelay(slashDuration));
    }
    //private IEnumerator ResetDampingAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    stateMachine.hardLock.m_Damping = 0f; // ���� Damping ������ �ǵ�����
    //    Debug.Log("��������");
    //}
    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        slashTime += Time.deltaTime;

        if (slashTime >= slashDuration)
        {
            //stateMachine.transform.position = stateMachine.slashPoint;
            stateMachine.hardLock.m_Damping = 0f;
            stateMachine.SwitchState(new MoveState(stateMachine));
        }
    }

    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        //stateMachine.velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        Move();

        if(slashTime >= slashDuration)
        {
            if (CheckSlope())
            {
                // ���鿡���� x�� �ӵ��� �����ϰ�, �߷��� ����
                stateMachine.velocity.x = 0f;
                stateMachine.velocity.z = 0f;

                // y�� �ӵ��� 0���� ����� ���� �� ĳ���Ͱ� �����ǰ� ����
                stateMachine.velocity.y = 0f;
            }
            else
            {
                stateMachine.velocity.y += Physics.gravity.y * Time.fixedDeltaTime; // �߷��� ����
            }
        }
        
        else
        {
            stateMachine.velocity = Vector3.zero;
        }

        // �ϰ� �ӵ��� ����
        if (stateMachine.velocity.y < 0f)
        {
            stateMachine.velocity.y = Mathf.Max(stateMachine.velocity.y, -50f); // �ִ� �ϰ� �ӵ� ����
        }

        //if (stateMachine.input.isGrounded && dampingCoroutine == null)
        //    stateMachine.SwitchState(new MoveState(stateMachine));
    }
    public override void Exit()
    {
        //stateMachine.hardLock.m_Damping = 0f;
        stateMachine.input.isSlash = false;
        stateMachine.slashPoint = stateMachine.transform.position;
        stateMachine.input.slashDamage = false;
        stateMachine.slideBox.SetActive(false);

        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSitState : BaseState
{
    public TutorialSitState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float speed;
    bool isExiting;
    float transitionTime; // ��ȯ�� ���Ǵ� �ð�
    float startTime;
    Vector3 initialHeadPosition;
    private int grapplingLayer;
    private int grapplingPointLayer;
    private CameraInfomation cameraInformation;

    public override void Enter()
    {
        grapplingLayer = LayerMask.GetMask("Grappling");
        grapplingPointLayer = LayerMask.GetMask("GrapplingPoint");

        stateMachine.capsuleCollider.center = new Vector3(0, -0.3f, 0);
        stateMachine.capsuleCollider.height = 1.2f;
        stateMachine.input.isSit = true;
        speed = stateMachine.moveSpeed;
        stateMachine.moveSpeed /= 2f;
        isExiting = false;
        stateMachine.SitCollider.SetActive(true);

        stateMachine.sitPos = new Vector3(0, 0.125f, 0);

        // �ʱ� �� ����
        transitionTime = 0.1f;  // ��ȯ�� ����� �ð� (��)
        startTime = Time.time;  // ��ȯ ���� �ð� ���
        initialHeadPosition = stateMachine.head.localPosition; // ���� �Ӹ� ��ġ�� ����

        // Camera
        cameraInformation.setting = CameraSetting.Handheld_Normal_Mild;
        cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Sit_Amplitude;
        cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Sit_Frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        SoundManager.Instance.PlayEffectSound(SFX.Player_sit, stateMachine.transform);
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        if (!stateMachine.input.isGrounded)
        {
            stateMachine.SwitchState(new TutorialFallState(stateMachine));
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (stateMachine.input.explictSit)
                stateMachine.SwitchState(new TutorialSitState(stateMachine));
            else
                stateMachine.SwitchState(new TutorialIdleState(stateMachine));
        }
        if (!stateMachine.input.explictSit && !Input.GetKey(KeyCode.LeftControl))
        {
            stateMachine.SwitchState(new TutorialIdleState(stateMachine));
        }
        if (Input.GetKeyDown(KeyCode.Space) && !stateMachine.input.isJump)
        {
            isExiting = true;
            stateMachine.input.isJump = false;
            stateMachine.SwitchState(new TutorialJumpState(stateMachine));
        }

        if (Input.GetMouseButtonDown(1))
        {
            isExiting = true;
            stateMachine.ObjectInteraction();

            if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
                stateMachine.SwitchState(new GrapplingState(stateMachine));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isExiting = true;
            stateMachine.ObjectInteraction();

            if (stateMachine.targetGameObject.CompareTag("Door") || stateMachine.objectTag == "Cabinet"
                || stateMachine.objectTag == "Item" || stateMachine.objectTag == "Gun" || stateMachine.objectTag == "Button")
            {
                stateMachine.SwitchState(new InteractionState(stateMachine));
            }
        }

        //if(stateMachine.input.explictSit)
        //    stateMachine.SwitchState(new TutorialSitState(stateMachine));
    }

    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        MoveDirection();
        Move();

        float t = (Time.time - startTime) / transitionTime;
        if (t < 1f)
        {
            stateMachine.head.localPosition = Vector3.Lerp(initialHeadPosition, stateMachine.sitPos, t);
        }
        else
        {
            stateMachine.head.localPosition = stateMachine.sitPos;
        }

        if (isExiting)
        {
            if (!stateMachine.input.isJump)
                stateMachine.SwitchState(new TutorialIdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        cameraInformation.amplitude = 0f;
        cameraInformation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);
        stateMachine.moveSpeed = speed;
        stateMachine.SitCollider.SetActive(false);
    }
}

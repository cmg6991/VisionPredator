using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;


[DefaultExecutionOrder(-2)]
public class GrapplingState : BaseState, IListener
{
    public GrapplingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private Transform gunTip, camera, player;
    private LayerMask objectLayer;
    private LineRenderer lineRenderer;
    private SpringJoint joint;

    private float maxDistance = 1000f;
    private Vector3 grapplePoint;
    private Transform grappleTransform;
    private bool isHooking;
    private Transform direction;
    private GameObject gameObject;

    private bool none;
    private bool isActive;

    private bool isSoundStart;
    private CameraInfomation cameraInformation;

    private bool isGraaplingStart;

    private float distance;

    AnimatorStateInfo stateInfo;

    private float totalTime;

    public override void Enter()
    {
        stateMachine.ObjectInteraction();

        // LayerMask�� �����Ѵ�.
        objectLayer = LayerMask.GetMask("Object");

        stateMachine.velocity.y = Physics.gravity.y;
        // camera, gunTip, Player�� Transform�� �޾ƿ´�.
        camera = GameObject.Find("Virtual Camera").GetComponent<Transform>();
        gunTip = GameObject.Find("GunTip").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<Transform>();

        // gunTip�� �ִ� LineRenderer�� �޾ƿ´�.
        lineRenderer = GameObject.Find("GunTip").GetComponent<LineRenderer>();

        // Player�� SpringJoint�� �߰��Ѵ�.

        joint = player.gameObject.AddComponent<SpringJoint>();

        // Player�� NPC�� �浹���� �ʰ� �����Ѵ�.
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("NPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NormalNPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("StencilNPC"), true);

        //         StartGrappling();
        //         DrawRope();
        gameObject = new GameObject();
        direction = gameObject.transform;

        StartGrappling();

        stateMachine.animator.OnGrappling();
        stateMachine.animator.animator[0].speed = 4f;
        lineRenderer.positionCount = 2;

        // Camera
        cameraInformation.setting = CameraSetting.Wobble;
        cameraInformation.amplitude = stateMachine.cameraNoiseSetting.normal_Grappling_Amplitude;
        cameraInformation.frequency = stateMachine.cameraNoiseSetting.normal_Grappling_Frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

        totalTime = 0f;
        isGraaplingStart = true;
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        totalTime += Time.deltaTime;

        if (isGraaplingStart) 
        {
//             direction.rotation = stateMachine.direction.rotation;
//             direction.position += -Vector3.forward.normalized;
//             stateMachine.rigid.AddForce(direction.rotation * direction.position * 0.03f, ForceMode.Impulse);

            stateInfo = stateMachine.animator.animator[0].GetCurrentAnimatorStateInfo(0);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(index: 0, gunTip.position);
            lineRenderer.SetPosition(index: 1, grapplePoint);
            if (stateInfo.normalizedTime >= 0.9f)
            {
                stateMachine.animator.animator[0].speed = 0;
            }

            //if(totalTime > stateMachine.grapplingReadyTime && stateInfo.normalizedTime >= 0.9f)
            if(stateInfo.normalizedTime >= 0.9f)
            {
                GraaplingHooking();
                DrawRope();
                isGraaplingStart = false;
            }
            return;
        }

        DrawRope();
        RopeRebound();

        if(totalTime > 5f)
        {
            if (stateMachine.isTutorial)
                stateMachine.SwitchState(new TutorialJumpState(stateMachine));
            else
                stateMachine.SwitchState(new JumpState(stateMachine));
        }

        // �Ӹ��� ����� �� ���� isGrounded�� true�� �ȴ� => ����� ��� ������ �ٲ�� �� ����.
        if (stateMachine.input.isGrounded && isHooking)
        {
            if (stateMachine.isTutorial)
                stateMachine.SwitchState(new TutorialFallState(stateMachine));
            else
                stateMachine.SwitchState(new FallState(stateMachine));
        }

        // jump grappling�� �� �ٸ���.
        if (none && stateMachine.input.isGrounded)
        {
            if (stateMachine.isTutorial)
                stateMachine.SwitchState(new TutorialFallState(stateMachine));
            else
                stateMachine.SwitchState(new FallState(stateMachine));
        }

    }

    public override void FixedTick()
    {
        if (stateMachine.isPause)
            return;
        if (isHooking || none)
        {
            stateMachine.rigid.AddForce(new Vector3(0, Physics.gravity.y * 10f, 0));
        }
    }

    public override void Exit()
    {
        isHooking = false;
        none = false;
        Object.Destroy(gameObject);
        lineRenderer.positionCount = 0;
        stateMachine.animator.animator[0].speed = 1;

        if (joint != null) 
            Object.Destroy(joint);

        EventManager.Instance.RemoveEvent(EventType.Hooking);
        stateMachine.targetGameObject = null;
        isGraaplingStart = false;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NormalNPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("StencilNPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("NPC"), true);
    }



    void StartGrappling()
    {
        if (isActive)
            return;

        RaycastHit hit;
        isActive = true;
        isSoundStart = false;
        
        // ī�޶� ��ġ, �� ��ġ, ���, �ִ� �Ÿ�, � LayerMask��?
        if (Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance, objectLayer))
        {
            isHooking = false;

            grappleTransform = hit.transform;
            grapplePoint = hit.point;

            float distanceFromPoint = Vector3.Distance(a: player.position, b: grapplePoint);

            distance = distanceFromPoint;

            Debug.Log(distance);

            // ���� ��¼�� grappling point�� �ɷ��� �ϴµ� �ٸ����� �ɸ���
            IsRayTarget(grappleTransform, distance);
            SoundManager.Instance.PlayEffectSound(SFX.Grappling_Start, stateMachine.transform);

            

            // ���Ⱑ �װ��ΰ�?

            //             if (joint == null)
            //                 joint = player.gameObject.AddComponent<SpringJoint>();
            // 
            //             joint.autoConfigureConnectedAnchor = false;
            //             joint.connectedAnchor = grapplePoint;
            // 
            // 
            //             // Grapple point �Ÿ��� ������ �� �ִ�.
            //             /// ���� ��ġ�� ������ �� �ִ�.
            //             joint.maxDistance = distance * 0.2f;
            //             joint.minDistance = distance * 0.1f;
            // 
            //             // �̰� ��Ȳ�� �°� �����ϸ�ȴ�.
            //             /// �ӵ��� ������ �� �ִ�?
            //             // joint.spring = 3f;
            //             // joint.massScale = 1.0f;
            //             joint.damper = 0.1f;
            // 
            //             // position Count�� �̿��ؼ� lineRenderer�� �׸���.
            //             lineRenderer.positionCount = 2;
            //             SoundManager.Instance.PlayEffectSound(SFX.Grappling_Start, stateMachine.transform);

        }
        else
        {
            Object.Destroy(joint);
            none = true;
        }
    }

    private void GraaplingHooking()
    {
        EventManager.Instance.AddEvent(EventType.Hooking, OnEvent);

        if (joint == null)
            joint = player.gameObject.AddComponent<SpringJoint>();

        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;


        // Grapple point �Ÿ��� ������ �� �ִ�.
        /// ���� ��ġ�� ������ �� �ִ�.
        joint.maxDistance = distance * 0.2f;
        joint.minDistance = distance * 0.1f;

        // �̰� ��Ȳ�� �°� �����ϸ�ȴ�.
        /// �ӵ��� ������ �� �ִ�?
        joint.spring = stateMachine.grapplingSpring;
        joint.massScale = stateMachine.graappingMassScale;
        joint.damper = stateMachine.grapplingDamper;

        // position Count�� �̿��ؼ� lineRenderer�� �׸���.
    }


    void StopGrappling()
    {
        stateMachine.animator.animator[0].speed = 1f;

        lineRenderer.positionCount = 0;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NormalNPC"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("StencilNPC"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("NPC"), false);
        isActive = false;

        if(!isSoundStart)
        {
            cameraInformation.amplitude = 0f;
            cameraInformation.frequency = 0f;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInformation);

            SoundManager.Instance.PlayEffectSound(SFX.Grappling_End, stateMachine.transform);
            isSoundStart = true;
        }


        // Add Component ����
        if (joint != null)
        {
            Object.Destroy(joint);
        }
    }

    void DrawRope()
    {
        if (!joint)
            return;

        if (joint.connectedAnchor != grapplePoint)
            return;

        // �׸� ��ġ gun Tip�� ��ġ, grapplePoint ��ġ
        lineRenderer.SetPosition(index: 0, gunTip.position);
        lineRenderer.SetPosition(index: 1, grapplePoint);
    }

    void RopeRebound()
    {
        // ����Ű�� �޾ƿ´�.
        float horizon = stateMachine.input.hAxis;
        float vertical = stateMachine.input.vAxis;

        // ���� Ű�� ���� �����δ�. AddForce�� ���ش�.

        // ȸ���� �޾ƿ��� �ʹ�.
        direction.rotation = stateMachine.direction.rotation;

        // �Ź� ���ö����� 0,0,0���� �ʱ�ȭ ���ش�.
        direction.position = Vector3.zero;

        /// ����
        if (horizon == -1)
            direction.position += -Vector3.right.normalized  /** stateMachine.grapplingSpee*/;
        /// ������
        if (horizon == 1)
            direction.position += Vector3.right.normalized /** stateMachine.grapplingSpeed*/;
        /// �Ʒ�
        if (vertical == -1)
            direction.position += -Vector3.forward.normalized /** stateMachine.grapplingSpeed*/;
        /// ��
        if (vertical == 1)
            direction.position += Vector3.forward.normalized /** stateMachine.grapplingSpeed*/;

        // direction ���� �ް� Addforce�� ���ִµ�, SRT -> Rotaion * Transpose�� ���ָ� �ȴ�. 0.1 �����ָ� �ʹ� ���� �����.
        stateMachine.rigid.AddForce(direction.rotation * direction.position * 0.1f, ForceMode.Impulse);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Hooking:
                {
                    StopGrappling();
                    isHooking = true;
                    none = false;
                }
                break;
        }
    }

    // LayerMask ���� �̿��ϰ� �;� �׷��� �� �� ������
    void IsRayTarget(Transform _transform, float distance)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        Camera camera = null;

        if (brain != null)
            camera = brain.OutputCamera;

        if (camera != null)
        {
            // camera���� viewport Point�� �����´�.
            Vector3 viewportPoint = camera.WorldToViewportPoint(_transform.position);

            // viewport ���� Ray�� ��� �׸���.
            Ray ray = camera.ViewportPointToRay(viewportPoint);

            // Layer Mask�� ����� �Գ�

            LayerMask enemyLayerMask = ~LayerMask.GetMask("NPC", "StencilNPC", "NormalNPC");

            // Ray �� ��ü���� ��� ��´�.
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, enemyLayerMask);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // �ƴ� ���ڱ� Layer 13, 11�� �ߵǴ��� ���ڱ� 2^n���� �ǰ� ����?

            // ù��° Ray ���µ� layer�� Grappling�� �ƴϸ� false�� �����Ѵ�.
            // ���⼭ Layer ��ſ� tag�� �ϸ� Layer ������ ���� �� ���� �� ����.
            string targetTag = default;

            foreach (RaycastHit hit in hits)
            {
                targetTag = hit.transform.gameObject.tag;

                if (targetTag == "GrapplingPoint")
                    grapplePoint = hit.transform.position;
            }
        }
    }
}

/// �ʿ� ���������� Ȥ�� �𸣴� ����ٰ� �����ص���.
//if (Input.GetKeyDown(KeyCode.F))

//         if (Input.GetMouseButtonDown(1))
//         {
//             StartGrappling();
//         }
//         if (Input.GetKeyDown(KeyCode.LeftControl))
//         {
//             stateMachine.SwitchState(new SlashState(stateMachine));
//         }

//         if (Input.GetKeyDown(KeyCode.Q))
//         {
//             dashSpeed = 5.0f;
//             dashKeyDown = true;
//             totalTime = 0.0f;
//         }

//if (Input.GetKeyUp(KeyCode.F))
//         if (Input.GetMouseButtonUp(1))
//         {
//             StopGrappling();
//             isHooking = true;
//             none = false;
//         }
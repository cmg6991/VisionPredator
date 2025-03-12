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

        // LayerMask를 세팅한다.
        objectLayer = LayerMask.GetMask("Object");

        stateMachine.velocity.y = Physics.gravity.y;
        // camera, gunTip, Player의 Transform을 받아온다.
        camera = GameObject.Find("Virtual Camera").GetComponent<Transform>();
        gunTip = GameObject.Find("GunTip").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<Transform>();

        // gunTip에 있는 LineRenderer를 받아온다.
        lineRenderer = GameObject.Find("GunTip").GetComponent<LineRenderer>();

        // Player에 SpringJoint를 추가한다.

        joint = player.gameObject.AddComponent<SpringJoint>();

        // Player와 NPC가 충돌하지 않게 설정한다.
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

        // 머리에 닿았을 때 순간 isGrounded가 true가 된다 => 사실은 닿고 있으면 바뀌는 것 같다.
        if (stateMachine.input.isGrounded && isHooking)
        {
            if (stateMachine.isTutorial)
                stateMachine.SwitchState(new TutorialFallState(stateMachine));
            else
                stateMachine.SwitchState(new FallState(stateMachine));
        }

        // jump grappling은 좀 다르다.
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
        
        // 카메라 위치, 쏠 위치, 출력, 최대 거리, 어떤 LayerMask에?
        if (Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance, objectLayer))
        {
            isHooking = false;

            grappleTransform = hit.transform;
            grapplePoint = hit.point;

            float distanceFromPoint = Vector3.Distance(a: player.position, b: grapplePoint);

            distance = distanceFromPoint;

            Debug.Log(distance);

            // 가끔 어쩌다 grappling point에 걸려야 하는데 다른곳에 걸리네
            IsRayTarget(grappleTransform, distance);
            SoundManager.Instance.PlayEffectSound(SFX.Grappling_Start, stateMachine.transform);

            

            // 여기가 그곳인가?

            //             if (joint == null)
            //                 joint = player.gameObject.AddComponent<SpringJoint>();
            // 
            //             joint.autoConfigureConnectedAnchor = false;
            //             joint.connectedAnchor = grapplePoint;
            // 
            // 
            //             // Grapple point 거리를 조절할 수 있다.
            //             /// 갈고리 위치를 조절할 수 있다.
            //             joint.maxDistance = distance * 0.2f;
            //             joint.minDistance = distance * 0.1f;
            // 
            //             // 이건 상황에 맞게 조절하면된다.
            //             /// 속도를 조절할 수 있다?
            //             // joint.spring = 3f;
            //             // joint.massScale = 1.0f;
            //             joint.damper = 0.1f;
            // 
            //             // position Count를 이용해서 lineRenderer를 그린다.
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


        // Grapple point 거리를 조절할 수 있다.
        /// 갈고리 위치를 조절할 수 있다.
        joint.maxDistance = distance * 0.2f;
        joint.minDistance = distance * 0.1f;

        // 이건 상황에 맞게 조절하면된다.
        /// 속도를 조절할 수 있다?
        joint.spring = stateMachine.grapplingSpring;
        joint.massScale = stateMachine.graappingMassScale;
        joint.damper = stateMachine.grapplingDamper;

        // position Count를 이용해서 lineRenderer를 그린다.
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


        // Add Component 삭제
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

        // 그릴 위치 gun Tip의 위치, grapplePoint 위치
        lineRenderer.SetPosition(index: 0, gunTip.position);
        lineRenderer.SetPosition(index: 1, grapplePoint);
    }

    void RopeRebound()
    {
        // 방향키를 받아온다.
        float horizon = stateMachine.input.hAxis;
        float vertical = stateMachine.input.vAxis;

        // 방향 키에 따라서 움직인다. AddForce를 해준다.

        // 회전값 받아오고 싶다.
        direction.rotation = stateMachine.direction.rotation;

        // 매번 들어올때마다 0,0,0으로 초기화 해준다.
        direction.position = Vector3.zero;

        /// 왼쪽
        if (horizon == -1)
            direction.position += -Vector3.right.normalized  /** stateMachine.grapplingSpee*/;
        /// 오른쪽
        if (horizon == 1)
            direction.position += Vector3.right.normalized /** stateMachine.grapplingSpeed*/;
        /// 아래
        if (vertical == -1)
            direction.position += -Vector3.forward.normalized /** stateMachine.grapplingSpeed*/;
        /// 위
        if (vertical == 1)
            direction.position += Vector3.forward.normalized /** stateMachine.grapplingSpeed*/;

        // direction 값을 받고 Addforce를 해주는데, SRT -> Rotaion * Transpose를 해주면 된다. 0.1 안해주면 너무 빨라서 해줬다.
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

    // LayerMask 까지 이용하고 싶어 그러면 될 거 같은데
    void IsRayTarget(Transform _transform, float distance)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        Camera camera = null;

        if (brain != null)
            camera = brain.OutputCamera;

        if (camera != null)
        {
            // camera에서 viewport Point를 가져온다.
            Vector3 viewportPoint = camera.WorldToViewportPoint(_transform.position);

            // viewport 에서 Ray를 쏘고 그린다.
            Ray ray = camera.ViewportPointToRay(viewportPoint);

            // Layer Mask를 해줘야 먹네

            LayerMask enemyLayerMask = ~LayerMask.GetMask("NPC", "StencilNPC", "NormalNPC");

            // Ray 쏜 물체들을 모두 담는다.
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, enemyLayerMask);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // 아니 갑자기 Layer 13, 11로 잘되더니 갑자기 2^n으로 되고 있지?

            // 첫번째 Ray 쐈는데 layer가 Grappling이 아니면 false로 리턴한다.
            // 여기서 Layer 대신에 tag로 하면 Layer 개수를 줄일 수 있을 것 같다.
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

/// 필요 없어졌지만 혹시 모르니 여기다가 저장해두자.
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
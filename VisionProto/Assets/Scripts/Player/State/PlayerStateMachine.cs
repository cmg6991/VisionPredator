using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class PlayerStateMachine : StateMachine, IListener
{
    public Vector3 velocity;
    public float moveSpeed = 10f;
    public float jumpForce = 2f;
    public float grapplingSpeed = 2.0f;
    public float dashDistance = 10f;
    public float dashDuration = 0.2f;
    public float dashSpeed = 10f;
    public float slideDistance = 15f;
    public float slideSpeed = 3f;
    public float slideDuration = 0.5f;
    public float fallSpeed = 5f;

    public float VPSpeed = 30f;
    public float VPJumeForce = 3f;

    public float maskValue = 0.3f;

    [SerializeField]
    public float grapplingReadyTime = 0.5f;

    public Transform direction { get; private set; }
    public Transform mainCamera { get; private set; }
    public Transform head { get; private set; }
    public Transform groundCheck;
    public Rigidbody rigid;
    public CapsuleCollider capsuleCollider;
    public GameObject slideBox;
    public BoxCollider boxCollider;
    public InputTest input;
    public Vector3 originPos;
    public Vector3 sitPos;
    public Vector3 slashPoint;

    public PlayerAnimator animator;

    // 마우스를 통해 인식하는 GameObject
    public GameObject targetGameObject;
    // 무기를 장착했니?
    public bool isEquiped;
    // 마우스를 통해 인식하는 Layer
    public int layerMask;
    // Object Tag
    public string objectTag;

    // Player 초기 Spawn을 알려주는 bool
    public bool isSpawn;

    // VP 상태인가요?
    public bool isVPState;

    public string gunName;

    public GameObject VPStateRange;

    //vp상태랑 인간상태 쿨타임 전환시간
    public bool coolActive = false;
    public float coolTime = 5f;
    public float lastVPStateTime;

    public Material windEffect;

    public CinemachineVirtualCamera virtualCamera;
    public CinemachineHardLockToTarget hardLock;

    public GameObject DashSphere;
    public SphereCollider spherecollider;
    public MeshRenderer meshRenderer;

    public GameObject SitCollider;
    public CapsuleCollider sitcollider;

    public GameObject vpSound;

    public float grapplingSpring = 10f;
    public float graappingMassScale = 1.0f;
    public float grapplingDamper = 0.01f;

    public bool isPause;

    public bool isInitPositionFalse;

    public float objectDistance = 2f;
    public float gunDistance = 5f;
    public float grapplingDistance = 10f;

    public CameraNoiseProfile cameraNoiseSetting;

    //튜토리얼 관련 변수
    public bool isTutorial;

    private void Start()
    {
        isVPState = true;
        boxCollider = slideBox.GetComponent<BoxCollider>();
        spherecollider = DashSphere.GetComponent<SphereCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        input = GetComponent<InputTest>();
        head = GameObject.Find("Head").GetComponent<Transform>();
        mainCamera = Camera.main.transform;
        direction = GameObject.Find("Direction").GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        hardLock = virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        sitcollider = SitCollider.GetComponent<CapsuleCollider>();



        GameObject vpState = Resources.Load<GameObject>("Player/VP State");
        VPStateRange = vpState;
        VPStateRange = Object.Instantiate(VPStateRange);

        TutorialWeapon tutorialWeapon = new TutorialWeapon();
        tutorialWeapon.isVPWeapon = isTutorial;
        tutorialWeapon.isDagger = isTutorial;

        EventManager.Instance.NotifyEvent(EventType.Tutorial, tutorialWeapon);
        DataManager.Instance.isTutorial = isTutorial;

        if(isTutorial)
            SwitchState(new TutorialIdleState(this));
        else
            SwitchState(new IdleState(this));

        originPos = /*transform.position - head.localPosition*/new Vector3(0, 1f, 0);
        sitPos = new Vector3(0, 0.125f, 0);
        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
        EventManager.Instance.AddEvent(EventType.EquipedGunName, OnEvent);
        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        isEquiped = false;

        animator = GetComponent<PlayerAnimator>();
        isSpawn = true;
        vpSound = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.VP_Am);
        EventManager.Instance.NotifyEvent(EventType.VPState, isVPState);
        DataManager.Instance.isVPState = isVPState;

        InitializeCameraSetting();
    }

    


    /// 여기다가 isEquiped를 추가해야 할 것 같아.
    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.isEquiped:
                {
                    isEquiped = (bool)param;
                }
                break;
            case EventType.EquipedGunName:
                {
                    if(param != null)
                        gunName = (string)param;
                }
                break;
            case EventType.isPause:
                {
                    isPause = (bool)param;
                }
                break;
        }
    }

    public void ObjectInteraction()
    {
        // 1. 마우스에 Ray를 쏜다.
        // 2. layer가 Grappling, Gun이면 각각에 맞는 State로 이동한다.
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        LayerMask npcDectorLayerMask = ~LayerMask.GetMask("NPC", "DeadNPC", "StencilNPC", "Player", "StackingCamera", "CCTVArea");

        RaycastHit hit;

        if (Physics.Raycast(cameraRay, out hit, 1000f, npcDectorLayerMask))
        {
            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            Camera cinemachineCamera = null;

            if (brain != null)
                cinemachineCamera = brain.OutputCamera;

            if (cinemachineCamera != null)
            {
                // camera에서 viewport Point를 가져온다.
                Vector3 viewportPoint = cinemachineCamera.WorldToViewportPoint(hit.transform.position);

                // viewport 에서 Ray를 쏘고 그린다.
                Ray ray = cinemachineCamera.ViewportPointToRay(viewportPoint);

                // Ray 쏜 물체들을 모두 담는다. 이상한 것 까지 다 담는다.
                RaycastHit[] hits = Physics.RaycastAll(ray, 10000f, npcDectorLayerMask);

                RaycastHit raycastHit = hit;

                // hit과 나의 거리를 계산하자
                float distance = Vector3.Distance(this.transform.position, hit.point);
                
                int layer = hit.transform.gameObject.layer;
                targetGameObject = hit.transform.gameObject;
                objectTag = hit.transform.gameObject.tag;
                layerMask = (int)Mathf.Pow(2, layer);

                Debug.Log(raycastHit.collider.gameObject.name);

                if (distance > objectDistance)
                {
                    // Tag, Item, Cabinet
                    if(objectTag == "Item" || objectTag == "Cabinet" || objectTag == "Door" || objectTag == "Button" || objectTag == "Gun")
                        objectTag = "Default";
                    return;
                }

                if(distance > gunDistance)
                {
                    if(objectTag == "Gun")
                        objectTag = "Default";
                    return;
                }

                if (distance > grapplingDistance)
                {
                    if(objectTag == "GrapplingPoint" || objectTag == "Grappling")
                        objectTag = "Default";
                    return;
                }

                if (hits.Length == 0)
                    return;

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                // 순회하면서 Grappling Point나 Grappling에 닿아야 하는데?
                
            }
        }
    }

    private void InitializeCameraSetting()
    {
        cameraNoiseSetting = new CameraNoiseProfile();

        cameraNoiseSetting.normal_Idle_Amplitude = 1.0f;
        cameraNoiseSetting.normal_Idle_Frequency = 1.0f;

        cameraNoiseSetting.vp_Idle_Amplitude = 1.0f;
        cameraNoiseSetting.vp_Idle_Frequency = 1.0f;

        cameraNoiseSetting.normal_Move_Amplitude = 1.0f;
        cameraNoiseSetting.normal_Move_Frequency = 1.0f;

        cameraNoiseSetting.vp_Move_Amplitude = 5f;
        cameraNoiseSetting.vp_Move_Frequency = 1f;

        cameraNoiseSetting.normal_Run_Amplitude = 3f;
        cameraNoiseSetting.normal_Run_Frequency = 1f;

        cameraNoiseSetting.normal_Jump_Amplitude = 10f;
        cameraNoiseSetting.normal_Jump_Frequency = 0.3f;

        cameraNoiseSetting.vp_Jump_Amplitude = -20f;
        cameraNoiseSetting.vp_Jump_Frequency = 1f;

        cameraNoiseSetting.normal_Sit_Amplitude = 0.5f;
        cameraNoiseSetting.normal_Sit_Frequency = 1f;

        cameraNoiseSetting.normal_Slide_Amplitude = 0f;
        cameraNoiseSetting.normal_Slide_Frequency = 0f;

        cameraNoiseSetting.vp_Dash_Amplitude = 0.8f;
        cameraNoiseSetting.vp_Dash_Frequency = 0.5f;

        cameraNoiseSetting.vp_Slash_Amplitude = 0.5f;
        cameraNoiseSetting.vp_Slash_Frequency = 0.8f;

        cameraNoiseSetting.normal_Grappling_Amplitude = 0.1f;
        cameraNoiseSetting.normal_Grappling_Frequency = 0.5f;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 boxSize = new Vector3(transform.lossyScale.x, 0.5f, transform.lossyScale.z);
    //    Gizmos.DrawCube(groundCheck.position, boxSize);
    //}
}

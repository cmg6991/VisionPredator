using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 권총 종류인 Bertta 92다.
/// </summary>
[DefaultExecutionOrder(4)]
public class Beretta92 : MonoBehaviour, IWeapon, IListener
{
    [SerializeField]
    private WeaponInformation weaponInformation;

    [SerializeField]
    private OutlineWeapon outlineWeapon;

    public Transform bulletTransform;
    private CameraShake cameraShake;

    public WeaponCollider weaponCollider;


    // 총 발사시 카메라 흔들림
    [SerializeField]
    private float amplitude = 1f;
    [SerializeField]
    private float frequency = 1f;

    // 장착할 위치
    private Transform gunContainer;

    // 총기가 위치해야할 공간
    public Transform gunTransform;
    private Vector3 throwDirection;

    // 총을 잡았을 때 총의 크기를 원래대로 돌릴 매직넘버다. 일단은..
    private readonly Vector3 position = new Vector3(0.278800011f, 0.657299995f, 0.0439999998f);
    private readonly Vector3 rotation = new Vector3(295.146759f, 118.789f, 86.246f);
    private readonly Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);
    private readonly Vector3 gunContainerPosition = new Vector3(-0.0900000036f, 0.170000002f, -0.0599999987f);

    private Rigidbody rigidbody;
    private MeshCollider meshCollider;

    // Ray Research에 필요한 변수들
    private Vector3 direction;
    private Vector3 targetPoint;


    private float totalTime;
    private int shotCount;
    private int currentBullet;

    private bool isEquipped;
    private bool isPickup;
    private bool isEmpty;

    private bool isChanging;
    private bool isShoting;

    [SerializeField]
    private float changeThrowingSpeed;

    private bool isShoot;

    // 총을 쏘고 있을 때 활성화 되는 Camera
    private bool isShotCamera;

    // 총을 쏘고 있을 때 사용하는 총 시간
    private float totalCameraTime;

    private UIBulletInformation bulletInfomation;

    private int throwingGunLayer;
    private int stackingLayer;
    private int gunLayer;

    //애니메이션 관련 변수
    private AnimationInformation Pinfo;

    // Camera 움직임
    CameraInfomation cameraInfomation;

    private bool isInitalize;
    private bool isPause;

    private bool isTutorial;

    // Light
    private Light light;
    void Start()
    {
        InitalizeRecoilSetting();
        Pickup();
    }

    void Update()
    {
        if (isPause)
            return;

        if (this.enabled)
            Pickup();

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // 어떤 곳에서 이걸 해주겠지? 이것을 받아서 사용하자.
            DataManager.Instance.isTutorial = true;
            isTutorial = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 어떤 곳에서 이걸 해주겠지? 이것을 받아서 사용하자.
            DataManager.Instance.isTutorial = false;
            isTutorial = false;
        }


        if (Input.GetMouseButtonDown(0))
        {
            Pinfo.stateName = "PAttack";
            Pinfo.layer = -1;
            Pinfo.normalizedTime = 0;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Pinfo);

            if (isEmpty && isEquipped)
            {
                Drop();
                return;
            }

            if (isShoting)
                return;

            SaveFovValue();

            if (cameraShake.gunRecoil)
            {
                Shot();
                RecoilFire();
            }
        }

        // 추가적인게 필요하다. 총을 쐈을 때 흔들림
        if (isShotCamera)
        {
            totalCameraTime += Time.deltaTime;

            if (totalCameraTime > frequency)
            {
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
                totalCameraTime = 0f;
                isShotCamera = false;
            }
        }

        if (isShoting)
        {
            //if(totalTime > weaponInformation.shotDelay * 0.5f)
            totalTime += Time.deltaTime;

            if (totalTime > weaponInformation.shotDelay)
            {
                isShoting = false;
                light.enabled = false;
                totalTime = 0f;
                cameraShake.isRecoil = true;
                cameraShake.isMouseDown = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            RecoilRecovery();
            return;
        }

        if (isEmpty)
            return;

        if (currentBullet >= weaponInformation.maxBullet)
        {
            isEmpty = true;
        }
        else
            isEmpty = false;

        //         if (isEmpty)
        //             return;

        // 연사 불가능
        //         if (Input.GetMouseButton(0))
        //         {
        //             totalTime += Time.deltaTime;
        //             if (totalTime > shotDelay)
        //             {
        //                 Shot();
        //                 RecoilFire();
        //                 totalTime = 0f;
        //             }
        //         }


    }

    public void SetUP(Vector3 position, Quaternion rotation)
    {
        this.gameObject.transform.localPosition = position;
        this.gameObject.transform.localRotation = rotation;

        if (!isInitalize)
        {
            InitalizeRecoilSetting();
            return;
        }

        gunTransform.gameObject.tag = "Gun";
        gunTransform.gameObject.layer = gunLayer;

        currentBullet = 0;
        gunTransform.transform.position = Vector3.zero;
        gunTransform.transform.rotation = Quaternion.identity;
        bulletInfomation.currentBullet = currentBullet;
        EventManager.Instance.RemoveEvent(EventType.Pickup);
        isEquipped = false;
        isEmpty = false;
        outlineWeapon.isDone = false;
        outlineWeapon.isRePosition = false;
        outlineWeapon.isStop = false;
        weaponCollider.boxCollider.enabled = false;
        isTutorial = DataManager.Instance.isTutorial;

        outlineWeapon.isRePosition = true;
        Invoke("FalseTestBool", 1.0f);
    }

    private void FalseTestBool()
    {
        Rigidbody rigidbody = gunTransform.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
    }

    // 초반에 필요한 반동 세팅
    public void InitalizeRecoilSetting()
    {
        isInitalize = true;

        // Weapon information setting
        weaponInformation = new WeaponInformation();
        weaponInformation.xRecoil = 0f;
        weaponInformation.yRecoil = 0.5f;
        //weaponInformation.yRecoil = 0f; 
        weaponInformation.snappiness = 20f;
        weaponInformation.returnSpeed = 50f;
        weaponInformation.recoilSpeed = 1000f;

        weaponInformation.bulletSpeed = 130f;
        weaponInformation.distance = 1500f;
        weaponInformation.maxBullet = 12;
        weaponInformation.shotDelay = 0.1f;
        weaponInformation.throwSpeed = 60f;

        changeThrowingSpeed = 5f;

        // cameraShake setting
        cameraShake = FindObjectOfType<CameraShake>();
        cameraShake.weaponInformation = weaponInformation;

        // Gun bullet Setting

        meshCollider = GetComponentInChildren<MeshCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        currentBullet = 0;
        shotCount = 0;

        // 이 스크립트가 활성화 되었다는 것은 활성화가 되었다는 뜻이다.
        isEquipped = false;
        isEmpty = false;

        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);

        // 튜토리얼 전용 추가
        isTutorial = DataManager.Instance.isTutorial;

        bulletInfomation = new UIBulletInformation();
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;
        bulletInfomation.weaponSelect = UIWeaponSelect.Pistol;

        cameraInfomation = new CameraInfomation();
        cameraInfomation.setting = CameraSetting.PistolNoise;

        int throwLayer = LayerMask.GetMask("ThrowWeapon");
        throwingGunLayer = (int)(Mathf.Log(throwLayer) / Mathf.Log(2));

        int stacking = LayerMask.GetMask("StackingCamera");
        stackingLayer = (int)(Mathf.Log(stacking) / Mathf.Log(2));

        int gun = LayerMask.GetMask("Gun");
        gunLayer = (int)(Mathf.Log(gun) / Mathf.Log(2));

        gunTransform.gameObject.layer = gunLayer;
        gunTransform.gameObject.tag = "Gun";

        // setactive false 되어 있는 자식들을 전부 true로 하고 싶어

        foreach (Transform child in bulletTransform.transform)
        {
            if (!child.gameObject.activeSelf)
                child.gameObject.SetActive(true);
        }

        light = bulletTransform.GetComponentInChildren<Light>();
        light.enabled = false;
    }


    // 총알을 쐈을 때 초기 설정값.
    public void SaveFovValue()
    {
        cameraShake.isMouseDown = true;
        cameraShake.isRecoil = false;
        cameraShake.mouseDistance = Vector3.zero;
    }


    // 사격했을 때의 반동
    public void RecoilFire()
    {
        // Camera Shake는 반동에 따른 카메라의 이동만 해야한다.
        // 반동의 계산은 여기서 해주자.

        float recoilX = weaponInformation.xRecoil;
        float recoilY = weaponInformation.yRecoil;
        float recoilZ = 0f;
        cameraShake.targetRotaion += new Vector3(recoilZ, recoilY, recoilX);

        shotCount++;
    }

    // 총알이 날라간다.
    public void Shot()
    {
        if (isEmpty) return;
        isShoting = true;
        isShotCamera = true;

        cameraInfomation.amplitude = amplitude;
        cameraInfomation.frequency = frequency;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

        //cameraShake.SetCameraNoise(amplitude, frequency);

        Rigidbody bulletRigidBody;

        float bulletSpeed = weaponInformation.bulletSpeed;
        RayResearch();

        if (currentBullet < weaponInformation.maxBullet)
        {
            // bullet 생성
            //GameObject bullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);
            GameObject bullet = PoolManager.Instance.GetPooledObject("PBeretta92", bulletTransform.position, Quaternion.identity);
            bullet.transform.position = bulletTransform.position;
            //bullet.transform.rotation = Quaternion.identity;
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            bulletRigidBody = bullet.GetComponent<Rigidbody>();
        }
        else
            bulletRigidBody = null;

        if (currentBullet < weaponInformation.maxBullet && bulletRigidBody != null)
        {
            EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = true);
            bulletRigidBody.velocity = direction * bulletSpeed;
            light.enabled = true;
        }
        else
        {
            isEmpty = true;
            RecoilRecovery();
        }
        currentBullet++;

        // Bullet 상태
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
        EffectManager.Instance.ExecutionEffect(Effect.PistolShot, bulletTransform.position, Quaternion.LookRotation(direction), 1f);
        SoundManager.Instance.PlayEffectSound(SFX.Pistol, this.transform.parent);

        if (isTutorial)
        {
            /// 튜토리얼에서 무기 던지기 튜토리얼이 아니면 총알이 무제한이다.
            if (TutorialManager.Instance.isBulletInfinite)
            {
                if (currentBullet >= weaponInformation.maxBullet - 1)
                {
                    currentBullet = 0;
                }
            }
        }
    }

    // 반동 회복
    public void RecoilRecovery()
    {

        //cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.isRecoil = true;
        cameraShake.isMouseDown = false;
        shotCount = 0;
        EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = false);
    }

    // 무기 주웠을 때
    public void Pickup()
    {
        if (!isEquipped)
        {
            EventManager.Instance.AddEvent(EventType.Change, OnEvent);
            EventManager.Instance.AddEvent(EventType.GunInformation, OnEvent);
            EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
            EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);

            gunContainer = GameObject.Find("GunContainer").GetComponent<Transform>();

            if (this.transform.parent != null)
                SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform.parent);
            else
                SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform);

            EventManager.Instance.NotifyEvent(EventType.EquipedGunName, "Pistol");

            //무기 아이들 상태 애니메이션
            Pinfo.stateName = "PDraw";
            Pinfo.layer = -1;
            Pinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Pinfo);

            //Pinfo.stateName = "PIdle";
            //Pinfo.layer = -1;
            //Pinfo.normalizedTime = 0f;
            //EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Pinfo);
        }

        /// Pickup 했을 때 퀘스트가 완료되면 이렇게 넘어갈 변수가 필요해.
        if (isTutorial)
        {
            if (TutorialManager.Instance.currentState == TutorialStage.Draw || 
                TutorialManager.Instance.currentState == TutorialStage.HandAttack)
                TutorialManager.Instance.isPickUp = true;
            else
                TutorialManager.Instance.isPickUp = false;
        }

        weaponCollider.isEquipped = true;
        weaponCollider.enabled = false;
        outlineWeapon.isDone = false;
        outlineWeapon.isMouseEnter = false;
        outlineWeapon.GunLayer();

        gunTransform.gameObject.layer = stackingLayer;
        cameraShake.weaponInformation = weaponInformation;
        isEquipped = true;
        isPickup = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = scale;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        gunTransform.localPosition = position;
        gunTransform.localRotation = Quaternion.Euler(rotation);
        gunTransform.localScale = Vector3.one;

        gunContainer.transform.localPosition = Vector3.zero;
    }

    // 무기 던졌을 때
    public void Drop()
    {
        EventManager.Instance.AddEvent(EventType.Throwing, OnEvent);
        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
        SoundManager.Instance.PlayEffectSound(SFX.Weapon_Throwing, this.transform.parent);
        EventManager.Instance.NotifyEvent(EventType.EquipedGunName, "");
        EventManager.Instance.RemoveEvent(EventType.isPause, OnEvent);

        cameraInfomation.amplitude = 0f;
        cameraInfomation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

        weaponCollider.enabled = true;
        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.isTrigger = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = true;

        outlineWeapon.isRePosition = false;
        outlineWeapon.isDone = true;
        gunTransform.gameObject.layer = throwingGunLayer;
        gunTransform.gameObject.tag = "ThrowWeapon";

        EventManager.Instance.NotifyEvent(EventType.isEquiped, false);

        cameraInfomation.amplitude = 0f;
        cameraInfomation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
        //cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.weaponInformation = default;
        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // 총의 local Transform을 돌린다.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one;

        // 총의 원본을 돌린다.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one * 0.15f;

        // direction을 다시 찍어야해..
        RayResearch();
        rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;

        Pinfo.stateName = "PThrow";
        Pinfo.layer = -1;
        Pinfo.normalizedTime = 0f;
        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Pinfo);

        if (isTutorial)
            TutorialManager.Instance.isThrow = true;
    }

    void RayResearch()
    {
        // Rigidbody component
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        float distance = weaponInformation.distance;

        LayerMask layerMask = ~LayerMask.GetMask("Bullet", "Ignore Raycast", "Default", "DeadNPC", "Player", "StackingCamera", "UI", "CCTVArea", "Gun");

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance, layerMask);


        // 일정 거리 이하는 잘 모르겠고 총을 쏠 위치 Ray도 아닌듯
        // Bullet을 쏠 때 


        if (hits.Length > 0)
        {
            RaycastHit closetHit = hits[0];
            float closetDistance = Vector3.Distance(bulletTransform.position, closetHit.point);

            foreach (var hit in hits)
            {
                float bulletDistance = Vector3.Distance(bulletTransform.position, hit.point);
                if (bulletDistance == closetDistance)
                {
                    closetHit = hit;
                    closetDistance = bulletDistance;
                }
            }

            Vector3 forward = Camera.main.transform.forward;

            targetPoint = closetHit.point;
            direction = (targetPoint - bulletTransform.position).normalized;
            throwDirection = forward;
        }
        else
        {
            direction = cameraRay.direction;
            throwDirection = direction;
        }
    }

    void ChangeWeapon()
    {
        EventManager.Instance.NotifyEvent(EventType.isEquiped, false);
        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
        EventManager.Instance.NotifyEvent(EventType.EquipedGunName, "");
        EventManager.Instance.RemoveEvent(EventType.isPause, OnEvent);

        cameraShake.weaponInformation = default;
        outlineWeapon.isDone = false;
        outlineWeapon.isRePosition = false;

        cameraInfomation.amplitude = 0f;
        cameraInfomation.frequency = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

        gunTransform.gameObject.layer = gunLayer;

        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = false;
        weaponCollider.enabled = false;

        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // 총의 local Transform을 돌린다.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one;

        // 총의 원본을 돌린다.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one * 0.15f;

        // direction을 다시 찍어야해..
        RayResearch();
        //rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        Vector3 velocity = throwDirection;
        rigidbody.velocity = velocity * changeThrowingSpeed;
        rigidbody.useGravity = true;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Throwing:
                {
                    bool paramType = (bool)param;
                    isPickup = paramType;
                    rigidbody.useGravity = true;
                    rigidbody.velocity = Vector3.zero;
                    weaponCollider.enabled = false;
                    weaponCollider.isthrowing = false;
                    isChanging = paramType;
                    EventManager.Instance.RemoveEvent(EventType.Throwing);
                    EventManager.Instance.RemoveEvent(EventType.Pickup);
                    EventManager.Instance.RemoveEvent(EventType.GunInformation, OnEvent);
                    gameObject.SetActive(paramType);
                    this.enabled = paramType;
                }
                break;
            case EventType.Pickup:
                {
                    if (!isChanging)
                        Pickup();
                }
                break;
            case EventType.Change:
                {
                    isChanging = true;
                    ChangeWeapon();
                    this.enabled = false;
                    EventManager.Instance.RemoveEvent(EventType.Change);
                }
                break;
            case EventType.GunInformation:
                {
                    GameObject gameObject = (GameObject)param;

                    string name2 = gameObject.transform.parent.gameObject.name;
                    string name3 = this.name;

                    if (name2 == name3)
                    {
                        // 이러면 전체적으로 다 바뀌고 바로 검사하지 않을가? 테스트 ㄱㄱ
                        // 그러네 전체적으로 다 바뀐 후에 
                        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
                    }
                }
                break;
            case EventType.isPause:
                {
                    isPause = (bool)param;
                }
                break;
        }
    }
}

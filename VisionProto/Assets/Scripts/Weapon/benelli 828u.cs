using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// 샷건 종류인 benelli828u다.
/// </summary>
/// 
[DefaultExecutionOrder(4)]
public class benelli828u : MonoBehaviour, IWeapon, IListener
{
    [SerializeField]
    private WeaponInformation weaponInformation;

    [SerializeField]
    private OutlineWeapon outlineWeapon;

    // 실제 샷건 총알 Bullet
    public GameObject shotGunRealBulletFactory;

    // 총구 위치
    public Transform bulletTransform;
    private Transform playerRotation;
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
    private readonly Vector3 position = new Vector3(0.388000011f, 1.76499999f, -0.0780000016f);
    private readonly Vector3 rotation = new Vector3(282.327545f, 71.0938492f, 132.57048f);
    private readonly Vector3 scale = new Vector3(1.5f, 1.5f, 1.5f);

    private Rigidbody rigidbody;
    private MeshCollider meshCollider;

    // Ray Research에 필요한 변수들
    private Vector3 direction;
    private Vector3 targetPoint;

    [SerializeField]
    private float spreadX;
    [SerializeField]
    private float spreadY;
    [SerializeField]
    private int bulletCount;

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

    private AnimationInformation Sinfo;

    // Camera 움직임
    CameraInfomation cameraInfomation;

    private bool isInitalize;
    private bool isPause;

    private bool isTutorial;

    private Light light;
    // Start is called before the first frame update
    void Start()
    {
        InitalizeRecoilSetting();
        Pickup();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPause)
            return;

        if (this.enabled)
            Pickup();

        if (Input.GetMouseButtonDown(0))
        {
            Sinfo.stateName = "SAttack";
            Sinfo.layer = -1;
            Sinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);
            if (isEmpty)
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
            totalTime += 0.1f;

            if (totalTime > weaponInformation.shotDelay)
            {
                isShoting = false;
                totalTime = 0f;
                cameraShake.isRecoil = true;
                light.enabled = false;
                cameraShake.isMouseDown = false;
            }
        }

        if (currentBullet >= weaponInformation.maxBullet)
        {
            isEmpty = true;
        }
        else
            isEmpty = false;

        if (Input.GetMouseButtonUp(0))
        {
            RecoilRecovery();
        }


        //         if (isEmpty)
        //             return;

        // 연사가 불가능하네
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

//         weaponCollider.enabled = false;
//         weaponCollider.isthrowing = false;
        //EventManager.Instance.RemoveEvent(EventType.Pickup);
        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);
        currentBullet = 0;
        gunTransform.transform.position = Vector3.zero;
        gunTransform.transform.rotation = Quaternion.identity;
        bulletInfomation.currentBullet = currentBullet;
        isEquipped = false;
        isEmpty = false;
        outlineWeapon.isDone = false;
        outlineWeapon.isRePosition = false;
        outlineWeapon.isStop = false;
        weaponCollider.boxCollider.enabled = false;

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
        weaponInformation.yRecoil = 2f;
        //weaponInformation.yRecoil = 0f;
        weaponInformation.snappiness = 10f;
        weaponInformation.returnSpeed = 50f;
        weaponInformation.recoilSpeed = 1000f;

        weaponInformation.bulletSpeed = 50f;
        weaponInformation.distance = 800f;
        weaponInformation.maxBullet = 2;
        weaponInformation.shotDelay = 0.5f;
        weaponInformation.throwSpeed = 50f;

        changeThrowingSpeed = 5f;

        bulletCount = 10;

        spreadX = 0.3f;
        spreadY = 0.2f;

        // cameraShake setting
        cameraShake = FindObjectOfType<CameraShake>();
        cameraShake.weaponInformation = weaponInformation;
        playerRotation = GameObject.Find("Direction").GetComponent<Transform>();


        meshCollider = GetComponentInChildren<MeshCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        currentBullet = 0;
        shotCount = 0;

        // 이 스크립트가 활성화 되었다는 것은 활성화가 되었다는 뜻이다.
        isEquipped = false;
        isEmpty = false;

        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);
        isTutorial = DataManager.Instance.isTutorial;

        totalTime = 0f;

        bulletInfomation = new UIBulletInformation();
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;
        bulletInfomation.weaponSelect = UIWeaponSelect.ShotGun;

        Sinfo = new AnimationInformation();

        cameraInfomation = new CameraInfomation();
        cameraInfomation.setting = CameraSetting.ShotgunNoise;

        int throwLayer = LayerMask.GetMask("ThrowWeapon");
        throwingGunLayer = (int)(Mathf.Log(throwLayer) / Mathf.Log(2));

        int stacking = LayerMask.GetMask("StackingCamera");
        stackingLayer = (int)(Mathf.Log(stacking) / Mathf.Log(2));

        int gun = LayerMask.GetMask("Gun");
        gunLayer = (int)(Mathf.Log(gun) / Mathf.Log(2));

        gunTransform.gameObject.layer = gunLayer;
        gunTransform.gameObject.tag = "Gun";

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

        // bullet 생성
        //GameObject realBullet = Instantiate(shotGunRealBulletFactory, bulletTransform.position, bulletTransform.rotation);

        GameObject realBullet;

        // 허상 총알 개수를 정할 수 있다.
        Dictionary<GameObject, Rigidbody> bullets = new Dictionary<GameObject, Rigidbody>();

        float distance = weaponInformation.distance;
        float bulletSpeed = weaponInformation.bulletSpeed;

        if (currentBullet < weaponInformation.maxBullet)
        {
            realBullet = PoolManager.Instance.GetPooledObject("PRealBenelli828u");
            realBullet.transform.position = bulletTransform.position;
            realBullet.transform.rotation = Quaternion.LookRotation(direction);
            PBenelliRealBullet realScript = realBullet.GetComponent<PBenelliRealBullet>();
            
            if(realScript != null) 
                StartCoroutine(realScript.BulletDestory(1.0f));


            for (int i = 0; i < bulletCount; i++)
            {
                //GameObject imageBullet = Instantiate(shotGunImageBulletFactory, bulletTransform.position, bulletTransform.rotation);

                GameObject imageBullet = PoolManager.Instance.GetPooledObject("PImageBenelli828u", bulletTransform.position, Quaternion.identity);
                imageBullet.transform.position = bulletTransform.position;
                imageBullet.transform.rotation = bulletTransform.rotation;

                //Collider bulletCollider = imageBullet.GetComponent<Collider>();
                Rigidbody bulletRigidbody = imageBullet.GetComponent<Rigidbody>();
                // 이러면 벽에 맞고 사라지는게 불가능한데? Collider를 꺼서 그렇다. 지금은 이렇게하고
                // Shotgun image Bullet을 만들어서 적에는 상관 없고 벽이나 바닥에 닿으면 사라지게 하는 스크립트 하나 만들어서 넣자.
                //bulletCollider.enabled = false;
                bullets.Add(imageBullet, bulletRigidbody);
            }

            // world * local 실제 Shot Bullet이다. Player가 바라보는 방향으로 Bullet을 쏴야 한다.
            realBullet.transform.rotation = playerRotation.rotation * shotGunRealBulletFactory.transform.rotation;

            // Real Bullet Rigidbody component
        }
        else realBullet = null;

        Rigidbody realBulletRigidBody;

        if (realBullet != null)
            realBulletRigidBody = realBullet.GetComponent<Rigidbody>();
        else realBulletRigidBody = null;

        RayResearch();

        if (currentBullet < weaponInformation.maxBullet && realBulletRigidBody != null)
        {
            foreach (var bullet in bullets)
            {
                Vector3 randomDirection = direction
                    + new Vector3(
                    Random.Range(-spreadX, spreadX),
                    Random.Range(-spreadY, spreadY),
                    0);

                bullet.Key.transform.rotation = Quaternion.LookRotation(randomDirection);
                bullet.Value.velocity = randomDirection * bulletSpeed;
            }
            EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = true);
            realBulletRigidBody.velocity = direction * bulletSpeed;
            light.enabled = true;
        }

        currentBullet++;

        // Bullet 상태
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        SoundManager.Instance.PlayEffectSound(SFX.Shotgun, this.transform.parent);
        //EffectManager.Instance.ExecutionEffect(Effect.ShotGunShot, bulletTransform.position, Quaternion.LookRotation(direction));
        EffectManager.Instance.ExecutionEffect(Effect.ShotGunShot, bulletTransform.position, Quaternion.LookRotation(direction), 1f);
        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);

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

            if(this.transform.parent != null)
                SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform.parent);
            else
                SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform);

            EventManager.Instance.NotifyEvent(EventType.EquipedGunName, "ShotGun");

            Sinfo.stateName = "SDraw";
            Sinfo.layer = -1;
            Sinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);
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
        outlineWeapon.isDone = true;
        outlineWeapon.GunLayer();

        gunTransform.gameObject.layer = stackingLayer;
        cameraShake.weaponInformation = weaponInformation;
        isEquipped = true;
        isPickup = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = scale;

        gunTransform.localPosition = position;
        gunTransform.localRotation = Quaternion.Euler(rotation);
        gunTransform.localScale = Vector3.one;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

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

        cameraInfomation.frequency = 0f;
        cameraInfomation.amplitude = 0f;
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
        this.transform.position = bulletTransform.position;
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one * 0.15f;

        // direction을 다시 찍어야해..
        RayResearch();
        //rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;
        
        rigidbody.AddForce(throwDirection * weaponInformation.throwSpeed, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;

        Sinfo.stateName = "SThrow";
        Sinfo.layer = -1;
        Sinfo.normalizedTime = 0f;
        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Sinfo);

        if (isTutorial)
            TutorialManager.Instance.isThrow = true;
    }

    void RayResearch()
    {
        // Rigidbody component
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        float distance = weaponInformation.distance;

        LayerMask layerMask = ~LayerMask.GetMask("Bullet", "Ignore Raycast", "Default", "DeadNPC", "Player", "StackingCamera", "UI", "CCTVArea");

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance, layerMask);

        if (hits.Length > 0)
        {
            //System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
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

        cameraInfomation.frequency = 0f;
        cameraInfomation.amplitude = 0f;
        EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

        cameraShake.weaponInformation = default;
        outlineWeapon.isDone = false;
        outlineWeapon.isRePosition = false;

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

        //this.enabled = false;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Throwing:
                {
                    bool paramType = (bool)param;
                    isPickup = paramType;
                    isChanging = paramType;
                    rigidbody.useGravity = true;
                    EventManager.Instance.RemoveEvent(EventType.Throwing, OnEvent);
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
                    EventManager.Instance.RemoveEvent(EventType.Change);
                    this.enabled = false;
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

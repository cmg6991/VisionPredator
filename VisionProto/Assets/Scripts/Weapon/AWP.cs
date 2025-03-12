

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// ���� ���� ������ AWP�̴�.
/// </summary>
public class AWP : MonoBehaviour, IWeapon, IListener
{
    [SerializeField]
    private WeaponInformation weaponInformation;

    [SerializeField]
    private OutlineWeapon outlineWeapon;

    public Transform bulletTransform;
    private CameraShake cameraShake;

    public WeaponCollider weaponCollider;

    // �� �߻�� ī�޶� ��鸲
    public float amplitude;
    public float frequency;

    // ������ ��ġ
    private Transform gunContainer;

    // �ѱⰡ ��ġ�ؾ��� ����
    public Transform gunTransform;
    private Vector3 throwDirection;

    // 총을 잡았을 때 총의 크기를 원래대로 돌릴 매직넘버다. 일단은..
    private readonly Vector3 position = new Vector3(0.003000599f, -0.3000108f, -1.390003f);
    private readonly Vector3 rotation = new Vector3(0f, 177.255f, -90f);
    private readonly Vector3 scale = new Vector3(1339.226f, 20.04176f, 213.8984f);

    private Rigidbody rigidbody;
    private MeshCollider meshCollider;

    // Ray Research�� �ʿ��� ������
    private Vector3 direction;
    private Vector3 targetPoint;


    private float totalTime;
    private int shotCount;
    private int currentBullet;

    private bool isEquipped;
    private bool isPickup;
    [SerializeField]
    private bool isEmpty;

    private bool isChanging;
    private bool isShoting;

    [SerializeField]
    private float changeThrowingSpeed;

    GameObject bullet;

    //애니메이션 관련 변수
    AnimationInformation info;
    [SerializeField]
    private WeaponSetting weaponSetting;
    
    UIBulletInformation bulletInfomation;

    private bool isShoot;

    private int throwingGunLayer;

    void Start()
    {
        InitalizeRecoilSetting();
        Pickup();
    }

    void Update()
    {
        if (this.enabled)
            Pickup();

        if (Input.GetMouseButtonUp(0))
        {
            RecoilRecovery();
        }

        // shot�� ���� �� �ð��� �帣�� ����. Return

        if (isShoting && !isEmpty)
        {
            totalTime += Time.deltaTime;

            if (totalTime > weaponInformation.shotDelay) 
            {
                isShoting = false;
                totalTime = 0f;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            //playerAnimator.Play("Fire", -1, 0)
            info.stateName = "Fire";
            info.layer = -1;
            info.normalizedTime = 0;

            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, info);

            if (isEmpty)
                Drop();

            if (isShoting)
                return;

            SaveFovValue();

            if(cameraShake.gunRecoil)
            {
                Shot();
                RecoilFire();
            }
        }

        if (currentBullet >= weaponInformation.maxBullet)
        {
            isEmpty = true;
            RecoilRecovery();
        }
        else
            isEmpty = false;

        if (isEmpty)
             return;

        // �굵 ���� �Ұ����̳�
        if (Input.GetMouseButton(0))
        {
            totalTime += Time.deltaTime;
            if (totalTime > weaponInformation.shotDelay)
            {
                Shot();
                RecoilFire();
                totalTime = 0f;
            }
        }
    }



    // �ʹݿ� �ʿ��� �ݵ� ����
    public void InitalizeRecoilSetting()
    {
        // Weapon information setting
        weaponInformation = new WeaponInformation();
        weaponInformation.xRecoil = 0f;
        weaponInformation.yRecoil = 0.1f;
        weaponInformation.snappiness = 10f;
        weaponInformation.returnSpeed = 50f;
        weaponInformation.recoilSpeed = 1000f;

        weaponInformation.bulletSpeed = 300f;
        weaponInformation.distance = 1500f;
        weaponInformation.maxBullet = 500;
        weaponInformation.shotDelay = 0.01f;
        weaponInformation.throwSpeed = 50f;

        changeThrowingSpeed = 100f;

        // cameraShake setting
        cameraShake = FindObjectOfType<CameraShake>();
        cameraShake.weaponInformation = weaponInformation;

        // Gun bullet Setting
        gunContainer = GameObject.Find("GunContainer").GetComponent<Transform>();

        meshCollider = GetComponentInChildren<MeshCollider>();
        rigidbody = GetComponentInChildren<Rigidbody>();

        currentBullet = 0;
        shotCount = 0;

        // �� ��ũ��Ʈ�� Ȱ��ȭ �Ǿ��ٴ� ���� Ȱ��ȭ�� �Ǿ��ٴ� ���̴�.
        isEquipped = false;
        isEmpty = false;

        EventManager.Instance.AddEvent(EventType.Pickup, OnEvent);

        //amplitude = 1f;
        //frequency = 1f;

        bulletInfomation = new UIBulletInformation();
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;
        bulletInfomation.weaponSelect = UIWeaponSelect.Dagger;

        int throwLayer = LayerMask.GetMask("ThrowWeapon");

        throwingGunLayer = (int)(Mathf.Log(throwLayer) / Mathf.Log(2));


        info = new AnimationInformation();
        
    }


    // �Ѿ��� ���� �� �ʱ� ������.
    public void SaveFovValue()
    {
        cameraShake.isMouseDown = true;
        cameraShake.isRecoil = false;
        cameraShake.mouseDistance = Vector3.zero;
    }


    // ������� ���� �ݵ�
    public void RecoilFire()
    {
        // Camera Shake�� �ݵ��� ���� ī�޶��� �̵��� �ؾ��Ѵ�.
        // �ݵ��� ����� ���⼭ ������.

        float recoilX = weaponInformation.xRecoil;
        float recoilY = weaponInformation.yRecoil;
        float recoilZ = 0f;
        //cameraShake.isRecoil = true;

        if(shotCount < 30)
            cameraShake.targetRotaion += new Vector3(recoilZ, recoilY, recoilX);
        else
        {
            recoilY = 0;
            recoilX = 0;
            recoilZ = 0;
            cameraShake.targetRotaion += new Vector3(recoilZ, recoilY, recoilX);
        }
            
        shotCount++;
    }

    // �Ѿ��� ���󰣴�.
    public void Shot()
    {
        if (isEmpty) return;
        isShoting = true;

        cameraShake.SetCameraNoise(amplitude, frequency);

        Rigidbody bulletRigidBody;

        if (currentBullet < weaponInformation.maxBullet)
        {
            // bullet ����
            //GameObject bullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);
            bullet = PoolManager.Instance.GetPooledObject("PAWP");
            bullet.transform.position = bulletTransform.position;
            bullet.transform.rotation = bulletTransform.rotation;

            bulletRigidBody = bullet.GetComponent<Rigidbody>();
        }
        else
            bulletRigidBody = null;

        RayResearch();
        float bulletSpeed = weaponInformation.bulletSpeed;

        if (currentBullet < weaponInformation.maxBullet && bulletRigidBody != null)
        {

            bulletRigidBody.velocity = direction * bulletSpeed;
            ///������ ����
            EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = true);
        }
        else
        {
            isEmpty = true;
            RecoilRecovery();
        }
        currentBullet++;

        // Bullet ����
        bulletInfomation.currentBullet = currentBullet;
        bulletInfomation.maxBullet = weaponInformation.maxBullet;

        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);

    }

    // �ݵ� ȸ��
    public void RecoilRecovery()
    {
        cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.isRecoil = true;
        cameraShake.isMouseDown = false;
        shotCount = 0;
        EventManager.Instance.NotifyEvent(EventType.playerShot, isShoot = false);
    }

    // ���� �ֿ��� ��
    public void Pickup()
    {
        if(!isEquipped)
        {
            EventManager.Instance.AddEvent(EventType.Change, OnEvent);
            EventManager.Instance.AddEvent(EventType.GunInformation, OnEvent);
            EventManager.Instance.NotifyEvent(EventType.WeaponBullet, bulletInfomation);
            SoundManager.Instance.PlayEffectSound(SFX.Weapon_Draw, this.transform);
        }
        // ���⼭ �Ѱ� �Ѿ� ���� �ʿ�

        weaponCollider.isEquipped = true;
        weaponCollider.enabled = false;
        outlineWeapon.isDone = true;
        outlineWeapon.GunLayer();

        cameraShake.weaponInformation = weaponInformation;
        isEquipped = true;
        isPickup = true;
        
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        gunTransform.localPosition = position;
        gunTransform.localRotation = Quaternion.Euler(rotation);
        gunTransform.localScale = scale;

        gunContainer.transform.localPosition = new Vector3(-0.0799999982f, 0.25999999f, -0.0799999982f);
    }

    // ���� ������ ��
    public void Drop()
    {
        EventManager.Instance.AddEvent(EventType.Throwing, OnEvent);
        // �� ��� �ֳĸ� ���� ���� ���ؼ� false ���ִ� ���̴�.

        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
        SoundManager.Instance.PlayEffectSound(SFX.Weapon_Drop, this.transform);

        weaponCollider.enabled = true;
        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.isTrigger = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = true;

        outlineWeapon.isDone = true;
        gunTransform.gameObject.layer = throwingGunLayer;
        gunTransform.gameObject.tag = "ThrowWeapon";

        EventManager.Instance.NotifyEvent(EventType.isEquiped, weaponCollider.isEquipped);

        cameraShake.SetCameraNoise(0f, 0f);

        cameraShake.weaponInformation = default;
        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // ���� local Transform�� ������.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // ���� ������ ������.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction�� �ٽ� ������..
        RayResearch();
        rigidbody.velocity = throwDirection * weaponInformation.throwSpeed;

        float random = Random.Range(-1f, 1f);
        rigidbody.AddTorque(new Vector3(random, random, random) * 10f);

        this.enabled = false;
    }

    void RayResearch()
    {
        // Rigidbody component
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        float distance = weaponInformation.distance;

        LayerMask layerMask = ~LayerMask.GetMask("Bullet");

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance, layerMask);

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

            targetPoint = closetHit.point;
            direction = (targetPoint - bulletTransform.position).normalized;
            throwDirection = direction;
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


        cameraShake.weaponInformation = default;

        weaponCollider.isEquipped = false;
        weaponCollider.boxCollider.enabled = true;
        weaponCollider.isthrowing = true;
        weaponCollider.enabled = false;
        outlineWeapon.isDone = false;

        isEquipped = false;
        isPickup = false;

        transform.SetParent(null);
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // ���� local Transform�� ������.
        gunTransform.localPosition = Vector3.zero;
        gunTransform.localRotation = Quaternion.Euler(Vector3.zero);
        gunTransform.localScale = Vector3.one * 100f;

        // ���� ������ ������.
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one;

        // direction�� �ٽ� ������..
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
                    rigidbody.useGravity = true;
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
                    ChangeWeapon();
                    isChanging = true;
                    EventManager.Instance.RemoveEvent(EventType.Change);
                    this.enabled = false;
                }
                break;
            case EventType.GunInformation: 
                {
                    // param�� awp type�� �ƴ��� ��� �� �� ����?
                    // �ٵ� �� �� ���� ���� �� �� ���� �ʳ�?

                    // param�� AWP���� �ƴ��� Ȯ���� �� ����? ���?
                    {
//                         bool awp = param.GetType() == typeof(AWP);
// 
//                         AWP awp3 = param as AWP;
// 
//                         object type = param.GetType();
//                         string name = param.ToString();
// 
// 
//                         string name2 = gameObject.transform.parent.gameObject.ToString();
//                         string name3 = this.ToString();
// 
//                         if (param is AWP)
//                         {
//                             AWP awp2 = (AWP)param;
//                         }
                    }

                    GameObject gameObject = (GameObject)param;

                    // ã�Ҵ� �̸����� �� ����
                    string name2 = gameObject.transform.parent.gameObject.name;                   
                    string name3 = this.name;


                    if(name2 == name3)
                    {
                        // �̷��� ��ü������ �� �ٲ�� �ٷ� �˻����� ������? �׽�Ʈ ����
                        // �׷��� ��ü������ �� �ٲ� �Ŀ� 
                        EventManager.Instance.NotifyEvent(EventType.isEmptyBullet, this.isEmpty);
                    }
                    
                }
                break;
        }
    }
}

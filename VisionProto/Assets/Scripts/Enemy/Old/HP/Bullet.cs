using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Bullet : MonoBehaviour
{
    private Transform targetPlayer; 

    public float moveSpeed = 400f;   //=>

    private Rigidbody rb;
    Vector3 direction;
    Vector3 moveDirection;
    public float inaccuracyAngle = 500000f;

    public bool isPistols = false;
    public bool isAssault = false;
    public bool isShot = false;
    public bool isSniping = false;
    public bool isMachine = false;
    public bool isRailGun = false;

    private int pistolDamage = 10;
    private int assaultDamage = 10;
    private int shotgunNearDamage = 30;
    private int shotgunMiddleDamage = 20;
    private int shotgunFarDamage = 10;

    float distance;
    EnemyController enemyController;
    private TrailRenderer tr;

    private static readonly HashSet<string> objTag = new HashSet<string>
    { "Wall", "Floor", "Player", "Door", "Grappling", "GrapplingPoint", "Cabinet", "Item" , "SitWall" ,"Button"};

    private void Awake()
    {
        if(isPistols)
        {
            moveSpeed = 400f;
            inaccuracyAngle = 5f;
        }
        if (isAssault)
        {
            moveSpeed = 300f;
            inaccuracyAngle = 5f;
        }
        if (isShot)
        {
            moveSpeed = 500f;
            inaccuracyAngle = 10f;
        }
        if (isSniping)
        {
            moveSpeed = 900f;
            inaccuracyAngle =5f;
        }
        if(isMachine)
        {
            moveSpeed = 400;
            inaccuracyAngle = 1f;
        }
        if(isRailGun)
        {
            moveSpeed = 800;
            inaccuracyAngle = 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        // PlayerSettings 관련 코드가 여기에 있다면 이 부분을 에디터에서만 실행되도록 설정
        var companyName = PlayerSettings.companyName;
#endif
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
        enemyController = FindObjectOfType<EnemyController>();

    }

    private void FixedUpdate()
    {
        if (targetPlayer != null)
        {
            rb.velocity = moveDirection * moveSpeed;

        }
        else
        {
            // 예외처리: 목표가 없을 때, 총알을 제거하거나 다른 동작 수행
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        if (tr == null) return;
        tr.Clear();
    }

    //수치 조정을 하게 바꿔야해 
    Vector3 ApplyInaccuracy(Vector3 direction, float angle)
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            Random.Range(-angle, angle)
        );

        // 기존 방향에 무작위 회전 적용
        return randomRotation * direction;
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if(isPistols)
            {
                int modePistol = pistolDamage;

                if (DataManager.Instance.isEasyMode)
                    modePistol = (int)(modePistol * 0.3f);
                else if (DataManager.Instance.isNormalMode)
                    modePistol = (int)(modePistol * 0.5f);
                else if (DataManager.Instance.isHardMode)
                    modePistol = pistolDamage;

                damageable.Damaged(modePistol, transform.position, transform.position, this.gameObject); ///권총
            }
            if (isAssault)
            {
                int modeAssault = assaultDamage;

                if (DataManager.Instance.isEasyMode)
                    modeAssault = (int)(modeAssault * 0.3f);
                else if (DataManager.Instance.isNormalMode)
                    modeAssault = (int)(modeAssault * 0.5f);
                else if (DataManager.Instance.isHardMode)
                    modeAssault = assaultDamage;

                damageable.Damaged(modeAssault, transform.position, transform.position, this.gameObject); ///돌격 소총
            }
            if (isMachine)
            {
                damageable.Damaged(10, transform.position, transform.position, this.gameObject); ///머신건 
            }
            if (isRailGun)
            {
                damageable.Damaged(10, transform.position, transform.position, this.gameObject); ///레일건
            }
            if (isShot)
            {
                if (targetPlayer != null)
                {
                    distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

                    if(distance< 10)
                    {
                        int modeNearShot = shotgunNearDamage;

                        if (DataManager.Instance.isEasyMode)
                            modeNearShot = (int)(modeNearShot * 0.3f);
                        else if (DataManager.Instance.isNormalMode)
                            modeNearShot = (int)(modeNearShot * 0.5f);
                        else if (DataManager.Instance.isHardMode)
                            modeNearShot = shotgunFarDamage;

                        damageable.Damaged(modeNearShot, transform.position, transform.position, this.gameObject); ///10m이내
                    }

                    else if(distance>10 && distance <= 50)
                    {
                        int modeMiddleShot = shotgunMiddleDamage;

                        if (DataManager.Instance.isEasyMode)
                            modeMiddleShot = (int)(modeMiddleShot * 0.3f);
                        else if (DataManager.Instance.isNormalMode)
                            modeMiddleShot = (int)(modeMiddleShot * 0.5f);
                        else if(DataManager.Instance.isHardMode)
                            modeMiddleShot = shotgunMiddleDamage;

                        damageable.Damaged(modeMiddleShot, transform.position, transform.position, this.gameObject); ///10m초과 50 이하

                    }
                    else
                    {
                        int modeFarShot = shotgunFarDamage;

                        if (DataManager.Instance.isEasyMode)
                            modeFarShot = (int)(modeFarShot * 0.3f);
                        else if (DataManager.Instance.isNormalMode)
                            modeFarShot = (int)(modeFarShot * 0.5f);
                        else if (DataManager.Instance.isHardMode)
                            modeFarShot = shotgunFarDamage;

                        damageable.Damaged(modeFarShot, transform.position, transform.position, this.gameObject);///50 이상 (나머지)
                    }
                }
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objTag.Contains(collision.gameObject.tag))
        {
            gameObject.SetActive(false);

            if(collision.gameObject.CompareTag("Player"))
            {
                HitBulletInformation bulletInformation = new HitBulletInformation();
                Vector3 contactPoint = collision.contacts[0].point;

                // 두 벡터의 차를 구해서 거리를 구한다.
                Vector3 hitPositon = contactPoint - collision.gameObject.transform.position;

                Vector3 localHitPositon = collision.gameObject.transform.InverseTransformDirection(hitPositon);

                bulletInformation.hitPosition = localHitPositon;
                bulletInformation.bulletForward = this.transform.forward;
                EventManager.Instance.NotifyEvent(EventType.HitBulletRotation, bulletInformation);
            }

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("SitWall"))
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 hitPoint = contact.point;
                Vector3 hitNormal = contact.normal;
                Quaternion hitRotation = Quaternion.LookRotation(hitNormal);

                //EffectManager.Instance.ExecutionEffect(Effect.GunHit, hitPoint, hitRotation, collision.transform);
                if(isMachine)
                {
                    EffectManager.Instance.ExecutionEffect("Boss Decal", hitPoint, hitRotation, collision.transform, 1f);
                    EffectManager.Instance.ExecutionEffect("Boss Bullet Hit", hitPoint, hitRotation, collision.transform, 1f);
                }
                else if(isRailGun)
                {
                    EffectManager.Instance.ExecutionEffect("Boss Decal", hitPoint, hitRotation, collision.transform, 1f);
                    EffectManager.Instance.ExecutionEffect("Boss Lazer Bullet", hitPoint, hitRotation, collision.transform, 1f);
                }
                else
                {
                    EffectManager.Instance.ExecutionEffect("Bullet Decal", hitPoint, hitRotation, collision.transform, 1f);
                    EffectManager.Instance.ExecutionEffect("GunHit", hitPoint, hitRotation, collision.transform, 1f);
                }
            }
        }
    }

    void Pistol()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 10)
            {
                inaccuracyAngle = 0;
            }
            else if(distance > 10 && distance <= 30)
            {
                inaccuracyAngle = 3;
            }
            else
            {
                inaccuracyAngle = 5;
            }

        }
    }

    void Assult()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 10)
            {
                inaccuracyAngle = 0;
            }
            else if (distance > 10 && distance <= 30)
            {
                inaccuracyAngle = 3;
            }
            else if (distance > 30 && distance <= 50)
            {
                inaccuracyAngle = 5;
            }
            else
            {
                inaccuracyAngle = 6;
            }

        }
    }

    void Sinping()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 100)
            {
                inaccuracyAngle = 3;
            }
            else
            {
                inaccuracyAngle = 5;
            }

        }
    }

    void Machine()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 20)
            {
                inaccuracyAngle = 1;
            }
            else
            {
                inaccuracyAngle = 3;
            }

        }
    }

    public void SetUp(Transform pos, Transform target)
    {
        gameObject.transform.position = pos.position;
        //Debug.Log("리나야 " + target.transform.position);
        targetPlayer = target;
        direction = (targetPlayer.transform.position - transform.position).normalized;
        if (isPistols)
        {
            Pistol();
            //EffectManager.Instance.ExecutionEffect(Effect.PistolShot, pos);
            EffectManager.Instance.ExecutionEffect("PistolMuzzle", pos, 1f);
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
        if (isAssault)
        {
            Assult();
            //EffectManager.Instance.ExecutionEffect(Effect.PistolShot, pos);
            EffectManager.Instance.ExecutionEffect("RifleMuzzle", pos, 1f);
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
        if (isShot)
        {
            //Sinping();
            //EffectManager.Instance.ExecutionEffect(Effect.PistolShot, pos);
            EffectManager.Instance.ExecutionEffect("ShotGunMuzzle", pos, 1f);
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
        if(isMachine)
        {
            Machine();
            EffectManager.Instance.ExecutionEffect("Boss Bullet Muzzle", pos, 1f);
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
        if(isRailGun)
        {
            EffectManager.Instance.ExecutionEffect("Boss Lazer Muzzle Effect", pos, 1f);
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
        //transform.LookAt(targetPlayer);

        moveDirection = ApplyInaccuracy(direction.normalized, inaccuracyAngle);
  
    }
}

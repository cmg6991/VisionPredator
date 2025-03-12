using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet이 들어있는 Pbullet 
    public GameObject bulletFactory;
    public GameObject shotGunBulletFactory;
    //public GameObject shotGunImageBulletFactory;

    public Transform playerRotation;

    // Bullet이 발사될 총구 위치
    public Transform bulletTransform;

    // 총이 발사되면 카메라가 흔들려야 한다.
    private CameraShake cameraShake;

    // 총알이 한번에 왕창 나오지 않게 시간을 조절했다.
    private float totalTime;

    // 총알 거리
    [SerializeField]
    private float distance;

    // 총알 속도
    [SerializeField]
    private float bulletSpeed;

    // 퍼지는 총알
    [SerializeField]
    private float spreadX, spreadY;

    // 샷건 총알 개수
    [SerializeField]
    private int bulletCount;

    private void Start()
    {
        distance = 500f;
        bulletSpeed = 100f;
        cameraShake = FindObjectOfType<CameraShake>();
        spreadX = 0.3f;
        spreadY = 0.25f;
        bulletCount = 10;
    }

    //LayerMask layerMask = ~LayerMask.GetMask("Bullet

    void Update()
    {
 
    }


    void Shoot()
    {
        // bullet 생성
        GameObject bullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);

        // Rigidbody component
        Rigidbody bulletRigidBody = bullet.GetComponent<Rigidbody>();

        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance);

        if (hits.Length > 0)
        {
            RaycastHit closetHit = hits[0];
            float closetDistance = Vector3.Distance(bulletTransform.position, closetHit.point);

            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(bulletTransform.position, hit.point);
                if (distance < closetDistance)
                {
                    closetHit = hit;
                    closetDistance = distance;
                }
            }

            Vector3 targetPoint = closetHit.point;
            Vector3 direction = (targetPoint - bulletTransform.position).normalized;

            bulletRigidBody.velocity = direction * bulletSpeed;

        }
        else
        {
            Vector3 direction = cameraRay.direction;
            bulletRigidBody.velocity = direction * bulletSpeed;
        }

    }

    /// <summary>
    /// 샷건의 총알은 좀 다르다.
    /// 마름모 범위 안에서 랜덤한 총알이 발사된다.
    /// 랜덤 범위인데 사실은 원 아닐까?
    /// </summary>
    void ShotGunShoot()
    {
        // bullet 생성
        GameObject realBullet = Instantiate(shotGunBulletFactory, bulletTransform.position, bulletTransform.rotation);
        
        // 6, 5

        // 가상 총알 10개로 해볼까?
        Dictionary<GameObject, Rigidbody> bullets = new Dictionary<GameObject, Rigidbody>();

        // 10개 담았다.
        for(int i = 0; i < bulletCount; i++)
        {
            GameObject imageBullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);
            Collider bulletCollider = imageBullet.GetComponent<Collider>();
            Rigidbody bulletRigidbody = imageBullet.GetComponent<Rigidbody>();
            // 이러면 벽에 맞고 사라지는게 불가능한데? Collider를 꺼서 그렇다. 지금은 이렇게하고
            // Shotgun image Bullet을 만들어서 적에는 상관 없고 벽이나 바닥에 닿으면 사라지게 하는 스크립트 하나 만들어서 넣자.
            bulletCollider.enabled = false;
            bullets.Add(imageBullet, bulletRigidbody); 
        }

        // world * local 실제 Shot Bullet이다. Player가 바라보는 방향으로 Bullet을 쏴야 한다.
        realBullet.transform.rotation = playerRotation.rotation * shotGunBulletFactory.transform.rotation;
        
        // Real Bullet Rigidbody component
        Rigidbody realBulletRigidBody = realBullet.GetComponent<Rigidbody>();

        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance);
        
        // 마우스 지점에 hit Collider가 닿았다면 그 지점에 총을 쏘도록 하자.
        if (hits.Length > 0)
        {
            // 첫번째 지점
            RaycastHit closetHit = hits[0];
            float closetDistance = Vector3.Distance(bulletTransform.position, closetHit.point);

            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(bulletTransform.position, hit.point);
                if (distance < closetDistance)
                {
                    closetHit = hit;
                    closetDistance = distance;
                }
            }

            // Real Bullet이 나가는 방향
            Vector3 targetPoint = closetHit.point;
            Vector3 direction = (targetPoint - bulletTransform.position).normalized;

            // image Bullet이 나가는 방향 x: -3 ~ 3, y: -2.5 ~ 2.5  6,5 사각형에 들어와야 하니까.    
            foreach (var bullet in bullets)
            {
                Vector3 randomDirection = direction
                    + new Vector3(
                    Random.Range(-spreadX, spreadX),
                    Random.Range(-spreadY, spreadY),
                    0);

                bullet.Value.velocity = randomDirection * bulletSpeed;
            }
            
            // Real Bullet 총알이 나간다.
            realBulletRigidBody.velocity = direction * bulletSpeed;
        }
        else
        {
            Vector3 direction = cameraRay.direction;

            foreach (var bullet in bullets)
            {
                Vector3 randomDirection = direction
                    + new Vector3(
                    Random.Range(-spreadX, spreadX),
                    Random.Range(-spreadY, spreadY),
                    0);

                bullet.Value.velocity = randomDirection * bulletSpeed;
            }

            realBulletRigidBody.velocity = direction * bulletSpeed;
        }
    }
}

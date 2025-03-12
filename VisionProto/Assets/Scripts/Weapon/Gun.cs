using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet�� ����ִ� Pbullet 
    public GameObject bulletFactory;
    public GameObject shotGunBulletFactory;
    //public GameObject shotGunImageBulletFactory;

    public Transform playerRotation;

    // Bullet�� �߻�� �ѱ� ��ġ
    public Transform bulletTransform;

    // ���� �߻�Ǹ� ī�޶� ������ �Ѵ�.
    private CameraShake cameraShake;

    // �Ѿ��� �ѹ��� ��â ������ �ʰ� �ð��� �����ߴ�.
    private float totalTime;

    // �Ѿ� �Ÿ�
    [SerializeField]
    private float distance;

    // �Ѿ� �ӵ�
    [SerializeField]
    private float bulletSpeed;

    // ������ �Ѿ�
    [SerializeField]
    private float spreadX, spreadY;

    // ���� �Ѿ� ����
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
        // bullet ����
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
    /// ������ �Ѿ��� �� �ٸ���.
    /// ������ ���� �ȿ��� ������ �Ѿ��� �߻�ȴ�.
    /// ���� �����ε� ����� �� �ƴұ�?
    /// </summary>
    void ShotGunShoot()
    {
        // bullet ����
        GameObject realBullet = Instantiate(shotGunBulletFactory, bulletTransform.position, bulletTransform.rotation);
        
        // 6, 5

        // ���� �Ѿ� 10���� �غ���?
        Dictionary<GameObject, Rigidbody> bullets = new Dictionary<GameObject, Rigidbody>();

        // 10�� ��Ҵ�.
        for(int i = 0; i < bulletCount; i++)
        {
            GameObject imageBullet = Instantiate(bulletFactory, bulletTransform.position, bulletTransform.rotation);
            Collider bulletCollider = imageBullet.GetComponent<Collider>();
            Rigidbody bulletRigidbody = imageBullet.GetComponent<Rigidbody>();
            // �̷��� ���� �°� ������°� �Ұ����ѵ�? Collider�� ���� �׷���. ������ �̷����ϰ�
            // Shotgun image Bullet�� ���� ������ ��� ���� ���̳� �ٴڿ� ������ ������� �ϴ� ��ũ��Ʈ �ϳ� ���� ����.
            bulletCollider.enabled = false;
            bullets.Add(imageBullet, bulletRigidbody); 
        }

        // world * local ���� Shot Bullet�̴�. Player�� �ٶ󺸴� �������� Bullet�� ���� �Ѵ�.
        realBullet.transform.rotation = playerRotation.rotation * shotGunBulletFactory.transform.rotation;
        
        // Real Bullet Rigidbody component
        Rigidbody realBulletRigidBody = realBullet.GetComponent<Rigidbody>();

        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit[] hits = Physics.RaycastAll(cameraRay, distance);
        
        // ���콺 ������ hit Collider�� ��Ҵٸ� �� ������ ���� ��� ����.
        if (hits.Length > 0)
        {
            // ù��° ����
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

            // Real Bullet�� ������ ����
            Vector3 targetPoint = closetHit.point;
            Vector3 direction = (targetPoint - bulletTransform.position).normalized;

            // image Bullet�� ������ ���� x: -3 ~ 3, y: -2.5 ~ 2.5  6,5 �簢���� ���;� �ϴϱ�.    
            foreach (var bullet in bullets)
            {
                Vector3 randomDirection = direction
                    + new Vector3(
                    Random.Range(-spreadX, spreadX),
                    Random.Range(-spreadY, spreadY),
                    0);

                bullet.Value.velocity = randomDirection * bulletSpeed;
            }
            
            // Real Bullet �Ѿ��� ������.
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

using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Pool;

/// <summary>
/// 세부적인 행동을 하는 클래스
/// 적들말이야 적들! 
///
/// 김예리나 작성
/// </summary>
[DefaultExecutionOrder(100)] //속성, 나중에 실행되라고 하는 것이다.
public class TestBehavior : MonoBehaviour
{
    public Animator m_Animator;
    //public Animator animator { get { return m_Animator; } }
    private Vector3 targetPosition;
    public Transform player;
    public Transform playerHead;
    public GameObject bullet;
    private float timer;  //사실 이건 잘못됨.
    private Coroutine spawnCoroutine;
    public GameObject eye;
    public NavMeshAgent agent;
    public bool isAttackable = false;
    public bool wannaAttack = false;
    public bool isCanRun = false;
    public bool behindWall = false;
    public bool movingDone = false;
    public float movebackDistance = 0.1f;
    private bool wasWallback; // 이전 프레임의 벽 감지 상태
    private bool isMoving;

    public bool isPistols = false;
    public bool isAssault = false;
    public bool isShot = false;
    public bool isSniping = false;

    public bool isDetectable = false; //흠..
    public float runSpeed = 30f;

    public EnemyHP enemyHP;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        SceneLinkedSMB<TestBehavior>.Initialise(m_Animator, this);
        agent = GetComponent<NavMeshAgent>();
        enemyHP = GetComponent<EnemyHP>();
    }
    public void Chase()
    {
        //if(isSniping)
        //{
        //    StopAgent();
        //}

        agent.SetDestination(player.position);

        if (agent.isOnOffMeshLink)
        {
            m_Animator.SetBool("Jump", true);
            m_Animator.SetBool("Chase", false);
        }
        else
        {
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Chase", true);
        }
        //transform.position = Vector3.MoveTowards(transform.position, player.position, 10 * Time.deltaTime);
    }

    public void Run()
    {
        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = 0; // Y축을 무시
        directionToPlayer.Normalize();

        if(behindWall)
        {
            //targetPosition = transform.position + transform.forward * 10;
            StartCoroutine(MoveForwardCoroutine(transform.forward * runSpeed));
        }
        else
        {
            // 플레이어의 반대 방향으로 이동
            targetPosition = transform.position - directionToPlayer * 10;
        }

        if(!isMoving)
        {
        MoveToPosition(targetPosition);

        }
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        // 목표 위치로 이동
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 50 * Time.deltaTime);
        transform.position = newPosition;

        if(transform.position == newPosition)
        {
            movingDone = true;
        }
    }

    private IEnumerator MoveForwardCoroutine(Vector3 forwardDirection)
    {
        isMoving = true;

        Vector3 targetPosition = transform.position + forwardDirection;

        while (Vector3.Distance(transform.position, targetPosition) > movebackDistance)
        {
            // 목표 위치로 이동
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 50 * Time.deltaTime);
            transform.position = newPosition;
            yield return null;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Y축을 무시
        transform.forward = directionToPlayer;

        isMoving = false;
        movingDone = true;
    }

    public void StopAgent()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    public void reMoveAgent()
    {
        agent.isStopped = false;
        //agent.SetDestination(player.position);
    }



    public void Attack()
    {
        //// 타이머 업데이트
        //timer += Time.deltaTime;

        //// 일정 시간 간격으로 물체 생성
        //if (timer >= 1)
        //{
        //    // 물체 생성
        //    GameObject spawnedObject = Instantiate(bullet, eye.transform.position, eye.transform.rotation);

        //    // 물체 수명 설정 후 제거
        //    Destroy(spawnedObject, 3);

        //    // 타이머 초기화
        //    timer = 0f;
        //}

        // 목표 지점에 도달했을 때
        //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            //StopAgent();
        }



        timer += Time.deltaTime;
        if (timer >= 1 && spawnCoroutine == null)
        {
            if(isPistols)
            {

                spawnCoroutine = StartCoroutine(SpawnBullet());
            }
            if(isAssault && isAttackable)
            {
                spawnCoroutine = StartCoroutine(SpawnBullet2());
            }
            if(isSniping)
            {
                spawnCoroutine = StartCoroutine(SpawnBullet3());

            }
            if(isShot&& isAttackable)
            {
                spawnCoroutine = StartCoroutine(SpawnBullet4());
            }
            timer = 0f;
        }
    }

    //public void MoveChange()
    //{
    //    if(transform.position == targetPosition)
    //    {
    //        isMoveDone = true;
    //    }
    //}
    private IEnumerator SpawnBullet()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 12;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.3f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                ///김예리나 이거 수정했다. 
                //bullet1.SetUp(eye.transform, playerHead.gameObject);
                bulletsSpawned++;

                ///TO DO : 도대체 뭐야 김예리나
                StartCoroutine(test(bullet, "Ebullet"));
            }
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet2()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 30;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet1");
            Bullet bullet2 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                //bullet2.SetUp(eye.transform, playerHead.gameObject);
                bulletsSpawned++;

                ///TO DO : 도대체 뭐야 김예리나
                StartCoroutine(test(bullet, "Ebullet1"));
            }
        }
        yield return new WaitForSeconds(4f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet3()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 5;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(1f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet2");
            Bullet bullet3 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                //bullet3.SetUp(eye.transform, playerHead.gameObject);
                bulletsSpawned++;

                ///TO DO : 도대체 뭐야 김예리나
                StartCoroutine(test(bullet, "Ebullet2"));
            }
        }
        yield return new WaitForSeconds(5f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet4()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 6; // 총 생성할 총알 수
        const int bulletsPerWave = 3; // 한 번에 생성할 총알 수

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet3");
                Bullet bullet4 = bullet.GetComponent<Bullet>();
                if (bullet != null)
                {
                    //bullet4.SetUp(eye.transform, playerHead.gameObject);
                    bulletsSpawned++;
                    StartCoroutine(test(bullet, "Ebullet3"));

                    if (bulletsSpawned >= maxBulletsToSpawn)
                    {
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    IEnumerator test(GameObject a, string tag)
    {
        yield return new WaitForSeconds(15);

        PoolManager.Instance.ReturnToPool(a,tag);
    }

    //private void bulletDestroy()
    //{
    //    if (pooledObject != null)
    //    {

    //        PoolManager.Instance.ReturnToPool(pooledObject);
    //    }
    //}

    public void StopBullet()
    {
        //코루틴을 가져왔더니 실행중인 코루틴을 참조하지 못했었다.
        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        //StopAllCoroutines();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Detector"))
        {
            isDetectable = true;
        }
    }
}

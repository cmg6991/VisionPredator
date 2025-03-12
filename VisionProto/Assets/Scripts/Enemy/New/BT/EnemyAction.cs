using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using static BTNode;

public abstract class EnemyAction
{
    public abstract NodeState Execute();
    public abstract NodeState Stop();

    protected void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineRunner.Instance.StartCoroutine(coroutine);
    }
}

public class IdleSightAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private float radius;
    private float detectionAngle;
    private Animator animator;
    private NavMeshAgent agent;

    public IdleSightAction(NavMeshAgent agent, Transform player, Transform center, float radius, float detectionAngle, Animator animator)
    {
        this.player = player;
        this.center = center;
        this.radius = radius;
        this.detectionAngle = detectionAngle;
        this.animator = animator;
        this.agent = agent;
    }

    public override NodeState Execute()
    {
        agent.isStopped = true;
        float distanceToCenter = Vector3.Distance(player.position, center.transform.position);
        animator.Play("Idle_1");

        //Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        //RaycastHit hit;

        //// ���̰� ��Ҵ��� Ȯ��
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    // �±װ� "Door"���� Ȯ��
        //    if (hit.collider.CompareTag("Door"))
        //    {
        //        return NodeState.Failure;
        //    }
        //}

        if (distanceToCenter <= radius)
        {

            Vector3 toPlayer = player.position - center.transform.position;
            Vector3 toPlayerFlat = toPlayer;
            toPlayerFlat.y = 0;

            float angleToPlayer = Vector3.Angle(toPlayerFlat, agent.transform.forward);

            float halfDetectionAngle = detectionAngle / 2f;

            if (angleToPlayer <= halfDetectionAngle)
            {
                return NodeState.Failure;
            }
        }

        return NodeState.Success;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }
}

public class IdleSoundAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private float radius;
    private BoolWrapper isShot;

    public IdleSoundAction(Transform player, Transform center, float radius, BoolWrapper isShot)
    {
        this.player = player;
        this.center = center;
        this.radius = radius;
        this.isShot = isShot;
    }

    public override NodeState Execute()
    {
        float distanceToCenter = Vector3.Distance(player.position, center.transform.position);

        if (distanceToCenter <= radius)
        {
            if (isShot.Value)
            {
                return NodeState.Failure;
            }
        }

        return NodeState.Success;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }
}

public class PatrolAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private NavMeshAgent navmeshEnemy;
    private float radius;
    private float pradius;
    private Vector3 currentDestination;
    private bool isMoving = false;
    private Animator animator;
    private Transform eye;
    private Coroutine spawnCoroutine;
    private float timer;
    private int npcType;
    float stuckTime = 0f; // ���� ��ġ�� �ӹ� �ð�

    private bool once;
    private GameObject sound;
    bool jumpFlag;

    public PatrolAction(Transform player, Transform center, NavMeshAgent navMeshAgent, float radius ,float pRadius, Transform eye,int npcType,Animator animator )
    {
        this.player = player;
        this.center = navMeshAgent.transform;
        this.navmeshEnemy = navMeshAgent;
        this.radius = radius;
        this.pradius = pRadius;
        this.animator = animator;
        this.eye = eye;
        navmeshEnemy.isStopped = false;
        this.npcType = npcType;
    }

    public override NodeState Execute()
    {
        if(!once)
        {
            once = true;
            sound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Enemy_Run, navmeshEnemy.transform);
        }

        navmeshEnemy.isStopped = false; //��� ���⿡ �ִ°� ��� �ȵȴ�..
        navmeshEnemy.updateRotation = false;

        Vector3 directionToPlayer = (player.position - navmeshEnemy.transform.position).normalized;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            
            Vector3 rotation = Quaternion.Slerp(navmeshEnemy.transform.rotation, lookRotation, Time.deltaTime * navmeshEnemy.angularSpeed).eulerAngles;
            navmeshEnemy.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        }

        bool isMovingBackward = IsMovingBackward(navmeshEnemy, directionToPlayer);

        if (!navmeshEnemy.isOnOffMeshLink&&isMovingBackward)
        {
            //Debug.Log("Agent�� �ڷ� �̵� ��");
            animator.Play("WalkBackwards");
            jumpFlag = false;
        }
        else if (navmeshEnemy.isOnOffMeshLink && !jumpFlag)
        {
            animator.Play("Jump");
            SoundManager.Instance.PlayEffectSound(SFX.Enemy_Jump, navmeshEnemy.transform);
            jumpFlag = true;
        }
        else if(!navmeshEnemy.isOnOffMeshLink && !isMovingBackward)
        {
            animator.Play("chase");
            jumpFlag = false;
        }

        float distanceToPlayer = Vector3.Distance(player.position, center.transform.position);
        timer += Time.deltaTime;
        if (timer >= 1 && spawnCoroutine == null)
        {
            if (npcType == 0)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet());
            }
            else if (npcType == 1)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet2());
            }
            else if (npcType == 2)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet3());
            }
            timer = 0f;
        }

        //RaycastHit hit;
        //Ray backwardRay = new Ray(navmeshEnemy.transform.position, -navmeshEnemy.transform.forward);
        //if (Physics.Raycast(backwardRay, out hit, 3f))
        //{
        //    if (hit.collider.CompareTag("Door"))
        //    {
        //        return NodeState.Failure;
        //    }
        //}

        if (distanceToPlayer >= radius || Input.GetKeyDown(KeyCode.K))
        {
            //Debug.Log("�Ѿ���?");
            if (spawnCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            isMoving = false;
            once = false;
            if(sound != null) 
            {

            SoundManager.Instance.DestroyObject(sound);
            }
            return NodeState.Failure;
        }
        float elapsedTime = 0f; // ��� �ð� ����
        float checkInterval = 1.0f; // 1�ʸ��� �˻�
        float stuckDuration = 1.3f; // 3�� ���� ���� �ڸ��� �ӹ��� ���
        Vector3 lastPosition; // ������ ��ġ ����
        
        float positionThreshold = 3.0f; // NPC�� ��ǥ ��ġ�� �����ϱ� ���� ��� ����

        if (navmeshEnemy != null && player != null)
        {
            Vector3 currentDestination = navmeshEnemy.destination;

            // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ� üũ
            float distanceToTarget = Vector3.Distance(navmeshEnemy.transform.position, currentDestination);

            // NPC�� ��ǥ ��ġ�� �ʹ� ������ stuckTime�� ������Ŵ
            if (Mathf.Abs(navmeshEnemy.transform.position.x - currentDestination.x) <= positionThreshold &&
                Mathf.Abs(navmeshEnemy.transform.position.y - currentDestination.y) <= positionThreshold &&
                Mathf.Abs(navmeshEnemy.transform.position.z - currentDestination.z) <= positionThreshold)
            {
                stuckTime += Time.deltaTime;
            }
            else
            {
                stuckTime = 0f; // ��ġ�� ����Ǹ� �ʱ�ȭ
            }

            // 3�� �̻� ���� �ڸ��� ���� ���
            if (stuckTime >= stuckDuration)
            {
                Vector3 randomPosition = GetRandomPositionOutsideCircle(player.transform.position, pradius, distanceToPlayer);
                navmeshEnemy.SetDestination(randomPosition);
                stuckTime = 0f; // �ð� �ʱ�ȭ
            }

            // �������� ���ų� ��ǥ������ �����ϸ� ���ο� ���� ��ġ ����
            if (!isMoving || navmeshEnemy.remainingDistance <= navmeshEnemy.stoppingDistance)
            {
                isMoving = true;
                Vector3 randomPosition = GetRandomPositionOutsideCircle(player.transform.position, pradius, distanceToPlayer);
                navmeshEnemy.SetDestination(randomPosition);
            }

            // ��ǥ ������ �����ߴ��� Ȯ��
            if (navmeshEnemy.remainingDistance <= navmeshEnemy.stoppingDistance)
            {
                isMoving = false;
            }

            return NodeState.Running;
        }


        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isMoving = false;
        if (sound != null)
        {

            SoundManager.Instance.DestroyObject(sound);
        }
        return NodeState.Failure;
    }

    private Vector3 GetRandomPositionOutsideCircle(Vector3 center, float innerRadius, float outerRadius)
    {
        const int maxAttempts = 10; // ��ȿ�� ��ġ�� ã�� ���� �ִ� �õ� Ƚ��
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // outerRadius �ȿ��� ������ ����� �Ÿ��� ���ϵ� innerRadius �ٱ��ʿ����� ã��
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
            Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);

            Vector3 targetPosition = center + randomDirection;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(targetPosition, out navHit, outerRadius, NavMesh.AllAreas))
            {
                Vector3 resultPosition = navHit.position;

                // Ư�� y���� ����� ���
                float[] possibleValues = { -49.1f, -44.7f, -44.8f, -31.6f, -32.6f, -42.4f, -40.1f, -37.5f, -35.1f, -32.5f };
                resultPosition.y = possibleValues[Random.Range(0, possibleValues.Length)];

                // ��ǥ ��ġ�� �� ��ó�� ���� ������ ��ȯ
                if (!IsNearWall(resultPosition))
                {
                    return resultPosition;
                }
            }

            attempts++;
        }

        return center; // ��ȿ�� ��ġ�� ã�� ���ϸ� ���� �߽� ��ġ�� ��ȯ
    }

    private bool IsNearWall(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.Raycast(position, position + Vector3.forward * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position - Vector3.forward * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position + Vector3.right * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position - Vector3.right * 1.0f, out hit, NavMesh.AllAreas))
        {
            return true; 
        }
        return false;
    }

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
                bullet1.SetUp(eye.transform, player.transform);
                SoundManager.Instance.PlayEffectSound(SFX.Pistol, navmeshEnemy.transform);
                //Debug.Log(".�����߾ȳ�" + player.transform.position);
                bulletsSpawned++;

                ///TO DO : ����ü ���� �迹����
                //StartCoroutine(test(bullet, "Ebullet"));
            }
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet2()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 30;
        bool playFirstSound = true; // ���带 �����ϱ� ���� �÷���

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet1");
            Bullet bullet2 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet2.SetUp(eye.transform, player.transform);
                SoundManager.Instance.PlayEffectSound(SFX.Rifle_1, navmeshEnemy.transform);
//                 if (playFirstSound)
//                 {
//                 }
//                 else
//                 {
//                     //SoundManager.Instance.PlayEffectSound(SFX.Rifle_2, navmeshEnemy.transform);
//                 }
                playFirstSound = !playFirstSound;
                bulletsSpawned++;
            }
        }
        yield return new WaitForSeconds(4f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet3()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 6; // �� ������ �Ѿ� ��
        const int bulletsPerWave = 3; // �� ���� ������ �Ѿ� ��

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet2");
                Bullet bullet4 = bullet.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet4.SetUp(eye.transform, player.transform);
                    SoundManager.Instance.PlayEffectSound(SFX.Shotgun, navmeshEnemy.transform);
                    bulletsSpawned++;

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

    //�� ���� �� �� �ִ°�? 
    public bool CanReach(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        navmeshEnemy.CalculatePath(targetPosition, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    //�ڷ� ���� �ִ°ž�? 
    private bool IsMovingBackward(NavMeshAgent agent, Vector3 directionToPlayer)
    {
        // �̵� ���� (Velocity ����)�� �÷��̾ ���� ������ ��
        Vector3 agentVelocity = agent.velocity.normalized;
        float dotProduct = Vector3.Dot(agentVelocity, directionToPlayer);

        // dotProduct�� -0.5 ���϶��, �̵� ������ �÷��̾� �ݴ������� ���ϰ� �ִ� ����
        return dotProduct < -0.5f;
    }
}

public class ChaseAction : EnemyAction
{
    private Vector3 initialPlayerPosition;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float chaseDuration = 3f; 
    private float chaseTimer = 0f;
    private bool hasReachedDestination = false;
    private bool isInitialized = false;
    private Animator animator;
    private float timer;
    private Coroutine spawnCoroutine;
    private Transform eye;

    private Transform center;
    private float radius;
    private float detectionAngle;

    float stuckTime = 0f; // ���� ��ġ�� �ӹ� �ð�

    private bool once;
    private GameObject sound;
    private int npcType;
    public ChaseAction(Transform player, NavMeshAgent navMeshAgent, Transform eye, Transform center, float radius, float detectionAngle, int npctype,Animator animator)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
        this.eye = eye;

        this.center = center;
        this.radius = radius;
        this.detectionAngle = detectionAngle;

        this.npcType = npctype;
    }

    public override NodeState Execute()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.updateRotation = false;
        if (!isInitialized)
        {
            initialPlayerPosition = player.position;
            isInitialized = true;
            //Debug.Log("�ʱ���ġ " + initialPlayerPosition);
        }

        timer += Time.deltaTime;
        if (timer >= 1 && spawnCoroutine == null)
        {
            if (npcType == 0)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet());
            }
            else if (npcType == 1)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet2());
            }
            else if (npcType == 2)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet3());
            }
            timer = 0f;
        }

        if (!once)
        {
            once = true;
            sound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Enemy_Run, navMeshAgent.transform);
        }

        animator.Play("chase");

        Vector3 directionToPlayer = (player.position - navMeshAgent.transform.position).normalized;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);


            Vector3 rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, lookRotation, Time.deltaTime * navMeshAgent.angularSpeed).eulerAngles;
            navMeshAgent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        }

        float distanceToCenter = Vector3.Distance(player.position, center.transform.position);

        //if (distanceToCenter <= radius)
        //{

        //    Vector3 toPlayer = player.position - center.transform.position;
        //    Vector3 toPlayerFlat = toPlayer;
        //    toPlayerFlat.y = 0;

        //    float angleToPlayer = Vector3.Angle(toPlayerFlat, navMeshAgent.transform.forward);

        //    float halfDetectionAngle = detectionAngle / 4f;

        //    if (angleToPlayer <= halfDetectionAngle)
        //    {
        //        once = false;
        //        SoundManager.Instance.DestroyObject(sound);
        //        return NodeState.Failure;
        //    }
        //}

        float elapsedTime = 0f; // ��� �ð� ����
        float checkInterval = 1.0f; // 1�ʸ��� �˻�
        float stuckDuration = 1.3f; // 3�� ���� ���� �ڸ��� �ӹ��� ���
        Vector3 lastPosition; // ������ ��ġ ����

        float positionThreshold = 3.0f; // NPC�� ��ǥ ��ġ�� �����ϱ� ���� ��� ����

        if (navMeshAgent != null && player != null)
        {

            if (navMeshAgent != null && player != null)
            {
                Vector3 currentDestination = navMeshAgent.destination;

                // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ� üũ
                float distanceToTarget = Vector3.Distance(navMeshAgent.transform.position, currentDestination);

                // ��ġ ��: NPC�� ��ǥ ��ġ���� �Ÿ� ���̸� ��� ������ üũ
                if (Mathf.Abs(navMeshAgent.transform.position.x - currentDestination.x) <= positionThreshold &&
                    Mathf.Abs(navMeshAgent.transform.position.y - currentDestination.y) <= positionThreshold &&
                    Mathf.Abs(navMeshAgent.transform.position.z - currentDestination.z) <= positionThreshold)
                {
                    stuckTime += Time.deltaTime; // ���� ��ġ�� �ӹ��� �ð� ����

                }
                else
                {
                    stuckTime = 0f; // ��ġ�� ����Ǹ� �ð� �ʱ�ȭ
                }

                // 3�� �̻� ���� �ڸ��� ���� ���
                if (stuckTime >= stuckDuration)
                { 
                    stuckTime = 0f; // �ð� �ʱ�ȭ 
                    once = false;
                    if(sound != null) 
                    {

                    SoundManager.Instance.DestroyObject(sound);
                    }
                    return NodeState.Failure;
                }
            }
        }

            if (!hasReachedDestination)
            {
            navMeshAgent.SetDestination(initialPlayerPosition);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    hasReachedDestination = true;
                    isInitialized = false;
                    chaseTimer = 0f; 
                    //Debug.Log("��ǥ ��ġ ����");
                    if (spawnCoroutine != null)
                    {
                        CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                        spawnCoroutine = null;
                    }
                    once = false;
                    if (sound != null)
                    {

                        SoundManager.Instance.DestroyObject(sound);
                    }
                    return NodeState.Failure;
                }
            }
        }
        else
        {
            
            chaseTimer += Time.deltaTime;

            
            if (chaseTimer >= chaseDuration)
            {
                isInitialized = false;
                if (spawnCoroutine != null)
                {
                    CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
                if (sound != null)
                {

                    if (sound != null)
                    {

                        SoundManager.Instance.DestroyObject(sound);
                    }
                }
                return NodeState.Failure;
            }
        }

        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
        }
        if (sound != null)
        {

            SoundManager.Instance.DestroyObject(sound);
        }
        return NodeState.Failure;
    }

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
                bullet1.SetUp(eye.transform, player.transform);
                SoundManager.Instance.PlayEffectSound(SFX.Pistol, navMeshAgent.transform);
                //Debug.Log(".�����߾ȳ�" + player.transform);
                bulletsSpawned++;

                ///TO DO : ����ü ���� �迹����
                //StartCoroutine(test(bullet, "Ebullet"));
            }
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet2()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 30;
        bool playFirstSound = true; // ���带 �����ϱ� ���� �÷���

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet1");
            Bullet bullet2 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet2.SetUp(eye.transform, player.transform);
                if (playFirstSound)
                {
                    SoundManager.Instance.PlayEffectSound(SFX.Rifle_1, navMeshAgent.transform);
                }
                else
                {
                    //SoundManager.Instance.PlayEffectSound(SFX.Rifle_2, navMeshAgent.transform);
                }
                playFirstSound = !playFirstSound;
                bulletsSpawned++;
            }
        }
        yield return new WaitForSeconds(4f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet3()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 6; // �� ������ �Ѿ� ��
        const int bulletsPerWave = 3; // �� ���� ������ �Ѿ� ��

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet2");
                Bullet bullet4 = bullet.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet4.SetUp(eye.transform, player.transform);
                    SoundManager.Instance.PlayEffectSound(SFX.Shotgun, navMeshAgent.transform);
                    bulletsSpawned++;

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

    //�� ���� �� �� �ִ°�? 
    public bool CanReach(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(targetPosition, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }
}
public class HurtAction : EnemyAction
{
    private FloatWrapper hpWrapper;
    private Animator animator;
    private NavMeshAgent agent;
    private BoolWrapper isHit = new BoolWrapper(false);
    private bool isWaiting = false;
    private bool isFirstExecution;
    private float timer = 0f;


    public HurtAction(NavMeshAgent agent ,FloatWrapper hpWrapper, BoolWrapper ishit, Animator animator)
    {
        this.hpWrapper = hpWrapper;
        this.animator = animator;
        this.agent = agent;
        isHit = ishit;
    }

    public override NodeState Execute()
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            timer = 0f; // Ÿ�̸� �ʱ�ȭ
            isWaiting = true; // ��� ���·� ����
            SoundManager.Instance.PlayEffectSound(SFX.Enemy_Hurt, agent.transform); 
        }

        // Ÿ�̸Ӹ� Time.deltaTime���� ������Ű��
        if (isWaiting)
        {
            timer += Time.deltaTime;
            agent.isStopped = true;
            isWaiting = true;
            animator.Play("Hurt");
            //StartCoroutine(WaitAndReturn());

            // 1�ʰ� ������ ��� ���¸� �����ϰ� Failure ��ȯ
            //if (timer >= 1f)
            //{
            //    isWaiting = false;
            //    return NodeState.Failure;
            //}

            return NodeState.Success; // ��� �߿��� Running ���� ��ȯ
        }

        // 1�ʰ� ���� �Ŀ��� Failure ��ȯ
        return NodeState.Success;
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(1f);

        isWaiting = false; // ��� ���� ����
    }

    public override NodeState Stop()
    {
        isFirstExecution = false;
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }
}

public class DieAction : EnemyAction
{
    private NavMeshAgent agent;
    Transform a;
    private Rigidbody rigid;
    Animator animator;
    private FloatWrapper isHit;
    private int npctype;
    private bool isDead;
    private bool once;
    private bool Tuto;

    public DieAction(NavMeshAgent agent, FloatWrapper isHit, int npcType, Animator animator, bool tuto)
    {
        this.agent = agent;
        a = agent.transform;
        this.animator = animator;
        this.isHit = isHit;
        this.npctype = npcType;
        Tuto = tuto;

        //rigid = agent.gameObject.gameObject.GetComponent<Rigidbody>();
        //rigid.useGravity = true;

    }

    public override NodeState Execute()
    {
        if(isHit.Value > 0)
        {
            return NodeState.Failure;
        }

        if(!isDead && !Tuto)
        {
            //Debug.Log(agent.transform.position);
            switch (npctype)
            {
                case 0:
                    GameObject a = PoolManager.Instance.GetPooledObject("Bere", agent.transform.position, Quaternion.identity);
                    Beretta92 pistol = a.GetComponent<Beretta92>();
                    pistol.SetUP(agent.transform.position, Quaternion.identity);
                    break;
                case 1:
                    GameObject a1 =  PoolManager.Instance.GetPooledObject("Scar", agent.transform.position, Quaternion.identity);
                    FNSCAR rifle = a1.GetComponent<FNSCAR>();
                    rifle.SetUP(agent.transform.position, Quaternion.identity);
                    break;
                case 2:
                    GameObject a2 = PoolManager.Instance.GetPooledObject("Bene", agent.transform.position, Quaternion.identity);
                    benelli828u shotgun = a2.GetComponent<benelli828u>();
                    shotgun.SetUP(agent.transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Weapon_Drop, agent.transform);
            isDead = true;
        }

        if(!once)
        {
            once = true;
            int randomIndex = Random.Range(0, 3);

            switch (randomIndex)
            {
                case 0:
                    SoundManager.Instance.PlayEffectSound(SFX.Enemy_Death_1, agent.transform);
                    break;
                case 1:
                    SoundManager.Instance.PlayEffectSound(SFX.Enemy_Death_2, agent.transform);
                    break;
                case 2:
                    SoundManager.Instance.PlayEffectSound(SFX.Enemy_Death_3, agent.transform);
                    break;
            }
        }

        //Debug.Log("���");
        //animator.enabled = false;
        ChangeLayersRecursively(a, "DeadNPC");
        //ChangeTagRecursively(agent.gameObject, "DeadNPC");
        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }

    public void ChangeLayersRecursively(Transform parent, string layerName)
    {
        // �θ� ������Ʈ�� ���̾� ����
        parent.gameObject.layer = LayerMask.NameToLayer(layerName);

        // ��� �ڽ� ������Ʈ�� ���� ��������� ���̾� ����
        foreach (Transform child in parent)
        {
            ChangeLayersRecursively(child, layerName);
        }
    }

    public void ChangeTagRecursively(GameObject obj, string tag)
    {
        if (obj != null)
        {
            obj.tag = tag;
            foreach (Transform child in obj.transform)
            {
                ChangeTagRecursively(child.gameObject, tag);
            }
        }
    }
}

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("CoroutineRunner");
                _instance = obj.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(obj); 
            }
            return _instance;
        }
    }
}
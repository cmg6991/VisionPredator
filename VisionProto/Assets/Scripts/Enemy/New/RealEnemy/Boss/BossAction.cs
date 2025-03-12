using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static BTNode;

/// <summary>
/// 코루틴러너는 일반적 액션에 있음
/// </summary>
public abstract class BossAction
{
    public abstract NodeState Execute();
    public abstract NodeState Stop();

    protected void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineRunner.Instance.StartCoroutine(coroutine);
    }
}

public class BIdlee : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isFirstExecution;
    private float distanceToCenter;
    GameObject idleSound;
    bool gogogo;

    BossHP rHP;

    public BIdlee(NavMeshAgent agent, Transform player, Animator animator)
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        rHP = agent.gameObject.GetComponent<BossHP>();
        gogogo = false;
    }
    public override NodeState Execute()
    {
        if(isFirstExecution) 
        {
            return NodeState.Failure;
        }

        if (!isFirstExecution)
        {
            animator.speed = 0;
            rHP.enabled = true;
            StartCoroutine(DelayLog());
            isFirstExecution = true;
        }

        Debug.Log("아이들실행");
        //rHP.enabled = false;
        distanceToCenter = Vector3.Distance(player.position, agent.transform.position);
        
        //animator.speed = 1;
        //if (!isFirstExecution)
        //{
        //    isFirstExecution = true;
        //    //idleSound = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Boss_Idle, animator.gameObject.transform);
        //}

        if (distanceToCenter <= 45 && gogogo)
        {
            //FirstExecution = false;
            rHP.enabled = true;
            //SoundManager.Instance.DestroyObject(idleSound);
            //animator.speed = 1;
            //isFirstExecution = true;
            return NodeState.Failure;
        }

        return NodeState.Success;

    }

    private IEnumerator DelayLog()
    {
        Vector3 startPosition = agent.transform.position + new Vector3(0,9,0);
        Vector3 targetPosition = startPosition;
        targetPosition.y += -6.95f; 

        float elapsedTime = 0f;

        animator.speed = 1f;

        while (elapsedTime < 0.4f)
        {
            agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.4f);
            elapsedTime += Time.deltaTime;
            animator.Play("Armature|slam", -1, 0.7f);

            yield return null;
        }
        agent.transform.position = agent.transform.position + new Vector3(0, 9, 0);

        EffectManager.Instance.ExecutionEffect(Effect.BossAppear, agent.transform);
        yield return new WaitForSeconds(1f); // 3초 대기
        animator.Play("Scene", -1, 0.0f);
        yield return new WaitForSeconds(2.0f); // 3초 대기

        gogogo = true;
    }

    public override NodeState Stop() { return NodeState.Failure; }
}

public class BGrog : BossAction
{
    private bool isFirstExecution;
    private bool isWaiting;
    private Animator animator;
    private float timer = 0f;
    public BossUI bossUI;
    private FloatWrapper hp;
    private BoolWrapper grog;
    private BoolWrapper grog2;

    public BGrog(Animator animator, FloatWrapper bossHP, BoolWrapper boolWrapper, BoolWrapper boolWrapper2)
    {
        isFirstExecution = false;
        isWaiting = false;
        this.animator = animator;
        bossUI = GameObject.Find("BossHP").GetComponent<BossUI>();
        hp = bossHP;
        grog = boolWrapper;
        grog2 = boolWrapper2;
    }

    public override NodeState Execute()
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            timer = 0f; // 타이머 초기화
            StartCoroutine(DecreaseGrogBar());
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Stun, animator.gameObject.transform);
        }

        timer += Time.deltaTime;
        animator.Play("Armature|Stun");
        if (timer >= 1.6f)
        {
            Debug.Log("그로기 끝");
            if (hp.Value > 1500)
            {
                grog.Value = true;
            }
            else
            {
                grog.Value = true;
                grog2.Value = true;
            }

            bossUI.grogBar.fillAmount = 1f;
            timer = 0f;
            isFirstExecution = false;
            return NodeState.Failure;
        }

        return NodeState.Success;
    }

    public override NodeState Stop()
    {
        return NodeState.Failure;
    }

    private IEnumerator DecreaseGrogBar()
    {
        float duration = 1.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Debug.Log("그로기 중");
            grog.Value = false;
            bossUI.grogBar.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bossUI.grogBar.fillAmount = 0f;
    }
}

public class BPazeOne : BossAction
{
    private Transform player;
    private Transform RPlayer;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private LineRenderer lineRenderer;
    public float moveDuration = 15f;

    BossAttack bossAttack;
    BossDash bossDash;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그

    private float distanceToCenter;

    private Transform machineTrm;

    IntWrapper grabCount;
    private BoxCollider boxCol;

    FloatWrapper hp;
    bool isAttacking;
    //Torso torso;

    public BPazeOne(NavMeshAgent agent, Transform player, Animator animator, Transform machineTrm, IntWrapper a, FloatWrapper HP) 
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        this.machineTrm = machineTrm;
        grabCount = a;
        lineRenderer = GameObject.Find("Grab").GetComponent<LineRenderer>();
        bossAttack = agent.gameObject.GetComponentInChildren<BossAttack>();
        bossDash = agent.gameObject.GetComponentInChildren<BossDash>();
        boxCol = agent.gameObject.GetComponent<BoxCollider>();
        hp = HP;
    }
    public override NodeState Execute() 
    {
        Debug.Log(hasFinishedPattern);

        if (!isFirstExecution)
        {
            grabCount.Value = 0;
            isFirstExecution = true;
            pattern = Random.Range(0, 3);
        }

        distanceToCenter = Vector3.Distance(player.position, agent.transform.position);
        if (!hasFinishedPattern)
        {
            if (distanceToCenter <= 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern3();
                        break;
                    case 1:
                        PerformPattern2();
                        break;
                    case 2:
                        PerformPattern4();
                        break;
                }
            }
            else
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern0();
                        break;
                    case 1:
                        PerformPattern1();
                        break;
                    case 2:
                        PerformPattern2();
                        break;
                }
            }
        }
        Debug.Log(grabCount.Value);

        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");

            if (distanceToCenter <= 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                hasFinishedPattern = false;
                isFirstExecution = false;
                lineRenderer.positionCount = 0;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                lineRenderer.positionCount = 0;
                return NodeState.Failure;
            }
        }

        return NodeState.Running;
    }

    private void PerformPattern0()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Machine Gun");



        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern1()
    {
        // 패턴 1의 동작
        Debug.Log("패턴 1 실행");
        timer += Time.deltaTime;
        if (isAttacking)
        {
            animator.Play("Armature|Grab");
        }
        else
        {
            animator.Play("Scene");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Grabbing(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true; 
            }));

            timer = 0f;
        }
    }

    private void PerformPattern2()
    {
        // 패턴 2의 동작
        Debug.Log("패턴 2 실행");
        timer += Time.deltaTime;
        if(isAttacking)
        {
            animator.Play("Armature|Dash");
        }
        else
        {
            animator.Play("Scene");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Dash(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 00 실행");
        timer += Time.deltaTime;
        if (isAttacking)
        {
            animator.Play("Armature|slam");
        }
        else
        {
            animator.Play("Scene");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Jump(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern4()
    {
        // 패턴 2의 동작
        Debug.Log("패턴 22 실행");
        timer += Time.deltaTime;
        if (isAttacking)
        {
            animator.Play("Armature|attack");
        }
        else
        {
            animator.Play("Scene");
        }


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(AttackClose(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }

    private IEnumerator SpawnBullet(System.Action onComplete = null)
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 30;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.2f);

            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);


                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            }

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet3");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet1.SetUp(machineTrm, player.transform);
                bulletsSpawned++;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Boss_MachineGun, animator.gameObject.transform);
        }
        yield return new WaitForSeconds(1f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = true;
        onComplete?.Invoke();
    }

    private IEnumerator Grabbing(System.Action onComplete)
    {
        RPlayer = this.player.parent;
        Vector3 startPosition = RPlayer.position;
        Vector3 targetPosition = agent.transform.position + new Vector3(0f, 2, 0);
        float elapsedTime = 0f;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Grab, animator.gameObject.transform);

        Vector3 rPlayerPosition = RPlayer.transform.position;

        Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;

        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
        rotationToPlayer.eulerAngles = new Vector3(0, rotationToPlayer.eulerAngles.y, rotationToPlayer.eulerAngles.z);

        EffectManager.Instance.ExecutionEffect(Effect.BossGrab, agent.transform.position + agent.transform.forward * 5f + new Vector3(0f, 2), rotationToPlayer);

        // Loop to continue checking the position for 0.2 seconds
        while (elapsedTime < 0.5f)
        {
            // Update the direction to the player and rotation
            directionToPlayer = (player.position - agent.transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }

            elapsedTime += Time.deltaTime; // Increase elapsed time
            yield return null; // Wait until the next frame to continue the loop
        }

        isAttacking = true;
        Vector3 playerPosition = player.transform.position;
        if (Mathf.Abs(rPlayerPosition.x - playerPosition.x) > 5 ||
            Mathf.Abs(rPlayerPosition.y - playerPosition.y) > 5 ||
            Mathf.Abs(rPlayerPosition.z - playerPosition.z) > 5)
        {
            isAttacking = false;
            yield return null;
        }
        else
        {
            while (elapsedTime < moveDuration)
            {
                if (grabCount.Value > 2)
                {
                    SoundManager.Instance.PlayEffectSound(SFX.Boss_GrabOff, animator.gameObject.transform);
                    lineRenderer.positionCount = 0;

                    yield return new WaitForSeconds(2f);
                    bossAttack.enabled = false;
                    spawnCoroutine = null;
                    onComplete?.Invoke();
                    grabCount.Value = 0;
                    boxCol.enabled = true;

                    yield break;
                }

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(index: 0, agent.transform.position + new Vector3(0f, 2, 0));
                lineRenderer.SetPosition(index: 1, player.transform.position + new Vector3(0.3f, -1, 0));

                RPlayer.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;

                bossAttack.enabled = true;
                bossAttack.damage = 20;
                boxCol.enabled = false;

                yield return null;
            }
        }

        bossAttack.enabled = false;
        boxCol.enabled = true;
        grabCount.Value = 0;
        lineRenderer.positionCount = 0;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_GrabOff, animator.gameObject.transform);
        isAttacking = false;
        yield return new WaitForSeconds(1f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = true;
        onComplete?.Invoke();
    }

    private IEnumerator Dash(System.Action onComplete)
    {
        Vector3 startPosition = agent.transform.position;
        Vector3 initialTargetPosition = player.transform.position + new Vector3(0, -2, 0);
        float elapsedTime = 0f;

        // 2초 동안 플레이어의 위치를 추적
        float trackingDuration = 2f;
        float trackingElapsedTime = 0f;
        Vector3 targetPosition = initialTargetPosition;

        while (trackingElapsedTime < trackingDuration)
        {
            // 2초 동안 지속적으로 플레이어 위치 업데이트
            targetPosition = player.transform.position + new Vector3(0, -2, 0);

            // 플레이어 방향으로 회전
            Vector3 directionToPlayer = (targetPosition - agent.transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }

            trackingElapsedTime += Time.deltaTime;
            yield return null;
        }
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, agent.transform);
        Quaternion rotationToPlayer = Quaternion.LookRotation((targetPosition - startPosition).normalized);
        rotationToPlayer.eulerAngles = new Vector3(0, rotationToPlayer.eulerAngles.y, rotationToPlayer.eulerAngles.z);

        EffectManager.Instance.ExecutionEffect(Effect.BossDash, agent.transform.position + agent.transform.forward * 5f, rotationToPlayer);


        while (elapsedTime < 0.5f) //여기 변경
        {
            isAttacking = true;
            bossDash.enabled = true;
            bossDash.damage = 15;
            agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.5f); //여기도 함께
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bossDash.enabled = false;
        isAttacking = false;
        yield return new WaitForSeconds(1.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = true;
        onComplete?.Invoke();
    }

    private IEnumerator Jump(System.Action onComplete)
    {
        Vector3 startPosition = agent.transform.position;
        Vector3 targetPosition = player.transform.position + new Vector3(0, -1f, 0);
        float jumpHeight = 5f; // 점프 높이
        float jumpDuration = 0.6f; // 점프 지속 시간
        float elapsedTime = 0f;

        // 3초 동안 플레이어의 위치를 추적
        float trackingDuration = 3f;
        float trackingElapsedTime = 0f;
        Vector3 a = Vector3.zero;
        while (trackingElapsedTime < trackingDuration)
        {
            Vector3 playerPosition = player.transform.position;
            targetPosition = playerPosition + new Vector3(0, -1f, 0);
            a = playerPosition;
            a.y = -32.7f;

            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }

            trackingElapsedTime += Time.deltaTime;
            yield return null;
        }

        // 3초 후 갱신된 'a' 위치에 효과 생성
        EffectManager.Instance.ExecutionEffect(Effect.BossCutDownDecal, a, Quaternion.identity);



        while (elapsedTime < jumpDuration)
        {
            isAttacking = true;
            float progress = elapsedTime / jumpDuration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

            float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

            agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

            elapsedTime += Time.deltaTime;

            bossDash.enabled = true;
            bossDash.damage = 15;

            if (elapsedTime >= jumpDuration)
            {
                agent.transform.position = targetPosition;
                isAttacking = false;
                break;
            }

            yield return null;
        }

        // 정확한 목표 위치에 도달
        EffectManager.Instance.ExecutionEffect(Effect.BossCutDown, agent.transform);
        agent.transform.position = targetPosition;

        EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);


        bossDash.enabled = false;
        yield return new WaitForSeconds(1.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = true;
        isAttacking = false;
        onComplete?.Invoke();
    }

    private IEnumerator AttackClose(System.Action onComplete)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1) //여기 변경
        {
            isAttacking = true;
            bossDash.enabled = true;
            bossDash.damage = 10;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        bossDash.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Attack, agent.transform);
        yield return new WaitForSeconds(0.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = true;
        onComplete?.Invoke();
    }

    public override NodeState Stop() {
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = false;
        isFirstExecution = false;
        lineRenderer.positionCount = 0;
        return NodeState.Failure; 
    }
}

public class BPazeOneClose : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그

    BossDash bossDash; 

    private float distanceToCenter;
    private IntWrapper grabCount;

    FloatWrapper hp;


    public BPazeOneClose(NavMeshAgent agent, Transform player, Animator animator, IntWrapper a, FloatWrapper HP)
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        grabCount = a;
        bossDash = agent.gameObject.GetComponentInChildren<BossDash>();
        hp = HP;
    }

    public override NodeState Execute()
    {
        Debug.Log("페이즈원클로즈");

        if (!isFirstExecution)
        {
            isFirstExecution = true;
            pattern = Random.Range(0, 3);
        }

        if (!hasFinishedPattern)
        {
            switch (pattern)
            {
                case 0:
                    PerformPattern3();
                    break;
                case 1:
                    PerformPattern1();
                    break;
                case 2:
                    PerformPattern4();
                    break;
            }
        }


        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter > 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                return NodeState.Failure;
            }
        }
        return NodeState.Running;
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 00 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|slam");

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Jump(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern1()
    {
        Debug.Log("패턴 11 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Dash");


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Dash(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern4()
    {
        // 패턴 2의 동작
        Debug.Log("패턴 22 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|attack");

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(AttackClose(() =>
            {
                spawnCoroutine = null;
                hasFinishedPattern = true;
            }));

            timer = 0f;
        }
    }


    private IEnumerator Jump(System.Action onComplete)
    {
        yield return new WaitForSeconds(3f);

        Vector3 playerPosition = player.transform.position;
        Vector3 startPosition = agent.transform.position;
        Vector3 targetPosition = player.transform.position + new Vector3(0,-1f,0);

        float jumpHeight = 5f; // 점프 높이
        float jumpDuration = 1f; // 점프 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float progress = elapsedTime / jumpDuration;            
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

            float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

            agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

            elapsedTime += Time.deltaTime;

            bossDash.enabled = true;
            bossDash.damage = 15;

            if (elapsedTime >= jumpDuration)
            {
                agent.transform.position = targetPosition; 
                break; 
            }

            yield return null;
        }

        // 정확한 목표 위치에 도달
        agent.transform.position = targetPosition;
        EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);


        bossDash.enabled = false;
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator Dash(System.Action onComplete)
    {
        yield return new WaitForSeconds(2f);
        Vector3 startPosition = agent.transform.position;
        Vector3 targetPosition = player.transform.position + new Vector3(0, -2, 0);
        float elapsedTime = 0f;
        Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, agent.transform);

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);


            Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
            agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        }

        while (elapsedTime < 3) //여기 변경
        {
            bossDash.enabled = true;
            bossDash.damage = 15;
            agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 3); //여기도 함께
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bossDash.enabled = false;
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator AttackClose(System.Action onComplete)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1) //여기 변경
        {
            bossDash.enabled = true;
            bossDash.damage = 10;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bossDash.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Attack, agent.transform);
        yield return new WaitForSeconds(1f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    public override NodeState Stop() { return NodeState.Failure; }
}

public class BPazeTwo : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그
    BossHP rHP;
    BossDash bossDash;

    private float distanceToCenter;
    private Transform railPivot;

    FloatWrapper hp;
    bool isAttacking;

    PlayerStateMachine p;

    public BPazeTwo(NavMeshAgent agent, Transform player, Animator animator, Transform Pivot, FloatWrapper HP)
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        railPivot = Pivot;
        rHP = agent.gameObject.GetComponent<BossHP>();
        bossDash = agent.gameObject.GetComponentInChildren<BossDash>();
        hp = HP;
        p = GameObject.Find("Player").gameObject.GetComponent<PlayerStateMachine>();
    }
    public override NodeState Execute() 
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            pattern = Random.Range(0, 3);
        }

        if (!hasFinishedPattern)
        {
            if (distanceToCenter <= 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern0();
                        break;
                    case 1:
                        PerformPattern1();
                        break;
                    case 2:
                        PerformPattern2();
                        break;
                }


            }
            else
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern3();
                        break;
                    case 1:
                        PerformPattern2();
                        break;
                    case 2:
                        PerformPattern1();
                        break;
                }
            }
        }


        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter > 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                return NodeState.Failure;
            }
        }

        return NodeState.Running; 
    }

    private void PerformPattern0()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|RailGun");



        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(RailGun(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }
    bool isGot;
    bool once = false;
    private void PerformPattern1()
    {
        Debug.Log("패턴 1 실행");
        timer += Time.deltaTime;

        //EffectManager.Instance.ExecutionEffect("Shield Effect", agent.transform, 10f);
        if(isAttacking)
        {
            if (isGot && !once)
            {
                p.moveSpeed = 2.5f;
                p.jumpForce = 1;
                p.VPSpeed = 4;
                p.VPJumeForce = 1;
                once = true;
            }
            animator.Play("Armature|Shield");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            StartCoroutine(ExecuteEffectCoroutine());
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Invincibility(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern2()
    {
        Debug.Log("패턴 2 실행");
        timer += Time.deltaTime;
        if(isAttacking)
        {
            animator.Play("Armature|Dash");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(DDash(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        if (isAttacking)
        {
            animator.Play("Armature|slam");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(JJump(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private IEnumerator ExecuteEffectCoroutine()
{
    float duration = 1f; // 총 실행 시간
    float interval = 2f; // 간격
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        EffectManager.Instance.ExecutionEffect(Effect.BossScanner, agent.transform);
        yield return new WaitForSeconds(interval);
        elapsedTime += interval;
    }
}


    private IEnumerator RailGun(System.Action onComplete)
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 40;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);
            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);


                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            }
            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet4");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet1.SetUp(railPivot.transform, player.transform);
                bulletsSpawned++;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Railgun, animator.gameObject.transform);
        }
        yield return new WaitForSeconds(2f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator Invincibility(System.Action onComplete)
    {
        //rHP.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Shield, animator.gameObject.transform);
        isAttacking = true;
        float startTime = Time.time; // 시작 시간 기록

        while (Time.time - startTime < 1f)
        {
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter < 15)
            {
                isGot = true;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        //rHP.enabled = true;
        if(once)
        {
            StartCoroutine(something());
        }
        isAttacking = false;
        isGot = false;
        once = false;
        yield return new WaitForSeconds(1.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator something()
    {
        yield return new WaitForSeconds(10f);
        p.moveSpeed = 5;
        p.jumpForce = 2;
        p.VPSpeed =8;
        p.VPJumeForce =2.35f;
        yield return null;
    }

    private IEnumerator DDash(System.Action onComplete)
    {
        for (int i = 0; i < 2; i++) 
        {
            Vector3 startPosition = agent.transform.position;
            Vector3 initialTargetPosition = player.transform.position + new Vector3(0, -2, 0);
            float elapsedTime = 0f;

            // 2초 동안 플레이어의 위치를 추적
            float trackingDuration = 2f;
            float trackingElapsedTime = 0f;
            Vector3 targetPosition = initialTargetPosition;

            while (trackingElapsedTime < trackingDuration)
            {
                // 2초 동안 지속적으로 플레이어 위치 업데이트
                targetPosition = player.transform.position + new Vector3(0, -2, 0);

                // 플레이어 방향으로 회전
                Vector3 directionToPlayer = (targetPosition - agent.transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                    agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                }

                trackingElapsedTime += Time.deltaTime;
                yield return null;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, agent.transform);
            Quaternion rotationToPlayer = Quaternion.LookRotation((targetPosition - startPosition).normalized);
            rotationToPlayer.eulerAngles = new Vector3(0, rotationToPlayer.eulerAngles.y, rotationToPlayer.eulerAngles.z);

            EffectManager.Instance.ExecutionEffect(Effect.BossDash, agent.transform.position + agent.transform.forward * 5f, rotationToPlayer);



            while (elapsedTime < 0.5f)
            {
                isAttacking = true;
                bossDash.enabled = true;
                bossDash.damage = 15;
                agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            bossDash.enabled = false;
            isAttacking = false;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator JJump(System.Action onComplete)
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -1f, 0);
            float jumpHeight = 5f; // 점프 높이
            float jumpDuration = 0.6f; // 점프 지속 시간
            float elapsedTime = 0f;

            // 3초 동안 플레이어의 위치를 추적
            float trackingDuration = 3f;
            float trackingElapsedTime = 0f;
            Vector3 a = Vector3.zero;
            while (trackingElapsedTime < trackingDuration)
            {
                Vector3 playerPosition = player.transform.position;
                targetPosition = playerPosition + new Vector3(0, -1f, 0);
                a = playerPosition;
                a.y = -32.7f;

                Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                    agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                }

                trackingElapsedTime += Time.deltaTime;
                yield return null;
            }

            // 3초 후 갱신된 'a' 위치에 효과 생성
            EffectManager.Instance.ExecutionEffect(Effect.BossCutDownDecal, a, Quaternion.identity);



            while (elapsedTime < jumpDuration)
            {
                isAttacking = true;
                float progress = elapsedTime / jumpDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

                float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

                agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

                elapsedTime += Time.deltaTime;

                bossDash.enabled = true;
                bossDash.damage = 15;

                if (elapsedTime >= jumpDuration)
                {
                    agent.transform.position = targetPosition;
                    break;
                }

                yield return null;
            }

            EffectManager.Instance.ExecutionEffect(Effect.BossCutDown, agent.transform);
            agent.transform.position = targetPosition;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);
            EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);
            bossDash.enabled = false;

        }
        yield return new WaitForSeconds(0.5f);

        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isAttacking = false;
        onComplete?.Invoke();
    }


    public override NodeState Stop() {
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = false;
        isFirstExecution = false;
        return NodeState.Failure; 
    }
}

public class BPazeTwoClose : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그
    BossHP rHP;
    BossDash boseDash;

    private float distanceToCenter;

    FloatWrapper hp;

    public BPazeTwoClose(NavMeshAgent agent, Transform player, Animator animator, FloatWrapper HP) 
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        rHP = agent.gameObject.GetComponent<BossHP>();
        boseDash = agent.gameObject.GetComponentInChildren<BossDash>();
        hp = HP;
    }
    public override NodeState Execute() 
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            pattern = Random.Range(0, 3);
        }

        if (!hasFinishedPattern)
        {
            switch (pattern)
            {
                case 0:
                    PerformPattern3();
                    break;
                case 1:
                    PerformPattern1();
                    break;
                case 2:
                    PerformPattern2();
                    break;
            }
        }


        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter > 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                return NodeState.Failure;
            }
        }

        return NodeState.Running;
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|slam");

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(JJump(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern1()
    {
        Debug.Log("패턴 1 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Dash");


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(DDash(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern2()
    {
        Debug.Log("패턴 2 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Shield");
        //EffectManager.Instance.ExecutionEffect("Shield Effect", agent.transform, 10f);


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Invincibility(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private IEnumerator JJump(System.Action onComplete)
    {
        for (int i = 0; i < 2; i++) 
        {
            yield return new WaitForSeconds(3f);

            Vector3 playerPosition = player.transform.position;
            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -1f, 0);

            float jumpHeight = 5f; // 점프 높이
            float jumpDuration = 1f; // 점프 지속 시간
            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                float progress = elapsedTime / jumpDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

                float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

                agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

                elapsedTime += Time.deltaTime;

                boseDash.enabled = true;
                boseDash.damage = 15;

                if (elapsedTime >= jumpDuration)
                {
                    agent.transform.position = targetPosition;
                    break;
                }

                yield return null;
            }

            agent.transform.position = targetPosition;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);
            EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);
            boseDash.enabled = false;

            //yield return new WaitForSeconds(1f);
        }

        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator DDash(System.Action onComplete)
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(2f);

            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -2, 0);
            float elapsedTime = 0f;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, agent.transform);


            while (elapsedTime < 3)
            {
                boseDash.enabled = true;
                boseDash.damage = 15;
                agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 3);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            boseDash.enabled = false;
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(2f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator Invincibility(System.Action onComplete)
    {
        rHP.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Shield, animator.gameObject.transform);

        yield return new WaitForSeconds(10f);
        rHP.enabled = true;

        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    public override NodeState Stop() {
        spawnCoroutine = null;
        hasFinishedPattern = false;
        isFirstExecution = false;
        return NodeState.Failure; 
    }
}

public class BPazeThree : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그
    BossHP rHP;
    BossDash boseDash;

    private float distanceToCenter;
    private Transform railPivot;

    FloatWrapper hp;
    bool isAttacking;
    PlayerStateMachine p;
    bool isGot;
    bool once = false;

    public BPazeThree(NavMeshAgent agent, Transform player, Animator animator, Transform Pivot, FloatWrapper HP)
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        railPivot = Pivot;
        rHP = agent.gameObject.GetComponent<BossHP>();
        boseDash = agent.gameObject.GetComponentInChildren<BossDash>();
        hp = HP;
        p = GameObject.Find("Player").gameObject.GetComponent<PlayerStateMachine>();
    }

    public override NodeState Execute()
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            pattern = Random.Range(0, 4);
        }

        if (!hasFinishedPattern)
        {
            if (distanceToCenter <= 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern4();
                        break;
                    case 1:
                        PerformPattern2();
                        break;
                    case 2:
                        PerformPattern1();
                        break;
                    case 3:
                        PerformPattern3();
                        break;
                }
            }
            else
            {
                switch (pattern)
                {
                    case 0:
                        PerformPattern0();
                        break;
                    case 1:
                        PerformPattern1();
                        break;
                    case 2:
                        PerformPattern2();
                        break;
                    case 3:
                        PerformPattern3();
                        break;
                }
            }
        }

        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter > 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                return NodeState.Failure;
            }
        }

        return NodeState.Failure; 
    }

    private void PerformPattern0()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|RailGun");



        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(RailGun(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern1()
    {
        Debug.Log("패턴 1 실행");
        timer += Time.deltaTime;
        //EffectManager.Instance.ExecutionEffect("Shield Effect", agent.transform, 10f);


        if (isAttacking)
        {
            if (isGot && !once)
            {
                p.moveSpeed = 2.5f;
                p.jumpForce = 1;
                p.VPSpeed = 4;
                p.VPJumeForce = 1;
                once = true;
            }
            animator.Play("Armature|Shield");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            StartCoroutine(ExecuteEffectCoroutine());
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Invincibility(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern2()
    {
        Debug.Log("패턴 2 실행");
        timer += Time.deltaTime;
        if(isAttacking)
        {
            animator.Play("Armature|Dash");

        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(DDash(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 3 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Disarm");

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Reflection(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern4()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        if(isAttacking)
        {
            animator.Play("Armature|slam");
        }

        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(JJump(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private IEnumerator ExecuteEffectCoroutine()
    {
        float duration = 1f; // 총 실행 시간
        float interval = 2f; // 간격
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            EffectManager.Instance.ExecutionEffect(Effect.BossScanner, agent.transform);
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }

    private IEnumerator RailGun(System.Action onComplete)   
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 40;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);
            Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);


                Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            }
            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet4");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet1.SetUp(railPivot.transform, player.transform);
                bulletsSpawned++;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Railgun, animator.gameObject.transform);

        }
        yield return new WaitForSeconds(2f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator Invincibility(System.Action onComplete)
    {
        //rHP.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Shield, animator.gameObject.transform);
        isAttacking = true;
        float startTime = Time.time; // 시작 시간 기록

        while (Time.time - startTime < 1f)
        {
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter < 10)
            {
                isGot = true;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        //rHP.enabled = true;
        if (once)
        {
            StartCoroutine(something());
        }
        isAttacking = false;
        isGot = false;
        once = false;
        yield return new WaitForSeconds(1.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator something()
    {
        yield return new WaitForSeconds(10f);
        //p.moveSpeed *= 2;
        //p.jumpForce *= 2;
        //p.VPSpeed *= 2;
        //p.VPJumeForce *= 2;
        p.moveSpeed = 5;
        p.jumpForce = 2;
        p.VPSpeed = 8;
        p.VPJumeForce = 2.35f;
        yield return null;
    }

    private IEnumerator DDash(System.Action onComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 startPosition = agent.transform.position;
            Vector3 initialTargetPosition = player.transform.position + new Vector3(0, -2, 0);
            float elapsedTime = 0f;

            // 2초 동안 플레이어의 위치를 추적
            float trackingDuration = 2f;
            float trackingElapsedTime = 0f;
            Vector3 targetPosition = initialTargetPosition;

            while (trackingElapsedTime < trackingDuration)
            {
                // 2초 동안 지속적으로 플레이어 위치 업데이트
                targetPosition = player.transform.position + new Vector3(0, -2, 0);

                // 플레이어 방향으로 회전
                Vector3 directionToPlayer = (targetPosition - agent.transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                    agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                }

                trackingElapsedTime += Time.deltaTime;
                yield return null;
            }
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, agent.transform);
            Quaternion rotationToPlayer = Quaternion.LookRotation((targetPosition - startPosition).normalized);
            rotationToPlayer.eulerAngles = new Vector3(0, rotationToPlayer.eulerAngles.y, rotationToPlayer.eulerAngles.z);

            EffectManager.Instance.ExecutionEffect(Effect.BossDash, agent.transform.position + agent.transform.forward * 5f, rotationToPlayer);




            while (elapsedTime < 0.5f)
            {
                isAttacking = true;
                boseDash.enabled = true;
                boseDash.damage = 15;
                agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            boseDash.enabled = false;
            isAttacking = false;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isAttacking = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// 플레이어의 도움이 필요함
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator Reflection(System.Action onComplete)
    {
        rHP.enabled = false;
        GameObject shieldObject = EffectManager.Instance.ExecutionEffectObject(Effect.Shield, agent.transform);
        yield return new WaitForSeconds(6f);
        rHP.enabled = true;
        GameObject.Destroy(shieldObject);

        yield return new WaitForSeconds(0.5f);
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        onComplete?.Invoke();
    }

    private IEnumerator JJump(System.Action onComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -1f, 0);
            float jumpHeight = 5f; // 점프 높이
            float jumpDuration = 0.6f; // 점프 지속 시간
            float elapsedTime = 0f;

            // 3초 동안 플레이어의 위치를 추적
            float trackingDuration = 3f;
            float trackingElapsedTime = 0f;
            Vector3 a = Vector3.zero;
            while (trackingElapsedTime < trackingDuration)
            {
                Vector3 playerPosition = player.transform.position;
                targetPosition = playerPosition + new Vector3(0, -1f, 0);
                a = playerPosition;
                a.y = -32.7f;

                Vector3 directionToPlayer = (player.position - agent.transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    Vector3 rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed).eulerAngles;
                    agent.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                }

                trackingElapsedTime += Time.deltaTime;
                yield return null;
            }

            // 3초 후 갱신된 'a' 위치에 효과 생성
            EffectManager.Instance.ExecutionEffect(Effect.BossCutDownDecal, a, Quaternion.identity);




            while (elapsedTime < jumpDuration)
            {
                isAttacking = true;
                float progress = elapsedTime / jumpDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

                float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

                agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

                elapsedTime += Time.deltaTime;

                boseDash.enabled = true;
                boseDash.damage = 20;

                if (elapsedTime >= jumpDuration)
                {
                    agent.transform.position = targetPosition;
                    break;
                }

                yield return null;
            }

            agent.transform.position = targetPosition;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);

            EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);

            boseDash.enabled = false;

        }
        yield return new WaitForSeconds(0.5f);

        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isAttacking = false;
        onComplete?.Invoke();
    }

    public override NodeState Stop() {
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        hasFinishedPattern = false;
        isFirstExecution = false;
        return NodeState.Failure;
    }
}

public class BPazeThreeClose : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;
    private int pattern;
    private Coroutine spawnCoroutine;
    private float timer;

    private bool hasFinishedPattern = false; // 패턴 완료 여부 플래그
    REnemyHP rHP;
    BossDash boseDash;

    private float distanceToCenter;

    FloatWrapper hp;


    public BPazeThreeClose(NavMeshAgent agent, Transform player, Animator animator, FloatWrapper HP) 
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        rHP = agent.gameObject.GetComponent<REnemyHP>();
        boseDash = agent.gameObject.GetComponentInChildren<BossDash>();
        hp = HP;
    }
    public override NodeState Execute() 
    {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            pattern = Random.Range(0, 4);
        }

        if (!hasFinishedPattern)
        {
            switch (pattern)
            {
                case 0:
                    PerformPattern4();
                    break;
                case 1:
                    PerformPattern1();
                    break;
                case 2:
                    PerformPattern2();
                    break;
                case 3:
                    PerformPattern3();
                    break;
            }
        }


        if (hasFinishedPattern)
        {
            Debug.Log("패턴 종료");
            distanceToCenter = Vector3.Distance(player.position, agent.transform.position);

            if (distanceToCenter > 20) //여기가 거리이니까 나중에 수치를 빼긴해야해 
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                //pazeoneclose를 실행해야함
                return NodeState.Running;
            }
            else
            {
                isFirstExecution = false;
                hasFinishedPattern = false;
                return NodeState.Failure;
            }
        }

       

        return NodeState.Failure;
    }

    private void PerformPattern4()
    {
        Debug.Log("패턴 0 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|slam");


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(JJump(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern1()
    {
        Debug.Log("패턴 1 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Dash");


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(DDash(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    
    }

    private void PerformPattern2()
    {
        Debug.Log("패턴 2 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Shield");
        //EffectManager.Instance.ExecutionEffect("Shield Effect", agent.transform, 10f);


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Invincibility(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private void PerformPattern3()
    {
        Debug.Log("패턴 3 실행");
        timer += Time.deltaTime;
        animator.Play("Armature|Disarm");


        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(Reflection(() =>
            {
                hasFinishedPattern = true;
                spawnCoroutine = null;
            }));

            timer = 0f;
        }
    }

    private IEnumerator JJump(System.Action onComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(3f);

            Vector3 playerPosition = player.transform.position;
            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -1f, 0);
            Vector3 a = player.transform.position + new Vector3(0, -1.4f, 0);
            EffectManager.Instance.ExecutionEffect(Effect.BossCutDownDecal, a, Quaternion.identity);


            float jumpHeight = 5f; // 점프 높이
            float jumpDuration = 1f; // 점프 지속 시간
            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                float progress = elapsedTime / jumpDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

                float verticalOffset = Mathf.Sin(Mathf.PI * smoothProgress) * jumpHeight; // 중간에 최고점

                agent.transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset, horizontalPosition.z);

                elapsedTime += Time.deltaTime;

                boseDash.enabled = true;
                boseDash.damage = 20;

                if (elapsedTime >= jumpDuration)
                {
                    agent.transform.position = targetPosition;
                    break;
                }

                yield return null;
            }

            agent.transform.position = targetPosition;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Slam, animator.gameObject.transform);

            EffectManager.Instance.ExecutionEffect("Boss Slash Effect", agent.transform, 1f);

            boseDash.enabled = false;

            //yield return new WaitForSeconds(1f);
        }

        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator DDash(System.Action onComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2f);

            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -2, 0);
            float elapsedTime = 0f;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Dash, animator.gameObject.transform);


            while (elapsedTime < 3)
            {
                boseDash.enabled = true;
                boseDash.damage = 20;
                agent.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 3);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            boseDash.enabled = false;
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(2f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator Invincibility(System.Action onComplete)
    {
        rHP.enabled = false;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Shield, animator.gameObject.transform);

        yield return new WaitForSeconds(10f);
        rHP.enabled = true;

        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    /// <summary>
    /// 플레이어의 도움이 필요함
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator Reflection(System.Action onComplete)
    {
        rHP.enabled = false;
        yield return new WaitForSeconds(6f);
        rHP.enabled = true;

        yield return new WaitForSeconds(1f);
        spawnCoroutine = null;
        onComplete?.Invoke();
    }

    public override NodeState Stop()
    {
        spawnCoroutine = null;
        hasFinishedPattern = false;
        isFirstExecution = false;
        return NodeState.Failure;
    }

}


public class BDistance : BossAction
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isFirstExecution;

    private float distanceToCenter;

    public BDistance(NavMeshAgent agent, Transform player, Animator animator) 
    {
        this.player = player;
        this.agent = agent;
        this.animator = animator;
    }
    public override NodeState Execute()
    {
        distanceToCenter = Vector3.Distance(player.position, agent.transform.position);
        if (!isFirstExecution)
        {
            isFirstExecution = true;

        }

        if (distanceToCenter <= 10) //여기가 거리이니까 나중에 수치를 빼긴해야해 
        {
            isFirstExecution = false;
            //pazeoneclose를 실행해야함
        }
        else
        {
            isFirstExecution = false;
            //pazeone을 실행해야함
            

        }
        return NodeState.Failure;
    }

    public override NodeState Stop() { return NodeState.Failure; }
}

public class BDie : BossAction
{
    private Animator animator;
    private bool isFirstExecution;
    GameObject cutScene;

    public BDie(Animator animator, GameObject obj)
    {
        this.animator = animator;
        cutScene = obj;
    }
    public override NodeState Execute() {
        if (!isFirstExecution)
        {
            isFirstExecution = true;
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Death, animator.gameObject.transform);
            cutScene.SetActive(true);
        }

        animator.Play("Armature|Death");
        return NodeState.Success; }

    public override NodeState Stop() { return NodeState.Failure; }
}
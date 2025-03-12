using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BTNode;
using static ChaseAction;

public abstract class BaseEnemy : MonoBehaviour, IListener
{
    protected BTNode baseBehaviorTree;
    protected Dictionary<EnemyActionType, EnemyAction> actions;

    public Transform player;
    public Transform npceye;
    public Transform[] upperBody;
    public Transform[] child;

    private Vector3 detectionCenter; 

    public float detectionRadius;

    public float playerRadius;

    public float detectionAngle;

    public float detectionSound;

    private BoolWrapper something = new BoolWrapper(false);
    private BoolWrapper npchitted = new BoolWrapper(false);
    public NavMeshAgent agent;
    protected Animator animator;

    public FloatWrapper HP = new FloatWrapper(50f);
    private FloatWrapper lastHP = new FloatWrapper(50f); // 이전 HP 값을 저장할 변수
    public int enemyType = 0;
    private bool isStop;
    private Coroutine resetHittedCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public bool tutoweap;
    public enum EnemyActionType
    {
        IdleAction,
        IdleSAction,
        PatrolAction,
        ChaseAction,
        HurtAction,
        DieAction
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Head").transform;
        }
        
        detectionCenter = npceye.transform.position;

        DeactivateRagdoll();
        InitializeActions();
        InitializeBehaviorTree();
        EventManager.Instance.AddEvent(EventType.playerShot, OnEvent);
        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        lastHP.Value = HP.Value;
        //EventManager.Instance.AddEvent(EventType.NPCHit, OnEvent);
    }

    // Update is called once per frame
    protected void Update()
    {
        if(isStop)
        {
            actions[EnemyActionType.PatrolAction].Stop();
            actions[EnemyActionType.ChaseAction].Stop();
            return;
        }

        if (HP.Value <= 0)
        {
            actions[EnemyActionType.PatrolAction].Stop();
            actions[EnemyActionType.ChaseAction].Stop();
            actions[EnemyActionType.DieAction].Execute();
            ActivateRagdoll();
        }
        else if (HP.Value != lastHP.Value)
        {

            if (resetHittedCoroutine == null) // 코루틴이 이미 실행 중이 아니라면 실행
            {
                resetHittedCoroutine = StartCoroutine(ResetNpcHitted());
            }
            actions[EnemyActionType.PatrolAction].Stop();
            actions[EnemyActionType.ChaseAction].Stop();
            actions[EnemyActionType.HurtAction].Execute();
            //lastHP.Value = HP.Value;

            // 새로운 코루틴 시작

        }
        else
        {
            actions[EnemyActionType.HurtAction].Stop();
            NodeState result = baseBehaviorTree.Execute();
        }
        //NodeState result1 = baseBehaviorTree.Execute();
        //Debug.Log($"{GetType().Name} Behavior Tree Result: " + result1);
    }

    protected abstract void InitializeBehaviorTree();

    protected virtual void InitializeActions()
    {
        actions = new Dictionary<EnemyActionType, EnemyAction>
    {
        { EnemyActionType.IdleAction, new IdleSightAction(agent, player, npceye, detectionRadius, detectionAngle, animator) },
        { EnemyActionType.IdleSAction, new IdleSoundAction(player, npceye, detectionRadius, something) },
        { EnemyActionType.PatrolAction, new PatrolAction(player, npceye, agent, detectionRadius,playerRadius, child[0].transform, enemyType, animator) },
        { EnemyActionType.ChaseAction, new ChaseAction(player, agent, child[0].transform, npceye, detectionRadius, detectionAngle, enemyType, animator) },
        { EnemyActionType.HurtAction, new HurtAction(agent, HP, npchitted, animator) },
        { EnemyActionType.DieAction, new DieAction(agent, HP, enemyType, animator,tutoweap) }
    };
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.playerShot:
                {
                    if (param is bool shot)
                    {
                        something.Value = shot;
                    }
                }
                break;
            case EventType.isPause:
                isStop = (bool)param;
                break;
            //case EventType.NPCHit:
            //    {
            //        npchitted = (bool)param;
            //    }
            //    break;
            //case EventType.playerShot:
            //    {
            //        actions["HurtAction"].Execute();
            //    }
            //    break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("ThrowWeapon"))
        {
            npchitted.Value = true;
        }
    }

    private IEnumerator ResetNpcHitted()
    {
        yield return new WaitForSeconds(0.5f);

        // HP를 갱신
        lastHP.Value = HP.Value;

        // 코루틴이 끝났음을 표시
        resetHittedCoroutine = null;
    }

    /// <summary>
    /// TO DO 이따가 정리 
    /// </summary>
    public void ActivateRagdoll()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; // 애니메이터 비활성화
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // 레그돌 활성화
        }
        agent.enabled = false;
    }

    public void DeactivateRagdoll()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true; // 애니메이터 활성화
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true; // 레그돌 비활성화
        }
    }
}

public class BoolWrapper
{
    public bool Value;

    public BoolWrapper(bool value)
    {
        Value = value;
    }
}

[Serializable]
public class FloatWrapper
{
    public float Value;

    public FloatWrapper(float value)
    {
        Value = value;
    }
}
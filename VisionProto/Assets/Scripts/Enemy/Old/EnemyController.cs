using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �������� NPC ��Ʈ�ѷ��� Ŭ�������� ������
/// �װ� �ƿ� �����ϰ� �ٽ� ����Ŵ�.
/// 
/// �迹���� �ۼ�
/// </summary>

public enum EnemyState
{
    Idle,
    Chase, //���⿡ ������ ����
    Attack,
    WalkBack,
    Death //��� �߰��ɰ��� 
}
public class EnemyController : MonoBehaviour
{
    private EnemyHP enemyHP;
    private EnemyStateMachine<EnemyState> stateMachine;
    private TestBehavior testBehavior;
    [SerializeField] private PlayerDetector playerDetector; // ����ȭ�� �ʵ�� ����
    public float attackDistance;

    /// <summary>
    /// �÷��̾��� ��ġ�� ���� �޶����� �ϱ� ���ؼ�..��..
    /// </summary>
    public bool isFarSec;
    public bool isFarThr;

    /// <summary>
    /// debug
    /// </summary>
    public bool istestDetected;

    private void Awake()
    {
        stateMachine = new EnemyStateMachine<EnemyState>();
        enemyHP = GetComponent<EnemyHP>();
        stateMachine.Initialize(EnemyState.Idle);
        testBehavior = GetComponent<TestBehavior>();
        //attackDistance = 30;
    }

    // Start is called before the first frame update
    void Start()
    {
        float delay = Random.Range(0, 2);
        //Debug.Log(delay);
        stateMachine.AddState(EnemyState.Idle, () =>
        {
            //Debug.Log("Idle ����");
            
            Invoke("AttackMode", delay);
            if (enemyHP.HP <= 0)
            {
                stateMachine.ChangeState(EnemyState.Death);
                testBehavior.m_Animator.SetTrigger("Death");
            }

        } );

        stateMachine.AddState(EnemyState.Chase, () =>
        {
            //Debug.Log("Chase ����");

            if (enemyHP.HP <= 0)
            {
                testBehavior.m_Animator.SetBool("Attack", false);
                testBehavior.m_Animator.SetTrigger("Death");
                stateMachine.ChangeState(EnemyState.Death);
            }

            if (testBehavior.isAttackable)
            {
                stateMachine.ChangeState(EnemyState.Attack);
                testBehavior.m_Animator.SetBool("Chase", false);
                testBehavior.m_Animator.SetBool("Attack", true);
            }

        });
        stateMachine.AddState(EnemyState.Attack, () =>
        {
            //Debug.Log("Attack ����");

            if (enemyHP.HP <= 0)
            {
                testBehavior.m_Animator.SetBool("Attack", false);
                testBehavior.m_Animator.SetTrigger("Death");
                stateMachine.ChangeState(EnemyState.Death);
            }

            if (testBehavior.wannaAttack)
            {
                if (!testBehavior.isAttackable)
                {
                    stateMachine.ChangeState(EnemyState.Chase);
                    testBehavior.m_Animator.SetBool("Chase", true);
                    testBehavior.m_Animator.SetBool("Attack", false);
                }
            }
            else if(!testBehavior.wannaAttack && testBehavior.isCanRun) 
            {
                stateMachine.ChangeState(EnemyState.WalkBack);
                testBehavior.m_Animator.SetBool("Back", true);
                testBehavior.m_Animator.SetBool("Attack", false);
            }
            

        });
        stateMachine.AddState(EnemyState.WalkBack, () =>
        {
            if (testBehavior.movingDone)
            {
                stateMachine.ChangeState(EnemyState.Attack);
                testBehavior.m_Animator.SetBool("Back", false);
                testBehavior.m_Animator.SetBool("Attack", true);
                testBehavior.movingDone = false;
                //Invoke("DelayAttack", 0.5f);
            }
            //Debug.Log("WalkBack ����");
        });
        stateMachine.AddState(EnemyState.Death, () =>
        {
            //Debug.Log("Death ����");
        });

        
    }

    private void DelayAttack()
    {
        stateMachine.ChangeState(EnemyState.Attack);
        testBehavior.m_Animator.SetBool("Back", false);
        testBehavior.m_Animator.SetBool("Attack", true);
    }

    private void Update()
    {
        if(testBehavior.isDetectable)
        {

        stateMachine.UpdateCurrentState();
        }

        Vector3 rayDirection = -transform.forward;

        RaycastHit hit;

        AttackScopeDetermine();
        PlayerHP();

        if (Physics.Raycast(testBehavior.eye.transform.position, rayDirection, out hit, 10))
        {
            //Debug.Log($"{hit.collider.tag}");

            if (hit.collider.tag == "Wall")
            {
                //Debug.Log("���� ����");
                testBehavior.behindWall = true;
            }
        }
        else
        {
            testBehavior.behindWall = false;
            //Debug.Log("�����Ȱ� ����");
        }

        istestDetected = playerDetector.detectedPlayer;
        EventManager.Instance.NotifyEvent(EventType.detected, istestDetected);
    }

    void PlayerHP()
    {
        float distanceToCenter = Vector3.Distance(testBehavior.player.position, transform.position);

        if (distanceToCenter <= 10)
        {
            isFarSec = true;
            isFarThr = false;
        }
        else if(distanceToCenter>10 && distanceToCenter <= 50)
        {
            isFarThr = true;
            isFarThr = false;
        }
        else
        {
            isFarThr = false;
            isFarSec = false;
        }

    }

    void AttackScopeDetermine()
    {
        float distanceToCenter = Vector3.Distance(testBehavior.player.position, transform.position);

        if (distanceToCenter <= attackDistance)
        {
            testBehavior.isAttackable = true;
            Debug.Log("���ݹ��� ��");
        }
        else
        {
            testBehavior.isAttackable = false;
        }

        if (distanceToCenter <= 25)
        {
            testBehavior.isCanRun = true;
        }
        else
        {
            testBehavior.isCanRun = false;
        }
    }

    void AttackMode()
    {
        if (playerDetector.detectedPlayer)
        {
            if (testBehavior.wannaAttack)
            {
                if (testBehavior.isAttackable)
                {
                    stateMachine.ChangeState(EnemyState.Attack);
                    testBehavior.m_Animator.SetBool("Attack", true);
                }
                else if (!testBehavior.isAttackable)
                {
                    stateMachine.ChangeState(EnemyState.Chase);
                    testBehavior.m_Animator.SetBool("Chase", true);
                }
            }
            else if (!testBehavior.wannaAttack)
            {
                stateMachine.ChangeState(EnemyState.Attack);
                testBehavior.m_Animator.SetBool("Attack", true);
            }


        }
    }

   

}

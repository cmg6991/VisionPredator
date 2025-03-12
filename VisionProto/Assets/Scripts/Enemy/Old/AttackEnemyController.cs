//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ������ �ڵ��.
///// 
///// �迹���� �ۼ�
///// </summary>

//public enum AttackNPCState
//{
//    Idle,
//    Move,
//    Chase,
//    Attack,
//    Death
//}
//public class AttackEnemyController : MonoBehaviour
//{
//    private EnemyStateMachine<AttackNPCState> stateMachine;
//    private TestBehavior testBehavior;
//    private EnemyHP enemyHP;
//    public bool isDetectPlayer = false;
//    private Coroutine moveCoroutine;


//    public Vector3 center;
//    //Do not change value
//    public float radius => 30; //����
//    public float viewAngle => 20; //����

//    public float halfViewAngle;

//    public GameObject eyeOne;

//    public bool isdetectX = false;
//    public bool isdetectY = false;
//    private Coroutine test = null;

//    // Start is called before the first frame update
//    void Start()
//    {
//         stateMachine = new EnemyStateMachine<AttackNPCState>();
//        enemyHP = GetComponent<EnemyHP>(); 

//        stateMachine.Initialize(AttackNPCState.Idle);

//        stateMachine.AddState(AttackNPCState.Idle, () =>
//        {
//            Debug.Log("Idle ����");
//            if(enemyHP.HP <=0)
//            {
//                stateMachine.ChangeState(AttackNPCState.Death);
//                testBehavior.m_Animator.SetTrigger("Death");
//            }

//            if (isDetectPlayer)
//            {
//                stateMachine.ChangeState(AttackNPCState.Chase);
//                testBehavior.m_Animator.SetBool("Chase", true);
//            }
//            else
//            {
//                if(test == null)
//                {

//                test = StartCoroutine(ChangeMove());
//                }
//            }    
//        });

//        stateMachine.AddState(AttackNPCState.Move, () =>
//        {
//            Debug.Log("Move ����.");
//            if (enemyHP.HP <= 0)
//            {
//                stateMachine.ChangeState(AttackNPCState.Death);
//                testBehavior.m_Animator.SetBool("Move", false);
//                testBehavior.m_Animator.SetTrigger("Death");
//            }

//            if (!isDetectPlayer && testBehavior.isMoveDone)
//            {
//                test = null;
//                testBehavior.isMoveDone = false;
//                stateMachine.ChangeState(AttackNPCState.Idle);
//                testBehavior.m_Animator.SetBool("Move", false);
//            }
//            else if(isDetectPlayer)
//            {
//                testBehavior.isMoveDone = false;
//                test = null;
//                stateMachine.ChangeState(AttackNPCState.Chase);
//                testBehavior.m_Animator.SetBool("Move", false);
//                testBehavior.m_Animator.SetBool("Chase", true);
//            }
//        });

//        stateMachine.AddState(AttackNPCState.Chase, () =>
//        {
//            Debug.Log("Chase ����.");

//            if (enemyHP.HP <= 0)
//            {
//                stateMachine.ChangeState(AttackNPCState.Death);
//                testBehavior.m_Animator.SetBool("Chase", false);
//                testBehavior.m_Animator.SetTrigger("Death");
//            }

//            float distanceToCenter = Vector3.Distance(testBehavior.player.position, center);
//            if (distanceToCenter <= 50)
//            {
//                stateMachine.ChangeState(AttackNPCState.Attack);
//                testBehavior.m_Animator.SetBool("Chase", false);
//                testBehavior.m_Animator.SetBool("Attack", true);
//            }
//            else if(distanceToCenter > 100 )
//            {
//                testBehavior.isMoveDone = false;
//                isDetectPlayer = false;
//                stateMachine.ChangeState(AttackNPCState.Idle);
//                testBehavior.m_Animator.SetBool("Chase", false);
//                testBehavior.m_Animator.SetBool("Move", false);
//            }
//        });

//        stateMachine.AddState(AttackNPCState.Attack, () =>
//        {

//            Debug.Log("Attack ����.");

//            if (enemyHP.HP <= 0)
//            {
//                stateMachine.ChangeState(AttackNPCState.Death);
//                testBehavior.m_Animator.SetBool("Attack", false);
//                testBehavior.m_Animator.SetTrigger("Death");
//            }

//            float distanceToCenter = Vector3.Distance(testBehavior.player.position, center);

//            if (distanceToCenter > 50)
//            {
//                stateMachine.ChangeState(AttackNPCState.Chase);
//                testBehavior.m_Animator.SetBool("Attack", false);
//                testBehavior.m_Animator.SetBool("Chase", true);
//            }

//        });

//        stateMachine.AddState(AttackNPCState.Death, () =>
//        {
//            Debug.Log("Death ����.");
//        });

//        testBehavior = GetComponent<TestBehavior>();

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        center = eyeOne.transform.position;
//        halfViewAngle = viewAngle / 2f; //����³�? ���� ��ŸƮ������
//        stateMachine.UpdateCurrentState();
//        DetectedPlayer();

//    }

//    public void DetectedPlayer()
//    {
//        float distanceToCenter = Vector3.Distance(testBehavior.player.position, center);

//        if (distanceToCenter <= radius)
//        {
//            //Debug.Log("�ϴ� �� �ȿ� ���Դ�. ���� ������� �ȴ�.");



//            ///����
//            Vector3 toPlayerFlat = testBehavior.player.position - center;
//            toPlayerFlat.y = 0; //y ���� ������� �ʰڴ�.

//            float angleToPlayerX = Vector3.Angle(toPlayerFlat, Vector3.forward) * Mathf.Deg2Rad;

//            float halfDetectionAngleRadX = 30f * Mathf.Deg2Rad / 2f;

//            if (angleToPlayerX <= halfDetectionAngleRadX)
//            {
//                //Debug.Log("���� �ɷ��� x �þ߹��� �ɸ�");
//                isdetectX = true;
//            }
//            else
//            {
//                isdetectX = false;

//            }

//            Vector3 toPlayerFlatY = testBehavior.player.position - center;
//            toPlayerFlatY.x = 0;

//            ///����
//            //y���� ������ �ٲپ� �ξ �̺κ��� �ʿ��ߴ�. 
//            Quaternion yRotationOffset = Quaternion.Euler(-5f, 0f, 0f); // y�� �þ� ������ ���� ������ (5��)
//            Vector3 rotatedToPlayerFlatY = yRotationOffset * toPlayerFlatY; // ȸ���� y�� �þ� ����
//            float angleToPlayerY = Vector3.Angle(rotatedToPlayerFlatY, Vector3.forward) * Mathf.Deg2Rad;
//            //float angleToPlayerY = Vector3.Angle(toPlayerFlatY, Vector3.forward) * Mathf.Deg2Rad;
//            float halfDetectionAngleRadY = 20f * Mathf.Deg2Rad / 2f;

//            if (angleToPlayerY <= halfDetectionAngleRadY)
//            {
//                //Debug.Log("y�� �þ� ���� ���� �ֽ��ϴ�.");
//                isdetectY = true;
//            }
//            else
//            {
//                isdetectY = false;

//            }

//            if (isdetectX && isdetectY)
//            {
//                isDetectPlayer = true;
//                Debug.Log("�ɸ�����");
//            }
//            else
//            {
//                isdetectX = false;
//                isdetectY = false;
//                isDetectPlayer = false;
//            }
//        }
//    }

//    private IEnumerator ChangeMove()
//    {
//        yield return new WaitForSeconds(3f);
//        stateMachine.ChangeState(AttackNPCState.Move);
//        testBehavior.m_Animator.SetBool("Move", true);
//    }
//}

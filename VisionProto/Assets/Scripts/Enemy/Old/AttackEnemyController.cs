//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 끔찍한 코드다.
///// 
///// 김예리나 작성
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
//    public float radius => 30; //범위
//    public float viewAngle => 20; //각도

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
//            Debug.Log("Idle 상태");
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
//            Debug.Log("Move 상태.");
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
//            Debug.Log("Chase 상태.");

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

//            Debug.Log("Attack 상태.");

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
//            Debug.Log("Death 상태.");
//        });

//        testBehavior = GetComponent<TestBehavior>();

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        center = eyeOne.transform.position;
//        halfViewAngle = viewAngle / 2f; //여기맞나? 원래 스타트였긴함
//        stateMachine.UpdateCurrentState();
//        DetectedPlayer();

//    }

//    public void DetectedPlayer()
//    {
//        float distanceToCenter = Vector3.Distance(testBehavior.player.position, center);

//        if (distanceToCenter <= radius)
//        {
//            //Debug.Log("일단 원 안에 들어왔다. 좋아 여기까진 된다.");



//            ///길이
//            Vector3 toPlayerFlat = testBehavior.player.position - center;
//            toPlayerFlat.y = 0; //y 축을 고려하지 않겠다.

//            float angleToPlayerX = Vector3.Angle(toPlayerFlat, Vector3.forward) * Mathf.Deg2Rad;

//            float halfDetectionAngleRadX = 30f * Mathf.Deg2Rad / 2f;

//            if (angleToPlayerX <= halfDetectionAngleRadX)
//            {
//                //Debug.Log("제발 걸려라 x 시야범위 걸림");
//                isdetectX = true;
//            }
//            else
//            {
//                isdetectX = false;

//            }

//            Vector3 toPlayerFlatY = testBehavior.player.position - center;
//            toPlayerFlatY.x = 0;

//            ///높이
//            //y축의 각도를 바꾸어 두어서 이부분이 필요했다. 
//            Quaternion yRotationOffset = Quaternion.Euler(-5f, 0f, 0f); // y축 시야 벡터의 각도 오프셋 (5도)
//            Vector3 rotatedToPlayerFlatY = yRotationOffset * toPlayerFlatY; // 회전된 y축 시야 벡터
//            float angleToPlayerY = Vector3.Angle(rotatedToPlayerFlatY, Vector3.forward) * Mathf.Deg2Rad;
//            //float angleToPlayerY = Vector3.Angle(toPlayerFlatY, Vector3.forward) * Mathf.Deg2Rad;
//            float halfDetectionAngleRadY = 20f * Mathf.Deg2Rad / 2f;

//            if (angleToPlayerY <= halfDetectionAngleRadY)
//            {
//                //Debug.Log("y축 시야 범위 내에 있습니다.");
//                isdetectY = true;
//            }
//            else
//            {
//                isdetectY = false;

//            }

//            if (isdetectX && isdetectY)
//            {
//                isDetectPlayer = true;
//                Debug.Log("걸리는중");
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

using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Death 상태의 FSM 상태
/// 
/// 김예리나 작성
/// </summary>
public class DeathFSM : SceneLinkedSMB<TestBehavior>
{
    /// <summary>
    /// 처음 들어올 때 한번만 실행된다.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("사망");
        m_MonoBehaviour.StopAgent();
        m_MonoBehaviour.m_Animator.enabled = false;
        m_MonoBehaviour.agent.enabled = false;
        m_MonoBehaviour.enemyHP.Died();
        //m_MonoBehaviour.Invoke("DeathDestroy", 0.5f);
        //m_MonoBehaviour.StartCoroutine(InvokeDeathDestroy()); //Invoke로 바꾸고 싶은데 오류가 난다.
    }

}

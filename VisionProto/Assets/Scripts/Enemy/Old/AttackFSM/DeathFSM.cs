using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Death ������ FSM ����
/// 
/// �迹���� �ۼ�
/// </summary>
public class DeathFSM : SceneLinkedSMB<TestBehavior>
{
    /// <summary>
    /// ó�� ���� �� �ѹ��� ����ȴ�.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("���");
        m_MonoBehaviour.StopAgent();
        m_MonoBehaviour.m_Animator.enabled = false;
        m_MonoBehaviour.agent.enabled = false;
        m_MonoBehaviour.enemyHP.Died();
        //m_MonoBehaviour.Invoke("DeathDestroy", 0.5f);
        //m_MonoBehaviour.StartCoroutine(InvokeDeathDestroy()); //Invoke�� �ٲٰ� ������ ������ ����.
    }

}

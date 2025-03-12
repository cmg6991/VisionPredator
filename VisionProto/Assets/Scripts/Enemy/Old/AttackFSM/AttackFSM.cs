using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFSM : SceneLinkedSMB<TestBehavior>
{
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.Attack();
        m_MonoBehaviour.StopAgent();
        //m_MonoBehaviour.Chase();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.StopBullet();
        m_MonoBehaviour.reMoveAgent();
    }

}

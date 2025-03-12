using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseFSM:  SceneLinkedSMB<TestBehavior>
{
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.Chase();
        m_MonoBehaviour.Attack();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.StopBullet();
    }
}

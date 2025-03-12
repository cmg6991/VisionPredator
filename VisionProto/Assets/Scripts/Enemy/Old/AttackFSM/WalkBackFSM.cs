using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class WalkBackFSM : SceneLinkedSMB<TestBehavior>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.agent.enabled = false;
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.Run();
        m_MonoBehaviour.Attack();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.agent.enabled = true;
    }
}

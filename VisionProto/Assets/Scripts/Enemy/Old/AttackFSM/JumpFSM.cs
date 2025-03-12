using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFSM : SceneLinkedSMB<TestBehavior>
{
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.Chase();
    }
}

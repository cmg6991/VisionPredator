using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class HitFSM : SceneLinkedSMB<TestBehavior>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("아프다");
        m_MonoBehaviour.StopAgent();
        //m_MonoBehaviour.Invoke("DeathDestroy", 0.5f);
        //m_MonoBehaviour.StartCoroutine(InvokeDeathDestroy()); //Invoke로 바꾸고 싶은데 오류가 난다.
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.reMoveAgent();
    }
}

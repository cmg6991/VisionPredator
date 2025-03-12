using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    public Transform target; // 따라갈 대상
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트

    void Start()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // 목표 위치로 설정
            agent.SetDestination(target.position);
        }
    }

}

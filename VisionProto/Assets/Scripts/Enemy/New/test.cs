using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    public Transform target; // ���� ���
    private NavMeshAgent agent; // NavMeshAgent ������Ʈ

    void Start()
    {
        // NavMeshAgent ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // ��ǥ ��ġ�� ����
            agent.SetDestination(target.position);
        }
    }

}

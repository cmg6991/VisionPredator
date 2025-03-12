using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���ʸ����̾� 
/// 
/// ���� �þ߹����� �Ҹ�����, �߰ݹ���. 
/// </summary>
public class EnemySight : MonoBehaviour
{
    private BaseEnemy baseEnemy;
    public float radius;
    public float radiusnoise;
    public float viewAngle;
    Vector3 center;
    float halfViewAngle;

    private void Start()
    {
        baseEnemy = GetComponentInParent<BaseEnemy>();
    }

    ////�ϵ��ڵ��� �ٿ����ϴµ�..�Լ��� ���� �ǹ̰� �����
    private void OnDrawGizmos()
    {
        if (baseEnemy == null) return;

        radius = baseEnemy.detectionRadius;
        //center = baseEnemy.gameObject.transform.position;
        viewAngle = baseEnemy.detectionAngle;
        radiusnoise = baseEnemy.detectionSound;

        center = transform.position;
        halfViewAngle = viewAngle / 2f;

        Quaternion leftRotation = Quaternion.Euler(0, -halfViewAngle, 0f);
        Quaternion rightRotation = Quaternion.Euler(0, halfViewAngle, 0f);

        Quaternion leftRotation2 = Quaternion.Euler(-halfViewAngle + 5, 0, 0f);
        Quaternion rightRotation2 = Quaternion.Euler(halfViewAngle + 5, 0, 0f);

        Vector3 leftVector = leftRotation * transform.forward;
        Vector3 rightVector = rightRotation * transform.forward;

        Vector3 leftVector2 = leftRotation2 * transform.forward;
        Vector3 rightVector2 = rightRotation2 * transform.forward;

        //��ü ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, radiusnoise);

        //���μ�
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, leftVector * radius);
        Gizmos.DrawRay(center, rightVector * radius);
        //����
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, leftVector2 * radius);
        Gizmos.DrawRay(center, rightVector2 * radius);
    }
}

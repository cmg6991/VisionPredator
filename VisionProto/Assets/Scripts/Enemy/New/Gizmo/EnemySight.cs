using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 와 태초마을이야 
/// 
/// 적의 시야범위와 소리범위, 추격범위. 
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

    ////하드코딩을 줄여야하는데..함수로 빼도 의미가 없어보임
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

        //전체 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, radiusnoise);

        //가로선
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, leftVector * radius);
        Gizmos.DrawRay(center, rightVector * radius);
        //세로
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, leftVector2 * radius);
        Gizmos.DrawRay(center, rightVector2 * radius);
    }
}

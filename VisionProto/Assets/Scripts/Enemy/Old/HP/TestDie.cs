using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

/// <summary>
/// ���߿� ���� �� �� ���� �׳� ������ �ص�
/// 
/// �迹���� �ۼ�
/// </summary>
public class TestDie : MonoBehaviour
{
    /// <summary>
    /// ���߿� �����ؾ���
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("�浹");
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(20, transform.position, transform.position, this.gameObject);
        }
    }
}

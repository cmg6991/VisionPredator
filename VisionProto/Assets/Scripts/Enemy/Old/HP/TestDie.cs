using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

/// <summary>
/// 나중에 삭제 걍 적 어케 죽나 보려고 해둠
/// 
/// 김예리나 작성
/// </summary>
public class TestDie : MonoBehaviour
{
    /// <summary>
    /// 나중에 생각해야함
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("충돌");
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(20, transform.position, transform.position, this.gameObject);
        }
    }
}

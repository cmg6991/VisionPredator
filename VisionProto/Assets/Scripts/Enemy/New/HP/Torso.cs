using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torso : MonoBehaviour
{
    private void Start()
    {
        //��Ȱ��ȭ�� ���� ����;
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(40, transform.position, transform.position, this.gameObject);

        }
    }
}

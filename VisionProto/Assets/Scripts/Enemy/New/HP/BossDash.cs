using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDash : MonoBehaviour
{
    public int damage;
    private bool isInvulnerable = false;  // ���� ���� �÷���

    private void Start()
    {
        //Ȱ��ȭ�� ���� ����
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (isInvulnerable) return;  // ���� �����̸� ����

        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(damage, transform.position, transform.position, this.gameObject);
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1);
        isInvulnerable = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDash : MonoBehaviour
{
    public int damage;
    private bool isInvulnerable = false;  // 무적 상태 플래그

    private void Start()
    {
        //활성화를 위한 빈통
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (isInvulnerable) return;  // 무적 상태이면 무시

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DaggerCollider : MonoBehaviour, IListener
{
    public int headDamaged = 10;
    private int damaged;
    private BoxCollider boxCollider;
    public bool isVPState;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.DaggerInformation, OnEvent);
        boxCollider = GetComponent<BoxCollider>();

        // Collider 비활성화
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        // 베기랑 찌르기의 데미지가 다르다.
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }
            if (collision.gameObject.CompareTag("EHead"))
            {
                // 헤드 데미지
                damageable.Damaged(damaged + headDamaged, transform.position, transform.position, this.gameObject);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                // 일반 데미지
                damageable.Damaged(damaged, transform.position, transform.position, this.gameObject);
            }
        }
    }


    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType)
        {
            case EventType.DaggerInformation:
                {
                    // Collider 활성화와 Damage를 조절한다.
                    DaggerInformation daggerInformation = (DaggerInformation)param; 
                    damaged = daggerInformation.damaged;
                    boxCollider.enabled = daggerInformation.isEnableCollider;
                    boxCollider.isTrigger = daggerInformation.isTrueTrigger;
                }
                break;
        }
    }
}

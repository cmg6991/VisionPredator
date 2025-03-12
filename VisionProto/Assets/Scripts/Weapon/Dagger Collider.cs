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

        // Collider ��Ȱ��ȭ
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        // ����� ����� �������� �ٸ���.
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
                // ��� ������
                damageable.Damaged(damaged + headDamaged, transform.position, transform.position, this.gameObject);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                // �Ϲ� ������
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
                    // Collider Ȱ��ȭ�� Damage�� �����Ѵ�.
                    DaggerInformation daggerInformation = (DaggerInformation)param; 
                    damaged = daggerInformation.damaged;
                    boxCollider.enabled = daggerInformation.isEnableCollider;
                    boxCollider.isTrigger = daggerInformation.isTrueTrigger;
                }
                break;
        }
    }
}

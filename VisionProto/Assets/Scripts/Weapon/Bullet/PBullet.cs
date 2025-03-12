using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBullet : MonoBehaviour, IListener
{
    bool isDetected;

    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("충돌");
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Floor")
        {
            gameObject.SetActive(false);
        }

        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }

            if (collider.CompareTag("EHead"))
            {
                // 헤드샷 처리
                damageable.Damaged(50, transform.position, transform.position, this.gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                // 일반 데미지 처리
                damageable.Damaged(20, transform.position, transform.position, this.gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.detected:
                {
                    isDetected = (bool)param;
                }
                break;
        }
    }
}

  
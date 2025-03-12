using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAWPBullet : MonoBehaviour, IListener
{
    private bool isDetected;


    public int normalDamage = 50;
    public int headDamage = 50;



    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            normalDamage = 0;
    }


    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if(enemyController != null )
            {
                Debug.Log( "리나 다녀감" + enemyController.istestDetected);
                if (!enemyController.istestDetected)
                    return;
            }


            if (collider.CompareTag("EHead"))
            {
                // 헤드샷 처리
                damageable.Damaged(headDamage, transform.position, transform.position, this.gameObject);
                PoolManager.Instance.ReturnToPool(this.gameObject, "PAWP");
                gameObject.SetActive(false);
            }
            else if (collider.CompareTag("NPC"))
            {
                // 일반 데미지 처리
                damageable.Damaged(normalDamage, transform.position, transform.position, this.gameObject);
                
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            gameObject.SetActive(false);
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

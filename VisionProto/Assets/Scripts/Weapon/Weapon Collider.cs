using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EventManager;

public class WeaponCollider : MonoBehaviour, IListener
{
    public bool isEquipped;
    public bool isthrowing;
    

    bool isDetected;

    public int throwingHeadDamage = 50;
    public int throwingDamage = 30;

    public BoxCollider boxCollider;

    private void Start()
    {
        isEquipped = true;
        isthrowing = false;
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        boxCollider = GetComponent<BoxCollider>();

    }


    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("NPC") || collision.gameObject.CompareTag("EHead")
           || collision.gameObject.CompareTag("Door") || collision.gameObject.CompareTag("Grappling") || collision.gameObject.CompareTag("GrapplingPoint")
           || collision.gameObject.CompareTag("Cabinet") || collision.gameObject.CompareTag("Item") || collision.gameObject.CompareTag("Untagged")) && isthrowing)
        {
            
            EventManager.Instance.NotifyEvent(EventType.Throwing, false);
            SoundManager.Instance.PlayEffectSound(SFX.Gunhit, collision.transform);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isEquipped)
            return;

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
                damageable.Damaged(throwingHeadDamage, transform.position, transform.position, this.gameObject);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                // 일반 데미지
                damageable.Damaged(throwingDamage, transform.position, transform.position, this.gameObject);
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

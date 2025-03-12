using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PFNSCARBullet : MonoBehaviour, IListener
{
    bool isDetected;

    public int normalDamage = 10;
    public int headDamage = 30;

    private int normalLayer;
    private int stencilLayer;
    private Effect effect;



    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        //Invoke("BulletFalse", 3f);

        int powNormalLayer = LayerMask.GetMask("NormalNPC");
        int powStencilLayer = LayerMask.GetMask("StencilNPC");

        normalLayer = (int)Mathf.Ceil(Mathf.Log(powNormalLayer) / Mathf.Log(2));
        stencilLayer = (int)Mathf.Ceil(Mathf.Log(powStencilLayer) / Mathf.Log(2));
    }

    void BulletFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

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
                // 庆靛鸡 贸府
                damageable.Damaged(headDamage, transform.position, transform.position, this.gameObject);
                this.gameObject.SetActive(false);
            }
            else if (collider.CompareTag("NPC"))
            {
                // 老馆 单固瘤 贸府
                damageable.Damaged(normalDamage, transform.position, transform.position, this.gameObject);
                this.gameObject.SetActive(false);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (DataManager.Instance.objectTag.Contains(collision.gameObject.tag))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            Vector3 direction = DataManager.Instance.playerPosition.position - hitPoint;

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("SitWall"))
            {
                EffectManager.Instance.ExecutionEffect(Effect.BulletDecal, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform, 1f);
                EffectManager.Instance.ExecutionEffect(Effect.GunHit, hitPoint, Quaternion.LookRotation(direction), collision.transform, 1f);
                SoundManager.Instance.PlayEffectSound(SFX.Gunhit, collision.transform);
            }

            if (collision.gameObject.layer == normalLayer || collision.gameObject.layer == stencilLayer)
            {
                //RandomHitEffect();
                EffectManager.Instance.ExecutionEffect(Effect.EnemyHit, hitPoint, Quaternion.LookRotation(hitNormal), 1f);
            }
            gameObject.SetActive(false);

        }
    }

    private void RandomHitEffect()
    {
        int startNumber = 0;
        int finalNumber = 2;
        int randomNumber = Random.Range(startNumber, finalNumber);

        switch (randomNumber)
        {
            case 0:
                effect = Effect.EnemyHit1;
                break;
            case 1:
                effect = Effect.EnemyHit2;
                break;
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

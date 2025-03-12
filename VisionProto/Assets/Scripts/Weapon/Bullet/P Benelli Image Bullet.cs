using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;
#if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

public class PBenelliImageBullet : MonoBehaviour
{
    // Decal을 적용할 때 필요한 건?
    // 총알의 Rotation이지 않을까?

    private int normalLayer;
    private int stencilLayer;
    private Effect effect;

    void Start()
    {
        int powNormalLayer = LayerMask.GetMask("NormalNPC");
        int powStencilLayer = LayerMask.GetMask("StencilNPC");

        normalLayer = (int)Mathf.Ceil(Mathf.Log(powNormalLayer) / Mathf.Log(2));
        stencilLayer = (int)Mathf.Ceil(Mathf.Log(powStencilLayer) / Mathf.Log(2));
    }


    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("NPC")
        //    || collision.gameObject.CompareTag("Door") || collision.gameObject.CompareTag("Grappling") || collision.gameObject.CompareTag("GrapplingPoint")
        //    || collision.gameObject.CompareTag("Cabinet") || collision.gameObject.CompareTag("Item") || collision.gameObject.CompareTag("Untagged"))

        if (DataManager.Instance.objectTag.Contains(collision.gameObject.tag))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            Vector3 direction = DataManager.Instance.playerPosition.position - hitPoint;

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
            {
                EffectManager.Instance.ExecutionEffect(Effect.BulletDecal, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform, 1f);
                EffectManager.Instance.ExecutionEffect(Effect.GunHit, hitPoint, Quaternion.LookRotation(direction), collision.transform, 1f);
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
}

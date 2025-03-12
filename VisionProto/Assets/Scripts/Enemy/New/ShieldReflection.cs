using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldReflection : MonoBehaviour
{
    PlayerHP ph;
    // Start is called before the first frame update
    void Start()
    {
        ph = GameObject.Find("Player").GetComponent<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Melee") || other.gameObject.layer == LayerMask.NameToLayer("ThrowWeapon") || other.gameObject.layer == LayerMask.NameToLayer("bullet"))
        {
            //other.gameObject.name
            //damageable.Damaged(20, transform.position, transform.position, this.gameObject);
         Debug.Log( other.gameObject.name);
            SoundManager.Instance.PlayEffectSound(SFX.Boss_ShieldAttack, this.gameObject.transform);
            ph.Damaged(20, transform.position, transform.position, this.gameObject);
        }
    }
}

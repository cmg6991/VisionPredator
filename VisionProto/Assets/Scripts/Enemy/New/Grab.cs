using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public BoseBaseEnemy bs;
    // Start is called before the first frame update
    void Start()
    {
        bs = GetComponentInParent<BoseBaseEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Melee") || collision.gameObject.layer == LayerMask.NameToLayer("ThrowWeapon") || collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            bs.a.Value++;
            Debug.Log(bs.a.Value);
        }

    }
}

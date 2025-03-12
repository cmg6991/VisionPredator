using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torso : MonoBehaviour
{
    private void Start()
    {
        //비활성화를 위한 깡통;
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(40, transform.position, transform.position, this.gameObject);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitEffect : MonoBehaviour
{
    public GameObject hitEffect;
    private GameObject hitVFX;

    // Start is called before the first frame update
    void Start()
    {
        hitVFX = Instantiate(hitEffect, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (hitVFX != null)
        {
            hitVFX.transform.position = transform.position;
            hitVFX.transform.forward = -gameObject.transform.forward;
        }
    }
}

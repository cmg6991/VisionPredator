using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMuzzleEffect : MonoBehaviour
{
    // ÃÑÀ» ½úÀ» ¶§ ³ª¿À´Â Effect
    public GameObject muzzle;
    private GameObject muzzleVFX;

    // Start is called before the first frame update
    void Start()
    {
        muzzleVFX = Instantiate(muzzle, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (muzzleVFX != null)
        {
            muzzleVFX.transform.position = transform.position;
            muzzleVFX.transform.forward = this.transform.forward;
        }
    }
}

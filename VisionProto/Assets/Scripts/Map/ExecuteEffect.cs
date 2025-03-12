using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteEffect : MonoBehaviour
{
    public float deltaTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectDestroy(deltaTime));
    }

    private IEnumerator EffectDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}

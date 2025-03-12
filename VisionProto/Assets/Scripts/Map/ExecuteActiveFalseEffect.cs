using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteActiveFalseEffect : MonoBehaviour
{
    public float deltaTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectDestroy(deltaTime));
    }

    public IEnumerator EffectDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        
        if(this != null)
            this.gameObject.SetActive(false);
    }
}

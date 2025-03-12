using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEffect : MonoBehaviour
{
    public GameObject backEngineDestory;

    private bool isdone;

    private float totalTime;

    public float timer = 1f;

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        if(totalTime > timer)
        {
            backEngineDestory.SetActive(false);

            if(totalTime > (timer + timer)) 
            {
                isdone = !isdone;
                backEngineDestory.SetActive(true);
                totalTime = 0;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCoroutine : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        CoroutineRunner[] existManagers = FindObjectsOfType<CoroutineRunner>();

        foreach (var runner in existManagers)
        {
            if (runner != null && runner.gameObject != null)
            {
                Destroy(runner.gameObject);
            }
        }
    }
}

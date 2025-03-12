using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectAsset : MonoBehaviour
{
    private Camera camera;
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        canvas = Object.FindObjectOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1))
        {
            EffectManager.Instance.ExecutionEffect(Effect.Dash, this.transform);
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            EffectManager.Instance.ExecutionEffect(Effect.VPSight, canvas.transform);
        }
    }
}

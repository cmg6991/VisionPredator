using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFPS : MonoBehaviour
{
    public TMP_Text textFPS;
    private float deltaTime = 0.0f;
    private bool isOn;

    void Start()
    {
        isOn = false;
        textFPS.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F9)) 
        {
            textFPS.text = "";
            isOn = !isOn;
        }

        if(isOn)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            textFPS.text = string.Format("{0:0.} FPS", fps);
        }
    }
}

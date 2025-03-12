using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIMouseSpeed : Singleton<UIMouseSpeed>
{
    [SerializeField]
    Slider mouseSlider;

    public CameraMove cameraMove;

    private void Awake()
    {
//         DontDestroyOnLoad(this);
//         this.gameObject.SetActive(false);
    }

    private void Start()
    {
        mouseSlider.onValueChanged.AddListener(UpdateMouseSpeed);

        if (cameraMove == null)
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (mainCamera != null)
                mainCamera.TryGetComponent<CameraMove>(out cameraMove);
        }

        if (cameraMove != null)
        {
            mouseSlider.value = cameraMove.mouseSpeed;
        }
    }

    void UpdateMouseSpeed(float value)
    {
        if (cameraMove == null)
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            if(mainCamera != null)
                mainCamera.TryGetComponent<CameraMove>(out cameraMove);
        }

        if(cameraMove != null)
        {
            cameraMove.mouseSpeed = value;
            UIManager.Instance.mouseSpeed = value;
        }
    }
}

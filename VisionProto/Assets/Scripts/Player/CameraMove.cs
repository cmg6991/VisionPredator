using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour, IListener
{
    public float mouseSpeed = 5f;

    float mouseX = 0f;
    float mouseY = 0f;

    Transform direction;
    public CinemachineVirtualCamera virtualCamera;
    CinemachinePOV pov;

    private bool isPause;

    // Start is called before the first frame update
    void Start()
    {
        direction = GameObject.Find("Direction").GetComponent<Transform>();
        pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        isPause = false;

        if(UIManager.Instance.mouseSpeed != default)
            mouseSpeed = UIManager.Instance.mouseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPause) 
            return;

        mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        pov.m_HorizontalAxis.Value += mouseX;
        direction.Rotate(Vector3.up,mouseX);
        pov.m_VerticalAxis.Value += -mouseY;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.isPause:
                {
                    isPause = (bool)param;
                }
                break;
        }
    }

    void SaveSpeed()
    {

    }
}

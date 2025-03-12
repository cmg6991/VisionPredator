using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    public GameObject firstDoor;
    public GameObject secondDoor;

    public GameObject playerObject;
    public GameObject bossInitPosition;

    public bool isReverse;
    public bool isClose;

    public bool isClearDoor;
    public bool isBossDoor;
    public GameObject cutScene;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject direction;
    CinemachinePOV pov;

    public bool isDone;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !isDone)
        {
            if (isClearDoor)
            {
                EventManager.Instance.NotifyEvent(EventType.Clear, true);
                EventManager.Instance.NotifyEvent(EventType.isPause, true);
                Time.timeScale = 1.0f;
            }

            if (isBossDoor)
            {
                playerObject.transform.position = bossInitPosition.transform.localPosition;
                playerObject.transform.rotation = Quaternion.Euler(0, 270, 0);

                pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();

                // Camera에서 Value 값 0 0으로 해주자.
                pov.m_HorizontalAxis.Value = 0f;
                pov.m_VerticalAxis.Value = 0f;
                direction.transform.localRotation = Quaternion.identity;

                cutScene.SetActive(true);
                // 컷씬 UI 생성
                EventManager.Instance.NotifyEvent(EventType.isPause, true);
                return;
            }

            if (firstDoor != null) 
            {
                OpenDoorAType doorAType = firstDoor.GetComponent<OpenDoorAType>();
                
                if(doorAType != null)
                {
                    if (!isClose)
                    {
                        doorAType.DoorOpen();
                    }
                    else
                        doorAType.DoorClose();
                }

                OpenDoorBType doorBType = firstDoor.GetComponent<OpenDoorBType>();

                if(doorBType != null )
                {
                    if (isReverse)
                        doorBType.DoorOpen(isReverse);
                    else
                        doorBType.DoorOpen(isReverse);
                }
            }

            if(secondDoor != null)
            {
                OpenDoorAType doorAType = secondDoor.GetComponent<OpenDoorAType>();

                if (doorAType != null)
                {
                    if (!isClose)
                    {
                        doorAType.DoorOpen();
                    }
                    else
                        doorAType.DoorClose();
                }
            }
        }
    }
}

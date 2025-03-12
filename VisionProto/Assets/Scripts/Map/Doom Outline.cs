using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomOutline : MonoBehaviour, IListener
{
    public GameObject doomOutline;

    private bool isVPState;

    private void Start()
    {
        EventManager.Instance.AddEvent(EventType.DoomOutline, OnEvent);
    }


    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.DoomOutline:
                {
                    isVPState = (bool)param;

                    if(isVPState)
                        doomOutline.SetActive(true);
                    else
                        doomOutline.SetActive(false);
                }
                break;
        }
    }
}

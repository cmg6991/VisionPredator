using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;

public class AssetFadeInFadeOut : MonoBehaviour, IListener
{
    public TransitionAnimator animator;

    private bool isVPState;

   

    private void Start()
    {
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

  

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                }
                break;
        }
    }

}

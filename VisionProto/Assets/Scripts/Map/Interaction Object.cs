using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionObject : MonoBehaviour, IListener
{
    public bool isDone;
    private bool isVPState;
    private bool isMouseEnter;
    CrossHairColor crossHairColor;

    private void Start()
    {
        crossHairColor = new CrossHairColor();
        isDone = false;
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    private void OnMouseEnter()
    {
        if (!isVPState && !isDone)
        {
            crossHairColor.information = CrossHairInformation.Interaction;
            crossHairColor.isAttack = false;
            isMouseEnter = true;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        }
    }

    private void OnMouseOver()
    {
        if (isVPState && isMouseEnter || isDone)
        {
            crossHairColor.information = CrossHairInformation.Normal;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

            isMouseEnter = false;
        }
    }

    private void OnMouseExit()
    {
        if (!isVPState)
        {
            crossHairColor.information = CrossHairInformation.Normal;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
            isMouseEnter = false;
        }
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

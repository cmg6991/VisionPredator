using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum CrossHairInformation
{
    Normal,
    Interaction,
    Attack,
    Kill,
}
public struct CrossHairColor
{
    public CrossHairInformation information;
    public bool isAttack;
}

public class CrossHair : MonoBehaviour, IListener
{
    public Image crossHair;
    public Image interactionObject;
    public Sprite baseImage;
    public Sprite attackImage;
    public Sprite interactionImage;
    public Sprite killImage;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.CrossHairColor, OnEvent);

        CrossHairColor crossHairColor = new CrossHairColor();
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
    }

    // isAttack�� True�� Ȱ��ȭ, false�� ��Ȱ��ȭ
    // �̰� �������� �������� �ҰŸ� �̰ɷ� �̹����� �ٸ� �ɷ� ��� �ϴ� ���ܵ���.
    private void SetCrossHairColor(CrossHairInformation infomation)
    {
        Color crossHairColor = crossHair.color;
        Color interactionColor = interactionObject.color;

        switch (infomation)
        {
            case CrossHairInformation.Normal:
                {
                    crossHair.color = Color.white;
                    crossHair.sprite = baseImage;
                    interactionColor.a = 0f;
                    interactionObject.color = interactionColor;
                    interactionObject.SetNativeSize();
                }
                break;
            case CrossHairInformation.Interaction:
                {
                    crossHair.color = Color.white;
                    crossHair.sprite = baseImage;
                    interactionObject.sprite = interactionImage;
                    interactionColor.a = 1f;
                    interactionObject.color = interactionColor;
                    interactionObject.SetNativeSize();
                }
                break;
            case CrossHairInformation.Attack:
                {
                    crossHair.color = Color.white;
                    crossHair.sprite = baseImage;
                    interactionObject.sprite = attackImage;
                    interactionColor.a = 1f;
                    interactionObject.color = interactionColor;
                    interactionObject.SetNativeSize();
                }
                break;
            case CrossHairInformation.Kill:
                {
                    crossHair.color = Color.red;
                    crossHair.sprite = baseImage;
                    interactionObject.sprite = killImage;
                    interactionColor.a = 1f;
                    interactionObject.color = interactionColor;
                    interactionObject.SetNativeSize();
                }
                break;
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.CrossHairColor:
                {
                    CrossHairColor crossHairColor = (CrossHairColor)param;
                    SetCrossHairColor(crossHairColor.information);
                }
                break;
        }
    }
}

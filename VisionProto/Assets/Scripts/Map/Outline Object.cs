using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutlineObject : MonoBehaviour, IListener
{
    // ���콺�� ����� ��
    public bool isMouseEnter;
    private int outlineLayer;
    private int objectLayer;
    private int stencilObjectLayer;
    public bool isDone;
    public bool isRePosition;
    private bool isVPState;

    public GameObject uiPosition;

    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject thirdObject;

    // �̸��� ��� �˱�?
    private readonly string objectContext = "Press F to Interaction";

    // ���� �ð� �Ŀ� ����ġ��
    public bool isStop;

    private WeaponUIInformation objectInformation;
    private CrossHairColor crossHairColor;

    // Start is called before the first frame update
    void Start()
    {
        int outlineLayerValue = LayerMask.GetMask("Outline");
        // �ش� �κ��� �� ������� �ʴ´�. �׷��� + 1�� �����. Object�� log_2�� �� ���� �ʴ´�.
        int gunLayerValue = LayerMask.GetMask("Object");
        
        int vpLayerVale = LayerMask.GetMask("StencilObject");

        isDone = false;
        isRePosition = false;
        isVPState = DataManager.Instance.isVPState;

        outlineLayer = (int)(Mathf.Log(outlineLayerValue) / Mathf.Log(2));
        objectLayer = (int)(Mathf.Log(gunLayerValue) / Mathf.Log(2)) + 1;
        stencilObjectLayer = (int)(Mathf.Log(vpLayerVale) / Mathf.Log(2)) + 1;
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);

        string modelName = this.gameObject.name;

        /// �� Script�� ������ ������ "F�� ���� ��ȣ�ۿ�"�� ���.
        // main_low, right_door_low, small_cabinet_low -> Cabinet 
        // cover_rotate_bolt_low, case_low -> Item 
        // Press F to Interaction
        objectInformation = new WeaponUIInformation();
        crossHairColor = new CrossHairColor();
 
        //objectContext = "Press F to Interaction";

        objectInformation.gunName = objectContext;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseEnter()
    {
        if (isVPState)
            return;

        // ���콺�� ������� layer�� Outline���� �ٲ�� �Ѵ�.
        if (!isDone)
        {
            isMouseEnter = true;

            if (firstObject != null)
                firstObject.gameObject.layer = outlineLayer;

            if(secondObject != null)
                secondObject.gameObject.layer = outlineLayer;

            if(thirdObject != null)
                thirdObject.gameObject.layer = outlineLayer;

            //this.gameObject.layer = outlineLayer;
           

            crossHairColor.information = CrossHairInformation.Interaction;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        }
    }

    public void PrintObjectName()
    {
        objectInformation.isMouseEnter = isMouseEnter;
        objectInformation.gunName = objectContext;
        objectInformation.transform = uiPosition.transform;
        EventManager.Instance.NotifyEvent(EventType.UIGunName, objectInformation);
    }

    public void PrintBlankObjectName()
    {
        objectInformation.isMouseEnter = isMouseEnter;
        objectInformation.gunName = "";
        objectInformation.transform = uiPosition.transform;
        EventManager.Instance.NotifyEvent(EventType.UIGunName, objectInformation);
    }

    public void MouseOver()
    {
        if (isVPState)
        {
            if (firstObject != null)
                firstObject.gameObject.layer = objectLayer;

            if (secondObject != null)
                secondObject.gameObject.layer = objectLayer;

            if (thirdObject != null)
                thirdObject.gameObject.layer = objectLayer;
            this.gameObject.layer = objectLayer;

            crossHairColor.information = CrossHairInformation.Normal;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

            if (!isDone)
            {
                objectInformation.isMouseEnter = isMouseEnter;
                //this.gameObject.layer = objectLayer;
                objectInformation.gunName = "";
                EventManager.Instance.NotifyEvent(EventType.UIGunName, objectInformation);
            }
        }
    }

    public void MouseExit()
    {
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

        isMouseEnter = false;
        objectInformation.isMouseEnter = isMouseEnter;

        if (firstObject != null)
            firstObject.gameObject.layer = objectLayer;

        if (secondObject != null)
            secondObject.gameObject.layer = objectLayer;

        if (thirdObject != null)
            thirdObject.gameObject.layer = objectLayer;
        //this.gameObject.layer = objectLayer;
        objectInformation.gunName = "";
        EventManager.Instance.NotifyEvent(EventType.UIGunName, objectInformation);
    }

    public void VPMouse()
    {
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

        isMouseEnter = false;
        objectInformation.isMouseEnter = isMouseEnter;

        if (firstObject != null)
            firstObject.gameObject.layer = stencilObjectLayer;

        if (secondObject != null)
            secondObject.gameObject.layer = stencilObjectLayer;

        if (thirdObject != null)
            thirdObject.gameObject.layer = stencilObjectLayer;
        //this.gameObject.layer = objectLayer;
        objectInformation.gunName = "";
        EventManager.Instance.NotifyEvent(EventType.UIGunName, objectInformation);
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

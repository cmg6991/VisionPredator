using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutlineWeapon : MonoBehaviour, IListener
{
    // ���콺�� ����� ��
    public bool isMouseEnter;
    private int outlineLayer;
    private int gunLayer;
    public bool isDone;
    public bool isRePosition;
    private bool isVPState;


    // �̸��� ��� �˱�?
    private string gunName;

    // ���� �ð� �Ŀ� ����ġ��
    public bool isStop;

    private WeaponUIInformation weaponInformation;
    private CrossHairColor crossHairColor;

    // Start is called before the first frame update
    void Start()
    {
        int outlineLayerValue = LayerMask.GetMask("Outline");
        int gunLayerValue = LayerMask.GetMask("Gun");
        isDone = false;
        isRePosition = false;
        isVPState = DataManager.Instance.isVPState;

        outlineLayer = (int)(Mathf.Log(outlineLayerValue) / Mathf.Log(2));
        gunLayer = (int)(Mathf.Log(gunLayerValue) / Mathf.Log(2));
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);

        string modelGunName = this.gameObject.name;

        weaponInformation = new WeaponUIInformation();
        crossHairColor = new CrossHairColor();

        switch (modelGunName)
        {
            case "ss":
                    gunName = "Shot Gun";
                break;
            case "PP 1":
                    gunName = "Pistol";
                break;
            case "ff":
                    gunName = "Rifle";
                break;
        }
        weaponInformation.gunName = gunName;
    }

    public void FixedUpdate()
    {
        if (isRePosition && !isVPState && !isStop)
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            Invoke("GunRePosition", 2.0f);
        }
    }

    private void Update()
    {
        
    }

    private void GunRePosition()
    {
        isStop = true;
    }

    public void GunLayer()
    {
        this.gameObject.layer = gunLayer;
    }

    public void MouseEnter()
    {
        if (isVPState)
            return;

        // ���콺�� ������� layer�� Outline���� �ٲ�� �Ѵ�.
        if (!isDone)
        {
            isMouseEnter = true;
            this.gameObject.layer = outlineLayer;

            crossHairColor.information = CrossHairInformation.Interaction;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        }
    }

    public void PrintWeaponName()
    {
        weaponInformation.isMouseEnter = isMouseEnter;
        weaponInformation.gunName = gunName;
        weaponInformation.transform = this.transform;
        EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
    }

    public void BlankWeaponName()
    {
        weaponInformation.isMouseEnter = isMouseEnter;
        weaponInformation.gunName = "";
        weaponInformation.transform = this.transform;
        EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
    }

    public void MouseOver()
    {
        if (isVPState)
        {
            this.gameObject.layer = gunLayer;

            crossHairColor.information = CrossHairInformation.Normal;
            crossHairColor.isAttack = false;
            EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

            if (!isDone)
            {
                weaponInformation.isMouseEnter = isMouseEnter;
                this.gameObject.layer = gunLayer;
                weaponInformation.gunName = "";
                EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
            }
        }
    }

    public void MouseExit()
    {
        // ���콺�� ���� �ʾ����� layer�� �ٽ� Gun���� ������.
        //         if (!isDone)
        //         {
        //         }
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

        isMouseEnter = false;
        weaponInformation.isMouseEnter = isMouseEnter;
        this.gameObject.layer = gunLayer;
        weaponInformation.gunName = "";
        EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
    }

    private void OnMouseEnter()
    {
//         if (isVPState)
//             return;
// 
//         // ���콺�� ������� layer�� Outline���� �ٲ�� �Ѵ�.
//         if (!isDone)
//         {
//             isMouseEnter = true;
//             this.gameObject.layer = outlineLayer;
//             weaponInformation.isMouseEnter = isMouseEnter;
//             weaponInformation.gunName = gunName;
//             weaponInformation.transform = this.transform;
//             EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
// 
//             crossHairColor.information = CrossHairInformation.Interaction;
//             crossHairColor.isAttack = false;
//             EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
//         }
    }

    private void OnMouseOver()
    {
//         if(isVPState)
//         {
//             this.gameObject.layer = gunLayer;
// 
//             crossHairColor.information = CrossHairInformation.Normal;
//             crossHairColor.isAttack = false;
//             EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
// 
//             if(!isDone)
//             {
//                 weaponInformation.isMouseEnter = isMouseEnter;
//                 this.gameObject.layer = gunLayer;
//                 weaponInformation.gunName = "";
//                 EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
//             }
//         }
    }

    private void OnMouseExit()
    {
        // ���콺�� ���� �ʾ����� layer�� �ٽ� Gun���� ������.
//         if (!isDone)
//         {
//         }
//             crossHairColor.information = CrossHairInformation.Normal;
//             crossHairColor.isAttack = false;
//             EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
// 
//             isMouseEnter = false;
//             weaponInformation.isMouseEnter = isMouseEnter;
//             this.gameObject.layer = gunLayer;
//             weaponInformation.gunName = "";
//             EventManager.Instance.NotifyEvent(EventType.UIGunName, weaponInformation);
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

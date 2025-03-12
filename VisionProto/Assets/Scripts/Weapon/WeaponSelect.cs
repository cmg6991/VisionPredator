using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VP 상태일때 VP
/// </summary>
public class WeaponSelect : MonoBehaviour, IListener
{
    public GameObject dagger;
    public GameObject fbxDagger;
    public GameObject vpWeapon;
    public AutoPickup autoPickup;

    public GameObject playerVPHand;
    public GameObject playerHumanHand;

    private bool isVPState;

    AnimationInformation Dinfo;
    UIBulletInformation uiWeaponSelect;

    // Start is called before the first frame update
    void Start()
    {
        isVPState = false;
        uiWeaponSelect = new UIBulletInformation();
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;

                    if (isVPState)
                    {
                        playerVPHand.SetActive(true);
                        playerHumanHand.SetActive(false);
                        Dinfo.stateName = "Draw";
                        Dinfo.layer = -1;
                        Dinfo.normalizedTime = 0f;
                        EventManager.Instance.NotifyEvent(EventType.VPAnimator, Dinfo);

                        uiWeaponSelect.currentBullet = 99f;
                        uiWeaponSelect.maxBullet = 99f;
                        uiWeaponSelect.weaponSelect = UIWeaponSelect.VPWeapon;
                        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, uiWeaponSelect);

                        dagger.SetActive(false);
                        fbxDagger.SetActive(false);
                        // Pickup 상태이면 notify Event로 change 해야 하는데 Auto Pickup도 꺼야함
                        autoPickup.isActive = false;
                        autoPickup.GunListClear();
                        EventManager.Instance.NotifyEvent(EventType.Change);
                        vpWeapon.SetActive(true);
                    }
                    else
                    {
                        playerVPHand.SetActive(false);
                        playerHumanHand.SetActive(true);
                        dagger.SetActive(true);
                        fbxDagger.SetActive(true);
                        vpWeapon.SetActive(false);
                        autoPickup.isActive = true;

                        uiWeaponSelect.currentBullet = 99f;
                        uiWeaponSelect.maxBullet = 99f;
                        uiWeaponSelect.weaponSelect = UIWeaponSelect.Dagger;
                        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, uiWeaponSelect);

                        Dinfo.stateName = "Dagger";
                        Dinfo.layer = -1;
                        Dinfo.normalizedTime = 0f;
                        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                    }
                }
                break;
        }
    }
}

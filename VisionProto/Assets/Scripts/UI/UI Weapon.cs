using System.Text;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UIWeaponSelect
{
    Dagger,
    VPWeapon,
    Rifle,
    ShotGun,
    Pistol,
}

public class UIWeapon : MonoBehaviour, IListener
{
    // 뭐가 필요할까?
    // 최대 탄알 개수
    // 현재 탄알 수만 가지고 오면 된다!
    public TMP_Text textCurrentBullet;
    public TMP_Text textMaxBullet;

    // Gun 표시
    public TMP_Text gunInformation;
    public Image backGroundGunInfo;

    // Gun Image
    public Image gumImage;

    // Test Sprite
    public Sprite vpWeapon;
    public Sprite shotgun;
    public Sprite dagger;
    public Sprite pistol;
    public Sprite rifle;

    [SerializeField]
    private float size = 5f;

    [SerializeField]
    private Vector3 offset = Vector3.one;

    [SerializeField]
    private float testDistance = 80f;

    private float currentBullet;    // 현재 탄알 수
    private float maxBullet;        // 최대 탄알 수

    private bool isMouseEnter;
    private bool isEquied;

    private Transform targetObjectTransform;

    void Start()
    {
        textCurrentBullet.text = "0";
        textMaxBullet.text = "/ 0";
        EventManager.Instance.AddEvent(EventType.WeaponBullet, OnEvent);
        EventManager.Instance.AddEvent(EventType.UIGunName, OnEvent);
        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
    }

    void Update()
    {
        if (isMouseEnter && (targetObjectTransform != null)) 
        {
            gunInformation.transform.position = Camera.main.WorldToScreenPoint(targetObjectTransform.transform.position + offset);
            backGroundGunInfo.rectTransform.position = Camera.main.WorldToScreenPoint(targetObjectTransform.transform.position + offset);
        }
    }

    private void ChangeBullet()
    {
        if(maxBullet == 99)
        {
            textCurrentBullet.transform.gameObject.SetActive(false);
            return;
        }

        textCurrentBullet.transform.gameObject.SetActive(true);
        float remainBullet = maxBullet - currentBullet;
        
        textCurrentBullet.text = remainBullet.ToString();

        //textMaxBullet.text = "/ "+maxBullet;
    }

    private IEnumerator WeaponUI()
    {
        yield return null;

        float width = gunInformation.rectTransform.rect.width;
        float height = gunInformation.rectTransform.rect.height;

        backGroundGunInfo.rectTransform.sizeDelta = new Vector2(width + size, height + (size * 0.5f));
    }

    private void WeaponImage(UIWeaponSelect select)
    {
        switch(select) 
        {
            case UIWeaponSelect.Dagger:
                {
                    gumImage.sprite = dagger;
                }
                break;
            case UIWeaponSelect.VPWeapon:
                {
                    gumImage.sprite = vpWeapon;
                }
                break;
            case UIWeaponSelect.Rifle:
                {
                    gumImage.sprite = rifle;
                }
                break;
            case UIWeaponSelect.Pistol:
                {
                    gumImage.sprite = pistol;
                }
                break;
            case UIWeaponSelect.ShotGun:
                {
                    gumImage.sprite = shotgun;
                }
                break;
            default:
                break;
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.WeaponBullet:
                {
                    UIBulletInformation bulletInformation = (UIBulletInformation)param;
                    currentBullet = bulletInformation.currentBullet;
                    maxBullet = bulletInformation.maxBullet;
                    // Update가 필요 없을 수도 있어.
                    WeaponImage(bulletInformation.weaponSelect);
                    ChangeBullet();
                }
                break;
            case EventType.isEquiped:
                {
                    isEquied = (bool)param;

                    // 장착하고 있을 때 UI가 안 나온다.
                    if(isEquied)
                    {
                        isMouseEnter = false;
                        gunInformation.gameObject.SetActive(false);
                        backGroundGunInfo.gameObject.SetActive(false);
                    }
                }
                break;
            case EventType.UIGunName: 
                {
                    WeaponUIInformation weaponInfo = (WeaponUIInformation)param;

                    isMouseEnter = weaponInfo.isMouseEnter;
                    gunInformation.text = weaponInfo.gunName;


                    StartCoroutine(WeaponUI());

                    targetObjectTransform = weaponInfo.transform;

                    if(targetObjectTransform != null )
                    {
                        gunInformation.transform.position = Camera.main.WorldToScreenPoint(targetObjectTransform.position);
                        backGroundGunInfo.rectTransform.position = Camera.main.WorldToScreenPoint(targetObjectTransform.position);
                    }

                    if (isMouseEnter)
                    {
                        gunInformation.gameObject.SetActive(true);
                        backGroundGunInfo.gameObject.SetActive(true);
                    }
                    else
                    {
                        gunInformation.gameObject.SetActive(false);
                        backGroundGunInfo.gameObject.SetActive(false);
                    }

                }
                break;
        }
    }
}

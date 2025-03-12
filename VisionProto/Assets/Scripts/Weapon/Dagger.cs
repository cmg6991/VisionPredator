using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ���⸦ ������ ���� �ʴٸ� �ܰ��� Ȱ��ȭ �ȴ�.
/// </summary>
public class Dagger : MonoBehaviour, IListener
{
    // �ִϸ��̼��� ����Ǹ� ���� ����� �� �ʿ� ����.
    // �ִϸ����Ϳ��� true false�� �ϸ�ȴ�. -> �װ� �ƴϴ�
    public GameObject dagger;
    public GameObject fbxDagger;
    public GameObject gunContainer;

    [SerializeField]
    private float yPositon;

    [SerializeField]
    private float xRotation;

    // ���� ������
    public int cutDamage = 10;
    // ��� ������
    public int pierceDamage = 20;
    // ���� �ݰ� ����
    public int cutRange = 120;
    // ���� �ӵ�
    public float cuttingSpeed = 2;
    // ��� �ӵ�
    public float pireceSpeed = 2;
    // ��� �ݰ� ���� (Dagger�� Z ������ �Ǿ� �ִ�.)
    public float pireceRange;

    // Decal ���� ��ġ
    [SerializeField]
    private float cutDistanceCamera = 5f;

    [SerializeField]
    private float pierceDistanceCamera = 10f;

    [SerializeField]
    private float cuttingDegree = 30f;

    private float startCutDegree;
    private float endCutDegree;

    private Vector3 initPosition;


    // �ܰ� ���� Collider�� ����� ���� �״��ұ�?
    private bool isCutting;
    private bool isCutting2;
    private bool isPiercing;
    private float totalTime;

    private bool nextAttack;

    private bool isEquiped;

    AnimationInformation Dinfo;

    private bool isVPState;
    UIBulletInformation uiWeaponSelect;

    private GameObject effectContanier;
    private TrailRenderer windTrailRenderer;
    private bool isPause;

    private bool isFindingTrailRenderer;

    private bool isTutorial;
    void Start()
    {
        totalTime = 0;
        startCutDegree = -cutRange * 0.5f;
        endCutDegree = cutRange * 0.5f;
        initPosition = this.transform.localPosition;
        pireceRange = dagger.transform.localScale.z;
        nextAttack = false;

        Dinfo = new AnimationInformation();
        uiWeaponSelect = new UIBulletInformation();

        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        EventManager.Instance.AddEvent(EventType.Tutorial, OnEvent);

        // �̰� �ܻ�ȿ����
        //effectContanier = GameObject.Find("Weapon Dummy");
        effectContanier = GameObject.Find("Knife Effect Contanier");

        
    }

    enum eWeapon
    {
        none,
        Weapon1,
        Weapon2,
    }

    eWeapon currentWeapon = eWeapon.none;
    eWeapon oldWeapon;

    private void Update()
    {
        // ���� �����ϰ� �ִٸ� ���� �ʴ´�.
        if (isEquiped || isPause || isTutorial)
            return;

        if(!isFindingTrailRenderer)
        {
            windTrailRenderer = fbxDagger.GetComponentInChildren<TrailRenderer>();

            if (windTrailRenderer == null)
                Debug.Log("None FBX Daager Get TrailRenderer");
            isFindingTrailRenderer = true;
        }

        KnifeController();
        ActionKnife();
        DelayKnife();
    }

    // Į�� ���ų� ��ų�
    void KnifeController()
    {
        // ��, ��� ���� �� �Է� ���ϰ�
        if (isCutting || isPiercing)
            return;

        // ��Ŭ������ �� ����
        if (Input.GetMouseButtonDown(0) && !nextAttack)
        {
            if (!isCutting2)
            {
                Dinfo.stateName = "DSlice";
                Dinfo.layer = -1;
                Dinfo.normalizedTime = 0f;
                EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                SoundManager.Instance.PlayEffectSound(SFX.Knife_Swing_2, this.transform.parent);
                isCutting2 = true;

                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.KnifeCutting_1;
                cameraInfomation.amplitude = 1f;
                cameraInfomation.frequency = 1f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }
            else
            {
                Dinfo.stateName = "DSlice 1";
                Dinfo.layer = -1;
                Dinfo.normalizedTime = 0f;
                EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                SoundManager.Instance.PlayEffectSound(SFX.Knife_Swing_1, this.transform.parent);
                isCutting2 = false;
                
                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.KnifeCutting_2;
                cameraInfomation.amplitude = 1f;
                cameraInfomation.frequency = 1f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

            }
            // Cutting �� 
            isCutting = true;
            nextAttack = true;
            windTrailRenderer.enabled = true;

            // �ܰ��� ���� ������ ������ Collider ������ �ѱ��.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, cutDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            CreateKnifeDecal(true);

            // Direction�� �޾ƿ;� �ǳ�
     
            // ������ �� �ٷ� start Degree�� �̵�
            this.transform.localRotation = Quaternion.Euler(0f, startCutDegree, 0f);
            return;
        }

        // ��Ŭ�� �ι� ���� �� ���
        if (Input.GetMouseButtonDown(0) && nextAttack)
        {
            Dinfo.stateName = "DSting";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Pierce ��
            isPiercing = true;
            nextAttack = false;
            windTrailRenderer.enabled = true;

            // �ܰ��� ��� ������ ������ Collider ������ �ѱ��.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, pierceDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            SoundManager.Instance.PlayEffectSound(SFX.Knife_Piercing, this.transform.parent);
            CreateKnifeDecal(false);

            CameraInfomation cameraInfomation = new CameraInfomation();
            cameraInfomation.setting = CameraSetting.KnifePiercing;
            cameraInfomation.amplitude = 1f;
            cameraInfomation.frequency = 1f;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

            // ������ �� �ʱ� ��ġ�� �̵�
            this.transform.position = initPosition;
            return;
        }
    }

    // �Է��� �޾����� �����ؾ���
    void ActionKnife()
    {
        // �ִϸ��̼��� �ִٸ� ����, �ƴ϶�� ���� ��ġ ������ �ؾ���
        // ������ ���� �ִϸ��̼��� �� �������� �߰� ���� Ÿ���� ���� �ִ�.

        // �ð��� ������ ���� -60 ~ 60���� ������ �Ѵ�.
        if (isCutting)
        {
            // Dagger -60 ~ 60 ���� �������� �ٽ� �������� ���ƿ´�.
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotation = Mathf.Lerp(startCutDegree, endCutDegree, time * cuttingSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);

            Vector3 yRotation = transform.localRotation.eulerAngles;

            // ������ �� �� ������ ��ġ������ ������ �Ǵ°Ŵϱ� ���� �� �ϸ� �� ��?
            // ��ġ�� ����

            if (currentRotation == endCutDegree)
            {
                DaggerInformation daggerInformation = SettingDaggerInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isCutting = false;
                windTrailRenderer.enabled = false;
                totalTime = 0;

                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.KnifeCutting_1;
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }

        }
        else if (isPiercing)
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            // Dagger Z ������ �����ؼ� ��⸦ �����Ѵ�.
            float currentPosition = Mathf.Lerp(initPosition.z, initPosition.z + pireceRange, pireceSpeed * time);
            transform.localPosition = new Vector3(0, 0, currentPosition);

            float roundPosition = (float)System.Math.Round(currentPosition, 2);
            float pirecePosition = (float)System.Math.Round(initPosition.z + pireceRange, 2);
            if (roundPosition == pirecePosition)
            {
                // ��Ⱑ ���� �� ����
                DaggerInformation daggerInformation = SettingDaggerInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
                transform.localPosition = initPosition;

                isPiercing = false;
                windTrailRenderer.enabled = false;
                totalTime = 0;

                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.KnifePiercing;
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }

        }
    }

    void DelayKnife()
    {
        // ������ �� ����
        if (isCutting)
        {
            totalTime += Time.deltaTime;
        }
        // ����� �� ����
        else if (isPiercing)
        {
            totalTime += Time.deltaTime;
        }
    }

    private void CreateKnifeDecal(bool _isCutting)
    {
        Camera mainCamera = Camera.main;

        if (_isCutting)
        {
            if (!isCutting2)
            {
                Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * cutDistanceCamera;
                GameObject knifeDecal = EffectManager.Instance.ExecutionEffectObject(Effect.CuttingDecal, spawnPosition, Quaternion.identity);
                //GameObject knifeEffect = EffectManager.Instance.ExecutionEffectObject(Effect.KnifeEffect, effectContanier.transform);
                //knifeEffect.transform.rotation *= Quaternion.Euler(0, 0, -cuttingDegree);
                knifeDecal.transform.LookAt(mainCamera.transform);
                knifeDecal.transform.rotation *= Quaternion.Euler(0, 0, cuttingDegree);
            }
            else
            {
                Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * cutDistanceCamera;
                GameObject knifeDecal = EffectManager.Instance.ExecutionEffectObject(Effect.CuttingDecal, spawnPosition, Quaternion.identity);
                //GameObject knifeEffect = EffectManager.Instance.ExecutionEffectObject(Effect.KnifeEffect, effectContanier.transform);
                knifeDecal.transform.LookAt(mainCamera.transform);
            }
        }
        else
        {
            Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * pierceDistanceCamera;
            GameObject knifeDecal = EffectManager.Instance.ExecutionEffectObject(Effect.PiercingDecal, spawnPosition, mainCamera.transform.rotation);
            knifeDecal.transform.LookAt(mainCamera.transform);
        }
    }

    /// <summary>
    /// �ܰ� ������ �ѱ��.
    /// </summary>
    /// <param name="enable">Collider�� Ȱ��ȭ �Ұų�?</param>
    /// <param name="trigger">Ʈ���Ÿ� Ȱ��ȭ �Ұų�?</param>
    /// <param name="damaged">������ ����</param>
    /// <returns></returns>
    DaggerInformation SettingDaggerInformation(bool enable, bool trigger, int damaged)
    {
        DaggerInformation daggerInformation = new DaggerInformation();
        daggerInformation.isTrueTrigger = trigger;
        daggerInformation.isEnableCollider = enable;
        daggerInformation.damaged = damaged;

        return daggerInformation;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.isEquiped:
                {
                    isEquiped = (bool)param;
                    isVPState = DataManager.Instance.isVPState;

                    if (!isEquiped && !isVPState)
                    {
                        dagger.gameObject.SetActive(true);
                        fbxDagger.SetActive(true);

                        uiWeaponSelect.currentBullet = 99f;
                        uiWeaponSelect.maxBullet = 99f;
                        uiWeaponSelect.weaponSelect = UIWeaponSelect.Dagger;
                        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, uiWeaponSelect);

                        Dinfo.stateName = "DIdle";
                        Dinfo.layer = -1;
                        Dinfo.normalizedTime = 0f;
                        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                    }
                    else
                    {
                        dagger.gameObject.SetActive(false);
                        fbxDagger.SetActive(false);
                    }
                }
                break;
            case EventType.VPState:
                {
                    isVPState = (bool)param;

                    if (!isEquiped && !isVPState)
                    {
                        dagger.gameObject.SetActive(true);
                        fbxDagger.SetActive(true);



                        uiWeaponSelect.currentBullet = 99f;
                        uiWeaponSelect.maxBullet = 99f;
                        uiWeaponSelect.weaponSelect = UIWeaponSelect.Dagger;
                        EventManager.Instance.NotifyEvent(EventType.WeaponBullet, uiWeaponSelect);

                        Dinfo.stateName = "DIdle";
                        Dinfo.layer = -1;
                        Dinfo.normalizedTime = 0f;
                        EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
                    }
                    else
                    {
                        dagger.gameObject.SetActive(false);
                        fbxDagger.SetActive(false);
                    }
                }
                break;
            case EventType.isPause:
                isPause = (bool)param;
                break;
            case EventType.Tutorial:
                TutorialWeapon weapon = (TutorialWeapon)param;
                isTutorial = weapon.isDagger;
                break;
        }
    }

}

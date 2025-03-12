using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 무기를 가지고 있지 않다면 단검이 활성화 된다.
/// </summary>
public class Dagger : MonoBehaviour, IListener
{
    // 애니메이션이 진행되면 내가 힘들게 할 필요 없다.
    // 애니메이터에서 true false로 하면된다. -> 그건 아니다
    public GameObject dagger;
    public GameObject fbxDagger;
    public GameObject gunContainer;

    [SerializeField]
    private float yPositon;

    [SerializeField]
    private float xRotation;

    // 베기 데미지
    public int cutDamage = 10;
    // 찌르기 데미지
    public int pierceDamage = 20;
    // 베기 반경 범위
    public int cutRange = 120;
    // 베는 속도
    public float cuttingSpeed = 2;
    // 찌르는 속도
    public float pireceSpeed = 2;
    // 찌르기 반경 범위 (Dagger의 Z 범위로 되어 있다.)
    public float pireceRange;

    // Decal 생성 위치
    [SerializeField]
    private float cutDistanceCamera = 5f;

    [SerializeField]
    private float pierceDistanceCamera = 10f;

    [SerializeField]
    private float cuttingDegree = 30f;

    private float startCutDegree;
    private float endCutDegree;

    private Vector3 initPosition;


    // 단검 전용 Collider를 만들고 끄고 켰다할까?
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

        // 이건 잔상효과로
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
        // 현재 장착하고 있다면 돌지 않는다.
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

    // 칼을 벨거냐 찌를거냐
    void KnifeController()
    {
        // 컷, 찌르기 중일 때 입력 못하게
        if (isCutting || isPiercing)
            return;

        // 좌클릭했을 때 베기
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
            // Cutting 중 
            isCutting = true;
            nextAttack = true;
            windTrailRenderer.enabled = true;

            // 단검의 베기 데미지 정보를 Collider 쪽으로 넘긴다.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, cutDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            CreateKnifeDecal(true);

            // Direction을 받아와야 되나
     
            // 시작할 때 바로 start Degree로 이동
            this.transform.localRotation = Quaternion.Euler(0f, startCutDegree, 0f);
            return;
        }

        // 좌클릭 두번 했을 때 찌르기
        if (Input.GetMouseButtonDown(0) && nextAttack)
        {
            Dinfo.stateName = "DSting";
            Dinfo.layer = -1;
            Dinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.PlayerAnimator, Dinfo);
            // Pierce 중
            isPiercing = true;
            nextAttack = false;
            windTrailRenderer.enabled = true;

            // 단검의 찌르기 데미지 정보를 Collider 쪽으로 넘긴다.
            DaggerInformation daggerInformation = SettingDaggerInformation(true, true, pierceDamage);
            EventManager.Instance.NotifyEvent(EventType.DaggerInformation, daggerInformation);
            SoundManager.Instance.PlayEffectSound(SFX.Knife_Piercing, this.transform.parent);
            CreateKnifeDecal(false);

            CameraInfomation cameraInfomation = new CameraInfomation();
            cameraInfomation.setting = CameraSetting.KnifePiercing;
            cameraInfomation.amplitude = 1f;
            cameraInfomation.frequency = 1f;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

            // 시작할 때 초기 위치로 이동
            this.transform.position = initPosition;
            return;
        }
    }

    // 입력을 받았으니 실행해야지
    void ActionKnife()
    {
        // 애니메이션이 있다면 실행, 아니라면 직접 위치 조절을 해야지
        // 서든을 보니 애니메이션은 그 언저리로 긋고 실제 타격은 따로 있다.

        // 시간이 끝나기 전에 -60 ~ 60도를 끝내야 한다.
        if (isCutting)
        {
            // Dagger -60 ~ 60 도로 지나가고 다시 원점으로 돌아온다.
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotation = Mathf.Lerp(startCutDegree, endCutDegree, time * cuttingSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);

            Vector3 yRotation = transform.localRotation.eulerAngles;

            // 어차피 한 번 고정된 위치에서만 실행이 되는거니까 끝날 때 하면 될 듯?
            // 위치는 고정

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

            // Dagger Z 방향을 조절해서 찌르기를 구현한다.
            float currentPosition = Mathf.Lerp(initPosition.z, initPosition.z + pireceRange, pireceSpeed * time);
            transform.localPosition = new Vector3(0, 0, currentPosition);

            float roundPosition = (float)System.Math.Round(currentPosition, 2);
            float pirecePosition = (float)System.Math.Round(initPosition.z + pireceRange, 2);
            if (roundPosition == pirecePosition)
            {
                // 찌르기가 끝날 때 실행
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
        // 베기일 때 실행
        if (isCutting)
        {
            totalTime += Time.deltaTime;
        }
        // 찌르기일 때 실행
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
    /// 단검 정보를 넘긴다.
    /// </summary>
    /// <param name="enable">Collider를 활성화 할거냐?</param>
    /// <param name="trigger">트리거를 활성화 할거냐?</param>
    /// <param name="damaged">데미지 정보</param>
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

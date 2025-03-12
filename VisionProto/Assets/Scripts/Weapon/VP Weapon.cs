using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �� ���� ���۰� ���õ� ����
/// ���� ������ ó���� Collider ó�� -> like Dagger Collider
/// �ܰ˰� VP Weapon�� 
/// </summary>
public class VPWeapon : MonoBehaviour, IListener
{
    public GameObject vpWeapon;
    
    // gunContainer�� Ȯ������ ?

    // ������ ������
    public int scratchDamage = 30;

    // ������ �ݰ� ����
    public int scratchRange = 240;

    // ������ �ӵ�
    public float scratchSpeed = 2;

    private float startScratchDegree;
    private float endScratchDegree;

    // ��Ŭ�� ��Ŭ�� ������ ������ �ٸ��ϱ�
    private bool isRightScratch;
    private bool isLeftScratch;
    private bool isNextAttack;

    private float totalTime;

    // ��Ŭ�� ��Ŭ�� ���� 240���� �����ϴ� �Ŵ�. 
    // �ִϸ��̼Ǹ� �޶��� �� �� ����.
    private bool isVPState;

    private bool isPause;

    private AnimationInformation VPinfo;
    private GameObject effectContanier;
    private VPWeaponEffect vpWeaponEffect;

    private PlayerStateMachine player;
    private AnimatorStateInfo stateInfo;

    private bool isLeftAttack;
    private bool isRightAttack;

    private TrailRenderer leftTrailRenderer;
    private TrailRenderer rightTrailRenderer;

    private bool isTutorial;
    void Start()
    {
        totalTime = 0f;
        startScratchDegree = -scratchRange * 0.5f;
        endScratchDegree = scratchRange * 0.5f;
        isNextAttack = false;
        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        EventManager.Instance.AddEvent(EventType.Tutorial, OnEvent);

        effectContanier = GameObject.Find("VP Effect Container");
        player = GameObject.Find("Player").GetComponent<PlayerStateMachine>();

        leftTrailRenderer = GameObject.Find("Left Hand Trail").GetComponent<TrailRenderer>();
        rightTrailRenderer = GameObject.Find("Right Hand Trail").GetComponent<TrailRenderer>();

        if(leftTrailRenderer == null)
            Debug.Log("None Left Hand Trail");

        if(rightTrailRenderer == null)
            Debug.Log("None Right Hand Trail");

        if (effectContanier != null)
            vpWeaponEffect = effectContanier.GetComponent<VPWeaponEffect>();
        else
            Debug.Log("None VP Effect Container");

        if (vpWeaponEffect == null)
            Debug.Log("None VP Weapon Effect");

        leftTrailRenderer.enabled = false;
        rightTrailRenderer.enabled = false;
    }

    void Update()
    {
        if (isPause || isTutorial)
            return;

        VPAnimation();
        VPWeaponController();
        ActionVPWeapon();
        DelayVPWeapon();


        /// VP Weapon ����
        // VP Weapon ������ ���� 
        // 1. Auto pickup�� ������. 
        // 2. ������ �ִ� ����� ��� ���� ? ��ȹ�� ������ �� ���� �װ� ������ ����. -> �׳� �ٴڿ� �����°��ϵ�
        // 3. Dagger �ʵ� �׷� VP State���� �˾ƾ� �ϳ�. Dagger�� Ȱ��ȭ �Ǹ� �� �Ǵϱ�
        // 4. ������ �����ϸ� �� ��
        // �ٵ� ���� ������ �� �� �� ���� �����ϸ� ������ ���� �� ������? Collider�� ó���ϴϱ�
        // �׸�ŭ map ũ�� ��� ������ Map Size�� �ø��ٰ� �����ϱ� -> ���� �װ� ��ȹ���� �ֳ�

        /// VP Weapon�� ���� ��Ȳ
        // �׷��� Collider �κп��� Player ���� �Ÿ��� Ȯ���ϰ� �������� ������ �Ѵٴ� �ǵ�?
        // Screen ���� or Player - Enemy ���� ray�� ���� ������ ���� �ƴϸ� ���� ����
        // �Ѵ� �غ��� �������� ����.
        // �տ����� ��¦�� �������� ������ �� ������ �����Ѵ�.
        // �ڿ����� �� �������� ������ ���� �ȵɰ� ������

        /// ó��
        // VWC : VP Weapon Collider , VW : WP Weapon
        // �׷��� VWC Collider -> VWC NPC ���� -> VW ���� �� Ray üũ (�Լ��� ����ؼ� VMC���� ����) -> VWC ������ ó��
        // �ٵ� ������ ó���� ���ʿ��� ����ϰ� ������? Collider �ʿ��� ����ϰ� �;� 
        // �ִϸ��̼��̳� ������ �̰� ������ ������ ���ʿ� ó������.
    }

    private void VPAnimation()
    {
        // Player�� Animator�� �����;� �� �� ������ ��� �����ñ�?
        // Animator�� GetCurrentAnimatorStateInfo�� �����ͼ�
        if (!isLeftAttack && !isRightAttack)
            return;

        stateInfo = player.animator.animator[1].GetCurrentAnimatorStateInfo(0);

        if(stateInfo.normalizedTime >= 0.5f)
        {
            if(isLeftAttack)
            {
                vpWeaponEffect.isLeftAttack = true;
                vpWeaponEffect.leftEffect.transform.localScale = vpWeaponEffect.initPosition;
                isLeftAttack = false;
            }

            if(isRightAttack)
            {
                vpWeaponEffect.isRightAttack = true;
                vpWeaponEffect.rightEffect.transform.localScale = vpWeaponEffect.initPosition;
                isRightAttack = false;
            }
        }
    }


    private void VPWeaponController()
    {
        if (isRightScratch || isLeftScratch)
            return;

        if (Input.GetMouseButtonDown(0) && !isNextAttack)
        {
            isNextAttack = true;
            isLeftScratch = true;

            // VP ���� ������ 
            VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(true, true, scratchDamage);
            EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);

            // VP ���� �ִϸ��̼�
            VPinfo.stateName = "VPLAttack";
            VPinfo.layer = -1;
            VPinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.VPAnimator, VPinfo);
            SoundManager.Instance.PlayEffectSound(SFX.VP_Attack1, this.transform);

            CameraInfomation cameraInfomation = new CameraInfomation();
            cameraInfomation.setting = CameraSetting.VPLeftAttack;
            cameraInfomation.amplitude = -10f;
            cameraInfomation.frequency = 1f;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

            isLeftAttack = true;
            leftTrailRenderer.enabled = true;

            // VP ���� Collider ������ �ѱ���.
            this.transform.localRotation = Quaternion.Euler(0f, startScratchDegree, 0f);
            return;
        }

        if (Input.GetMouseButtonDown(0) && isNextAttack)
        {
            isNextAttack = false;
            isRightScratch = true;

            // VP ���� ������ 
            VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(true, true, scratchDamage);
            EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);

            // VP ���� �ִϸ��̼�
            VPinfo.stateName = "VPRAttack";
            VPinfo.layer = -1;
            VPinfo.normalizedTime = 0f;
            EventManager.Instance.NotifyEvent(EventType.VPAnimator, VPinfo);
            SoundManager.Instance.PlayEffectSound(SFX.VP_Attack2, this.transform);

            CameraInfomation cameraInfomation = new CameraInfomation();
            cameraInfomation.setting = CameraSetting.VPLeftAttack;
            cameraInfomation.amplitude = 10f;
            cameraInfomation.frequency = 1f;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);

            isRightAttack = true;
            rightTrailRenderer.enabled = true;

            this.transform.localRotation = Quaternion.Euler(0f, endScratchDegree, 0f);
            return;
        }
    }

    private void ActionVPWeapon()
    {
        if(isRightScratch)
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotaion = Mathf.Lerp(startScratchDegree, endScratchDegree, time * scratchSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotaion, 0f);

       
            if(currentRotaion == endScratchDegree)
            {
                // ���� ����
                VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isRightScratch = false;
                rightTrailRenderer.enabled = false;
                totalTime = 0f;

                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.VPLeftAttack;
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }
        }
        else if(isLeftScratch)
        {
            float time = totalTime;
            time = Mathf.Clamp01(time);

            float currentRotaion = Mathf.Lerp(endScratchDegree, startScratchDegree, time * scratchSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotaion, 0f);

            if(currentRotaion == startScratchDegree)
            {
                // ���� ���� 
                VPWeaponInformation vpWeaponInformation = SettingVPWeaponInformation(false, true, default);
                EventManager.Instance.NotifyEvent(EventType.VPWeaponInformation, vpWeaponInformation);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                isLeftScratch = false;
                leftTrailRenderer.enabled = false;
                totalTime = 0f;

                CameraInfomation cameraInfomation = new CameraInfomation();
                cameraInfomation.setting = CameraSetting.VPLeftAttack;
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }
        }
    }

    private void DelayVPWeapon()
    {
        if (isRightScratch)
            totalTime += Time.deltaTime;
        else if(isLeftScratch)
            totalTime += Time.deltaTime;
    }


    private VPWeaponInformation SettingVPWeaponInformation(bool enable, bool trigger, int damaged)
    {
        VPWeaponInformation vpWeaponInformation = new VPWeaponInformation();
        vpWeaponInformation.isTrueTrigger = trigger;
        vpWeaponInformation.isEnableCollider = enable;
        vpWeaponInformation.damaged = damaged;

        return vpWeaponInformation;
    }

    /// VP ���� ó�� ���
    // VP State ��������? Ȯ���Ϸ��� Player state���� �޾ƿ��� ���� �� ����.
    // Event manager�� �����ϸ� �ǰڴ� 1:1 �̴ϱ�
    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.isPause:
                isPause = (bool)param;
                break;
            case EventType.Tutorial:
                TutorialWeapon weapon = (TutorialWeapon)param;
                isTutorial = weapon.isVPWeapon;
                break;
        }
    }


}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


[DefaultExecutionOrder(-1)]
public class UIHP : MonoBehaviour, IListener
{
    public Image imageHP;
    public TMP_Text textHP;

    public float HPSpeed = 0.1f;
    public float HPSubSpeed = 10f;

    private float modelHP;   // model(실제 data) Player 체력
    private float viewHP;    // view(UI) Player 체력 

    private bool isChange;

    private bool isInjury;      // 부상
    private bool isRecovery;    // 회복

    private bool isInvincible;  // 무적 상태

    // 다쳤을 때 Camera_Shake
    [SerializeField]
    private float normal_Injury_Amplitude = 3.0f;
    [SerializeField]
    private float normal_Injury_Frequency = 1.0f;

    [SerializeField]
    private float vp_Injury_Amplitude = 5.0f;
    [SerializeField]
    private float vp_Injury_Frequency = 1.0f;

    private bool testDamageOn;
    private bool testBloodOn;
    private bool setValue;
    private bool testWaveHP;

    // 데미지를 입었을 때의 속도
    [SerializeField]
    private float damageDOFSpeed = 2.0f;      // 카메라 일렁이는 속도
    [SerializeField]
    private float damageVignetteSpeed = 1.0f; // 주위 화면 일렁이는 속도

    // 계속 일렁이는 속도
    [SerializeField]
    private float waveDOFSpeed = 2.0f;      // 카메라 일렁이는 속도
    [SerializeField]
    private float waveVignetteSpeed = 1.0f; // 주위 화면 일렁이는 속도

    [SerializeField]
    private float minDOF = 150;
    [SerializeField]
    private float minVignette = 0.7f;

    [SerializeField]
    private float waveHP = 30f;

    // 여기서 Post Processing 
    private DepthOfField depthOfField;
    private Vignette vignette;

    private bool isVPState;
    private float totalTime;
    private bool once;
    private bool isDead;

    private CameraInfomation cameraInfomation;

    private bool isLowHP;

    // Start is called before the first frame update
    void Start()
    {
        // 초기 Player HP는 100이지?
        modelHP = 100;
        viewHP = 100;
        EventManager.Instance.AddEvent(EventType.PlayerHPUI, OnEvent);
        totalTime = 0f;

        Initalize();
        isInvincible = false;
        setValue = true;
        testWaveHP = true;
        once = false;
        isDead = false;

        cameraInfomation = new CameraInfomation();
        cameraInfomation.setting = CameraSetting.Handheld_Normal_Mild;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            isInvincible = true;
        }

        SetDOFAndVignette();

        // 일렁거리는 효과 중에 피 회복이 된건가?
        if (isLowHP && !testDamageOn)
        {
            if (testWaveHP)
            {
                depthOfField.focalLength.value += waveDOFSpeed;
                vignette.intensity.value += Time.deltaTime * waveVignetteSpeed;

                // 최대치면 줄어든다.
                if (depthOfField.focalLength.value == 300 && vignette.intensity.value == 1)
                {
                    testWaveHP = false;
                }
            }
            else
            {
                depthOfField.focalLength.value -= waveDOFSpeed;
                vignette.intensity.value -= Time.deltaTime * waveVignetteSpeed;

                if (depthOfField.focalLength.value <= minDOF && vignette.intensity.value <= minVignette)
                {
                    testWaveHP = true;
                    isLowHP = false;
                }
            }
        }

        // Update를 많이 안 거치는게 좋으니까 change 할 때만 들어오도록 하자.
        if (!isChange)
            return;

        CurrentUIHP();
        CameraShaking();
    }

    private void Initalize()
    {
        Volume globalVolume = FindObjectOfType<Volume>();

        globalVolume.profile.TryGet(out depthOfField);
        globalVolume.profile.TryGet(out vignette);

        if (depthOfField == null)
            Debug.Log("None Global Volume -> Depth Of Field");

        if (vignette == null)
            Debug.Log("None Global Volume -> Vignette");

        imageHP.fillAmount = 1.0f;
        imageHP.type = Image.Type.Filled;
        imageHP.fillMethod = Image.FillMethod.Horizontal;
        imageHP.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    private void CameraShaking()
    {
        if (isDead)
            return;

        if (!once)
            return;

        totalTime += Time.deltaTime;

        // VP 상태이거나 아닐때

        if (isVPState)
        {
            cameraInfomation.amplitude = vp_Injury_Amplitude;
            cameraInfomation.frequency = vp_Injury_Frequency;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
        }
        else
        {
            cameraInfomation.amplitude = normal_Injury_Amplitude;
            cameraInfomation.frequency = normal_Injury_Frequency;
            EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
        }

        if (isVPState)
        {
            if (totalTime > vp_Injury_Frequency)
            {
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
                once = false;
                totalTime = 0f;
            }
        }
        else
        {
            if (totalTime > normal_Injury_Frequency)
            {
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
                once = false;
                totalTime = 0f;
            }
        }

    }

    private void CurrentUIHP()
    {
        // view HP
        if (modelHP != viewHP)
        {
            float currentHP = viewHP - modelHP;

            if (currentHP > 0)
            {
                isInjury = true;
                // 여기서 Global Volume 실행 Vignette랑 Depth of Field 설정
                testDamageOn = true;
                SoundManager.Instance.PlayEffectSound(SFX.Player_Hurt, this.transform);
            }
            else
            {
                isRecovery = true;
            }
        }

        if (isRecovery)
        {
            viewHP += Time.deltaTime * HPSpeed;
            imageHP.fillAmount += HPSpeed * Time.deltaTime;
            //textHP.text = (Mathf.Ceil(imageHP.fillAmount * 100)).ToString();

            if ((Mathf.Ceil(imageHP.fillAmount * 100)) >= modelHP)
            {
                if (viewHP > 100)
                {
                    viewHP = 100f;
                    modelHP = 100f;
                }
                imageHP.fillAmount = DowntotheSecondDecimalPlace(modelHP * 0.01f);
                viewHP = modelHP;
                //textHP.text = modelHP.ToString();
                isRecovery = false;
                isChange = false;
                SoundManager.Instance.PlayEffectSound(SFX.Heal, this.transform);

            }
        }

        if (isInjury)
        {
            viewHP -= Time.deltaTime * HPSpeed;
            imageHP.fillAmount -= HPSpeed * Time.deltaTime;
            //textHP.text = (Mathf.Ceil(imageHP.fillAmount * 100)).ToString();
            once = true;

            if (modelHP <= 0)
            {
                isDead = true;
                cameraInfomation.amplitude = 0f;
                cameraInfomation.frequency = 0f;
                EventManager.Instance.NotifyEvent(EventType.CameraShake, cameraInfomation);
            }

            if ((Mathf.Ceil(imageHP.fillAmount * 100)) <= modelHP)
            {
                if (viewHP < 0)
                {
                    viewHP = 0;
                    modelHP = 0;
                }


                imageHP.fillAmount = DowntotheSecondDecimalPlace(modelHP * 0.01f);
                viewHP = modelHP;
                //textHP.text = modelHP.ToString();
                isInjury = false;
                isChange = false;
            }

            if (!isInvincible)
            {
                /// 일단 체력 0되면 You Died UI가 안 뜨게 설정해놓자.
                if (imageHP.fillAmount <= 0.0f)
                {
                    viewHP = 0;
                    modelHP = 0;

                    Cursor.lockState = CursorLockMode.None;
                    SoundManager.Instance.AllSoundRemove();
                    SoundManager.Instance.PlayEffectSound(SFX.Player_Death, this.transform);
                    EventManager.Instance.NotifyEvent(EventType.PlayerDead, true);
                    EventManager.Instance.NotifyEvent(EventType.isPause, true);
                    EventManager.Instance.NotifyEvent(EventType.DeadFadeInOut, true);
                }
            }
        }

        // 일렁 거리는 효과다. 일정 피 이하면 실행되는데
        if (viewHP <= waveHP && setValue)
            testBloodOn = true;
        else
            testBloodOn = false;
    }

    // HP

    private void ChangeHP(int hp)
    {
        isChange = true;
        modelHP = hp;
    }

    // 이 함수는 Depth of Field랑 Vignette를 조절한다.
    private void SetDOFAndVignette()
    {
        // depthOfField
        // vignette

        // 조건은 피가 waveHP 이하, 데미지를 입지 않았을 때 실행
        if (testBloodOn && !testDamageOn)
        {
            isLowHP = true;
        }

        if (!testDamageOn)
            return;

        if (setValue)
        {
            // 이건 맞았을 떄
            depthOfField.focalLength.value += damageDOFSpeed;
            vignette.intensity.value += Time.deltaTime * damageVignetteSpeed;

            if (depthOfField.focalLength.value == 300 && vignette.intensity.value == 1)
            {
                setValue = false;
            }
        }
        else
        {
            depthOfField.focalLength.value -= damageDOFSpeed;
            vignette.intensity.value -= Time.deltaTime * damageVignetteSpeed;

            if (depthOfField.focalLength.value == 1 && vignette.intensity.value == 0)
            {
                setValue = true;
                testDamageOn = false;
            }
        }
    }

    private float DowntotheSecondDecimalPlace(float value)
    {
        value *= 100;

        value = Mathf.Ceil(value);

        value *= 0.01f;

        return value;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerHPUI:
                {
                    ChangeHP((int)param);
                }
                break;
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                }
                break;
        }
    }

}

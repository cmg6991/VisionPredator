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

    private float modelHP;   // model(���� data) Player ü��
    private float viewHP;    // view(UI) Player ü�� 

    private bool isChange;

    private bool isInjury;      // �λ�
    private bool isRecovery;    // ȸ��

    private bool isInvincible;  // ���� ����

    // ������ �� Camera_Shake
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

    // �������� �Ծ��� ���� �ӵ�
    [SerializeField]
    private float damageDOFSpeed = 2.0f;      // ī�޶� �Ϸ��̴� �ӵ�
    [SerializeField]
    private float damageVignetteSpeed = 1.0f; // ���� ȭ�� �Ϸ��̴� �ӵ�

    // ��� �Ϸ��̴� �ӵ�
    [SerializeField]
    private float waveDOFSpeed = 2.0f;      // ī�޶� �Ϸ��̴� �ӵ�
    [SerializeField]
    private float waveVignetteSpeed = 1.0f; // ���� ȭ�� �Ϸ��̴� �ӵ�

    [SerializeField]
    private float minDOF = 150;
    [SerializeField]
    private float minVignette = 0.7f;

    [SerializeField]
    private float waveHP = 30f;

    // ���⼭ Post Processing 
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
        // �ʱ� Player HP�� 100����?
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

        // �Ϸ��Ÿ��� ȿ�� �߿� �� ȸ���� �Ȱǰ�?
        if (isLowHP && !testDamageOn)
        {
            if (testWaveHP)
            {
                depthOfField.focalLength.value += waveDOFSpeed;
                vignette.intensity.value += Time.deltaTime * waveVignetteSpeed;

                // �ִ�ġ�� �پ���.
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

        // Update�� ���� �� ��ġ�°� �����ϱ� change �� ���� �������� ����.
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

        // VP �����̰ų� �ƴҶ�

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
                // ���⼭ Global Volume ���� Vignette�� Depth of Field ����
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
                /// �ϴ� ü�� 0�Ǹ� You Died UI�� �� �߰� �����س���.
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

        // �Ϸ� �Ÿ��� ȿ����. ���� �� ���ϸ� ����Ǵµ�
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

    // �� �Լ��� Depth of Field�� Vignette�� �����Ѵ�.
    private void SetDOFAndVignette()
    {
        // depthOfField
        // vignette

        // ������ �ǰ� waveHP ����, �������� ���� �ʾ��� �� ����
        if (testBloodOn && !testDamageOn)
        {
            isLowHP = true;
        }

        if (!testDamageOn)
            return;

        if (setValue)
        {
            // �̰� �¾��� ��
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

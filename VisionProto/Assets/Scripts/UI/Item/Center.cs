using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Center : MonoBehaviour, IListener
{
    private bool isPause;

    private GameObject settingUIObject;
    public GameObject uiSetting;
    public GameObject mainButton;
    public GameObject retryButton;
    public GameObject skipButton;
    public GameObject tutorialSettingButton;
    public GameObject optionButton;
    private GameObject settingUIObject2;

    private bool isDead;
    private bool isClear;
    private bool isFindObject;

    private CinemachineVirtualCamera playerCamera;
    private CinemachinePOV cameraPOV;

    public float direction = 1f;

    public float deadYPosition = 1f;
    public float deadXPosition = 1f;
    public float deadSpeed = 1f;

    private Vector3 forwardBullet;
    private Vector3 hitPosition;
    private float bulletDirectionX;
    private float bulletDirectionY;

    private float hitPositionX;
    private float hitPositionY;
    private float hitPositionZ;

    public bool isTutorial;

    private void Awake()
    {
        CoroutineRunner[] existManagers = FindObjectsOfType<CoroutineRunner>();

        foreach (var runner in existManagers)
        {
            if (runner != null && runner.gameObject != null)
            {
                Destroy(runner.gameObject);
            }
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        isPause = false;


        uiSetting = UIManager.Instance.UISetting;
        mainButton = UIManager.Instance.mainButton;
        retryButton = UIManager.Instance.retryButton;
        skipButton = UIManager.Instance.skipButton;
        tutorialSettingButton = UIManager.Instance.tutorialSettingButton;
        optionButton = UIManager.Instance.optionButton;


        if (uiSetting != null)
            uiSetting.SetActive(false);

        EventManager.Instance.AddEvent(EventType.isPause, OnEvent);
        EventManager.Instance.AddEvent(EventType.HitBulletRotation, OnEvent);
        EventManager.Instance.AddEvent(EventType.PlayerDead, OnEvent);
        EventManager.Instance.AddEvent(EventType.Clear, OnEvent);
        EventManager.Instance.AddEvent(EventType.DeadFadeInOut, OnEvent);
        SoundManager.Instance.PlayBGMSound(BGM.Ambience);

        if(isTutorial)
        {
            TutorialManager.Instance.Initialized();
        }
    }

    private void Update()
    {
        if (isDead)
        {
            if (!isFindObject)
            {
                // Virtual Camera를 찾고 
                GameObject camera = GameObject.Find("Virtual Camera");
                camera.TryGetComponent<CinemachineVirtualCamera>(out playerCamera);
                EventManager.Instance.RemoveEvent(EventType.HitBulletRotation, OnEvent);
                if (playerCamera == null)
                    Debug.Log("None playerCamera");
                else
                {
                    cameraPOV = playerCamera.GetCinemachineComponent<CinemachinePOV>();

                    VPRenderFeature renderFeature;
                    camera.transform.parent.TryGetComponent<VPRenderFeature>(out renderFeature);
                    // 이거를 해도 Time Scale이 0이여서 실행이 안된다. 그래서 죽기 전까지 가능
                    Time.timeScale = 1.0f;
                    renderFeature.FadeInFadeOut();
                    isFindObject = true;
                }
            }

            CalculationDirection();
        }

        // Scene 재시작
//         if (Input.GetKeyDown(KeyCode.F7) && !isDead)
//         {
//             Scene currentScene = SceneManager.GetActiveScene();
//             SceneManager.LoadScene(currentScene.name);
// 
//             EventManager.Instance.RemoveAllEvent();
//             Time.timeScale = 1;
//         }

        if (Input.GetKeyDown(KeyCode.Escape) && !isPause && !isDead)
        {
            // ui 설정창이 나와야겠지?
            EventManager.Instance.NotifyEvent(EventType.isPause, true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPause && !isDead)
        {
            BtnExit();
        }
    }

    private void DeadCameraAction(float _x, float _y)
    {
        cameraPOV.m_VerticalAxis.Value -= _y * deadSpeed;
        cameraPOV.m_HorizontalAxis.Value -= _x * deadSpeed;

        if (cameraPOV.m_VerticalAxis.Value <= -90)
        {
            cameraPOV.m_VerticalAxis.Value = -90;
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.isPause:
                {
                    isPause = (bool)param;

                    if (isClear)
                        return;

                    if (isPause)
                    {
                        Time.timeScale = 0f;
                        Cursor.lockState = CursorLockMode.None;

                        if (!isDead)
                        {
                            if (uiSetting)
                                uiSetting.SetActive(true);
                            if(mainButton)
                                mainButton.SetActive(true);
                            if(retryButton)
                                retryButton.SetActive(true);
                            if(isTutorial)
                            {
                                if(skipButton)
                                    skipButton.SetActive(true);
                                if (tutorialSettingButton) 
                                    tutorialSettingButton.SetActive(true);
                                if(optionButton)
                                    optionButton.SetActive(false);
                            }
                            else if(!isTutorial)
                            {
                                if(skipButton)
                                    skipButton.SetActive(false);
                                if(tutorialSettingButton)
                                   tutorialSettingButton.SetActive(false);
                                if(optionButton)
                                    optionButton.SetActive(true);
                            }    
                        }
                        else
                        {
                            if (uiSetting)
                                uiSetting.SetActive(false);
                            if (mainButton)
                                mainButton.SetActive(false);
                            if (retryButton)
                                retryButton.SetActive(false);
                            if (isTutorial)
                            {
                                if (skipButton)
                                    skipButton.SetActive(false);
                                if (tutorialSettingButton)
                                    tutorialSettingButton.SetActive(false);
                                if (optionButton)
                                    optionButton.SetActive(true);
                            }
                            else if (!isTutorial)
                            {
                                if (skipButton)
                                    skipButton.SetActive(true);
                                if (tutorialSettingButton)
                                    tutorialSettingButton.SetActive(true);
                                if (optionButton)
                                    optionButton.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        Time.timeScale = 1f;
                        Cursor.lockState = CursorLockMode.Locked;

                        if (uiSetting)
                        {
                            if (uiSetting.activeSelf)
                                uiSetting.SetActive(false);
                        }
                    }
                }
                break;
            case EventType.PlayerDead:
                {
                    isDead = (bool)param;
                }
                break;
            case EventType.DeadFadeInOut:
                {
                    Time.timeScale = 1f;
                }
                break;
            case EventType.HitBulletRotation:
                {
                    HitBulletInformation bulletInformation;
                    bulletInformation = (HitBulletInformation)param;

                    hitPosition = bulletInformation.hitPosition;
                    forwardBullet = bulletInformation.bulletForward;

                    /// 맞은 위치에서 시작 

                    // Bullet Direction의 속도만큼 카메라 회전
                    hitPositionX = hitPosition.x;
                    // Bullet Direction                    
                    hitPositionY = hitPosition.y;

                    Camera camera = Camera.main;
                    Vector3 bulletDirection = camera.transform.InverseTransformDirection(forwardBullet);

                    bulletDirectionX = bulletDirection.x;
                    bulletDirectionY = bulletDirection.y;

                }
                break;
            case EventType.Clear:
                isClear = (bool)param;
                break;
        }
    }

    // 어떤 방향으로 죽을지 알아야 한다.
    private void CalculationDirection()
    {
        if (hitPosition.z > 0)
        {
            deadXPosition = hitPosition.x;
            deadYPosition = -bulletDirectionY;
        }
        else
        {
            deadXPosition = -hitPosition.x;
            deadYPosition = -bulletDirectionY;
        }

        DeadCameraAction(deadXPosition, deadYPosition);
    }

    public void BtnExit()
    {
        EventManager.Instance.NotifyEvent(EventType.isPause, false);
    }
}

public struct HitBulletInformation
{
    public Vector3 hitPosition;
    public Vector3 bulletForward;
}
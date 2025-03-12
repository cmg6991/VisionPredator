using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum TutorialStage
{
    VPIdle,
    VPMovement,
    VPJump,
    VPSight,
    VPWeapon,
    VPDash,
    VPSlash,
    ChangeVP,
    Idle,
    Movement,
    Run,
    Jump,
    Sit,
    Slide,
    HandAttack,
    Draw,
    Shot,
    Throw,
    Interaction,
    Grappling,
    End
}

public class TutorialManager : Singleton<TutorialManager>
{
    public TutorialStage currentState;
    public TextMeshProUGUI tutorialText;
    public bool tutorialJump;
    public bool tutorialDash;
    public bool tutorialSlash;
    public bool tutorialChange;
    public bool tutorialRun;
    public bool tutorialHumanJump;
    public bool tutorialSit;
    public bool tutorialSlide;
    public bool tutorialInteraction;
    public bool tutorialGrappling;

    public bool isBulletInfinite;
    public bool isThrow;
    public bool isPickUp;

    bool isTransition;
    Color origin;


    public void Initialized()
    {
        //tutorialText = GameObject.Find("Text(TMP)").GetComponent<TextMeshProUGUI>();
        currentState = TutorialStage.VPIdle;
        origin = tutorialText.color;

        TutorialText();
    }

    //코루틴
    private IEnumerator Wait3Seconds(TutorialStage stage, TutorialStage previousStage)
    {
        yield return new WaitForSeconds(1);
        currentState = stage;
        
        ChangeTextColor(origin);

        if (currentState == TutorialStage.VPSight || currentState == TutorialStage.ChangeVP || currentState == TutorialStage.Grappling)
        {
            TutorialText(); // 현재 상태에 맞는 튜토리얼 텍스트 업데이트
        }
        else
        {
            tutorialText.text = "다음 지역으로 이동";
            yield return new WaitForSeconds(0.5f); // 메시지를 보여주기 위해 1초 대기
            TutorialText(); // 현재 상태에 맞는 튜토리얼 텍스트 업데이트
        }

        isTransition = false;

        // 문 열렸을 때 실행
        TutorialDoorState openDoor;
        openDoor.stage = previousStage;
        openDoor.isOpen = true;
        EventManager.Instance.NotifyEvent(EventType.TutorialOpenDoor, openDoor);
    }

    public void NextState()
    {
        if (isTransition)
            return;
        isTransition = true;
        ChangeTextColor(Color.green);
        switch (currentState)
        {
            case TutorialStage.VPIdle:
                {
                    // 초록색으로 변경
                    StartCoroutine(Wait3Seconds(TutorialStage.VPMovement, TutorialStage.VPIdle));

                    //currentState = TutorialStage.VPMovement;
                    tutorialJump = true;
                    break;
                }
                
            case TutorialStage.VPMovement:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.VPJump, TutorialStage.VPMovement));
                    break;
                }
                
            case TutorialStage.VPJump:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.VPSight, TutorialStage.VPJump));
                    break;
                }
            case TutorialStage.VPSight:
                {
                    TutorialWeapon tutorialWeapon = new TutorialWeapon();
                    tutorialWeapon.isVPWeapon = false;
                    tutorialWeapon.isDagger = true;

                    EventManager.Instance.NotifyEvent(EventType.Tutorial, tutorialWeapon);
                    StartCoroutine(Wait3Seconds(TutorialStage.VPWeapon, TutorialStage.VPSight));
                    tutorialDash = true;
                    break;
                }
            case TutorialStage.VPWeapon:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.VPDash,TutorialStage.VPWeapon));
                    tutorialSlash = true;
                    break;
                }
            case TutorialStage.VPDash:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.VPSlash, TutorialStage.VPDash));
                    tutorialChange = true;
                    break;
                }
            case TutorialStage.VPSlash:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.ChangeVP, TutorialStage.VPSlash));
                    break;
                }
            case TutorialStage.ChangeVP:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Movement, TutorialStage.ChangeVP));
                    tutorialRun = true;
                    break;
                }
            case TutorialStage.Movement:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Run, TutorialStage.Movement));
                    tutorialHumanJump = true;
                    break;
                }
            case TutorialStage.Run:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Jump, TutorialStage.Run));
                    tutorialSit = true;
                    break;
                }
            case TutorialStage.Jump:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Sit, TutorialStage.Jump));
                    tutorialSlide = true;
                    break;
                }
            case TutorialStage.Sit:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Slide, TutorialStage.Sit));
                    TutorialWeapon tutorialWeapon = new TutorialWeapon();
                    tutorialWeapon.isVPWeapon = true;
                    tutorialWeapon.isDagger = false;

                    EventManager.Instance.NotifyEvent(EventType.Tutorial, tutorialWeapon);
                    tutorialInteraction = true;
                    break;
                }
            case TutorialStage.Slide:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.HandAttack, TutorialStage.Slide));
                    break;
                }
            case TutorialStage.HandAttack:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Draw, TutorialStage.HandAttack));
                    isBulletInfinite = true;
                    break;
                }
            case TutorialStage.Draw:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Shot, TutorialStage.Draw));
                    isBulletInfinite = false;
                    break;
                }
            case TutorialStage.Shot:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Throw, TutorialStage.Shot));
                    
                    break;
                }
            case TutorialStage.Throw:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Interaction, TutorialStage.Throw));
                    tutorialGrappling = true;
                    break;
                }
            case TutorialStage.Interaction:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.Grappling, TutorialStage.Interaction));
                    break;
                }
            case TutorialStage.Grappling:
                {
                    StartCoroutine(Wait3Seconds(TutorialStage.End, TutorialStage.Grappling));
                    break;
                }

        }
        //ChangeTextColor(origin);
        //TutorialText();
    }
    void TutorialText()
    {
        switch (currentState)
        {
            case TutorialStage.VPIdle:
                {
                    tutorialText.text = "WASD를 눌러 이동 / 마우스를 돌려 시야 확인";
                    break;
                }
            case TutorialStage.VPMovement:
                {
                    tutorialText.text = "SPACE를 눌러 점프";
                }
                break;
            case TutorialStage.VPJump:
                {
                    tutorialText.text = "적과 오브젝트는 특정 색상으로 표시, 적은 투시";
                }
                break;
            case TutorialStage.VPSight:
                {
                    tutorialText.text = "마우스 좌 클릭을 눌러 적을 처치 및 체력 회복";
                    break;
                }

            case TutorialStage.VPWeapon:
                {
                    tutorialText.text = "SHIFT 키를 눌러 적을 처치";
                    break;
                }
            case TutorialStage.VPDash:
                {
                    tutorialText.text = "점프 후, CRTL키를 눌러 내려찍기로 적을 처치";
                    break;
                }
            case TutorialStage.VPSlash:
                {
                    tutorialText.text = "R키를 눌러 인간 상태로 전환";
                    break;
                }
            case TutorialStage.ChangeVP:
                {
                    tutorialText.text = "WASD를 눌러 이동/마우스를 돌려 시야를 회전";
                    break;
                }
            case TutorialStage.Movement:
                {
                    tutorialText.text = "SHIFT키를 길게 눌러 달리기";
                    break;
                }
            case TutorialStage.Run:
                {
                    tutorialText.text = "SPACE를 눌러 점프";
                    break;
                }
            case TutorialStage.Jump:
                {
                    tutorialText.text = "CRTL키를 눌러 앉기";
                    break;
                }
            case TutorialStage.Sit:
                {
                    tutorialText.text = "이동 키 + CRTL 키를 눌러 슬라이드";
                    break;
                }
            case TutorialStage.Slide:
                {
                    tutorialText.text = "마우스 좌 클릭을 눌러 적을 처치";
                    break;
                }
            case TutorialStage.HandAttack:
                {
                    tutorialText.text = "F키를 눌러 무기를 픽업/특정 범위 무기를 자동 픽업";
                    break;
                }
            case TutorialStage.Draw:
                {
                    tutorialText.text = "마우스 좌 클릭을 눌러 무기 공격 및 적 처치";
                    break;
                }
            case TutorialStage.Shot:
                {
                    tutorialText.text = "공격 키를 눌러 총알 없는 무기 던지기";
                    break;
                }
            case TutorialStage.Throw:
                {
                    tutorialText.text = "F키를 눌러 상호작용";
                    break;
                }
            case TutorialStage.Interaction:
                {
                    tutorialText.text = "거리에 맞춰 CCTV를 조준하고 마우스 우 클릭을 눌러 와이어 사용";
                    break;
                }
            case TutorialStage.Grappling:
                {
                    tutorialText.text = "튜토리얼 종료";
                    StartCoroutine(ShowMessage());
                    break;
                }
        }
    }

    void ChangeTextColor(Color color)
    {
        tutorialText.color = color;
    }

    private IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(1);
        TutorialNextBossX();
    }

    public void TutorialNextBossX()
    {
        //SceneController.Instance.GameStart();

        //         UIManager.Instance.soundCanvas.SetActive(true);
        //         UIManager.Instance.playCanvas.SetActive(true);
        //         UIManager.Instance.UISetting.SetActive(true);

        UIManager.Instance.UISetting.SetActive(false);
        GameObject canvas = GameObject.Find("Canvas");

        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            EventManager.Instance.RemoveAllEvent();
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
            StartCoroutine(EndOfFrameRoutine());
        }
        else Debug.Log("None Prefab");

        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(28.34f, -47.5f, -23f);
        playerInfo.rotation = Quaternion.identity;

        DataManager.Instance.SaveData(playerInfo);
    }

    IEnumerator EndOfFrameRoutine()
    {
        // 현재 프레임이 끝날 때까지 대기
        yield return new WaitForEndOfFrame();


        DataManager.Instance.isEasyMode = false;
        DataManager.Instance.isNormalMode = false;
        DataManager.Instance.isModeSelect = false;
        DataManager.Instance.isHardMode = true;

        // 여기에서 코드를 실행
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "LevelDesign");   
        //EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Tutorial");
        //EventManager.Instance.NotifyEvent(EventType.LoadingScene, "BossX");   
    }
}

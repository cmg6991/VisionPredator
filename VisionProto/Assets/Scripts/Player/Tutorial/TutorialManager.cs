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

    //�ڷ�ƾ
    private IEnumerator Wait3Seconds(TutorialStage stage, TutorialStage previousStage)
    {
        yield return new WaitForSeconds(1);
        currentState = stage;
        
        ChangeTextColor(origin);

        if (currentState == TutorialStage.VPSight || currentState == TutorialStage.ChangeVP || currentState == TutorialStage.Grappling)
        {
            TutorialText(); // ���� ���¿� �´� Ʃ�丮�� �ؽ�Ʈ ������Ʈ
        }
        else
        {
            tutorialText.text = "���� �������� �̵�";
            yield return new WaitForSeconds(0.5f); // �޽����� �����ֱ� ���� 1�� ���
            TutorialText(); // ���� ���¿� �´� Ʃ�丮�� �ؽ�Ʈ ������Ʈ
        }

        isTransition = false;

        // �� ������ �� ����
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
                    // �ʷϻ����� ����
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
                    tutorialText.text = "WASD�� ���� �̵� / ���콺�� ���� �þ� Ȯ��";
                    break;
                }
            case TutorialStage.VPMovement:
                {
                    tutorialText.text = "SPACE�� ���� ����";
                }
                break;
            case TutorialStage.VPJump:
                {
                    tutorialText.text = "���� ������Ʈ�� Ư�� �������� ǥ��, ���� ����";
                }
                break;
            case TutorialStage.VPSight:
                {
                    tutorialText.text = "���콺 �� Ŭ���� ���� ���� óġ �� ü�� ȸ��";
                    break;
                }

            case TutorialStage.VPWeapon:
                {
                    tutorialText.text = "SHIFT Ű�� ���� ���� óġ";
                    break;
                }
            case TutorialStage.VPDash:
                {
                    tutorialText.text = "���� ��, CRTLŰ�� ���� �������� ���� óġ";
                    break;
                }
            case TutorialStage.VPSlash:
                {
                    tutorialText.text = "RŰ�� ���� �ΰ� ���·� ��ȯ";
                    break;
                }
            case TutorialStage.ChangeVP:
                {
                    tutorialText.text = "WASD�� ���� �̵�/���콺�� ���� �þ߸� ȸ��";
                    break;
                }
            case TutorialStage.Movement:
                {
                    tutorialText.text = "SHIFTŰ�� ��� ���� �޸���";
                    break;
                }
            case TutorialStage.Run:
                {
                    tutorialText.text = "SPACE�� ���� ����";
                    break;
                }
            case TutorialStage.Jump:
                {
                    tutorialText.text = "CRTLŰ�� ���� �ɱ�";
                    break;
                }
            case TutorialStage.Sit:
                {
                    tutorialText.text = "�̵� Ű + CRTL Ű�� ���� �����̵�";
                    break;
                }
            case TutorialStage.Slide:
                {
                    tutorialText.text = "���콺 �� Ŭ���� ���� ���� óġ";
                    break;
                }
            case TutorialStage.HandAttack:
                {
                    tutorialText.text = "FŰ�� ���� ���⸦ �Ⱦ�/Ư�� ���� ���⸦ �ڵ� �Ⱦ�";
                    break;
                }
            case TutorialStage.Draw:
                {
                    tutorialText.text = "���콺 �� Ŭ���� ���� ���� ���� �� �� óġ";
                    break;
                }
            case TutorialStage.Shot:
                {
                    tutorialText.text = "���� Ű�� ���� �Ѿ� ���� ���� ������";
                    break;
                }
            case TutorialStage.Throw:
                {
                    tutorialText.text = "FŰ�� ���� ��ȣ�ۿ�";
                    break;
                }
            case TutorialStage.Interaction:
                {
                    tutorialText.text = "�Ÿ��� ���� CCTV�� �����ϰ� ���콺 �� Ŭ���� ���� ���̾� ���";
                    break;
                }
            case TutorialStage.Grappling:
                {
                    tutorialText.text = "Ʃ�丮�� ����";
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
            // ���� �ȵ� ������ ���� �ؾ� ��.
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
        // ���� �������� ���� ������ ���
        yield return new WaitForEndOfFrame();


        DataManager.Instance.isEasyMode = false;
        DataManager.Instance.isNormalMode = false;
        DataManager.Instance.isModeSelect = false;
        DataManager.Instance.isHardMode = true;

        // ���⿡�� �ڵ带 ����
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, "LevelDesign");   
        //EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Tutorial");
        //EventManager.Instance.NotifyEvent(EventType.LoadingScene, "BossX");   
    }
}

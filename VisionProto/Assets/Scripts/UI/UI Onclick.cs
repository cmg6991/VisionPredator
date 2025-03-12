using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Onclick�� �ְ� ����ϸ� �ȴ�.
/// </summary>
public class UIOnclick : MonoBehaviour
{
    /// <summary>
    /// Title���� ���ʷ� ������ ���۵� �� ����ϴ� �Լ�
    /// </summary>

    public void GameStart()
    {
        //SceneController.Instance.GameStart();

        //         UIManager.Instance.soundCanvas.SetActive(true);
        //         UIManager.Instance.playCanvas.SetActive(true);
        //         UIManager.Instance.UISetting.SetActive(true);

        //UIManager.Instance.UISetting.SetActive(false);

        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            EventManager.Instance.RemoveAllEvent();
            // ���� �ȵ� ������ ���� �ؾ� ��.
            StartCoroutine(EndOfFrameRoutine(0));
        }
        else Debug.Log("None Prefab");

        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(28.34f, -47.5f, -23f);
        playerInfo.rotation = Quaternion.identity;


        DataManager.Instance.SaveData(playerInfo);
        DataManager.Instance.currentSaveMap = AreaMap.None;
    }

    public void GameSkip()
    {
        //SceneController.Instance.GameStart();

        //         UIManager.Instance.soundCanvas.SetActive(true);
        //         UIManager.Instance.playCanvas.SetActive(true);
        //         UIManager.Instance.UISetting.SetActive(true);

        //UIManager.Instance.UISetting.SetActive(false);

        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            EventManager.Instance.RemoveAllEvent();
            // ���� �ȵ� ������ ���� �ؾ� ��.
            StartCoroutine(EndOfFrameRoutine(1));
        }
        else Debug.Log("None Prefab");

        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(28.34f, -47.5f, -23f);
        playerInfo.rotation = Quaternion.identity;

        DataManager.Instance.SaveData(playerInfo);
        DataManager.Instance.currentSaveMap = AreaMap.None;
    }

    /// <summary>
    /// ������ �����ϰ� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void GameExit()
    {
       SceneController.Instance.GameExit(); 
    }

    public void GameOption()
    {
        // ó������ ������ ���� ������ ���
        UIManager.Instance.UISetting.SetActive(true);

        UIManager.Instance.skipButton.SetActive(false);
        UIManager.Instance.tutorialSettingButton.SetActive(false);
        UIManager.Instance.mainButton.SetActive(false);
        UIManager.Instance.retryButton.SetActive(false);
        UIManager.Instance.optionButton.SetActive(true);

    }

    public void OptionExit()
    {
        UIManager.Instance.UISetting.SetActive(false);
        //UIManager.Instance.soundCanvas.SetActive(false);
        //UIManager.Instance.playCanvas.SetActive(false);

        EventManager.Instance.NotifyEvent(EventType.isPause, false);
    }

    /// <summary>
    /// Setting���� ���� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void SettingUI()
    {
        // Canvas�� �����´�.
        GameObject canvas = GameObject.Find("Canvas");
        
        // SettingUI Prefab�� �����ؼ� �ҷ�����.
        GameObject settingPrefab = Resources.Load<GameObject>("UI/Setting Canvas");

        if (settingPrefab != null)
        {
            Instantiate(settingPrefab, canvas.transform);
        }
        else Debug.Log("None Prefab");
    }

    /// <summary>
    /// Setting�� �����ϰ� ���� �� ����ϴ� �Լ�
    /// </summary>
    public void SettingExit()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1.0f;
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Audio ����
    /// </summary>
    //public void AudioUI()
    //{
    //    UIManager.Instance.soundCanvas.SetActive(true);
    //    UIManager.Instance.playCanvas.SetActive(false);

    //}

    //public void PlayUI()
    //{
    //    UIManager.Instance.soundCanvas.SetActive(false);
    //    UIManager.Instance.playCanvas.SetActive(true);
    //}


    IEnumerator EndOfFrameRoutine(int sceneNumber)
    {   
        // ���� �������� ���� ������ ���
        yield return new WaitForEndOfFrame();
        DataManager.Instance.isEasyMode = false;
        DataManager.Instance.isNormalMode = false;
        DataManager.Instance.isModeSelect = false;
        DataManager.Instance.isHardMode = true;

        if (sceneNumber == 0)
            EventManager.Instance.NotifyEvent(EventType.LoadingScene, "Tutorial");   
        else if(sceneNumber == 1)
            EventManager.Instance.NotifyEvent(EventType.LoadingScene, "LevelDesign");   
            //EventManager.Instance.NotifyEvent(EventType.LoadingScene, "BossX");  
        // ���⿡�� �ڵ带 ����
    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Onclick에 넣고 사용하면 된다.
/// </summary>
public class UIOnclick : MonoBehaviour
{
    /// <summary>
    /// Title에서 최초로 게임이 시작될 때 사용하는 함수
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
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
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
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
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
    /// 게임을 종료하고 싶을 때 사용하는 함수
    /// </summary>
    public void GameExit()
    {
       SceneController.Instance.GameExit(); 
    }

    public void GameOption()
    {
        // 처음에는 씹히는 듯한 느낌이 드네
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
    /// Setting으로 가고 싶을 때 사용하는 함수
    /// </summary>
    public void SettingUI()
    {
        // Canvas를 가져온다.
        GameObject canvas = GameObject.Find("Canvas");
        
        // SettingUI Prefab을 생성해서 불러오자.
        GameObject settingPrefab = Resources.Load<GameObject>("UI/Setting Canvas");

        if (settingPrefab != null)
        {
            Instantiate(settingPrefab, canvas.transform);
        }
        else Debug.Log("None Prefab");
    }

    /// <summary>
    /// Setting을 종료하고 싶을 때 사용하는 함수
    /// </summary>
    public void SettingExit()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1.0f;
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Audio 세팅
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
        // 현재 프레임이 끝날 때까지 대기
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
        // 여기에서 코드를 실행
    }
}

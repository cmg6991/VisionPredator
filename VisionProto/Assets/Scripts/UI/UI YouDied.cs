using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI OnClick에 넣고 사용하면 된다.
/// You Died Canvas에 넣고 사용하면 된다.
/// </summary>
public class UIYouDied : MonoBehaviour
{
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        // 일단은 Scene을 재시작하자.
        Scene currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currentScene.name);

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
            StartCoroutine(EndOfFrameRoutine(currentScene.name));
        }
        else Debug.Log("None Prefab");


        //         GameObject canvas = GameObject.Find("Canvas");
        // 
        //         GameObject youDiedPrefab = Resources.Load<GameObject>("UI/Loading");
        //         Cursor.lockState = CursorLockMode.None;
        // 
        //         if (youDiedPrefab != null)
        //         {
        //             Instantiate(youDiedPrefab, canvas.transform);
        //             // ㄴㄴ 안돼 끝나고 나서 해야 해.
        //             StartCoroutine(EndOfFrameRoutine());
        //         }
        //         else Debug.Log("None Prefab");

    }


    /// <summary>
    /// 진짜 나가기가 아닐거 아니야 타이틀로 가기 그런거 아닐까?
    /// </summary>
    public void GameExit()
    {
        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            // ㄴㄴ 안돼 끝나고 나서 해야 해.
            string sceneName = "Prototype UI";
            StartCoroutine(EndOfFrameRoutine(sceneName));
        }
        else Debug.Log("None Prefab");

        Time.timeScale = 1;
    }

    IEnumerator EndOfFrameRoutine(string sceneName)
    {
        // 현재 프레임이 끝날 때까지 대기
        yield return new WaitForEndOfFrame();
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, sceneName);
        EventManager.Instance.RemoveAllEvent();

        if(sceneName == "Prototype UI")
            UIManager.Instance.UISetting.SetActive(false);
    }
}

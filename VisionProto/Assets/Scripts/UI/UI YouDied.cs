using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI OnClick�� �ְ� ����ϸ� �ȴ�.
/// You Died Canvas�� �ְ� ����ϸ� �ȴ�.
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

        // �ϴ��� Scene�� ���������.
        Scene currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currentScene.name);

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            // ���� �ȵ� ������ ���� �ؾ� ��.
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
        //             // ���� �ȵ� ������ ���� �ؾ� ��.
        //             StartCoroutine(EndOfFrameRoutine());
        //         }
        //         else Debug.Log("None Prefab");

    }


    /// <summary>
    /// ��¥ �����Ⱑ �ƴҰ� �ƴϾ� Ÿ��Ʋ�� ���� �׷��� �ƴұ�?
    /// </summary>
    public void GameExit()
    {
        GameObject loadingPrefab = Resources.Load<GameObject>("UI/Loading");
        Cursor.lockState = CursorLockMode.None;

        if (loadingPrefab != null)
        {
            Instantiate(loadingPrefab);
            // ���� �ȵ� ������ ���� �ؾ� ��.
            string sceneName = "Prototype UI";
            StartCoroutine(EndOfFrameRoutine(sceneName));
        }
        else Debug.Log("None Prefab");

        Time.timeScale = 1;
    }

    IEnumerator EndOfFrameRoutine(string sceneName)
    {
        // ���� �������� ���� ������ ���
        yield return new WaitForEndOfFrame();
        EventManager.Instance.NotifyEvent(EventType.LoadingScene, sceneName);
        EventManager.Instance.RemoveAllEvent();

        if(sceneName == "Prototype UI")
            UIManager.Instance.UISetting.SetActive(false);
    }
}

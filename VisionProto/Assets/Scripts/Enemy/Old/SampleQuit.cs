using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleQuit : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    QuitGame();
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    RestartCurrentScene();
        //}

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    audioSource.mute = !audioSource.mute;
        //}
    }

    void QuitGame()
    {
        // 유니티 에디터에서 실행 중인지 확인
#if UNITY_EDITOR
        Debug.Log("게임이 꺼지고 있소.");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("게임이 꺼지고 있소.");
        Application.Quit(); 
#endif
    }

    void RestartCurrentScene()
    {
        // 현재 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        // 현재 씬을 재시작합니다.
        SceneManager.LoadScene(currentSceneName);
    }
}

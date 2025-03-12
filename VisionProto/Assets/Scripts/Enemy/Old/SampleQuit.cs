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
        // ����Ƽ �����Ϳ��� ���� ������ Ȯ��
#if UNITY_EDITOR
        Debug.Log("������ ������ �ּ�.");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("������ ������ �ּ�.");
        Application.Quit(); 
#endif
    }

    void RestartCurrentScene()
    {
        // ���� ���� �̸��� �����ɴϴ�.
        string currentSceneName = SceneManager.GetActiveScene().name;
        // ���� ���� ������մϴ�.
        SceneManager.LoadScene(currentSceneName);
    }
}

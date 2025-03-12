using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene Controller�� ���� ������ Untiy�� Scene Manager�� �־ �׷���.
/// Scene���� �����, ���� �������� �̵��� ����Ѵ�.
/// ����ϱ� ���ϰ��ϱ� ���� Enum���� ����
/// 0620 �̿뼺
/// </summary>
public class SceneController : Singleton<SceneController>
{
    List<string> paths;

    protected override void Awake()
    {
        this.transform.SetParent(null);
        base.Awake();

        ListAllScenes();
    }

    /// <summary>
    /// Scene �̸����� �����ϴ� �Լ�
    /// </summary>
    /// <param name="sceneName">Please Writing Scene Title Name</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ���ϰ� ������ �����
    // Project�� �ִ� ��� Scene���� �ϳ��� ��Ƽ� �����Ѵ�.
    // ���߿� ������ ���� �ϸ� ���� �� ����. -> enum ��������.

    void ListAllScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        paths = new List<string>();

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            paths.Add(path);
        }
    }

    public void GameStart()
    {
        LoadScene("ProtoType ver 3");
        PlayerInformation playerInfo = new PlayerInformation();

        // Game Init Position
        playerInfo.position = new Vector3(125.9f, 14.5f, -629.2f);
        playerInfo.rotation = Quaternion.identity;

        DataManager.Instance.SaveData(playerInfo);
    }
    /// <summary>
    /// �� ����
    /// </summary>
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

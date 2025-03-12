using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene Controller로 지은 이유는 Untiy에 Scene Manager가 있어서 그렇다.
/// Scene들의 재시작, 다음 스테이지 이동을 담당한다.
/// 사용하기 편하게하기 위해 Enum으로 관리
/// 0620 이용성
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
    /// Scene 이름으로 변경하는 함수
    /// </summary>
    /// <param name="sceneName">Please Writing Scene Title Name</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 뭐하고 싶은지 적어봐
    // Project에 있는 모든 Scene들을 하나로 담아서 보관한다.
    // 나중에 꺼내기 쉽게 하면 좋을 거 같아. -> enum 생각하자.

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
    /// 씬 종료
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

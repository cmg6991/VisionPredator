using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour, IListener
{
    public Sprite loadingImage1;
    public Sprite loadingImage2;
    //public Sprite loadingImage3;

    public Image backGround;

    // �����غ���.
    // Loading UI�� Instanitate�� ���ž�.
    // string ������ ��� �� �ž� 
    // event manager�� Instanitage�� �����ϰ� �� ������ notify�� �˸���.
    // �̰� ���? ��

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.LoadingScene, OnEvent);

        int startNumber = 0;
        int finalNumber = 2;
        int randomNumber = Random.Range(startNumber, finalNumber);

        switch (randomNumber)
        {
            case 0:
                backGround.sprite = loadingImage1;
                break;
            case 1:
                backGround.sprite = loadingImage2;
                break;
        }

        Time.timeScale = 1;
    }

    // �� 0 ~ 100�۱��� ä��� ���� �����ְ� ������ ������ ��� ��ȴ� ���� �� ����.
    // �ƴ� 
    IEnumerator LoadSceneProcess(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float progress = 0;

        while(!op.isDone)
        {
            progress = Mathf.MoveTowards(progress, op.progress, Time.deltaTime);

            if(progress >= 0.9f)
            {
                op.allowSceneActivation = true;
                EventManager.Instance.RemoveEvent(EventType.LoadingScene);
            }
            yield return null;
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.LoadingScene:
                {
                    string sceneName = (string)param;

                    StartCoroutine(LoadSceneProcess(sceneName));
                }
                break;
        }
    }

}

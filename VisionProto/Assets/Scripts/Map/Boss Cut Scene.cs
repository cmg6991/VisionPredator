using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BossCutScene : MonoBehaviour
{
    public VideoPlayer cutSceneVideo;
    public GameObject cutSceneObject;
    public GameObject BossObject;
    public GameObject bossAgent;

    private bool isStart;
    private bool isEnd = false;

    void Start()
    {
        cutSceneVideo.loopPointReached += OnVideoEnd;
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Openning);
        isStart = true;
    }

    private void OnDisable()
    {
        if(isStart && !isEnd)
        {
            cutSceneObject.SetActive(false);
            BossObject.SetActive(true);
            EventManager.Instance.NotifyEvent(EventType.isPause, false);
            SoundManager.Instance.AllSoundRemove();
            SoundManager.Instance.PlayBGMSound(BGM.Boss);
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Idle, bossAgent.transform);
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        cutSceneObject.SetActive(false);
        BossObject.SetActive(true);
        EventManager.Instance.NotifyEvent(EventType.isPause, false);
        SoundManager.Instance.AllSoundRemove();
        SoundManager.Instance.PlayBGMSound(BGM.Boss);
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Idle, bossAgent.transform);
        isEnd = true;
    }
}

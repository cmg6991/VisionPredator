using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BossDeadScene : MonoBehaviour
{
    public VideoPlayer deadSceneVideo;
    public GameObject deadSceneObject;

    // Start is called before the first frame update
    void Start()
    {
        deadSceneVideo.loopPointReached += OnVideoEnd;
    }


    void OnVideoEnd(VideoPlayer vp)
    {
        GameObject camera = GameObject.Find("Virtual Camera");

        if (camera == null)
            Debug.Log("None Virtual Camera");
        else
        {
            VPRenderFeature renderFeature;
            camera.transform.parent.TryGetComponent<VPRenderFeature>(out renderFeature);
            // 이거를 해도 Time Scale이 0이여서 실행이 안된다. 그래서 죽기 전까지 가능
            renderFeature.isGameClear = true;
            renderFeature.FadeInFadeOut();
        }
    }

}

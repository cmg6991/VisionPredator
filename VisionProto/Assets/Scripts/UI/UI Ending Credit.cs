using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class UIEndingCredit : MonoBehaviour
{
    public VideoPlayer endingVideo;
    public GameObject endingCanvas;

    void Start()
    {
        endingVideo.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        endingCanvas.SetActive(true);
        EventManager.Instance.RemoveAllEvent();
        SceneManager.LoadScene("Prototype UI");
        Cursor.lockState = CursorLockMode.None;
    }

}

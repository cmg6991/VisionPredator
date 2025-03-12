using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameClear : MonoBehaviour
{
    public bool gotoMainTitleScene;

    public GameObject[] gameImage;
    public GameObject endingCreditCanvas;
    public GameObject endingVideo;
    public GameObject creditSound;

    public float nextImageTime = 0.5f;

    private float totalTime;
    private int count = 0;


    private void Update()
    {
        if (gotoMainTitleScene)
            return;

        totalTime += Time.deltaTime;

        if(totalTime > nextImageTime)
        {
            totalTime = 0;
            if(count < gameImage.Length) 
                gameImage[count].SetActive(true);
            else
            {
                this.gameObject.SetActive(false);
                endingCreditCanvas.SetActive(true);
                endingVideo.SetActive(true);
                creditSound.SetActive(true);
                this.enabled = false;
                gotoMainTitleScene = true;
            }
            count++;
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject title;

    public GameObject[] images;


    public GameObject main;
    public GameObject retry;

    private float totalTime;
    private int count = 0;

    public float nextImageTime = 0.5f;

    private bool isDone;
    void Start()
    {
        foreach (var image in images) 
        {
            image.gameObject.SetActive(false);
        }
        Time.timeScale = 1.0f;
        main.SetActive(false);
        retry.SetActive(false);
        isDone = false;
    }

    void Update()
    {
        if (isDone)
            return;

        totalTime += Time.deltaTime;

        if(totalTime > nextImageTime)
        {
            totalTime = 0;
            if(count < images.Length)
                images[count].SetActive(true);
            else
            {
                main.SetActive(true);
                retry.SetActive(true);
                isDone = true;
                this.enabled = false;
            }
            count++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct WeaponUIInformation
{
    public bool isMouseEnter;
    public string gunName;
    public Transform transform;
}

public class UIManager : MonoBehaviour
{
    public GameObject UISetting;
    public GameObject mainButton;
    public GameObject retryButton;
    public GameObject skipButton;
    public GameObject tutorialSettingButton;
    public GameObject optionButton;

    public float mouseSpeed;
   
    public static UIManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            this.transform.SetParent(null);
            Instance = this;
            DontDestroyOnLoad(this);
        }

    }

    private void LoadUI()
    {

    }
    private void Start()
    {

    }


    public void GameStart()
    {
        SceneController.Instance.GameStart();
    }

    //public void PlayBtnClick()
    //{
    //    playIsClicked = true; 
    //    soundIsClicked =false; 

    //    UpdateButtonImages();
    //}

    //// Sound 버튼 클릭 시
    //public void SoundBtnClick()
    //{
    //    soundIsClicked = true;
    //    playIsClicked = false; 

    //    UpdateButtonImages();
    //}
    //private void UpdateButtonImages()
    //{
    //    if (playIsClicked)
    //    {
    //        playImage.sprite = sprites[0];  
    //    }
    //    else
    //    {
    //        playImage.sprite = sprites[1];  
    //    }

    //    if (soundIsClicked)
    //    {
    //        soundImage.sprite = sprites[2]; 
    //    }
    //    else
    //    {
    //        soundImage.sprite = sprites[3]; 
    //    }
    //}
}

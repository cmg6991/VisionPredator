using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundControl : Singleton<SoundControl>
{
    [SerializeField]
    AudioMixer audioMixer;
    //[SerializeField]
    //Slider masterSlider;
    [SerializeField]
    Slider bgmSlider;
    [SerializeField]
    Slider sfxSlider;

    private void Awake()
    {
//         DontDestroyOnLoad(this);
//         this.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(x => audioMixer.SetFloat("BGMVolume", AdjustVolume(x)));
        }

        if(sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(x => audioMixer.SetFloat("SFXVolume", AdjustVolume(x)));

        //if(masterSlider != null)
        //    masterSlider.onValueChanged.AddListener(x => audioMixer.SetFloat("MasterVolume", AdjustVolume(x)));

    }

    public float AdjustVolume(float value)
    {
        float clampValue = Mathf.Clamp(value, 0.0001f, 1f);

        float logValue = Mathf.Log10(clampValue) * 20f;
        return logValue;
    }

    public void Empty()
    {

    }
}

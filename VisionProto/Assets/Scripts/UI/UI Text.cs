using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIText : MonoBehaviour
{
    public GameObject text1;
    public float time = 3f;
    private static bool isShow = false;

    private const string showKey = "UIText";
    

    void Awake()
    {
        if (text1 != null)
        {
            text1.SetActive(false);
        }
    }

    private void OnEnable()
    {
        isShow = false;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt(showKey, 0) == 0 && text1 != null)
        {
            text1.SetActive(true);
            StartCoroutine(HideText());
            isShow = true;

            PlayerPrefs.SetInt(showKey, 1);
            PlayerPrefs.Save();
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10) && text1 != null)
        {
            text1.SetActive(false);
        }
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(time);
        text1.SetActive(false);
    }

    public void ResetUI()
    {
        PlayerPrefs.SetInt(showKey, 0);
        PlayerPrefs.Save();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            text1.SetActive(true);
            StartCoroutine(HideText());
        }
    }
}

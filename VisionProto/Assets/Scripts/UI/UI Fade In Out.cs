using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeInOut : MonoBehaviour
{
    public Image panel;
    public float fadeDuration = 1.0f;
    private float targetAlpha = 1.0f;

    public bool isFadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isFadeIn)
            FadeIn();
        else 
            FadeOut();
    }

    public void FadeIn()
    {
        targetAlpha = 1.0f;
        Color currentColor = panel.color;
        currentColor.a = targetAlpha;
        panel.color = currentColor;

        StartCoroutine(FadeTo(targetAlpha, fadeDuration));
    }

    public void FadeOut()
    {
        targetAlpha = 0f;
        Color currentColor = panel.color;
        currentColor.a = targetAlpha;
        panel.color = currentColor;

        StartCoroutine(FadeTo(targetAlpha, fadeDuration));
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        Color currentColor = panel.color;
        float startAlpha = currentColor.a;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float blend = Mathf.Clamp01(time / duration);
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, blend);
            panel.color = currentColor;
            yield return null;
        }

        currentColor.a = targetAlpha;
        panel.color = currentColor; 

    }
}

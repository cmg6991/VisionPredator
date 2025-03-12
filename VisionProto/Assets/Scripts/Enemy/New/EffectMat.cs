using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMat : MonoBehaviour
{
    private Material objectMaterial;
    private Color originalColor;

    [SerializeField] private float blinkSpeed = 1f;  
    [SerializeField] private float minAlpha = 0.2f;  
    [SerializeField] private float maxAlpha = 1f;    

    private void Start()
    {
        objectMaterial = GetComponent<Renderer>().material;
        originalColor = objectMaterial.color;

        StartCoroutine(BlinkAlpha());
    }

    private IEnumerator BlinkAlpha()
    {
        float alpha = maxAlpha;

        while (true)
        {
            alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * blinkSpeed, 1f));
            objectMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VPScanner : MonoBehaviour
{
    private Vector3 scale;

    public float waveSpeed = 1f;
    public float endScale = 5.1f;

    private Vector3 endScaleVector;
    // Start is called before the first frame update
    void Start()
    {
        endScaleVector = new Vector3(endScale - 0.1f, endScale - 0.1f, endScale - 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        float scaleX = this.transform.localScale.x;
        float currentScale = Mathf.Lerp(scaleX, endScale, Time.deltaTime * waveSpeed);

        scale.x = currentScale;
        scale.y = currentScale;
        scale.z = currentScale;

        this.transform.localScale = scale;

        if (this.transform.localScale.x >= endScaleVector.x)
            Destroy(this.gameObject);
    }

}

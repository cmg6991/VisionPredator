using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellingOpen : MonoBehaviour
{
    public GameObject firstObject;
    public GameObject secondObject;

    public GameObject bossObject;

    public float openSpeed = 2f;
    private bool isDone;
    private bool isClear;

    private Vector3 addFirst;
    private Vector3 addSecond;

    private void Start()
    {
        if (firstObject == null)
            Debug.Log("None First Object");

        if(secondObject == null)
            Debug.Log("None Second Object");

        addFirst = firstObject.transform.forward;
        addSecond = secondObject.transform.forward;
    }

    private void Update()
    {
        // First Object 의 Forward 방향만큼 더한다.
        if(!isDone)
        {
            SoundManager.Instance.PlayEffectSound(SFX.Open_Door, firstObject.transform);
            firstObject.gameObject.transform.rotation = Quaternion.Euler(180f, 90f, 0);
            secondObject.gameObject.transform.rotation = Quaternion.Euler(180f, -90f, 0);
            isDone = true;
            Invoke("OpenCelling", 3f);
        }

        firstObject.transform.localPosition += (firstObject.transform.forward * (Time.deltaTime * openSpeed));
        secondObject.transform.localPosition += (secondObject.transform.forward * (Time.deltaTime * openSpeed));

    }

    private void OpenCelling()
    {
        this.enabled = false;
        bossObject.SetActive(true);
    }

}

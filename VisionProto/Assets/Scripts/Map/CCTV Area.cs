using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVArea : MonoBehaviour
{
    public GameObject cctvCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // CCTV�� Ȱ��ȭ
            cctvCamera.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // CCTV�� Ȱ��ȭ
            cctvCamera.SetActive(false);
        }
    }
}

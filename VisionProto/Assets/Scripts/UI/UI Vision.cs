using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(3)]
public class UIVision : MonoBehaviour, IListener
{
    // Vision Cool Time을 받아와야 한다.
    [SerializeField]
    public float coolTime = 5.0f;
    private bool isVPState;
    public Image visionImage;
    public Image visionShader;

    public Sprite whiteVisionImage;
    public Sprite colseVisionImage;
    public Sprite openVisionImage;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    private IEnumerator CoolTime()
    {
        float elapsedTime = 0f;

        // 흰색으로 차오르는 거 구현
        visionImage.sprite = whiteVisionImage;
        visionShader.gameObject.SetActive(false);
        visionImage.fillAmount = 0f;

        while(elapsedTime < coolTime)
        {
            float fillProgress = elapsedTime / coolTime;

            visionImage.fillAmount = Mathf.Lerp(0f, 1f, fillProgress);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        visionImage.fillAmount = 1f;
        visionShader.gameObject.SetActive(true);

        if (isVPState)
            visionImage.sprite = colseVisionImage;
        else
            visionImage.sprite = openVisionImage;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType) 
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                    StartCoroutine(CoolTime());
                }
                break;
        }
    }
}

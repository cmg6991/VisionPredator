using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbsorbGauge : MonoBehaviour
{
    public DarkVisionGauge darkVision;
    public Image image;         // 해당 이미지 지금은 Test 용도라서 아무거나 넣자
    public Image coolTimeImage; // 해당 이미지는 Cool Time을 표시하기 위한 Image다.
    public float speed;         // 차는 속도
    public float gauge;         // Gauge
    public TMP_Text text;

    public bool isCoolDownTime;
    private float maxTime;
    private float currentTime;

    // public으로 말고 private로 받아오는 방법 없을까?
    public Player player;

    void Start()
    {
        StartSetting();
    }

    void Update()
    {
        // Player에서 받아와야 한다.
        // Gage들은 Player의 정보를 알아와야 해

        // Count 수에 따라 채워지게 하자. 그리고 시간에 맞게 올라가게끔 진행하자.

        if(!darkVision.isfull)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isCoolDownTime)
            {
                coolTimeImage.fillAmount = 0.0f;
                isCoolDownTime = true;
            }

            if(isCoolDownTime) 
            {
                // 겉 부분이 Top에서 돌아야 한다.
                // 활성화
                image.color = new Color(1f, 1f, 1f, 0.9f);
                coolTimeImage.color = new Color(1f, 0f, 0f);
                coolTimeImage.fillAmount += speed * Time.deltaTime;
                currentTime += Time.deltaTime;  // 사용안하고 있네
                maxTime -= Time.deltaTime;
                text.text = ((int)maxTime + 1).ToString();

                if (coolTimeImage.fillAmount >= 1.0f) 
                {
                    // 비활성화
                    image.color = new Color(1f, 0f, 0f, 0.5f);
                    coolTimeImage.color = new Color(0f, 0f, 0f);
                    isCoolDownTime = false;
                    currentTime = 0.0f;     // 사용안하고 있네
                    maxTime = 5.0f;
                    text.text = "";
                }
            }
        }
        else
        {
            // 비활성화
            coolTimeImage.fillAmount = 1.0f;
            image.color = new Color(1f, 0f, 0f, 0.5f);
            coolTimeImage.color = new Color(0f, 0f, 0f);
        }
    }

    void StartSetting()
    {
        /// 초기 Gauge와 Count, Gauge가 차오르는 속도
        speed = 0.2f;
        gauge = 0.2f;
        maxTime = 5.0f;

        /// 이미지 초기 설정도 필요 없을지도?
        image.type = Image.Type.Filled;
        image.color = new Color(1f, 0f, 0f, 0.5f);
        image.fillAmount = 1.0f;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;

        /// 쿨타임 이미지 초기 설정
        coolTimeImage.type = Image.Type.Filled;
        coolTimeImage.color = new Color(0f, 0f, 0f);
        coolTimeImage.fillAmount = 1.0f;
        coolTimeImage.fillMethod = Image.FillMethod.Radial360;
        coolTimeImage.fillOrigin = (int)Image.Origin360.Top;
        coolTimeImage.fillClockwise = true;

        text.text = "";
    }
}

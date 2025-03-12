using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbsorbGauge : MonoBehaviour
{
    public DarkVisionGauge darkVision;
    public Image image;         // �ش� �̹��� ������ Test �뵵�� �ƹ��ų� ����
    public Image coolTimeImage; // �ش� �̹����� Cool Time�� ǥ���ϱ� ���� Image��.
    public float speed;         // ���� �ӵ�
    public float gauge;         // Gauge
    public TMP_Text text;

    public bool isCoolDownTime;
    private float maxTime;
    private float currentTime;

    // public���� ���� private�� �޾ƿ��� ��� ������?
    public Player player;

    void Start()
    {
        StartSetting();
    }

    void Update()
    {
        // Player���� �޾ƿ;� �Ѵ�.
        // Gage���� Player�� ������ �˾ƿ;� ��

        // Count ���� ���� ä������ ����. �׸��� �ð��� �°� �ö󰡰Բ� ��������.

        if(!darkVision.isfull)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isCoolDownTime)
            {
                coolTimeImage.fillAmount = 0.0f;
                isCoolDownTime = true;
            }

            if(isCoolDownTime) 
            {
                // �� �κ��� Top���� ���ƾ� �Ѵ�.
                // Ȱ��ȭ
                image.color = new Color(1f, 1f, 1f, 0.9f);
                coolTimeImage.color = new Color(1f, 0f, 0f);
                coolTimeImage.fillAmount += speed * Time.deltaTime;
                currentTime += Time.deltaTime;  // �����ϰ� �ֳ�
                maxTime -= Time.deltaTime;
                text.text = ((int)maxTime + 1).ToString();

                if (coolTimeImage.fillAmount >= 1.0f) 
                {
                    // ��Ȱ��ȭ
                    image.color = new Color(1f, 0f, 0f, 0.5f);
                    coolTimeImage.color = new Color(0f, 0f, 0f);
                    isCoolDownTime = false;
                    currentTime = 0.0f;     // �����ϰ� �ֳ�
                    maxTime = 5.0f;
                    text.text = "";
                }
            }
        }
        else
        {
            // ��Ȱ��ȭ
            coolTimeImage.fillAmount = 1.0f;
            image.color = new Color(1f, 0f, 0f, 0.5f);
            coolTimeImage.color = new Color(0f, 0f, 0f);
        }
    }

    void StartSetting()
    {
        /// �ʱ� Gauge�� Count, Gauge�� �������� �ӵ�
        speed = 0.2f;
        gauge = 0.2f;
        maxTime = 5.0f;

        /// �̹��� �ʱ� ������ �ʿ� ��������?
        image.type = Image.Type.Filled;
        image.color = new Color(1f, 0f, 0f, 0.5f);
        image.fillAmount = 1.0f;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;

        /// ��Ÿ�� �̹��� �ʱ� ����
        coolTimeImage.type = Image.Type.Filled;
        coolTimeImage.color = new Color(0f, 0f, 0f);
        coolTimeImage.fillAmount = 1.0f;
        coolTimeImage.fillMethod = Image.FillMethod.Radial360;
        coolTimeImage.fillOrigin = (int)Image.Origin360.Top;
        coolTimeImage.fillClockwise = true;

        text.text = "";
    }
}

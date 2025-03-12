using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkVisionGauge : MonoBehaviour
{
    //public AbsorbGauge absorbGauge; // ����

    // ��ũ ���� UI�� ������ �ذ� 
    public GameObject darkVision;

    public Image TestPanel;

    private Image panel;
    private Image image;         // �ش� �̹��� ������ Test �뵵�� �ƹ��ų� ����

    private Image[] images;

    public float speed;         // ���� �ӵ�
    public float gauge;         // Gauge
    
    public bool isfull { get; private set; }         // Player �ʿ� �ְ��� Dark Vision�� Ȱ��ȭ �Ǿ����� ����
    private int count;          // �̰� Player �ʿ��� ���� ���̸� Count�� ���״ϱ� �̸� �����ص���.

    // public���� ���� private�� �޾ƿ��� ��� ������?
    public Player player;

    void Start()
    {
        initalize();
    }

    void Update()
    {
        // Player���� �޾ƿ;� �Ѵ�.
        // Gauge���� Player�� ������ �˾ƿ;� ��
        if (Input.GetKeyDown(KeyCode.E))
        {
            count++;
        }

        if(player.isDarkvision)
            TestPanel.gameObject.SetActive(true);
        else
            TestPanel.gameObject.SetActive(false);

//         if (((count * gauge) >= image.fillAmount) && !isfull)
//         {
//             image.fillAmount += speed * Time.deltaTime;
// 
//             if (image.fillAmount >= 1.0f)
//             {
//                 isfull = true;
//                 speed = 0.3f;
//                 panel.gameObject.SetActive(true);
//             }
//         }
// 
// 
//         if (isfull)
//         {
//             image.fillAmount -= speed * Time.deltaTime;
// 
//             if (image.fillAmount <= 0.0f)
//             {
//                 panel.gameObject.SetActive(false);
//                 isfull = false;
//                 speed = 0.5f;
//                 count = 0;
//             }
//         }
    }

    void initalize()
    {
        /// �ʱ� ����
        count = 0;
        speed = 0.5f;
        gauge = 0.334f;
// 
//         images = GetComponentsInChildren<Image>();
// 
//         image = images[0];
//         panel = images[1];
// 
//         image.type = Image.Type.Filled;
//         image.fillAmount = 0.0f;
//         image.fillMethod = Image.FillMethod.Vertical;
//         image.fillOrigin = (int)Image.OriginVertical.Bottom;
// 
//         panel.gameObject.SetActive(false);
        TestPanel.gameObject.SetActive(false);
    }
}

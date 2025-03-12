using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkVisionGauge : MonoBehaviour
{
    //public AbsorbGauge absorbGauge; // 보류

    // 다크 비전 UI를 넣으면 해결 
    public GameObject darkVision;

    public Image TestPanel;

    private Image panel;
    private Image image;         // 해당 이미지 지금은 Test 용도라서 아무거나 넣자

    private Image[] images;

    public float speed;         // 차는 속도
    public float gauge;         // Gauge
    
    public bool isfull { get; private set; }         // Player 쪽에 있겠지 Dark Vision이 활성화 되었으면 시작
    private int count;          // 이건 Player 쪽에서 적을 죽이면 Count를 셀테니까 미리 세팅해두자.

    // public으로 말고 private로 받아오는 방법 없을까?
    public Player player;

    void Start()
    {
        initalize();
    }

    void Update()
    {
        // Player에서 받아와야 한다.
        // Gauge들은 Player의 정보를 알아와야 해
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
        /// 초기 세팅
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

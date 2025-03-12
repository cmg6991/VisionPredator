using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestPotion : MonoBehaviour
{
    public Canvas canvas;
    public Player player;
    private Vector3 UIPosition;

    private UnityEngine.UI.Image[] backGrounds;

    private TMP_Text title;
    private TMP_Text explanation;
    private TMP_Text pickUp;

    // 먹었을 때, 닿았을 때
    bool isSetting;

    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsTargetVisible(transform))
        {
            if (player.IsRayTarget(transform))
            {
                backGrounds[0].gameObject.SetActive(true);
                backGrounds[0].transform.position = Camera.main.WorldToScreenPoint(transform.position);
                title.transform.position = Camera.main.WorldToScreenPoint(transform.position);

                if (isSetting)
                {
                    backGrounds[1].gameObject.SetActive(true);
                    backGrounds[1].transform.position = Camera.main.WorldToScreenPoint(transform.position + UIPosition);
                    explanation.transform.position = Camera.main.WorldToScreenPoint(transform.position + UIPosition);
                }
                else
                    backGrounds[1].gameObject.SetActive(false);
            }
            else
                backGrounds[0].gameObject.SetActive(false);
        }
        else
        {
            FalseTitleExplanation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 이건 item에 있는거 
        if (collision.gameObject.layer == 7)
        {
            gameObject.SetActive(false);
            FalseTitleExplanation();

            // 여기서 바로 하는게 아니라 UIManager로 보내자.
           // UIManager.Instance.AddPickup(backGrounds[2]);
        }
    }

    private void OnMouseEnter()
    {
        isSetting = true;
    }
    private void OnMouseExit()
    {
        isSetting = false;
    }

    void Initalize()
    {
        /// Canvas를 순회하면서 Image를 받아온다.
        backGrounds = canvas.GetComponentsInChildren<UnityEngine.UI.Image>();

        /// 순회한 Image에 있는 자식들을 순회해서 TMP_Text를 받아온다.
        title = backGrounds[0].GetComponentInChildren<TMP_Text>();
        explanation = backGrounds[1].GetComponentInChildren<TMP_Text>();
        pickUp = backGrounds[2].GetComponentInChildren<TMP_Text>();

        /// UIPosition
        UIPosition = new Vector3(0.0f, -0.5f, 0.0f);

        /// Text 
        title.text = "아드레날린";
        explanation.text = "이동 속도가 증가합니다";
        pickUp.text = "이동 속도가 증가합니다";

        /// Background Color 세팅
        foreach (var image in backGrounds)
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        /// Explanation 설명
        title.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        explanation.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        pickUp.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        /// FontSize 설정
        title.fontSize = 18;
        explanation.fontSize = 10;
        pickUp.fontSize = 10;

        /// Font 위치 설정
        title.alignment = TextAlignmentOptions.Center;
        explanation.alignment = TextAlignmentOptions.Center;
        pickUp.alignment = TextAlignmentOptions.Center;

        /// Game Object를 false로 한다.
        FalseTitleExplanation();
        backGrounds[2].gameObject.SetActive(false);
    }

    void FalseTitleExplanation()
    {
        backGrounds[0].gameObject.SetActive(false);
        backGrounds[1].gameObject.SetActive(false);
    }
}

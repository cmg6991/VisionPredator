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

    // �Ծ��� ��, ����� ��
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
        // �̰� item�� �ִ°� 
        if (collision.gameObject.layer == 7)
        {
            gameObject.SetActive(false);
            FalseTitleExplanation();

            // ���⼭ �ٷ� �ϴ°� �ƴ϶� UIManager�� ������.
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
        /// Canvas�� ��ȸ�ϸ鼭 Image�� �޾ƿ´�.
        backGrounds = canvas.GetComponentsInChildren<UnityEngine.UI.Image>();

        /// ��ȸ�� Image�� �ִ� �ڽĵ��� ��ȸ�ؼ� TMP_Text�� �޾ƿ´�.
        title = backGrounds[0].GetComponentInChildren<TMP_Text>();
        explanation = backGrounds[1].GetComponentInChildren<TMP_Text>();
        pickUp = backGrounds[2].GetComponentInChildren<TMP_Text>();

        /// UIPosition
        UIPosition = new Vector3(0.0f, -0.5f, 0.0f);

        /// Text 
        title.text = "�Ƶ巹����";
        explanation.text = "�̵� �ӵ��� �����մϴ�";
        pickUp.text = "�̵� �ӵ��� �����մϴ�";

        /// Background Color ����
        foreach (var image in backGrounds)
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        /// Explanation ����
        title.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        explanation.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        pickUp.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        /// FontSize ����
        title.fontSize = 18;
        explanation.fontSize = 10;
        pickUp.fontSize = 10;

        /// Font ��ġ ����
        title.alignment = TextAlignmentOptions.Center;
        explanation.alignment = TextAlignmentOptions.Center;
        pickUp.alignment = TextAlignmentOptions.Center;

        /// Game Object�� false�� �Ѵ�.
        FalseTitleExplanation();
        backGrounds[2].gameObject.SetActive(false);
    }

    void FalseTitleExplanation()
    {
        backGrounds[0].gameObject.SetActive(false);
        backGrounds[1].gameObject.SetActive(false);
    }
}

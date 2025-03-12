using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour
{
    public Image image;         // �ش� �̹���
    public Image panel;         // Game Over Panel
    public float speed;         // �پ��� �ӵ�

    public float HPSpeed;       // HP �پ��� �ӵ�
    private float HPSubSpeed;

    // �̰͵� �̸� �ص���.
    public float playerHP;      // Player ü��
    public float uiHP;          // UI ü��

    bool isAdd = false;
    bool isSub = false;

    void Start()
    {
        // Test ��
        playerHP = 100f;
        uiHP = playerHP;

        speed = 0.1f;
        HPSpeed = 0.1f;
        HPSubSpeed = 10f;
        image.fillAmount = 1.0f;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;

        panel.gameObject.SetActive(false);
    }

    void Update()
    {
        // �ش� �̹��� ä���
        if (Input.GetKeyDown(KeyCode.R))
        {
            image.fillAmount = 1.0f;
            panel.gameObject.SetActive(false);
            playerHP = 100f;
            uiHP = playerHP;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            AddHP();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SubHP();
        }

        // UI�� ����� ü���� Player�� ���� �ʴٸ� ����
        if (playerHP != uiHP)
        {
            // ui�� ����� ü�� - ���� Player ü��
            float currentHP = uiHP - playerHP;

            // Player 
            if (currentHP > 0)
            {
                // ����� ü���� �޾ƾ� �Ѵ�.
                isSub = true;
            }
            else
            {
                // ������ ü���� ȸ���Ѵ�.
                isAdd = true;
            }
        }

        // ȸ��
        if (isAdd)
        {
            uiHP += Time.deltaTime * HPSubSpeed;
            image.fillAmount += HPSpeed * Time.deltaTime;

            if (uiHP >= playerHP)
            {
                if (uiHP > 100)
                {
                    uiHP = 100f;
                    playerHP = 100f;
                }
                // ������ ���� �� ������.. ��...
                image.fillAmount = playerHP / 100f;
                uiHP = playerHP;
                isAdd = false;
            }
        }

        // �λ�
        if (isSub)
        {
            uiHP -= Time.deltaTime * HPSubSpeed;
            image.fillAmount -= HPSpeed * Time.deltaTime;

            if (uiHP <= playerHP)
            {
                if(uiHP < 0)
                {
                    uiHP = 0f;
                    playerHP = 0f;
                }

                image.fillAmount = playerHP / 100f;
                uiHP = playerHP;
                isSub = false;
            }
        }

        if (image.fillAmount <= 0.0f)
        {
            panel.gameObject.SetActive(true);
        }
    }

    // Update�� �˻��ϴ� �� ���� ü���� �޾��� �� �ش� UI�� �����ϴ°� ���� ������?
    // ü���� �޸� �����ؾ� �ϴµ� ���⼭ �׽�Ʈ�� �غ���.
    // �̹� ü���� �޾����ϱ� 

    void SubHP()
    {
        playerHP -= 17f;
    }

    void AddHP()
    {
        playerHP += 16f;
    }

}
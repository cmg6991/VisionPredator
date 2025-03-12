using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour
{
    public Image image;         // 해당 이미지
    public Image panel;         // Game Over Panel
    public float speed;         // 줄어드는 속도

    public float HPSpeed;       // HP 줄어드는 속도
    private float HPSubSpeed;

    // 이것도 미리 해두자.
    public float playerHP;      // Player 체력
    public float uiHP;          // UI 체력

    bool isAdd = false;
    bool isSub = false;

    void Start()
    {
        // Test 중
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
        // 해당 이미지 채우기
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

        // UI에 저장된 체력이 Player와 같지 않다면 실행
        if (playerHP != uiHP)
        {
            // ui에 저장된 체력 - 현재 Player 체력
            float currentHP = uiHP - playerHP;

            // Player 
            if (currentHP > 0)
            {
                // 양수면 체력이 달아야 한다.
                isSub = true;
            }
            else
            {
                // 음수면 체력을 회복한다.
                isAdd = true;
            }
        }

        // 회복
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
                // 나눗셈 별로 안 좋은데.. 흠...
                image.fillAmount = playerHP / 100f;
                uiHP = playerHP;
                isAdd = false;
            }
        }

        // 부상
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

    // Update시 검사하는 거 보다 체력이 달았을 때 해당 UI를 실행하는게 좋지 않을까?
    // 체력이 달면 실행해야 하는데 여기서 테스트를 해보자.
    // 이미 체력이 달았으니까 

    void SubHP()
    {
        playerHP -= 17f;
    }

    void AddHP()
    {
        playerHP += 16f;
    }

}
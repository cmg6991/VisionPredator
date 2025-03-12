using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public BoseBaseEnemy bassBaseEnemy;
    public Image[] hpBar;
    public Image grogBar;
    private int currentPhase = 1;
    private float currentMaxHP = 600f;
    private Color[] phaseColors = { new Color(1f, 0f, 0.23f), new Color(0f, 0.76f, 1f), new Color(0.82f, 0f, 1f) }; // RGB로 변환한 페이즈 색상
    private int phaseChangeCount = 0; // 페이즈 변경 횟수



    // Start is called before the first frame update
    void Start()
    {
        hpBar[0].color = phaseColors[0];
        hpBar[1].color = phaseColors[1];
        hpBar[2].color = phaseColors[2];
    }

    // Update is called once per frame
    void Update()
    {
        CheckPhaseChange();
        UpdateHPBar();
    }

    private void CheckPhaseChange()
    {
        if (phaseChangeCount >= 2) return; // 최대 두 번의 페이즈 변경만 허용

        if (currentPhase == 1 && bassBaseEnemy.HP.Value <= 4500)
        {
            currentPhase = 2;
            currentMaxHP = 3000f; // 최대 HP를 400으로 설정
            phaseChangeCount++;

        }
        else if (currentPhase == 2 && bassBaseEnemy.HP.Value <= 3000)
        {
            currentPhase = 3;
            currentMaxHP = 1500f; // 최대 HP를 200으로 설정
            phaseChangeCount++;
        }
    }

    private void UpdateHPBar()
    {
        float currentHP = bassBaseEnemy.HP.Value;



        // 각 페이즈의 HP 바를 0에서 1로 정규화
        //if (currentPhase == 1)
        {
            // 첫 번째 페이즈: 600에서 400까지
            float normalizedHP = (currentHP - 3000f) / (4500 - 3000f); // (현재 HP - 400) / (600 - 400)
            hpBar[0].fillAmount = Mathf.Clamp01(normalizedHP);
        }
        //else if (currentPhase == 2)
        {
            // 두 번째 페이즈: 400에서 200까지
            float normalizedHP = (currentHP - 1500f) / (3000f - 1500f); // (현재 HP - 200) / (400 - 200)
            hpBar[1].fillAmount = Mathf.Clamp01(normalizedHP);
        }
        //else if (currentPhase == 3)
        {
            // 세 번째 페이즈: 200에서 0까지
            float normalizedHP = currentHP / 1500f; // 200을 최대 HP로 설정
            hpBar[2].fillAmount = Mathf.Clamp01(normalizedHP);
        }
    }
}
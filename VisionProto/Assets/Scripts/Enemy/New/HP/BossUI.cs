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
    private Color[] phaseColors = { new Color(1f, 0f, 0.23f), new Color(0f, 0.76f, 1f), new Color(0.82f, 0f, 1f) }; // RGB�� ��ȯ�� ������ ����
    private int phaseChangeCount = 0; // ������ ���� Ƚ��



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
        if (phaseChangeCount >= 2) return; // �ִ� �� ���� ������ ���游 ���

        if (currentPhase == 1 && bassBaseEnemy.HP.Value <= 4500)
        {
            currentPhase = 2;
            currentMaxHP = 3000f; // �ִ� HP�� 400���� ����
            phaseChangeCount++;

        }
        else if (currentPhase == 2 && bassBaseEnemy.HP.Value <= 3000)
        {
            currentPhase = 3;
            currentMaxHP = 1500f; // �ִ� HP�� 200���� ����
            phaseChangeCount++;
        }
    }

    private void UpdateHPBar()
    {
        float currentHP = bassBaseEnemy.HP.Value;



        // �� �������� HP �ٸ� 0���� 1�� ����ȭ
        //if (currentPhase == 1)
        {
            // ù ��° ������: 600���� 400����
            float normalizedHP = (currentHP - 3000f) / (4500 - 3000f); // (���� HP - 400) / (600 - 400)
            hpBar[0].fillAmount = Mathf.Clamp01(normalizedHP);
        }
        //else if (currentPhase == 2)
        {
            // �� ��° ������: 400���� 200����
            float normalizedHP = (currentHP - 1500f) / (3000f - 1500f); // (���� HP - 200) / (400 - 200)
            hpBar[1].fillAmount = Mathf.Clamp01(normalizedHP);
        }
        //else if (currentPhase == 3)
        {
            // �� ��° ������: 200���� 0����
            float normalizedHP = currentHP / 1500f; // 200�� �ִ� HP�� ����
            hpBar[2].fillAmount = Mathf.Clamp01(normalizedHP);
        }
    }
}
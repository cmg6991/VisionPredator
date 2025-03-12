using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamageable
{
    public int HP;
    public int TutorialHP;
    public int currentHP;
    private InputTest input;
    private VPRenderFeature vpRenderFeature;
    private PlayerStateMachine playerStateMachine;
    private bool wasVPState = false;
    private bool isFirstCheck = true;
    public int decreaseHP;

    public bool isEasyMode;

    void Start()
    {
        input = FindObjectOfType<InputTest>();
        vpRenderFeature = FindObjectOfType<VPRenderFeature>();
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        if(playerStateMachine.isTutorial)
            currentHP = TutorialHP;
        else
            currentHP = HP;
        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
    }

    void Update()
    {
        // 모드별 데미지 처리 다르게
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DataManager.Instance.isModeSelect = true;
            DataManager.Instance.isEasyMode = true;
            DataManager.Instance.isNormalMode = false;
            DataManager.Instance.isHardMode = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DataManager.Instance.isModeSelect = true;
            DataManager.Instance.isEasyMode = false;
            DataManager.Instance.isNormalMode = true;
            DataManager.Instance.isHardMode = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DataManager.Instance.isModeSelect = true;
            DataManager.Instance.isEasyMode = false;
            DataManager.Instance.isNormalMode = false;
            DataManager.Instance.isHardMode = true;
        }

        if(!wasVPState && playerStateMachine.isVPState && !isFirstCheck && currentHP > 10)
        {
            currentHP -= decreaseHP;
            EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
        }

        isFirstCheck = false;
        wasVPState = playerStateMachine.isVPState;
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        if(!vpRenderFeature.isInvincibleState)
        {
            currentHP -= damage;
            EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
        }
        else
        {
            return;
        }

        if(!input.dashDamage)
        {
            currentHP -= damage;
            EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
        }
        
        //Debug.Log("currentHp : "+ currentHP);

        if(currentHP <= 0)
        {
            Died();
        }
    }

    public void Died()
    {
        Debug.Log("die");
    }
}

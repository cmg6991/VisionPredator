using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설계가 잘못된 것 같음
/// </summary>
public class HeadHP : MonoBehaviour, IDamageable
{
    public float HP; //디버깅용
    private BaseEnemy baseEnemy;
    private PlayerHP playerHP;
    private CrossHairColor crossHairColor;
    private PlayerStateMachine playerStateMachine;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        baseEnemy = GetComponentInParent<BaseEnemy>();
        playerHP = FindObjectOfType<PlayerHP>();
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        HP = baseEnemy.HP.Value;
        crossHairColor = new CrossHairColor();
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        HP -= damage;
        baseEnemy.HP.Value -= damage;

        crossHairColor.information = CrossHairInformation.Attack;
        crossHairColor.isAttack = true;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        Invoke("ReturnNormal", 0.2f);

        if (baseEnemy.HP.Value <= 0)
        {
            Died();
        }
    }

    private void ReturnNormal()
    {
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
    }

    public void Died()
    {
        if (playerStateMachine.isVPState)
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 10, 100);
            //Debug.Log("20씩 늘려놨다 일단");
        }
        else
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 20, 100);
            //Debug.Log("10씩 늘려놨다 일단");
        }

        crossHairColor.information = CrossHairInformation.Kill;
        crossHairColor.isAttack = true;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        Invoke("ReturnNormal", 0.4f);

        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, playerHP.currentHP);
        EventManager.Instance.NotifyEvent(EventType.NPCDeath, baseEnemy.gameObject);
    }
}

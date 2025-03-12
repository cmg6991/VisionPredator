using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REnemyHP : MonoBehaviour, IDamageable
{
    public float HP; //µð¹ö±ë¿ë
    private BaseEnemy baseEnemy;
    private PlayerHP playerHP;
    private CrossHairColor crossHairColor;
    private PlayerStateMachine playerStateMachine;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        baseEnemy = GetComponent<BaseEnemy>();
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
        if(playerStateMachine.isVPState)
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 20, 100);
            //Debug.Log("20¾¿ ´Ã·Á³ù´Ù ÀÏ´Ü");
        }
        else
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 10, 100);
            //Debug.Log("10¾¿ ´Ã·Á³ù´Ù ÀÏ´Ü");
        }

        crossHairColor.information = CrossHairInformation.Kill;
        crossHairColor.isAttack = true;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        Invoke("ReturnNormal", 0.4f);

        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, playerHP.currentHP);
        EventManager.Instance.NotifyEvent(EventType.NPCDeath, baseEnemy.gameObject);
    }
}

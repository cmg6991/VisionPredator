using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHP : MonoBehaviour, IDamageable
{
    public float HP; //µð¹ö±ë¿ë
    private BoseBaseEnemy baseEnemy;

    private CrossHairColor crossHairColor;

    private PlayerStateMachine playerStateMachine;
    private PlayerHP playerHP;

    private bool isDead;
    private void Start()
    {
        baseEnemy = GetComponent<BoseBaseEnemy>();
        HP = baseEnemy.HP.Value;
        crossHairColor = new CrossHairColor();
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        playerHP = FindObjectOfType<PlayerHP>();
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        if (!enabled) return;

        HP -= damage;
        baseEnemy.HP.Value -= damage;

        if (baseEnemy.HP.Value <= 0)
        {
            Died();
        }

        crossHairColor.information = CrossHairInformation.Attack;
        crossHairColor.isAttack = true;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        int randomIndex = Random.Range(0, 2);

        if (randomIndex == 0)
        {
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Hit_1, this.gameObject.transform);
        }
        else
        {
            SoundManager.Instance.PlayEffectSound(SFX.Boss_Hit_2, this.gameObject.transform);
        }

        Invoke("ReturnNormal", 0.2f);

        if (playerStateMachine.isVPState)
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 20, 100);
            //Debug.Log("20¾¿ ´Ã·Á³ù´Ù ÀÏ´Ü");
        }
        else
        {
            playerHP.currentHP = Mathf.Min(playerHP.currentHP + 10, 100);
            //Debug.Log("10¾¿ ´Ã·Á³ù´Ù ÀÏ´Ü");
        }
        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, playerHP.currentHP);

    }

    private void ReturnNormal()
    {
        crossHairColor.information = CrossHairInformation.Normal;
        crossHairColor.isAttack = false;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);

    }

    public void Died()
    {
        crossHairColor.information = CrossHairInformation.Kill;
        crossHairColor.isAttack = true;
        EventManager.Instance.NotifyEvent(EventType.CrossHairColor, crossHairColor);
        SoundManager.Instance.AllSoundRemove();
        SoundManager.Instance.PlayEffectSound(SFX.Boss_Ending);
        Invoke("ReturnNormal", 0.4f);
        //¸¶Áö¸·ÁÙ ¾È°¡Á®¿Ô´Âµ¥ ±¦Âú°ÚÁö
    }
}
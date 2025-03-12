using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PBenelliRealBullet : MonoBehaviour, IListener
{
    private Vector3 playerPosition;
    private float distance;

    // ���� �Ÿ����� ����
    public float nearDistance = 10f;
    public float farDistance = 50f;

    // ���� ������
    public int nearDamage = 50;
    public int midDamage = 30;
    public int farDamage = 10;

    bool isDetected;
    bool isShoot;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        EventManager.Instance.AddEvent(EventType.PlayerPosition, OnEvent);
    }

    public IEnumerator BulletDestory(float time)
    {
        SoundManager.Instance.PlayEffectSound(SFX.Gunhit);
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        // �Ÿ��� ���� �������� ���µ� ��� �ؾ��ұ�?
        // Event�� ó���ؾ� �� �� ������..
        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }
            // Player �Ÿ��� �޾ƿ��� PshotgunBullet ���� �Ÿ��� ����Ѵ�.
            if (collider.CompareTag("NPC"))
            {
                if (playerPosition != Vector3.zero)
                    distance = Vector3.Distance(playerPosition, this.transform.position);
                else return;

                // �Ÿ� ���
                if (distance < nearDistance)
                    damageable.Damaged(nearDamage, transform.position, transform.position, this.gameObject);
                else if (nearDistance <= distance && distance < farDistance)
                    damageable.Damaged(midDamage, transform.position, transform.position, this.gameObject);
                else if (farDistance <= distance)
                    damageable.Damaged(farDamage, transform.position, transform.position, this.gameObject);

            }
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerPosition:
                {
                    playerPosition = (Vector3)param;
                }
                break;
            case EventType.detected:
                {
                    isDetected = (bool)param;
                }
                break;
        }
    }

}

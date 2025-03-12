using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���� ���ؿ� ���ó���� �ϴ� �ڵ�
/// 
/// �迹���� �ۼ�
/// </summary>
public class EnemyHP : MonoBehaviour, IDamageable
{
    public int HP;
    public Player player;
    public TestBehavior TestBehavior; //�̰� ���������� �ȵǴµ� ������ 
    public Rigidbody eyeRigidbody;

    protected virtual void Start()
    {
        HP = 50;
        TestBehavior= GetComponent<TestBehavior>();
        eyeRigidbody = TestBehavior.eye.GetComponent<Rigidbody>();
    }
    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        TestBehavior.m_Animator.SetBool("Hit",true);
        TestBehavior.m_Animator.SetBool("Attack", false);
        TestBehavior.m_Animator.SetBool("Chase", false);
        //HP -= damage;

        if (HP <= 0)
        {
            eyeRigidbody.useGravity = true;
            TestBehavior.m_Animator.SetTrigger("Death");
            ///���Ӱ͵� �������� ������ ���� �Ǿ���.
            //gameObject.layer = LayerMask.NameToLayer("DeadNPC");
            ///�̰͵� ���д�. ��� �ٲ�
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
            ///���д�. �ֳĸ� �ٴڱ��� �հ�...�׷��� �Ǿ����
            ///���̾� ������ �ؾ��Ѵ�.�ٵ� 
            //for (int i = 0; i<TestBehavior.boxcolider.Length; i++)
            //{

            //TestBehavior.boxcolider[i].enabled = false;
            //}
        }
        Hit();
    }

    public void Hit()
    {
        //Debug.Log(this + "�׾����ϴ�");
        //��ü ������ ��Ÿ ���...



        Invoke("HitEvent", 1f);
    }

    public void HitEvent()
    {
        TestBehavior.m_Animator.SetBool("Hit", false);
        //TestBehavior.m_Animator.SetBool("Chase", true);

        TestBehavior.m_Animator.SetBool("Attack", true);
        //player.npcDied = true;
        //Destroy(this.gameObject);
    }

    public void Died()
    {
            ChangeLayersRecursively(transform, "DeadNPC");
            ChangeTagRecursively(this.gameObject, "DeadNPC");
    }

    /// <summary>
    /// ����Լ��� �ڽ��� ã�´� �̷�...������ �ݶ��̴� �밡�ٰ� �� ������
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="layerName"></param>
    public void ChangeLayersRecursively(Transform parent, string layerName)
    {
        // �θ� ������Ʈ�� ���̾� ����
        parent.gameObject.layer = LayerMask.NameToLayer(layerName);

        // ��� �ڽ� ������Ʈ�� ���� ��������� ���̾� ����
        foreach (Transform child in parent)
        {
            ChangeLayersRecursively(child, layerName);
        }
    }

    public void ChangeTagRecursively(GameObject obj, string tag)
    {
        if (obj != null)
        {
            obj.tag = tag;
            foreach (Transform child in obj.transform)
            {
                ChangeTagRecursively(child.gameObject, tag);
            }
        }
    }
}

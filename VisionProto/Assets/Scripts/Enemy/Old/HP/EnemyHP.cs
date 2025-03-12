using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 적의 피해와 사망처리를 하는 코드
/// 
/// 김예리나 작성
/// </summary>
public class EnemyHP : MonoBehaviour, IDamageable
{
    public int HP;
    public Player player;
    public TestBehavior TestBehavior; //이거 여기있으면 안되는데 ㅋㅋㅋ 
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
            ///ㅇㅣ것도 실패지만 성공의 길이 되었다.
            //gameObject.layer = LayerMask.NameToLayer("DeadNPC");
            ///이것도 실패다. 모두 바뀜
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
            ///실패다. 왜냐면 바닥까지 뚫고...그렇게 되어버림
            ///레이어 변경을 해야한다.근데 
            //for (int i = 0; i<TestBehavior.boxcolider.Length; i++)
            //{

            //TestBehavior.boxcolider[i].enabled = false;
            //}
        }
        Hit();
    }

    public void Hit()
    {
        //Debug.Log(this + "죽었습니다");
        //객체 삭제나 기타 등등...



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
    /// 재귀함수로 자식을 찾는다 이런...하지만 콜라이더 노가다가 더 귀찮다
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="layerName"></param>
    public void ChangeLayersRecursively(Transform parent, string layerName)
    {
        // 부모 오브젝트의 레이어 변경
        parent.gameObject.layer = LayerMask.NameToLayer(layerName);

        // 모든 자식 오브젝트에 대해 재귀적으로 레이어 변경
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

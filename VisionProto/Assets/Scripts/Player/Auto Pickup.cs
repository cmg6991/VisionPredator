using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class AutoPickup : MonoBehaviour, IListener
{
    // gun container의 자식을 확인한다.
    public GameObject gunContainer;

    // auto Pickup이 가능하냐 안하냐
    public bool isSetAutoPickup;

    private List<GameObject> gunObject = new List<GameObject>();

    // 살아있다.
    public bool isActive;
    public bool isVPState;
    private bool isEmptyBullet;
    private bool isEquiped;
    private SphereCollider sphereCollider;

    // Auto Pickup을 Trigger로 해야 하는데 총들은 Collider로 되어 있다.
    // 음.. boxCollider를 이용한 거를 Trigger로 활용하면 될 거 같다.
    LayerMask gunLayer;

    // VP 상태일 때 확인해야 해

    private void Start()
    {
        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
        EventManager.Instance.AddEvent(EventType.isEmptyBullet, OnEvent);
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
        gunLayer = LayerMask.GetMask("Gun");
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        if (!isActive || isVPState)
            return;

        if (gunObject.Count > 0 && !isEquiped)
        {
            // 안에 있으면 LINQ를 이용해서 거리 판별 후에 그것을 잡도록 하자.
            var sortedObjects = gunObject.OrderBy(obj => Vector3.Distance(this.transform.position, obj.transform.position)).ToList();
            GameObject gun = null;

            // 순회 해야해 하나만 검사하는 것이 아니야 empty까지 확인하는 거야.

            // 밑에는 enable이 중요한게 아니야 저건 해당 총을 줍기 위해 필요한거지
            // 중요한건 순회했는데 empty가 false가 나오는 거야

            // Player 거리에 따라 정렬된 총들이 들어간다.

            foreach (var obj in sortedObjects) 
            {
                // 주변에 아무 총도 없으면 전에 등록되었던 총이 계속 들어가는데,
                // 활성화 되어있지 않다면 없는거다. 계속 넘기자. -> 근데 전에 이미 등록된 것
                if (!obj.activeSelf)
                    continue;

                // 던지고 나서 사라지면 false로 고정해놔서 전에 감지됐던 총이 들어온다.
                EventManager.Instance.NotifyEvent(EventType.GunInformation, obj);
                if (isEmptyBullet)
                {
                    isEmptyBullet = false;
                    continue;
                }
                else
                {
                    // 가까운 gun을 잡았는데 벽 뒤에 있다? 그럼 줍기 ㄴㄴ혓 다음거 줍자.
                    // Ray를 쏠건데 npc 감지 ㄴㄴ, dector 감지 ㄴㄴ, object도 감지 ㄴㄴ

                    gun = obj;
                    Ray ray = new Ray(this.transform.parent.position, 
                            (gun.transform.position - this.transform.parent.position).normalized);
                    LayerMask layermask = ~LayerMask.GetMask("NPC", "Player", "DeadNPC", "Default");

                    RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, layermask);
                    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                    int hitlayer = hits[0].transform.gameObject.layer;
                    int resultLayer = (int)Mathf.Pow(2, hitlayer);

                    if (gunLayer == resultLayer)
                    {
                        gun = obj;
                        break;
                    }
                    else
                    {
                        gun = null;
                        continue;
                    }
                }
            }


            if (gun != null)
            {
                // gun의 script를 활성화하면 된다.
                MonoBehaviour[] gunScripts = gun.GetComponentsInParent<MonoBehaviour>();

                foreach (var script in gunScripts)
                {
                    // Statemachine에서 총을 장착하고 있다고 알려줘야 할 것 같은데?
                    // Script 상에서 true로 바뀌면 바로 바뀌나? 그건 아닐듯?
                    script.enabled = true;
                    isEquiped = true;
                    // 잠시만요 status에 있는 저거 활성화하려면 pickup 활성화 해줘야 하는거 아니야?
                    EventManager.Instance.NotifyEvent(EventType.isEquiped, isEquiped);
                }
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        // Gun Container가 비어있다면 Auto Pickup이 가능해진다.
        // 근처 총 Collider를 가져온다. Gun Layer를 통해 가져 오도록 한다.
        // 그리고 각 거리를 계산한다. 


        // 1. 총을 들고 있나? 있으면 Return, 주변에 총이 없어도 Clear를 해야한다.
        if (isEquiped)
        {
            gunObject.Clear();
            return;
        }

        // 2. 주위에 총만 집는다.
        // Layer를 추가하면 여러 검사하는 것들을 막을 수 있지 않을까?
        // 순회를 적게 할 수 있지 않을까?

        // layer pow 사용해서 체크해야 되지 않았나

        int layer = collision.gameObject.layer;
        int powLayer = (int)Mathf.Pow(2, layer);

        if (powLayer != gunLayer)
            return;

        // 3. Layer까지 체크했으면 총이 남는데 총 안에 있는 총알까지 확인해야 하는데 어떻게 확인하지?
        // 최초에 총을 안 집고 있으면 스크립트가 켜질일이 없다.
        // 그럼 총알이 Full이라는 뜻 그건 상관 없고
        // 최초의 총을 집고 다른 총을 고르면 현재 총알 개수를 알 수 있다.
        // 그러면 여기서 Event Manager를 만들고 총알이 비어있는지 차있는지 알려면 이벤트를 이용해야 할 거 같다.

        // 이제 총알만 비어 있는 것을 확인하면 될 것 같다.

        // Notify로 알린다..? 다른 방법을 찾아봐야 할 수도 있다.
        //EventManager.Instance.NotifyEvent(EventType.GunInformation, collision.gameObject);

        // 된 줄 알았는데 아니다. 새로 들어온 것들을 체크해야 해서 다시 true로 바꾼다.
        // 그렇다면.. 일단 총이란 총들은 다 가지고 오고
        // 위에서 검사를 실시할까?



        // 4. 총알까지 확인한 총을 제외하고 근처 총을 집는다.

        // List에 추가한다. 중복 값이 있으면 Add를 하면 안된다.
        // List 말고 Hash Set을 사용하면 중복 확인이 더 빠르다. O(1)이 걸리기 때문이다.
       
        bool isDuplicate = gunObject.Any(obj => obj == collision.gameObject);

        if (!isDuplicate)
            gunObject.Add(collision.gameObject);


        // List에 담아두고 거리를 계산한 뒤 가까운 것을 집는다.
        // 교체중일 때는 Auto Pickup이 활성화 되면 안 된다. 
    }
    
    public void GunListClear()
    {
        gunObject.Clear();
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        // 여기서는 총이 비었는지 안 비었는지 확인하고 싶어.
        switch (eventType)
        {
            case EventType.isEquiped:
                {
                    isEquiped = (bool)param;
                }
                break;
            case EventType.isEmptyBullet:
                {
                    //ThrowWeapon throwWeapon = (ThrowWeapon)param;
                    
                    // 음.. 무언가.. 필요해
                    // 총을 다 사용하고 던지면 isEmptBullet이 true로 고정이 되서 다음 총을 못 집는 현상 발생
                    // 그렇다면 Drop 하냐 마냐까지 정할까? Drop 했을 때 몇 초후에 isEmptyBullet을 false로 해서
                    // 던질 때 바로 잡기가 불가능하게 하자.

                    // 총알의 empty 유무를 확인
                    isEmptyBullet = (bool)param;

                    // throw 유무를 확인해서 몇 초후에 isEmptyBullet을 false로 만든다.

//                     if (throwWeapon.isThrow)
//                         Invoke("ThrowingWeapon", 0.3f);
                }
                break;
            case EventType.VPState:
                {
                    isVPState = (bool)param;

                    if (isVPState)
                        sphereCollider.enabled = false;
                    else
                        sphereCollider.enabled = true;
                    
                }
                break;
        }
    }

}

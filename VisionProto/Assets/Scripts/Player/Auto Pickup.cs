using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class AutoPickup : MonoBehaviour, IListener
{
    // gun container�� �ڽ��� Ȯ���Ѵ�.
    public GameObject gunContainer;

    // auto Pickup�� �����ϳ� ���ϳ�
    public bool isSetAutoPickup;

    private List<GameObject> gunObject = new List<GameObject>();

    // ����ִ�.
    public bool isActive;
    public bool isVPState;
    private bool isEmptyBullet;
    private bool isEquiped;
    private SphereCollider sphereCollider;

    // Auto Pickup�� Trigger�� �ؾ� �ϴµ� �ѵ��� Collider�� �Ǿ� �ִ�.
    // ��.. boxCollider�� �̿��� �Ÿ� Trigger�� Ȱ���ϸ� �� �� ����.
    LayerMask gunLayer;

    // VP ������ �� Ȯ���ؾ� ��

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
            // �ȿ� ������ LINQ�� �̿��ؼ� �Ÿ� �Ǻ� �Ŀ� �װ��� �⵵�� ����.
            var sortedObjects = gunObject.OrderBy(obj => Vector3.Distance(this.transform.position, obj.transform.position)).ToList();
            GameObject gun = null;

            // ��ȸ �ؾ��� �ϳ��� �˻��ϴ� ���� �ƴϾ� empty���� Ȯ���ϴ� �ž�.

            // �ؿ��� enable�� �߿��Ѱ� �ƴϾ� ���� �ش� ���� �ݱ� ���� �ʿ��Ѱ���
            // �߿��Ѱ� ��ȸ�ߴµ� empty�� false�� ������ �ž�

            // Player �Ÿ��� ���� ���ĵ� �ѵ��� ����.

            foreach (var obj in sortedObjects) 
            {
                // �ֺ��� �ƹ� �ѵ� ������ ���� ��ϵǾ��� ���� ��� ���µ�,
                // Ȱ��ȭ �Ǿ����� �ʴٸ� ���°Ŵ�. ��� �ѱ���. -> �ٵ� ���� �̹� ��ϵ� ��
                if (!obj.activeSelf)
                    continue;

                // ������ ���� ������� false�� �����س��� ���� �����ƴ� ���� ���´�.
                EventManager.Instance.NotifyEvent(EventType.GunInformation, obj);
                if (isEmptyBullet)
                {
                    isEmptyBullet = false;
                    continue;
                }
                else
                {
                    // ����� gun�� ��Ҵµ� �� �ڿ� �ִ�? �׷� �ݱ� ������ ������ ����.
                    // Ray�� ��ǵ� npc ���� ����, dector ���� ����, object�� ���� ����

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
                // gun�� script�� Ȱ��ȭ�ϸ� �ȴ�.
                MonoBehaviour[] gunScripts = gun.GetComponentsInParent<MonoBehaviour>();

                foreach (var script in gunScripts)
                {
                    // Statemachine���� ���� �����ϰ� �ִٰ� �˷���� �� �� ������?
                    // Script �󿡼� true�� �ٲ�� �ٷ� �ٲ? �װ� �ƴҵ�?
                    script.enabled = true;
                    isEquiped = true;
                    // ��ø��� status�� �ִ� ���� Ȱ��ȭ�Ϸ��� pickup Ȱ��ȭ ����� �ϴ°� �ƴϾ�?
                    EventManager.Instance.NotifyEvent(EventType.isEquiped, isEquiped);
                }
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        // Gun Container�� ����ִٸ� Auto Pickup�� ����������.
        // ��ó �� Collider�� �����´�. Gun Layer�� ���� ���� ������ �Ѵ�.
        // �׸��� �� �Ÿ��� ����Ѵ�. 


        // 1. ���� ��� �ֳ�? ������ Return, �ֺ��� ���� ��� Clear�� �ؾ��Ѵ�.
        if (isEquiped)
        {
            gunObject.Clear();
            return;
        }

        // 2. ������ �Ѹ� ���´�.
        // Layer�� �߰��ϸ� ���� �˻��ϴ� �͵��� ���� �� ���� ������?
        // ��ȸ�� ���� �� �� ���� ������?

        // layer pow ����ؼ� üũ�ؾ� ���� �ʾҳ�

        int layer = collision.gameObject.layer;
        int powLayer = (int)Mathf.Pow(2, layer);

        if (powLayer != gunLayer)
            return;

        // 3. Layer���� üũ������ ���� ���µ� �� �ȿ� �ִ� �Ѿ˱��� Ȯ���ؾ� �ϴµ� ��� Ȯ������?
        // ���ʿ� ���� �� ���� ������ ��ũ��Ʈ�� �������� ����.
        // �׷� �Ѿ��� Full�̶�� �� �װ� ��� ����
        // ������ ���� ���� �ٸ� ���� ���� ���� �Ѿ� ������ �� �� �ִ�.
        // �׷��� ���⼭ Event Manager�� ����� �Ѿ��� ����ִ��� ���ִ��� �˷��� �̺�Ʈ�� �̿��ؾ� �� �� ����.

        // ���� �Ѿ˸� ��� �ִ� ���� Ȯ���ϸ� �� �� ����.

        // Notify�� �˸���..? �ٸ� ����� ã�ƺ��� �� ���� �ִ�.
        //EventManager.Instance.NotifyEvent(EventType.GunInformation, collision.gameObject);

        // �� �� �˾Ҵµ� �ƴϴ�. ���� ���� �͵��� üũ�ؾ� �ؼ� �ٽ� true�� �ٲ۴�.
        // �׷��ٸ�.. �ϴ� ���̶� �ѵ��� �� ������ ����
        // ������ �˻縦 �ǽ��ұ�?



        // 4. �Ѿ˱��� Ȯ���� ���� �����ϰ� ��ó ���� ���´�.

        // List�� �߰��Ѵ�. �ߺ� ���� ������ Add�� �ϸ� �ȵȴ�.
        // List ���� Hash Set�� ����ϸ� �ߺ� Ȯ���� �� ������. O(1)�� �ɸ��� �����̴�.
       
        bool isDuplicate = gunObject.Any(obj => obj == collision.gameObject);

        if (!isDuplicate)
            gunObject.Add(collision.gameObject);


        // List�� ��Ƶΰ� �Ÿ��� ����� �� ����� ���� ���´�.
        // ��ü���� ���� Auto Pickup�� Ȱ��ȭ �Ǹ� �� �ȴ�. 
    }
    
    public void GunListClear()
    {
        gunObject.Clear();
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        // ���⼭�� ���� ������� �� ������� Ȯ���ϰ� �;�.
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
                    
                    // ��.. ����.. �ʿ���
                    // ���� �� ����ϰ� ������ isEmptBullet�� true�� ������ �Ǽ� ���� ���� �� ���� ���� �߻�
                    // �׷��ٸ� Drop �ϳ� ���ı��� ���ұ�? Drop ���� �� �� ���Ŀ� isEmptyBullet�� false�� �ؼ�
                    // ���� �� �ٷ� ��Ⱑ �Ұ����ϰ� ����.

                    // �Ѿ��� empty ������ Ȯ��
                    isEmptyBullet = (bool)param;

                    // throw ������ Ȯ���ؼ� �� ���Ŀ� isEmptyBullet�� false�� �����.

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

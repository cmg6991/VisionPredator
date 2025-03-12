using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VPWeaponCollider : MonoBehaviour, IListener
{

    private BoxCollider boxCollider;
    [SerializeField]
    private int damaged = 30;
    public bool isVPState;

    private GameObject targetGameObject;
    private string objectTag;
    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        EventManager.Instance.AddEvent(EventType.VPWeaponInformation, OnEvent);
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        // 단검, VP Weapon 데미지 처리는 어떻게 ?
        if (other.gameObject.CompareTag("NPC"))
        {

            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            // 인식이 안 되는게 맞다. 왜냐하면 Ray를 쏘려는데 피봇 위치가 아래에 있어서 맞지 않는다. 그래서 안 되는 거였다.
            if(other.gameObject.name == "bossRobot")
            {
                if (damageable != null)
                {
                    damageable.Damaged(damaged, transform.position, transform.position, this.gameObject);
                    return;
                }
            }

            // Target 부터 Ray를 쏜다. 닿으면 데미지 처리 
            ObjectInteraction(other.gameObject.transform.position);

            damageable = other.gameObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();

                if (enemyController != null)
                {
                    if (!enemyController.istestDetected)
                        return;
                }

                if (objectTag == "NPC")
                {
                    damageable.Damaged(damaged, transform.position, transform.position, this.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            // Target 부터 Ray를 쏜다. 닿으면 데미지 처리 
            targetGameObject = default;
            objectTag = default;
            layerMask = default;
        }
    }


    private void ObjectInteraction(Vector3 targetPosition)
    {
        //Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        LayerMask npcDectorLayerMask = ~LayerMask.GetMask("Default", "StackingCamera", "Player", "DeadNPC", "StencilNPC", "NPC", "Gun", "CCTVArea", "EBullet", "Melee");

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        Camera cinemachineCamera = null;

        if (brain != null)
            cinemachineCamera = brain.OutputCamera;

        RaycastHit hit;

        // camera에서 viewport Point를 가져온다.
        Vector3 viewportPoint = cinemachineCamera.WorldToViewportPoint(targetPosition);

        // viewport 에서 Ray를 쏘고 그린다.
        Ray ray = cinemachineCamera.ViewportPointToRay(viewportPoint);

        if (Physics.Raycast(ray, out hit, 1000f, npcDectorLayerMask))
        {
            if (cinemachineCamera != null)
            {
                // Ray 쏜 물체들을 모두 담는다. 이상한 것 까지 다 담는다.
                RaycastHit[] hits = Physics.RaycastAll(ray, 100f, npcDectorLayerMask);

                if (hits.Length == 0)
                    return;

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                // 순회하면서 Grappling Point나 Grappling에 닿아야 하는데?
                int layer = hits[0].transform.gameObject.layer;

                Debug.Log(hits[0].transform.gameObject + " VP Attack Layer");

                targetGameObject = hits[0].transform.gameObject;
                objectTag = hits[0].transform.gameObject.tag;
                layerMask = (int)Mathf.Pow(2, layer);
            }
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPWeaponInformation:
                {
                    VPWeaponInformation vpWeaponInformation = (VPWeaponInformation)param;
                    damaged = vpWeaponInformation.damaged;
                    boxCollider.enabled = vpWeaponInformation.isEnableCollider;
                    boxCollider.isTrigger = vpWeaponInformation.isTrueTrigger;
                }
                break;
        }
    }

}

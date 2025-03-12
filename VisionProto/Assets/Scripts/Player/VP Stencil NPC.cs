using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;


public class VPStencilNPC : MonoBehaviour
{
    private int npcLayer;
    private int npcStencilLayer;

    private int objectLayer;
    private int stencilObjectLayer;

    private int npcNormalLayer;

    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();

        // Layer ����.
        int powLayer = LayerMask.GetMask("NPC");
        int powStencilLayer = LayerMask.GetMask("StencilNPC");

        int powObjectLayer = LayerMask.GetMask("Object");
        int powStencilObjectLayer = LayerMask.GetMask("StencilObject");

        int powNormalLayer = LayerMask.GetMask("NormalNPC");

        npcLayer = (int)Mathf.Ceil(Mathf.Log(powLayer) / Mathf.Log(2));
        npcStencilLayer = (int)Mathf.Ceil(Mathf.Log(powStencilLayer) / Mathf.Log(2));

        objectLayer = (int)Mathf.Ceil(Mathf.Log(powObjectLayer) / Mathf.Log(2));
        stencilObjectLayer = (int)Mathf.Ceil(Mathf.Log(powStencilObjectLayer) / Mathf.Log(2));

        npcNormalLayer = (int)Mathf.Ceil(Mathf.Log(powNormalLayer) / Mathf.Log(2));
    }

    private void Update()
    {
        if(this.gameObject != null)
            this.transform.position = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����� �� NPC Layer�� ��ģ��. �� VP State�� ���� Ȱ��ȭ �Ѵ�.

        // Tag�� ���ؼ� ��ȣ�ۿ��� ������ Object�� �ٲٱ� Layer
        if (other.gameObject.CompareTag("GrapplingPoint"))
            other.gameObject.layer = stencilObjectLayer;

        if (other.gameObject.CompareTag("Item") || other.gameObject.CompareTag("Cabinet") || other.gameObject.CompareTag("Button"))
            SetLayerRecursively(other.gameObject, stencilObjectLayer);


        // VP State�� NPC �±װ� ���� �ٲ��� �Ѵ�. Normal NPC -> Stencil NPC
        if (other.gameObject.layer == npcNormalLayer)
        {
            SetLayerRecursively(other.gameObject, npcStencilLayer);
            other.gameObject.layer = npcNormalLayer;
        }
        //other.gameObject.layer = npcStencilLayer;

    }

    private void OnTriggerStay(Collider other)
    {
        // �߰��� VP ���¸� Ǯ������ StencilLayer ���� layer�� ���� NPC�� �ٲ۴�. -> ���߿� ������ ����� �׶� �ٲ���.
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == stencilObjectLayer)
            SetLayerRecursively(other.gameObject, objectLayer);

        // Normal NPC -> npcLayer
        if (other.gameObject.layer == npcNormalLayer)
        {
            SetLayerRecursively(other.gameObject, npcLayer);
            other.gameObject.layer = npcNormalLayer;
        }
    }

    // �ڽĵ��� ��ȸ�ϸ鼭 npc�� Tag�� �ٲ۴�.
    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj.name == "BossPivot" || obj.name == "BossPivotTwo")
        {
            return;
        }
        obj.layer = layer;
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
            objRenderer.forceRenderingOff = false;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
        //obj.layer = layer;
        //Renderer objRenderer = obj.gameObject.GetComponent<Renderer>();
        //if (objRenderer != null)
        //    objRenderer.forceRenderingOff = false;

        //foreach (Transform child in obj.transform)
        //{


        //    SetLayerRecursively(child.gameObject, layer);
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPlayer : MonoBehaviour
{
    // �ε����� ������ ����� ���� on off �� �� �ִ�.
    public bool isActiveGrappling;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActiveGrappling)
            return;

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") 
            || collision.gameObject.CompareTag("GrapplingPoint") || collision.gameObject.CompareTag("Grappling"))
        {
            EventManager.Instance.NotifyEvent(EventType.Hooking);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isActiveGrappling)
            return;

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("GrapplingPoint") || collision.gameObject.CompareTag("Grappling"))
        {
            EventManager.Instance.NotifyEvent(EventType.Hooking);
        }
    }
}

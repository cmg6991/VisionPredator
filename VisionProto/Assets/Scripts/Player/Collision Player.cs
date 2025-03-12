using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPlayer : MonoBehaviour
{
    // 부딪히면 갈고리가 끊기는 것을 on off 할 수 있다.
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

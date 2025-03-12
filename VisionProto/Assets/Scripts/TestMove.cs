using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public Rigidbody rb;

    private void FixedUpdate()
    {
        rb.MovePosition(rb.transform.position + transform.forward*2f*Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepupStair : MonoBehaviour
{
    // Player Climb
    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;
    public float muldeltaTime = 0.5f;

    public float mindistance = 0.1f;
    public float maxdistance = 0.2f;

    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        stepClimb();
    }

    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, mindistance))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, maxdistance))
            {
                /// Rigidbody가 잘 작동이 안 되어서 일단 Transform으로 했다.
                /// Rigidbody로 시도해보자.

                //                 {
                //                     Vector3 preTransform = transform.position;
                //                     Vector3 currentTransform = transform.position -= new Vector3(0f, -stepSmooth, 0f);
                //                 
                //                     transform.position = Vector3.Lerp(preTransform, currentTransform, Time.deltaTime * muldeltaTime);
                //                 }

                rigidbody.AddForce(Vector3.up * muldeltaTime, ForceMode.Impulse);

            }
        }
    }
}

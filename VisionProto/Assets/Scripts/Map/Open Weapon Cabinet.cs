using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeaponCabinet : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject mainDoor;

    public GameObject leftLight;
    public GameObject rightLight;

    public bool isControl;

    private Quaternion leftDoorOpen;
    private Quaternion rightDoorOpen;
    
    public float doorSpeed;
    private bool isOpen;
   

    int wallLayer;
    private void Start()
    {
        leftDoorOpen = Quaternion.Euler(0, 90f, 0);
        rightDoorOpen = Quaternion.Euler(0, -90f, 0);
        isOpen = false;
        int powLayer = LayerMask.GetMask("Wall");
        wallLayer = (int)Mathf.Ceil(Mathf.Log(powLayer) / Mathf.Log(2));
    }

    // Update is called once per frame
    void Update()
    {
        if(isControl)
        {
            // left door는 90
            leftDoor.transform.localRotation = Quaternion.Lerp(leftDoor.transform.localRotation, leftDoorOpen, Time.deltaTime * (doorSpeed));
            // right door는 -90
            rightDoor.transform.localRotation = Quaternion.Lerp(rightDoor.transform.localRotation, rightDoorOpen, Time.deltaTime * (doorSpeed));

            if(!isOpen)
            {
                // 여기서 OutlineObjecte에서 isDone도 True로 하면 좋겠다.
                leftLight.SetActive(true); 
                rightLight.SetActive(true);

                leftDoor.GetComponent<InteractionObject>().isDone = true;
                mainDoor.GetComponent<InteractionObject>().isDone = true;
                rightDoor.GetComponent<InteractionObject>().isDone = true;

                leftDoor.tag = "Wall";
                rightDoor.tag = "Wall";
                mainDoor.tag = "Wall";
                leftDoor.layer = wallLayer;
                rightDoor.layer = wallLayer;
                mainDoor.layer = wallLayer;
                isOpen = true;
            }
        }
    }
}

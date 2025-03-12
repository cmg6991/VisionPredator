using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTutorialDoor : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(leftDoor != null && rightDoor != null)
            {
                OpenTutorialDoor tutorialDoor;

                tutorialDoor = leftDoor.gameObject.GetComponent<OpenTutorialDoor>();
                if (tutorialDoor != null)
                    tutorialDoor.CloseDoor();

                tutorialDoor = rightDoor.gameObject.GetComponent<OpenTutorialDoor>();

                if (tutorialDoor != null)
                    tutorialDoor.CloseDoor();
            }
        }
    }
}

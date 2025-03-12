using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(leftDoor != null && rightDoor != null) 
            {
                OpenDoorAType doorAType = leftDoor.GetComponent<OpenDoorAType>();

                if(doorAType != null) 
                    doorAType.isEnemyContact = true;

                doorAType = rightDoor.GetComponent<OpenDoorAType>();

                if(doorAType != null) 
                    doorAType.isEnemyContact = true;
            }
        }
    }

}

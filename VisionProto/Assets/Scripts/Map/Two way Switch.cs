using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwowaySwitch : MonoBehaviour
{
    public bool isClose;

    public List<GameObject> areaCEnemys;

    private List<GameObject> triggerEnterEnemys = new List<GameObject>();

    private GameObject enemy;

    private bool isDone;
    private void Update()
    {
        if(isDone) return;

        if (!isClose)
            return;
        else
        {
            if(triggerEnterEnemys.Count > 0) 
            {
                foreach (GameObject area in triggerEnterEnemys)
                {
                    EventManager.Instance.NotifyEvent(EventType.NPCDeath, area);
                }

                isDone = true;
                this.enabled = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("NPC"))
        {
            foreach (GameObject area in areaCEnemys)
            {
                if (other.gameObject == area)
                {
                    enemy = area;
                    break;
                }
            }

            if(enemy != null)
                triggerEnterEnemys.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            foreach (GameObject area in areaCEnemys)
            {
                if (other.gameObject == area)
                {
                    enemy = area;
                    break;
                }
            }

            if (enemy != null)
                triggerEnterEnemys.Remove(enemy);
        }
    }
}

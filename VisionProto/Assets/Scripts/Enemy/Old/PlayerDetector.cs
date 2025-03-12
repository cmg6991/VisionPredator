using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public bool detectedPlayer = false;
    public GameObject player;
    private int previousNPCCount = 0;
    GameObject[] allNPCs;
    public void OnTriggerStay(Collider other)   
    {
        if (other.CompareTag("Player"))
        {
            if (!detectedPlayer)
            {
                Debug.Log("플레이어가 안에 있다능!");
                detectedPlayer = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 나갔다능!");
            detectedPlayer = false;
        }
    }

    private void Update()
    {
        if(detectedPlayer)
        {
            allNPCs = GameObject.FindGameObjectsWithTag("NPC");
            Debug.Log(previousNPCCount);
            Debug.Log(allNPCs.Length);

            if (allNPCs.Length != previousNPCCount)
            {
                previousNPCCount = allNPCs.Length;

                var sortedNPCs = allNPCs
               .Select(npc => new { NPC = npc, Behavior = npc.GetComponent<TestBehavior>() })
               .Where(n => n.Behavior != null && !n.Behavior.isSniping)
               .Select(n => new { n.NPC, n.Behavior, Distance = Vector3.Distance(player.transform.position, n.NPC.transform.position) })
               .OrderBy(n => n.Distance)
               .ToList();

                var selectedNPCs = sortedNPCs.Take(3).ToList();

                var notSelectedNPCs = sortedNPCs.Skip(3).ToList();

                foreach (var npc in selectedNPCs)
                {
                    TestBehavior behavior = npc.NPC.GetComponent<TestBehavior>();
                    if (behavior != null)
                    {
                        behavior.wannaAttack = true;
                    }
                }

                foreach (var npc in notSelectedNPCs)
                {
                    TestBehavior behavior = npc.NPC.GetComponent<TestBehavior>();
                    if (behavior != null)
                    {
                        behavior.wannaAttack = false;
                    }
                }
            }
        }


        

    }
}

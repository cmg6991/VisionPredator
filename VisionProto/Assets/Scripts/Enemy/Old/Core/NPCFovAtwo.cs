using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFovAtwo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        //2Â÷ ¹üÀ§
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 30);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, -transform.forward * 10);

        ////OJIMA
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, 20);
    }
}

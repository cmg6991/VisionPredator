using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SavePoint : MonoBehaviour
{
    public AreaMap saveAreaMap;
    public GameObject savePoint;

    private void OnTriggerEnter(Collider collision)
    {
        // Tag로 Player면 저장하자.
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInformation playerInfo = new PlayerInformation();
            playerInfo.position = savePoint.transform.position;
            playerInfo.rotation = Quaternion.LookRotation(savePoint.transform.forward);

            DataManager.Instance.SaveData(playerInfo);
            DataManager.Instance.currentSaveMap = saveAreaMap;
        }
    }
}

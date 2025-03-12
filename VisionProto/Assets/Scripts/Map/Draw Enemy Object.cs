using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEnemyObject : MonoBehaviour
{
    // Save Point���� ������Ǿ��� ���� �ڿ� �ִ� Enemy���� �׷����� �� �ȴ�.
    private AreaMap areaMap;

    public GameObject[] area_A_Enemy;
    public GameObject[] area_B_Enemy;
    public GameObject[] area_C_Enemy;
    public GameObject[] area_D_Enemy;
    public GameObject[] area_E_Enemy;
    public GameObject[] area_F_Enemy;
    public GameObject[] area_G_Enemy;

    // Area A 

    void Start()
    {
        areaMap = DataManager.Instance.currentSaveMap;

        if(areaMap != AreaMap.None)
            ClearEnemyFalse(areaMap);
    }

    // ��ȸ�� Object�� �ִ´�.
    private void RoundsArray(GameObject[] gameObjects)
    {
        foreach(GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    private void ClearEnemyFalse(AreaMap areaMap)
    {
        // ������ ���� Enemy���� ��Ȱ��ȭ �Ѵ�. 
        switch(areaMap) 
        {
            case AreaMap.A:
                RoundsArray(area_A_Enemy);
                break;
            case AreaMap.B:
                RoundsArray(area_A_Enemy);
                RoundsArray(area_B_Enemy);
                break;
            case AreaMap.C:
                RoundsArray(area_B_Enemy);
                RoundsArray(area_C_Enemy);
                break;
            case AreaMap.D:
                RoundsArray(area_C_Enemy);
                RoundsArray(area_D_Enemy);
                break;
            case AreaMap.F:
                RoundsArray(area_D_Enemy);
                RoundsArray(area_F_Enemy);
                break;
            case AreaMap.G:
                RoundsArray(area_E_Enemy);
                RoundsArray(area_F_Enemy);
                RoundsArray(area_G_Enemy);
                break;
            case AreaMap.H:
                RoundsArray(area_F_Enemy);
                RoundsArray(area_G_Enemy);
                break;
        }
    }
}

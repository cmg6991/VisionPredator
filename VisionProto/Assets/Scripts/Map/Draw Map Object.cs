using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaMap
{
    A, 
    B, 
    C, 
    D, 
    E, 
    F, 
    G, 
    H,
    None,
};

public class DrawMapObject : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public List<GameObject> enemyObjects;
    public AreaMap areaMap;

    private void ActiveMap(AreaMap map)
    {
        // 순회하면서 -1 ~ +1에 해당하는 obj만 활성화하고 나머지는 비활성화 시키고싶다.
        
        // 0번째일 때는 0 - 1 실행
        // 마지막일 때는 max max - 1 실행
        int mapIndex = (int)map;

        for (int i = 0; i < gameObjects.Count; i++) 
        {
            if(mapIndex -1 <= i && i <= mapIndex + 1 && 0 <= i && i < gameObjects.Count)
            {
                gameObjects[i].SetActive(true);
                enemyObjects[i].SetActive(true);
            }
            else
            {
                gameObjects[i].SetActive(false);
                enemyObjects[i].SetActive(false);
            }
        }

        // DataManger
    }

    private void ActiveMap()
    {
        switch(areaMap)
        {
            case AreaMap.A:
                ActiveMap(AreaMap.A);
                break;
            case AreaMap.B:
                ActiveMap(AreaMap.B);
                break;
            case AreaMap.C:
                ActiveMap(AreaMap.C);
                break;
            case AreaMap.D:
                ActiveMap(AreaMap.D);
                break;
            case AreaMap.E:
                ActiveMap(AreaMap.E);
                break;
            case AreaMap.F:
                ActiveMap(AreaMap.F);
                break;
            case AreaMap.G:
                ActiveMap(AreaMap.G);
                break;
            case AreaMap.H:
                ActiveMap(AreaMap.H);
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ActiveMap();
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestItemPool : MonoBehaviour
//{
//    public PoolManager poolManager;
//    public GameObject itemPool;
//    private List<GameObject> itemPools;
//    private int count = 0;

//    // Start is called before the first frame update
//    void Start()
//    {
//        itemPools = new List<GameObject>();

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.K))
//        {
//            AddItem();
//        }

//        if (Input.GetKeyUp(KeyCode.L))
//        {
//            SubItem();
//        }
//    }

//    public void AddItem()
//    {
//        GameObject item = poolManager.GetPooledObject();
//        itemPools.Add(item);
//        count++;
//    }

//    public void SubItem()
//    {
//        count--;
//        if (count >= 0)
//        {
//            poolManager.ReturnToPool(itemPools[0]);
//            itemPools.RemoveAt(0);
//        }
//        else count = 0;
//    }
//}
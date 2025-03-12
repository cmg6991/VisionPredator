using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 김예리나 작성
/// </summary>
public class PoolManager : MonoBehaviour
{
    // 풀링할 프리팹과 초기 풀 크기를 설정하기 위한 구조체
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    // 풀링할 프리팹 목록
    public List<Pool> pools;
    private Dictionary<string, List<GameObject>> poolDictionary;
    public static PoolManager Instance;

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogError($"프리팹 없다.");
                continue;
            }

            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                try
                {
                    GameObject obj = Instantiate(pool.prefab);
                    if (obj == null)
                    {
                        Debug.LogError($"널");
                        continue;
                    }

                    obj.SetActive(false);
                    objectPool.Add(obj);
                }
                catch (Exception e)
                {
                    Debug.LogError($"오류: {e.Message}");
                }
            }

            if (!poolDictionary.ContainsKey(pool.tag))
            {
                poolDictionary.Add(pool.tag, objectPool);
            }
            else
            {
                Debug.LogWarning($"태그오류");
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " 가 없어");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }

    /// <summary>
    /// Object를 Position과 Rotation을 이용해서 생성하는 함수
    /// </summary>
    /// <param name="tag">이름</param>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " 가 없어");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab, position, rotation);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }

    /// <summary>
    /// Object를 Position과 Rotation와 부모의 Transform을 이용해서 생성하는 함수\
    /// </summary>
    /// <param name="tag">이름</param>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <param name="parent">부모</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " 가 없어");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab, position, rotation, parent);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }

    /// <summary>
    /// Object를 부모의 Transform을 이용해서 생성하는 함수
    /// </summary>
    /// <param name="tag">이름</param>
    /// <param name="parent">부모 Transform</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " 가 없어");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab, parent);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }


    public void ReturnToPool(GameObject obj, string tag)
    {
        if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Contains(obj))
        {
            obj.SetActive(false);
        }
        else
        {
            Destroy(obj);
        }
    }
}

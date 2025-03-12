using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �迹���� �ۼ�
/// </summary>
public class PoolManager : MonoBehaviour
{
    // Ǯ���� �����հ� �ʱ� Ǯ ũ�⸦ �����ϱ� ���� ����ü
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    // Ǯ���� ������ ���
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
                Debug.LogError($"������ ����.");
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
                        Debug.LogError($"��");
                        continue;
                    }

                    obj.SetActive(false);
                    objectPool.Add(obj);
                }
                catch (Exception e)
                {
                    Debug.LogError($"����: {e.Message}");
                }
            }

            if (!poolDictionary.ContainsKey(pool.tag))
            {
                poolDictionary.Add(pool.tag, objectPool);
            }
            else
            {
                Debug.LogWarning($"�±׿���");
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
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
    /// Object�� Position�� Rotation�� �̿��ؼ� �����ϴ� �Լ�
    /// </summary>
    /// <param name="tag">�̸�</param>
    /// <param name="position">��ġ</param>
    /// <param name="rotation">ȸ��</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
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
    /// Object�� Position�� Rotation�� �θ��� Transform�� �̿��ؼ� �����ϴ� �Լ�\
    /// </summary>
    /// <param name="tag">�̸�</param>
    /// <param name="position">��ġ</param>
    /// <param name="rotation">ȸ��</param>
    /// <param name="parent">�θ�</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
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
    /// Object�� �θ��� Transform�� �̿��ؼ� �����ϴ� �Լ�
    /// </summary>
    /// <param name="tag">�̸�</param>
    /// <param name="parent">�θ� Transform</param>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // �ٸ� ��ũ��Ʈ���� ������ �� �ִ� �ν��Ͻ�
    public static T Instance
    {
        get
        {
            // �ν��Ͻ��� ������ ���� ����
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // ���� ���� ���� ��� ���� ����
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";

                    // �� ��ȯ �� �ı����� �ʵ��� ����
                    DontDestroyOnLoad(singletonObject);
                }
                
            }
            return instance;
        }
    }

    // ������ �̱��� �ı� �� ȣ��Ǵ� �޼���
    private void OnDestroy()
    {
        instance = null;
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            // ó�� ������ �̱��� �ν��Ͻ���� ����
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������ ������Ʈ�� �ı�
            Destroy(gameObject);
        }
    }
}

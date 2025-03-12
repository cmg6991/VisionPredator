using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // 다른 스크립트에서 접근할 수 있는 인스턴스
    public static T Instance
    {
        get
        {
            // 인스턴스가 없으면 새로 생성
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // 만약 씬에 없을 경우 새로 생성
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";

                    // 씬 전환 시 파괴되지 않도록 설정
                    DontDestroyOnLoad(singletonObject);
                }
                
            }
            return instance;
        }
    }

    // 생성된 싱글톤 파괴 시 호출되는 메서드
    private void OnDestroy()
    {
        instance = null;
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            // 처음 생성된 싱글톤 인스턴스라면 유지
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 오브젝트는 파괴
            Destroy(gameObject);
        }
    }
}

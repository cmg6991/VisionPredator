#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class HierachyObjectCounter : EditorWindow
{
    [MenuItem("Tools/Count Objects in Hierarchy")]
    public static void ShowWindow()
    {
        GetWindow<HierachyObjectCounter>("Object Counter");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Count Objects in Hierarchy"))
        {
            CountObjects();
        }
    }

    void CountObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = allObjects.Length;

        Debug.Log("���̾��Ű�� �ִ� ������Ʈ ����: " + count);
        EditorUtility.DisplayDialog("Object Count", "���̾��Ű�� �ִ� ������Ʈ ����: " + count, "OK");
    }
}
#endif
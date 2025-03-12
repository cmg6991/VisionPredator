#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LightEdit : EditorWindow
{
    [MenuItem("Tools/Scale Lights")]
    public static void ShowWindow()
    {
        GetWindow<LightEdit>("Scale Lights");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scale All Light Multiply 0.1"))
        {
            ScaleAllMinimum();
        }

        if (GUILayout.Button("Scale All Light Multiply 10.0"))
        {
            ScaleAllMaximum();
        }

        if (GUILayout.Button("Scale All Map Multiply 0.1"))
        {
            ScaleMapMinimum();
        }

        if (GUILayout.Button("Scale All Map Multiply 10.0"))
        {
            ScaleMapMaximum();
        }
    }

    private void ScaleMapMinimum()
    {
        GameObject map = GameObject.Find("RE_MAP");

        Undo.RecordObject(map, "Scale Map Properties"); // Undo ����� ���� ����մϴ�.

        map.transform.localScale *= 0.1f;

        EditorUtility.SetDirty(map); // ���� ������ ��ŷ�մϴ�.

        Debug.Log("Scaled all map' local Scale by 0.1.");
    }

    private void ScaleMapMaximum()
    {
        GameObject map = GameObject.Find("RE_MAP");

        Undo.RecordObject(map, "Scale Map Properties"); // Undo ����� ���� ����մϴ�.

        map.transform.localScale *= 10f;

        EditorUtility.SetDirty(map); // ���� ������ ��ŷ�մϴ�.

        Debug.Log("Scaled all map' local Scale by 10.");
    }


    private void ScaleAllMinimum()
    {
        // ���� ���� ��� Light ������Ʈ�� ã���ϴ�.
        Light[] lights = FindObjectsOfType<Light>();

        foreach (Light light in lights)
        {
            if (light.type == LightType.Rectangle || light.type == LightType.Disc)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.areaSize *= 0.1f; // range ���� 0.1�� ���մϴ�.
                light.intensity *= 0.1f;
                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }

            if(light.type == LightType.Point)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.range *= 0.1f;
                light.intensity *= 0.1f;

                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }

            if(light.type == LightType.Spot)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.range *= 0.1f;
                light.intensity *= 0.1f;

                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }
        }

        Debug.Log("Scaled all lights' width and height by 0.1.");
    }

    private void ScaleAllMaximum()
    {
        // ���� ���� ��� Light ������Ʈ�� ã���ϴ�.
        Light[] lights = FindObjectsOfType<Light>();

        foreach (Light light in lights)
        {
            if (light.type == LightType.Rectangle || light.type == LightType.Disc)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.areaSize *= 10f; // range ���� 0.1�� ���մϴ�.
                light.intensity *= 10f;

                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }

            if (light.type == LightType.Point)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.range *= 10f;
                light.intensity *= 10f;

                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }

            if (light.type == LightType.Spot)
            {
                Undo.RecordObject(light, "Scale Light Properties"); // Undo ����� ���� ����մϴ�.

                light.range *= 10f;
                light.intensity *= 10f;

                EditorUtility.SetDirty(light); // ���� ������ ��ŷ�մϴ�.
            }
        }

        Debug.Log("Scaled all lights' width and height by 10.");
    }

}
#endif
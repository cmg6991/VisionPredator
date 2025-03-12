#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SceneParser : EditorWindow
{
    private const string defaultPath = "Assets/Scenes/JsonParserScenes";

    [MenuItem("Predator_Parser/Save Scene Data as JSON")]
    public static void ShowWindow()
    {
        GetWindow<SceneParser>("Scene Parser");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Save Current Scene as JSON"))
        {
            SaveSceneDataAsJSON();
        }
    }

    private void SaveSceneDataAsJSON()
    {
        // Ensure the default directory exists
        if (!Directory.Exists(defaultPath))
        {
            Directory.CreateDirectory(defaultPath);
        }

        string path = EditorUtility.SaveFilePanel(
            "Save Scene Data as",
            defaultPath,
            "SceneData.scene",
            "scene");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Save operation was cancelled.");
            return;
        }

        SceneData sceneData = new SceneData();

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        foreach (var obj in rootObjects)
        {
            ParseGameObjectRecursive(obj, sceneData);
        }

        sceneData.PhysicsInfo.FrameRate = 60;
        sceneData.PhysicsInfo.Gravity = new SimpleVector3(Physics.gravity);

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter> { new MeshComponentConverter() }
        };

        string json = JsonConvert.SerializeObject(sceneData, Formatting.Indented, settings);

        try
        {
            File.WriteAllText(path, json);
            EditorUtility.DisplayDialog("Scene Saved", $"Scene data saved as JSON at {path}\n{sceneData.Entitys.Count} entities were saved.", "OK");
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to save JSON file: {ex.Message}", "OK");
        }
    }

    private void ParseGameObjectRecursive(GameObject obj, SceneData sceneData)
    {
        EntityData entityData = new EntityData();
        entityData.EntityID = (uint)obj.GetInstanceID();

        // TransformComponent
        TransformComponent transformComponent = new TransformComponent
        {
            Local_Location = new SimpleVector3(obj.transform.localPosition),
            Local_Quaternion = new SimpleQuaternion(obj.transform.localRotation),
            Local_Scale = new SimpleVector3(obj.transform.localScale)
        };
        entityData.Component.Add(transformComponent);

        // IDComponent
        IDComponent idComponent = new IDComponent
        {
            Name = obj.name
        };
        entityData.Component.Add(idComponent);

        // Collider/RigidBodyComponent
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            RigidBodyComponent rigidBodyComponent = new RigidBodyComponent
            {
                IsDynamic = obj.GetComponent<Rigidbody>() != null
            };

            if (collider is BoxCollider boxCollider)
            {
                rigidBodyComponent.BoxInfo = new BoxColliderInfo
                {
                    Extent = new SimpleVector3(boxCollider.size)
                };
                rigidBodyComponent.ColliderShape = 0; // BOX

            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                rigidBodyComponent.CapsuleInfo = new CapsuleColliderInfo
                {
                    HalfHeight = capsuleCollider.height / 2,
                    Radius = capsuleCollider.radius
                };
                rigidBodyComponent.ColliderShape = 2; // CAPSULE

            }
            else if (collider is SphereCollider sphereCollider)
            {
                rigidBodyComponent.SphereInfo = new SphereColliderInfo
                {
                    Radius = sphereCollider.radius
                };
                rigidBodyComponent.ColliderShape = 1; // SPHERE

            }
            else if (collider is MeshCollider meshCollider)
            {
                rigidBodyComponent.ColliderShape = 3; // CONVEX

            }

            rigidBodyComponent.DefaultColliderInfo = new ColliderInfo
            {
                AngleLock = new bool[] { false, false, false },
                Density = 1.0f,
                DynamicFriction = 1.0f,
                LinearLock = new bool[] { false, false, false },
                OffSet = new SimpleVector3(Vector3.zero),
                PhysicsLayer = 0,
                Restitution = 0.0f,
                StaticFriction = 1.0f,
                UseGravity = false
            };

            entityData.Component.Add(rigidBodyComponent);
        }

        // ParentComponent
        if (obj.transform.parent != null)
        {
            ParentComponent parentComponent = new ParentComponent
            {
                ParentID = (uint)obj.transform.parent.gameObject.GetInstanceID()
            };
            entityData.Component.Add(parentComponent);
        }

        // ChildComponent
        if (obj.transform.childCount > 0)
        {
            ChildComponent childComponent = new ChildComponent();
            foreach (Transform child in obj.transform)
            {
                childComponent.ChildrenID.Add((uint)child.gameObject.GetInstanceID());
            }
            entityData.Component.Add(childComponent);
        }

        // MeshComponent
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string fbxFileName = Path.GetFileName(assetPath);

                MeshComponent meshComponent = new MeshComponent
                {
                    FBX = fbxFileName, // Set the actual FBX file name
                    Pass = 8
                    //Filter = EMeshFilter.Static // Set the filter using the enum
                };

                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Lightmap settings
                    if (meshRenderer.lightmapIndex >= 0)
                    {
                        meshComponent.LightMapTiling = new SimpleVector2(meshRenderer.lightmapScaleOffset.x, meshRenderer.lightmapScaleOffset.y);
                        meshComponent.LightMapOffset = new SimpleVector2(meshRenderer.lightmapScaleOffset.z, meshRenderer.lightmapScaleOffset.w);
                        meshComponent.LightMapScale = (uint)meshRenderer.lightmapScaleOffset.z; // Assuming LightMapScale represents scale in one dimension
                        meshComponent.LightMapIndex = (uint)meshRenderer.lightmapIndex;
                    }
                    else
                    {
                        meshComponent.LightMapOffset = new SimpleVector2(0, 0);
                        meshComponent.LightMapTiling = new SimpleVector2(1, 1); // Default tiling to 1,1
                        meshComponent.LightMapScale = 0;
                        meshComponent.LightMapIndex = 0;
                    }
                }

                meshComponent.FBXAsIntArray = meshComponent.FBXAsIntArray; // Trigger the conversion
                entityData.Component.Add(meshComponent);
            }
            else
            {
                Debug.LogWarning($"Asset path for mesh in {obj.name} could not be found.");
            }
        }
        else if (meshFilter == null)
        {
            Debug.LogWarning($"{obj.name} does not have a MeshFilter component.");
        }
        else
        {
            Debug.LogWarning($"{obj.name} has a MeshFilter but no mesh assigned.");
        }

        sceneData.Entitys.Add(entityData);

        foreach (Transform child in obj.transform)
        {
            ParseGameObjectRecursive(child.gameObject, sceneData);
        }
    }
}
#endif

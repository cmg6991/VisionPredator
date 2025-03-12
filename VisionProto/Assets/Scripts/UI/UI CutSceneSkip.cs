using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICutSceneSkip : MonoBehaviour
{
    public GameObject cutScene;

    public void CutSceneSkip()
    {
        cutScene.SetActive(false);
    }
}
    

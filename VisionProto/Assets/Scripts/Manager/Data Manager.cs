using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Save Load 구현하기 위해 만든 것이다.
/// Save Load와 게임에 필요한 Data를 담기 위한 곳이다.
/// 게임 성능 향상을 위해 필요한 Date들을 담는 공간이다.
/// 이용성
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private string path;
    public bool isVPState { get; set; }
    public bool isTutorial { get; set; }
    public bool isThrowingWeapon { get; set; }

    public Transform playerPosition { get; set; }

    public AreaMap currentSaveMap;

    public HashSet<string> objectTag;
    public bool isEasyMode = false;
    public bool isNormalMode = false;
    public bool isModeSelect = false;
    public bool isHardMode = false;

    protected override void Awake()
    {
        this.transform.SetParent(null);

        base.Awake();

        path = Application.persistentDataPath + "/";  
        
        objectTag = new HashSet<string>
        {
            "Wall", "Floor", "NPC", "Door", "Grappling", "GrapplingPoint",
            "Cabinet", "Item", "Untagged", "Button", "SitWall"
        };

        currentSaveMap = AreaMap.None;
    }

    // data를 따로 만들 필요는 없다.
    public void SaveData(PlayerInformation _playerinfo)
    {
        string data = JsonUtility.ToJson(_playerinfo);

        File.WriteAllText(path + "PlayerInformation", data);
    }

    // 저장된 Player 정보를 Load 한다.
    public PlayerInformation LoadData()
    {
        string data = default;

        data = File.ReadAllText(path + "PlayerInformation");

        PlayerInformation dataInfomation = JsonUtility.FromJson<PlayerInformation>(data);

        return dataInfomation;
    }

    // Data에는 어떤게 필요할까? -> 현재 위치 정보만 들고 있으면 된다 !
    // 그러면 Transform 말고는 없나? 딱히 뭐 없는듯?
}

public struct PlayerInformation
{
    public Vector3 position;
    public Quaternion rotation;
}
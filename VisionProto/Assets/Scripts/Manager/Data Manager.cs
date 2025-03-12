using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Save Load �����ϱ� ���� ���� ���̴�.
/// Save Load�� ���ӿ� �ʿ��� Data�� ��� ���� ���̴�.
/// ���� ���� ����� ���� �ʿ��� Date���� ��� �����̴�.
/// �̿뼺
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

    // data�� ���� ���� �ʿ�� ����.
    public void SaveData(PlayerInformation _playerinfo)
    {
        string data = JsonUtility.ToJson(_playerinfo);

        File.WriteAllText(path + "PlayerInformation", data);
    }

    // ����� Player ������ Load �Ѵ�.
    public PlayerInformation LoadData()
    {
        string data = default;

        data = File.ReadAllText(path + "PlayerInformation");

        PlayerInformation dataInfomation = JsonUtility.FromJson<PlayerInformation>(data);

        return dataInfomation;
    }

    // Data���� ��� �ʿ��ұ�? -> ���� ��ġ ������ ��� ������ �ȴ� !
    // �׷��� Transform ����� ����? ���� �� ���µ�?
}

public struct PlayerInformation
{
    public Vector3 position;
    public Quaternion rotation;
}
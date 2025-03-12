using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum Effect
{
    BulletDecal,    // �Ѿ� ����
    PistolShot,     // ������ ���� ��
    RifleShot,      // �������� ���� ��
    ShotGunShot,    // ������ ���� ��
    CutDown,        // �������
    Scanner,        // ������� ������ �� ����� Wave Effect
    Dash,           // �뽬 ����Ʈ
    VPSight,        // VP �þ� �����϶� �ֺ��� �Ϸ��̴� ȭ�� Effect
    GunHit,         // ���� ���̳� �ٴڿ� �¾��� ��
    CuttingDecal,   // Į ���� 1
    PiercingDecal,  // Į ���
    KnifeEffect,    // Į ����Ʈ
    EnemyHit,       // ���� �¾��� �� �ߴ� ����Ʈ
    EnemyHit1,      // ���� �¾��� �� �ߴ� ����Ʈ 1
    EnemyHit2,      // ���� �¾��� �� �ߴ� ����Ʈ 2
    
    // Boss Effect
    Shield,         // �ǵ� ����Ʈ
    MachineGunShot, // ������ �ӽŰ��� �� �� ������ �ѱ� ����Ʈ
    MachineGunHit,  // �ӽŰ��� �Ѿ��� ���� ����� �ߴ� ����Ʈ
    RailGunShot,    // ���ϰ��� �� �� ������ �ѱ� ����Ʈ
    RailGunDecal,   // ���ϰ� ����
    BossCutDown,    // ������ ������� ���� �� ����� Effect
    BossCutDownDecal,    // ������ ������� ���� �� ����� Decal�� ����� Effect
    BossDash,       // ������ �뽬�� �� ������ ���̵� ����Ʈ
    BossGrab,       // ������ �׷��� �� ������ ���̵� ����Ʈ
    BossAppear,     // ���� �����ϴ� ����Ʈ
    BossEngineDestory,  // ���� ���� �ı� ����Ʈ
    BossEngineSmoke,    // ���� ���� ���� ����Ʈ
    BossSpark,      // ���� ����ũ ����Ʈ
    BossScanner,    // ���� ��ĳ�� ����Ʈ
}

/// <summary>
/// Effect�� ���õ� Resource�� ��� �ִ� Ŭ����
/// </summary>
public class EffectManager : Singleton<EffectManager>
{
    private GameObject[] effectObjects;

    private GameObject bulletDecal;
    private GameObject scanner;
    private WeaponMuzzleEffect[] gunShot;
    private GameObject gunHit;

    private GameObject effectGunShotResource;
    private GameObject effectResource;

    private GameObject knifeDecal;
    private GameObject knifePiercingDecal;

    private GameObject knifeEffect;

    private GameObject enemyEffect;
    private GameObject enemyEffect1;
    private GameObject enemyEffect2;

    private GameObject shieldEffect;
    private GameObject bossCutDownEffect;

    private GameObject[] bossEffects;
    protected override void Awake()
    {
        this.transform.SetParent(null);
        base.Awake();

        CreateEffect();
    }

    // �ѹ��� ��°� ���� ���ϴ� �̸��� Effect�� ��Ƶδ� ���� ���� ������?
    // Effect�� �� �� ������ �ѹ��� �� ��Ƶΰ� ����.
    void CreateEffect()
    {
        // Sound�� ����ϰ� �س���.
        effectObjects = Resources.LoadAll<GameObject>("EffectPrefab");

        bossEffects = Resources.LoadAll<GameObject>("EffectPrefab/Boss");

        // �̷������� �ϴ���, �ƴϸ� ������ �� �ް� �迭 �ȿ� ���� �͵��� �ϳ��� Ȯ���ؼ� �ִ���
        bulletDecal = Resources.Load<GameObject>("EffectPrefab/Bullet Decal");
        scanner = Resources.Load<GameObject>("EffectPrefab/Scanner");

        // GunShot Effect�� �������� �ڽĵ��� ��ȸ�ؼ� �ִ´�.
        effectGunShotResource = Resources.Load<GameObject>("EffectPrefab/GunShot");
        gunShot = effectGunShotResource.GetComponentsInChildren<WeaponMuzzleEffect>();

        // Gun Hit Effect�� �������� �ڽĵ��� ��ȸ�ؼ� �ִ´�.
        gunHit = Resources.Load<GameObject>("EffectPrefab/Hit 1");

        /// Į�� �ֵθ� �� ������ ����Ʈ�� Knife Decal�� Piercing Decal Load ��
        knifeDecal = Resources.Load<GameObject>("EffectPrefab/Knife Decal");
        knifePiercingDecal = Resources.Load<GameObject>("EffectPrefab/Knife Piercing Decal");

        /// Į�� �ֵθ� �� ������ ����Ʈ�� Load ��
        knifeEffect = Resources.Load<GameObject>("EffectPrefab/Knife Effect");

        /// Enemy�� �¾��� �� �ߴ� Effect�� Load ��
        enemyEffect = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect ");
        
        // �ؿ� �κ��� ������ �� �ϴ�.
        enemyEffect1 = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect 1");
        enemyEffect2 = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect 2");

        /// Shield Effect Load ��
        shieldEffect = Resources.Load<GameObject>("EffectPrefab/Boss/Shield Effect");
    }

    // Enum�� ������ String ���� ����ϴ� �Լ�
    private string EnumEffectSearchName(Effect effect)
    {
        string effectName = default;

        switch (effect) 
        {
            case Effect.BulletDecal:
                effectName = "Bullet Decal";
                break;
            case Effect.PistolShot:
                effectName = "PistolMuzzle";
                break;
            case Effect.ShotGunShot:
                effectName = "ShotGunMuzzle";
                break;
            case Effect.RifleShot:
                effectName = "RifleMuzzle";
                break;
            case Effect.GunHit:
                effectName = "GunHit";
                break;
            case Effect.KnifeEffect:
                effectName = "Knife Effect";
                break;
            case Effect.EnemyHit:
                effectName = "Enemy Gun Hit";
                break;
            case Effect.EnemyHit1:
                effectName = "Enemy Gun Hit 1";
                break;
            case Effect.EnemyHit2:
                effectName = "Enemy Gun Hit 2";
                break;
        }

        return effectName;
    }

    /// Exection Effect �Լ�

    /// ����Ʈ�� �θ� Transform�� �����ϴ� �Լ�
    public void ExecutionEffect(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect ����
        Object.Instantiate(effectResource, _transform);
    }

    /// Position�̶� Rotation�� �Է��ϸ� ������ִ� �Լ�
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion rotation)
    {
        SwitchEffectResource(effect);

        // Effect ����
        Object.Instantiate(effectResource, position, rotation);
    }

    /// ����Ʈ�� Position�� Quaternion�� Parent 
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect ����
        Object.Instantiate(effectResource, position, quaternion, parent);
    }

    /// Pool Manager�� �̿��� �����ϴ� �Լ� ���� �̸��� �Է��ؼ� Ȯ���ϴ� ���
    public void ExecutionEffect(string effectName, Vector3 position, Quaternion quaternion, float time)
    {
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, position, quaternion);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = position;
            effectObject.transform.rotation = quaternion;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }

    /// Enum ���� ã�� Pool Manager�� �̿��ؼ� Object�� ����Ѵ�.
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, float time)
    {
        string effectName = EnumEffectSearchName(effect);
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, position, quaternion);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = position;
            effectObject.transform.rotation = quaternion;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }

    /// Pool Manager�� �̿��� �����ϴ� �Լ� ���� �̸��� �Է��ؼ� Ȯ���ϴ� ���
    public void ExecutionEffect(string effectName, Vector3 position, Quaternion quaternion, Transform parent, float time)
    {
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, position, quaternion, parent);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = position;
            effectObject.transform.rotation = quaternion;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }

    /// Enum ���� ã�� Pool Manager�� �̿��ؼ� Object�� ����Ѵ�.
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, Transform parent, float time)
    {
        string effectName = EnumEffectSearchName(effect);
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, position, quaternion, parent);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = position;
            effectObject.transform.rotation = quaternion;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }

    /// Enum ���� ã�� Pool Manager�� �̿��ؼ� Object�� ����Ѵ�.
    public void ExecutionEffect(Effect effect, Transform parent, float time)
    {
        string effectName = EnumEffectSearchName(effect);
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, parent.transform.position, parent.transform.rotation, parent);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = parent.transform.position;
            effectObject.transform.rotation = parent.transform.rotation;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }

    /// Pool Manager�� �̿��� �����ϴ� �Լ� ���� �̸��� �Է��ؼ� Ȯ���ϴ� ���
    public void ExecutionEffect(string effectName, Transform parent, float time)
    { 
        GameObject effectObject = PoolManager.Instance.GetPooledObject(effectName, parent);
        ExecuteActiveFalseEffect setActiveFalseEffect;
        effectObject.TryGetComponent<ExecuteActiveFalseEffect>(out setActiveFalseEffect);

        if (setActiveFalseEffect != null)
        {
            effectObject.transform.position = parent.position;
            effectObject.transform.rotation = parent.rotation;
            StartCoroutine(setActiveFalseEffect.EffectDestroy(time));
        }
        else
            Debug.Log("None ExecuteActiveFalseEffect Script");
    }


    /// Exection Effect Object �Լ�

    /// Effect Object�� �����´�.
    public GameObject ExecutionEffectObject(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect ����
        GameObject gameObject = Object.Instantiate(effectResource, _transform);
        return gameObject;
    }

    /// Position�̶� Rotation�� �Է��ϸ� ������ִ� �Լ�
    public GameObject ExecutionEffectObject(Effect effect, Vector3 position, Quaternion rotation)
    {
        SwitchEffectResource(effect);

        // Effect ����
        GameObject gameObject = Object.Instantiate(effectResource, position, rotation);
        return gameObject;
    }

    /// Effect Object�� �����´�.
    public GameObject ExecutionEffectObject(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect ����
        GameObject gameObject = Object.Instantiate(effectResource, position, quaternion, parent);
        return gameObject;
    }

    // Enum���� Effect�� ã�� �Լ�
    private void SwitchEffectResource(Effect _effect)
    {
        switch(_effect) 
        {
            case Effect.BulletDecal:
                effectResource = bulletDecal;
                break;
            case Effect.PistolShot:
                effectResource = FindGunShotEffectName("Pistol Muzzle");
                break;
            case Effect.ShotGunShot:
                effectResource = FindGunShotEffectName("ShotGun Muzzle");
                break;
            case Effect.RifleShot:
                effectResource = FindGunShotEffectName("Riple Muzzle");
                break;
            case Effect.Scanner:
                effectResource = scanner;
                break;
            case Effect.CutDown:
                effectResource = FindEffectObjectsName("VP Cut down");
                break;
            case Effect.GunHit:
                effectResource = gunHit;
                break;
            case Effect.Dash:
                effectResource = FindEffectObjectsName("WindEffect");
                break;
            case Effect.VPSight:
                effectResource = FindEffectObjectsName("VP State Sight Effect");
                break;
            case Effect.CuttingDecal:
                effectResource = knifeDecal;
                break;
            case Effect.PiercingDecal:
                effectResource = knifePiercingDecal;
                break;
            case Effect.KnifeEffect:
                effectResource = knifeEffect;
                break;
            case Effect.EnemyHit:
                effectResource = enemyEffect;
                break;
            case Effect.EnemyHit1:
                effectResource = enemyEffect1;
                break;
            case Effect.EnemyHit2:
                effectResource = enemyEffect2;
                break;
            case Effect.Shield:
                effectResource = shieldEffect;
                break;
            case Effect.MachineGunShot:
                effectResource = FindBossEffectName("Boss Bullet Muzzle");
                break;
            case Effect.MachineGunHit:
                effectResource = FindBossEffectName("Boss Bullet Hit");
                break;
            case Effect.RailGunShot:
                effectResource = FindBossEffectName("Boss Lazer Muzzle Effect");
                break;
            case Effect.RailGunDecal:
                effectResource = FindBossEffectName("Boss Decal");
                break;
            case Effect.BossCutDown:
                effectResource = FindBossEffectName("Boss Slash Effect");
                break;
            case Effect.BossCutDownDecal:
                effectResource = FindBossEffectName("Boss Cut Down Decal");
                break;
            case Effect.BossDash:
                effectResource = FindBossEffectName("Dash");
                break;
            case Effect.BossGrab:
                effectResource = FindBossEffectName("Grab");
                break;
            case Effect.BossAppear:
                effectResource = FindBossEffectName("Boss Appear");
                break;
            case Effect.BossEngineDestory:
                effectResource = FindBossEffectName("Boss Back Engine Destory");
                break;
            case Effect.BossEngineSmoke:
                effectResource = FindBossEffectName("Boss Back Engine Smoke");
                break;
            case Effect.BossSpark:
                effectResource = FindBossEffectName("Boss Joint Spark Effect");
                break;
            case Effect.BossScanner:
                effectResource = FindBossEffectName("Boss Scanner");
                break;
            default:
                effectResource = null;
                break;
        }
    }

    /// <summary>
    /// ���� ���� �� �� ����Ʈ�� �ٸ� ���̱� ������ �̸����� ã��.
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    GameObject FindGunShotEffectName(string effectName)
    {
        GameObject effect = null;

        foreach (var obj in gunShot) 
        {
            if (obj.name == effectName)
                return obj.gameObject;
        }

        return effect;
    }

    /// <summary>
    /// ���� ����Ʈ �ȿ� �ִ� Effect���� �ҷ��´�.
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    GameObject FindBossEffectName(string effectName)
    {
        GameObject effect = null;

        foreach (var obj in bossEffects)
        {
            if (obj.name == effectName)
                return obj.gameObject;
        }

        return effect;
    }


    GameObject FindEffectObjectsName(string effectName)
    {
        GameObject effect = null;

        foreach (var obj in effectObjects)
        {
            if (obj.name == effectName)
                return obj.gameObject;
        }

        return effect;
    }

}

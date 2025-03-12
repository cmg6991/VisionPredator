using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum Effect
{
    BulletDecal,    // 총알 흔적
    PistolShot,     // 권총을 쐈을 때
    RifleShot,      // 라이플을 쐈을 때
    ShotGunShot,    // 샷건을 쐈을 때
    CutDown,        // 내려찍기
    Scanner,        // 내려찍기 공격할 때 생기는 Wave Effect
    Dash,           // 대쉬 이펙트
    VPSight,        // VP 시야 상태일때 주변에 일렁이는 화면 Effect
    GunHit,         // 총이 벽이나 바닥에 맞았을 때
    CuttingDecal,   // 칼 흔적 1
    PiercingDecal,  // 칼 찌르기
    KnifeEffect,    // 칼 이펙트
    EnemyHit,       // 적이 맞았을 때 뜨는 이펙트
    EnemyHit1,      // 적이 맞았을 때 뜨는 이펙트 1
    EnemyHit2,      // 적이 맞았을 때 뜨는 이펙트 2
    
    // Boss Effect
    Shield,         // 실드 이펙트
    MachineGunShot, // 보스가 머신건을 쏠 때 나오는 총구 이펙트
    MachineGunHit,  // 머신건의 총알이 벽에 닿았을 뜨는 이펙트
    RailGunShot,    // 레일건을 쏠 때 나오는 총구 이펙트
    RailGunDecal,   // 레일건 흔적
    BossCutDown,    // 보스가 내려찍기 했을 때 생기는 Effect
    BossCutDownDecal,    // 보스가 내려찍기 했을 때 생기는 Decal과 비슷한 Effect
    BossDash,       // 보스가 대쉬할 때 나오는 가이드 이펙트
    BossGrab,       // 보스가 그랩할 때 나오는 가이드 이펙트
    BossAppear,     // 보스 등장하는 이펙트
    BossEngineDestory,  // 보스 엔진 파괴 이펙트
    BossEngineSmoke,    // 보스 엔진 연기 이펙트
    BossSpark,      // 보스 스파크 이펙트
    BossScanner,    // 보스 스캐너 이펙트
}

/// <summary>
/// Effect와 관련된 Resource를 담고 있는 클래스
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

    // 한번에 담는것 보다 원하는 이름의 Effect를 담아두는 것이 맞지 않을까?
    // Effect도 몇 개 없으면 한번에 다 담아두고 있자.
    void CreateEffect()
    {
        // Sound와 비슷하게 해놨다.
        effectObjects = Resources.LoadAll<GameObject>("EffectPrefab");

        bossEffects = Resources.LoadAll<GameObject>("EffectPrefab/Boss");

        // 이런식으로 하던가, 아니면 위에서 다 받고 배열 안에 들은 것들을 하나씩 확인해서 넣던가
        bulletDecal = Resources.Load<GameObject>("EffectPrefab/Bullet Decal");
        scanner = Resources.Load<GameObject>("EffectPrefab/Scanner");

        // GunShot Effect를 가져오고 자식들을 순회해서 넣는다.
        effectGunShotResource = Resources.Load<GameObject>("EffectPrefab/GunShot");
        gunShot = effectGunShotResource.GetComponentsInChildren<WeaponMuzzleEffect>();

        // Gun Hit Effect를 가져오고 자식들을 순회해서 넣는다.
        gunHit = Resources.Load<GameObject>("EffectPrefab/Hit 1");

        /// 칼을 휘두를 때 나오는 이펙트를 Knife Decal과 Piercing Decal Load 중
        knifeDecal = Resources.Load<GameObject>("EffectPrefab/Knife Decal");
        knifePiercingDecal = Resources.Load<GameObject>("EffectPrefab/Knife Piercing Decal");

        /// 칼을 휘두를 때 나오는 이펙트를 Load 중
        knifeEffect = Resources.Load<GameObject>("EffectPrefab/Knife Effect");

        /// Enemy가 맞았을 때 뜨는 Effect를 Load 중
        enemyEffect = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect ");
        
        // 밑에 부분은 없어질 듯 하다.
        enemyEffect1 = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect 1");
        enemyEffect2 = Resources.Load<GameObject>("EffectPrefab/Enemy Hit Effect 2");

        /// Shield Effect Load 중
        shieldEffect = Resources.Load<GameObject>("EffectPrefab/Boss/Shield Effect");
    }

    // Enum을 넣으면 String 값을 출력하는 함수
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

    /// Exection Effect 함수

    /// 이펙트를 부모 Transform에 실행하는 함수
    public void ExecutionEffect(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        Object.Instantiate(effectResource, _transform);
    }

    /// Position이랑 Rotation을 입력하면 출력해주는 함수
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion rotation)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        Object.Instantiate(effectResource, position, rotation);
    }

    /// 이펙트를 Position과 Quaternion과 Parent 
    public void ExecutionEffect(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        Object.Instantiate(effectResource, position, quaternion, parent);
    }

    /// Pool Manager를 이용해 생성하는 함수 직접 이름을 입력해서 확인하는 방법
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

    /// Enum 으로 찾고 Pool Manager를 이용해서 Object를 출력한다.
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

    /// Pool Manager를 이용해 생성하는 함수 직접 이름을 입력해서 확인하는 방법
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

    /// Enum 으로 찾고 Pool Manager를 이용해서 Object를 출력한다.
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

    /// Enum 으로 찾고 Pool Manager를 이용해서 Object를 출력한다.
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

    /// Pool Manager를 이용해 생성하는 함수 직접 이름을 입력해서 확인하는 방법
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


    /// Exection Effect Object 함수

    /// Effect Object를 가져온다.
    public GameObject ExecutionEffectObject(Effect effect, Transform _transform)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        GameObject gameObject = Object.Instantiate(effectResource, _transform);
        return gameObject;
    }

    /// Position이랑 Rotation을 입력하면 출력해주는 함수
    public GameObject ExecutionEffectObject(Effect effect, Vector3 position, Quaternion rotation)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        GameObject gameObject = Object.Instantiate(effectResource, position, rotation);
        return gameObject;
    }

    /// Effect Object를 가져온다.
    public GameObject ExecutionEffectObject(Effect effect, Vector3 position, Quaternion quaternion, Transform parent)
    {
        SwitchEffectResource(effect);

        // Effect 생성
        GameObject gameObject = Object.Instantiate(effectResource, position, quaternion, parent);
        return gameObject;
    }

    // Enum으로 Effect를 찾는 함수
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
    /// 총을 쐈을 때 각 이펙트가 다를 것이기 때문에 이름으로 찾자.
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
    /// 보스 이펙트 안에 있는 Effect들을 불러온다.
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

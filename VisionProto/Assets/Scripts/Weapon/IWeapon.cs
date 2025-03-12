// 인터페이스를 만들었는데 그 다음에 어떻게 써야할지 모르겠다.
// Event Manager도 IListener가 있는데 이건 필요할 때 사용해서
// Weapon 이랑은 전혀 다르다.
// IState를 보니 StateMachine을 만들어 거기서 관리하는 것 같다.
// 나도 저렇게 해야하나??
// 일단 Interface를 만들었으니 생각을 해보자.



// 무기 정보
[System.Serializable]
public struct WeaponInformation
{
    // 반동에 관한 내용
    public float xRecoil;
    public float yRecoil;
    public float snappiness;
    public float returnSpeed;
    public float recoilSpeed;

    // 총알에 관한 내용들
    // Unity Engine 헤더를 추가하기 싫어서 GameObject를 넣지 않았다.
    public float bulletSpeed;
    public float distance;
    public int maxBullet;
    public float shotDelay;

    public float throwSpeed;

    // Struct에서 비어있는지 확인하려면.. 전부 확인해야 한다던데??
    public bool IsEmpty()
    {
        return xRecoil == default(float) &&
        yRecoil == default(float) &&
        snappiness == default(float) &&
        returnSpeed == default(float) &&
        recoilSpeed == default(float) &&

        bulletSpeed == default(float) &&
        distance == default(float) &&
        maxBullet == default(int) &&
        shotDelay == default(float) &&
        throwSpeed == default(float);
    }
}

// Tutorial 무기 사용여부
public struct TutorialWeapon
{
    public bool isDagger;
    public bool isVPWeapon;
}

// 단검 정보
public struct DaggerInformation
{
    public bool isTrueTrigger;
    public bool isEnableCollider;
    public int damaged;
}

// VP Weapon 정보
public struct VPWeaponInformation
{
    public bool isTrueTrigger;
    public bool isEnableCollider;
    public int damaged;
}

// 총알 정보
public struct BulletInformation
{
    // Vector를 사용하려면 namespace가 필요해서 넣고 싶지 않아서 float로 대체했다.
    public float directionX;
    public float directionY;
    public float directionZ;
    public float bulletSpeed;
}

// UI에 필요한 탄창 정보
public struct UIBulletInformation
{
    public float currentBullet;
    public float maxBullet;
    public UIWeaponSelect weaponSelect;
}

// 무기(총) Interface
public interface IWeapon
{
    // 초반에 필요한 반동 세팅
    public abstract void InitalizeRecoilSetting();
    // 총알을 쐈을 때 초기 설정값.
    public abstract void SaveFovValue();
    // 사격했을 때의 반동
    public abstract void RecoilFire();
    // 총알이 날라간다.
    public abstract void Shot();
    // 반동 회복
    public abstract void RecoilRecovery();
    // 무기 주웠을 때
    public abstract void Pickup();
    // 무기 던졌을 때
    public abstract void Drop();
}

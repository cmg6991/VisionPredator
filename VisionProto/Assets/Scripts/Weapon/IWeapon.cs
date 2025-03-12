// �������̽��� ������µ� �� ������ ��� ������� �𸣰ڴ�.
// Event Manager�� IListener�� �ִµ� �̰� �ʿ��� �� ����ؼ�
// Weapon �̶��� ���� �ٸ���.
// IState�� ���� StateMachine�� ����� �ű⼭ �����ϴ� �� ����.
// ���� ������ �ؾ��ϳ�??
// �ϴ� Interface�� ��������� ������ �غ���.



// ���� ����
[System.Serializable]
public struct WeaponInformation
{
    // �ݵ��� ���� ����
    public float xRecoil;
    public float yRecoil;
    public float snappiness;
    public float returnSpeed;
    public float recoilSpeed;

    // �Ѿ˿� ���� �����
    // Unity Engine ����� �߰��ϱ� �Ⱦ GameObject�� ���� �ʾҴ�.
    public float bulletSpeed;
    public float distance;
    public int maxBullet;
    public float shotDelay;

    public float throwSpeed;

    // Struct���� ����ִ��� Ȯ���Ϸ���.. ���� Ȯ���ؾ� �Ѵٴ���??
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

// Tutorial ���� ��뿩��
public struct TutorialWeapon
{
    public bool isDagger;
    public bool isVPWeapon;
}

// �ܰ� ����
public struct DaggerInformation
{
    public bool isTrueTrigger;
    public bool isEnableCollider;
    public int damaged;
}

// VP Weapon ����
public struct VPWeaponInformation
{
    public bool isTrueTrigger;
    public bool isEnableCollider;
    public int damaged;
}

// �Ѿ� ����
public struct BulletInformation
{
    // Vector�� ����Ϸ��� namespace�� �ʿ��ؼ� �ְ� ���� �ʾƼ� float�� ��ü�ߴ�.
    public float directionX;
    public float directionY;
    public float directionZ;
    public float bulletSpeed;
}

// UI�� �ʿ��� źâ ����
public struct UIBulletInformation
{
    public float currentBullet;
    public float maxBullet;
    public UIWeaponSelect weaponSelect;
}

// ����(��) Interface
public interface IWeapon
{
    // �ʹݿ� �ʿ��� �ݵ� ����
    public abstract void InitalizeRecoilSetting();
    // �Ѿ��� ���� �� �ʱ� ������.
    public abstract void SaveFovValue();
    // ������� ���� �ݵ�
    public abstract void RecoilFire();
    // �Ѿ��� ���󰣴�.
    public abstract void Shot();
    // �ݵ� ȸ��
    public abstract void RecoilRecovery();
    // ���� �ֿ��� ��
    public abstract void Pickup();
    // ���� ������ ��
    public abstract void Drop();
}

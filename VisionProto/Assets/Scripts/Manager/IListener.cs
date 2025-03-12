// �̺�Ʈ Ÿ���� ���Ѵ�.
// �̺�Ʈ Ÿ���� �߰��ϰ� ������ ���⿡ �߰��Ѵ�.

public enum EventType
{
    Hooking,
    detected,
    Throwing,
    isEquiped,
    Pickup, 
    Change,
    PlayerPosition,
    DaggerInformation,
    isEmptyBullet,
    GunInformation,
    playerShot,
    PlayerHPUI,
    WeaponBullet,
    PlayerAnimator,
    VPAnimator,
    NPCHit,
    LoadingScene,
    VPState,
    VPWeaponInformation,
    isPause,
    UIGunName,
    EquipedGunName,
    CameraShake,
    CrossHairColor,
    DoomOutline,
    NPCDeath,
    Tutorial,
    PlayerDead,
    DeadFadeInOut,
    HitBulletRotation,
    TutorialOpenDoor,
    Clear,
    END,
}


public interface IListener
{
    public void OnEvent(EventType eventType, object param = null);
}

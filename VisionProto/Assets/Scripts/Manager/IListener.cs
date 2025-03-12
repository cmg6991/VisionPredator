// 이벤트 타입을 정한다.
// 이벤트 타입을 추가하고 싶으면 여기에 추가한다.

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

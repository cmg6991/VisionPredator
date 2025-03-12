using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public enum BGM
{
    MainTitle_BGM,
    Ambience,
    VP_Am,
    Boss,
    Credit,
};

public enum SFX
{
    // Player State
    Player_Walk_1,
    Player_Walk_2,
    Player_Run_1,
    Player_Run_2,
    Player_Jump,
    Player_Slide,
    Player_sit,
    Player_Hurt,
    Player_Death,

    //VP
    VP_Attack1,
    VP_Attack2,
    VP_Dash,

    Grappling_Start,
    Grappling_End,
    GrandSlam,
    Weapon_Throwing,
    Weapon_Draw,
    Change, 
    Heal,

    // Enemy
    Enemy_Run,
    Enemy_Jump,
    Enemy_Hurt,
    Enemy_Death_1,
    Enemy_Death_2,
    Enemy_Death_3,

    Weapon_Drop,

    // Boss
    Boss_Hit_1,
    Boss_Hit_2,
    Boss_Death,
    Boss_MachineGun,
    Boss_Slam,
    Boss_Dash,
    Boss_Grab,
    Boss_GrabOff,
    Boss_Attack,
    Boss_Railgun,
    Boss_SkillOff,
    Boss_Shield,
    Boss_ShieldAttack,
    Boss_Stun,
    Boss_Idle,
    Boss_Openning,
    Boss_Ending,

    // Weapon
    Pistol,
    Shotgun,
    Rifle_1,
    Rifle_2,
    Knife_Swing_1,
    Knife_Swing_2,
    Knife_Piercing,

    // etc
    Interact,
    Object_Destroy,
    Open_Door,
    Close_Door,
    OpenDoor_Big,
    Gunhit,
    UI_Click,
    Clear,

    END,
};


/// <summary>
/// Sound Manager 제작
/// 모든 Sound들을 관리한다.
/// 사용하기 편하게하기 위해 Enum으로 관리
/// 0620 이용성
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    // 기획에서 정리한 CSV파일 Sound 들을 가지고 정리를 하면 될 것 같다. - Enum을 사용하지 못한다.

    // SoundClip 보다 SoundSource로 하자.
    // BGM 전용, SFX 전용 따로 만들어야 될 것 같아.

    private AudioSource[] bgmAudio;
    private AudioSource[] sfxAudio;
    private GameObject[] testObject;

    private Dictionary<BGM, AudioSource> activeBGMSounds = new Dictionary<BGM, AudioSource>();
    private Dictionary<SFX, AudioSource> activeSFXSounds = new Dictionary<SFX, AudioSource>();

    private AudioSource playBGMAudio;
    private AudioSource playSFXAudio;

    private List<AudioSource> activeSound = new List<AudioSource>();

    // 현재 재생되고 있는 Sound들을 전부 삭제해야 한다.
    // List에 담고 있다가 한 번에 삭제하면 될 거 같은데
    // 그 전에 삭제된 애들은?

    protected override void Awake()
    {
        this.transform.SetParent(null);
        base.Awake();

        CreateSound();
    }

    public void CreateSound()
    {
        /// Prefab 안에 있는 것을 선택하고 순회해서 bgmAudio, sfxAudio에 넣고 싶다.
        testObject = Resources.LoadAll<GameObject>("SoundPrefab");

        // TestObject에서 Children Component들을 순회해서 배열에 넣는다.
        bgmAudio = testObject[0].GetComponentsInChildren<AudioSource>();
        sfxAudio = testObject[1].GetComponentsInChildren<AudioSource>();
    }


    /// <summary>
    /// 기획에서 준 액셀에서 데이터들이 정렬되어 있을텐데 거기서 받아오게 할 예정이다!
    /// 지금은 임시로
    /// </summary>
    /// <param name="bgmSound"></param>
    public void PlayBGMSound(BGM bgmSound)
    {

        if (activeBGMSounds.ContainsKey(bgmSound))
        {
            Debug.LogWarning("Sound already playing : " + bgmSound.ToString());
            return;
        }

        BGMSwitch(bgmSound);

        AudioSource bgm = Object.Instantiate(playBGMAudio);
        activeSound.Add(bgm);
    }


    /// <summary>
    /// 기획에서 준 액셀에서 데이터들이 정렬되어 있을텐데 거기서 받아오게 할 예정이다!
    /// 지금은 임시로
    /// </summary>
    /// <param name="bgmSound"></param>
    public GameObject PlayAudioSourceBGMSound(BGM bgmSound)
    {
        BGMSwitch(bgmSound);

        AudioSource bgm = Object.Instantiate(playBGMAudio);
        activeSound.Add(bgm);
        GameObject bgmObject = bgm.gameObject;
        return bgmObject;
    }

    /// <summary>
    /// BGM 전용 Switch문
    /// </summary>
    /// <param name="_bgm"></param>
    private void BGMSwitch(BGM _bgm)
    {
        switch (_bgm)
        {
            case BGM.MainTitle_BGM:
                playBGMAudio = FindBGMName("MainTitle_BGM");
                break;
            case BGM.Ambience:
                playBGMAudio = FindBGMName("Ambience");
                break;
            case BGM.VP_Am:
                playBGMAudio = FindBGMName("VP_Am");
                break;
            case BGM.Credit:
                playBGMAudio = FindBGMName("Credit");
                break;
            case BGM.Boss:
                playBGMAudio = FindBGMName("Boss");
                break;
        }
    }

    public void PlayEffectSound(SFX effectSound)
    {
        SwithchSFX(effectSound);

        AudioSource sfx = Object.Instantiate(playSFXAudio);
        activeSound.Add(sfx);
    }


    /// <summary>
    /// Effect Sound를 Enum으로 찾아서 원하는 Transform에 출력한다.
    /// </summary>
    /// <param name="effectSound"></param>
    public void PlayEffectSound(SFX effectSound, Transform _transform)
    {
        // 이게 중복 Sound 처리가 안되게 막는 거다. 그건 아니야.

        if (activeSFXSounds.ContainsKey(effectSound))
        {
            Debug.LogWarning("Sound already playing : " + effectSound.ToString());
            return;
        }

        SwithchSFX(effectSound);
    
        AudioSource sfx = Object.Instantiate(playSFXAudio, _transform);
        activeSound.Add(sfx);
    }

    /// <summary>
    /// Effect Sound를 Enum으로 찾아서 원하는 Transform에 출력한다.
    /// </summary>
    /// <param name="effectSound"></param>
    public GameObject PlayAudioSourceEffectSound(SFX effectSound, Transform _transform)
    {
        SwithchSFX(effectSound);

        AudioSource sfx = Object.Instantiate(playSFXAudio, _transform);
        activeSound.Add(sfx);
        GameObject gameObject = sfx.gameObject;

        return gameObject;
    }

    /// <summary>
    /// SFX 전용 Switch 문
    /// </summary>
    /// <param name="_sfx"></param>
    private void SwithchSFX(SFX _sfx)
    {
        switch (_sfx)
        {
            case SFX.Player_Walk_1:
                playSFXAudio = FindSFXName("Player_Walk 1");
                break;
            case SFX.Player_Walk_2:
                playSFXAudio = FindSFXName("Player_Walk 2");
                break;
            case SFX.Player_Run_1:
                playSFXAudio = FindSFXName("Player_Run 1");
                break;
            case SFX.Player_Run_2:
                playSFXAudio = FindSFXName("Player_Run 2");
                break;
            case SFX.Player_Jump:
                playSFXAudio = FindSFXName("Player_Jump");
                break;
            case SFX.Player_Slide:
                playSFXAudio = FindSFXName("Player_Slide");
                break;
            case SFX.Player_sit:
                playSFXAudio = FindSFXName("Player_Sit");
                break;
            case SFX.Player_Hurt:
                playSFXAudio = FindSFXName("Player_Hurt");
                break;
            case SFX.Player_Death:
                playSFXAudio = FindSFXName("Player_Death");
                break;
            case SFX.VP_Attack1:
                playSFXAudio = FindSFXName("VP_Attack1");
                break;
            case SFX.VP_Attack2:
                playSFXAudio = FindSFXName("VP_Attack2");
                break;
            case SFX.VP_Dash:
                playSFXAudio = FindSFXName("VP_Dash");
                break;
            case SFX.Grappling_Start:
                playSFXAudio = FindSFXName("Grappling_Start");
                break;
            case SFX.Grappling_End:
                playSFXAudio = FindSFXName("Grappling_End");
                break;
            case SFX.Weapon_Throwing:
                playSFXAudio = FindSFXName("Weapon_Throwing");
                break;
            case SFX.Weapon_Draw:
                playSFXAudio = FindSFXName("Weapon_Draw");
                break;
            case SFX.Change:
                playSFXAudio = FindSFXName("Change");
                break;
            case SFX.Heal:
                playSFXAudio = FindSFXName("Heal");
                break;
            case SFX.Enemy_Run:
                playSFXAudio = FindSFXName("Enemy_Run");
                break;
            case SFX.Enemy_Jump:
                playSFXAudio = FindSFXName("Enemy_Jump");
                break;
            case SFX.Enemy_Hurt:
                playSFXAudio = FindSFXName("Enemy_Hurt");
                break;
            case SFX.Enemy_Death_1:
                playSFXAudio = FindSFXName("Enemy_Death 1");
                break;
            case SFX.Enemy_Death_2:
                playSFXAudio = FindSFXName("Enemy_Death 2");
                break;
            case SFX.Enemy_Death_3:
                playSFXAudio = FindSFXName("Enemy_Death 3");
                break;
            case SFX.Weapon_Drop:
                playSFXAudio = FindSFXName("Weapon_Drop");
                break;
            case SFX.GrandSlam:
                playSFXAudio = FindSFXName("GrandSlam");
                break;
            case SFX.Pistol:
                playSFXAudio = FindSFXName("Pistol");
                break;
            case SFX.Shotgun:
                playSFXAudio = FindSFXName("Shotgun");
                break;
            case SFX.Rifle_1:
                playSFXAudio = FindSFXName("Rifle");
                break;
            case SFX.Rifle_2:
                playSFXAudio = FindSFXName("Rifle 2");
                break;
            case SFX.Knife_Swing_1:
                playSFXAudio = FindSFXName("Knife_Cut 1");
                break;
            case SFX.Knife_Swing_2:
                playSFXAudio = FindSFXName("Knife_Cut 2");
                break;
            case SFX.Knife_Piercing:
                playSFXAudio = FindSFXName("Knife_Piercing");
                break;
            case SFX.Interact:
                playSFXAudio = FindSFXName("Interact");
                break;
            case SFX.Object_Destroy:
                playSFXAudio = FindSFXName("Object_Destroy");
                break;
            case SFX.Open_Door:
                playSFXAudio = FindSFXName("Open_Door");
                break;
            case SFX.Close_Door:
                playSFXAudio = FindSFXName("Close_Door");
                break;
            case SFX.OpenDoor_Big:
                playSFXAudio = FindSFXName("OpenDoor_Big");
                break;
            case SFX.Gunhit:
                playSFXAudio = FindSFXName("Gunhit");
                break;
            case SFX.UI_Click:
                playSFXAudio = FindSFXName("UI_Click");
                break;
            case SFX.Boss_Attack:
                playSFXAudio = FindSFXName("Boss_Attack");
                break;
            case SFX.Boss_Dash:
                playSFXAudio = FindSFXName("Boss_Dash");
                break;
            case SFX.Boss_Death:
                playSFXAudio = FindSFXName("Boss_Death");
                break;
            case SFX.Boss_Grab:
                playSFXAudio = FindSFXName("Boss_Grab");
                break;
            case SFX.Boss_GrabOff: 
                playSFXAudio = FindSFXName("Boss_GrabOff");
                break;
            case SFX.Boss_Hit_1: 
                playSFXAudio = FindSFXName("Boss_Hit 1");
                break;
            case SFX.Boss_Hit_2:
                playSFXAudio = FindSFXName("Boss_Hit 2");
                break;
            case SFX.Boss_Idle: 
                playSFXAudio = FindSFXName("Boss_Idle");
                break;
            case SFX.Boss_MachineGun:
                playSFXAudio = FindSFXName("Boss_MachineGun");
                break;
            case SFX.Boss_Railgun: 
                playSFXAudio = FindSFXName("Boss_Railgun");
                break;
            case SFX.Boss_Shield: 
                playSFXAudio = FindSFXName("Boss_Shield");
                break;
            case SFX.Boss_ShieldAttack: 
                playSFXAudio = FindSFXName("Boss_ShieldAttack");
                break;
            case SFX.Boss_SkillOff: 
                playSFXAudio = FindSFXName("Boss_SkillOff");
                break;
            case SFX.Boss_Slam:
                playSFXAudio = FindSFXName("Boss_Slam");
                break;
            case SFX.Boss_Stun: 
                playSFXAudio = FindSFXName("Boss_Stun");
                break;
            case SFX.Boss_Openning:
                playSFXAudio = FindSFXName("Boss_Openning");
                break;
            case SFX.Boss_Ending:
                playSFXAudio = FindSFXName("Boss_Ending");
                break;
            case SFX.Clear:
                playSFXAudio = FindSFXName("Clear");
                break;
        }
    }

    /// <summary>
    /// 필요 없는 Sound는 삭제 해야한다.
    /// </summary>
    /// <param name="destroyedObject"></param>
    public void DestroyObject(GameObject destroyedObject)
    {
        AudioSource audiosource = destroyedObject.GetComponent<AudioSource>();

        if (audiosource != null)
            activeSound.Remove(audiosource);

        Destroy(destroyedObject);
    }

    /// <summary>
    /// 현재 Play 되고 있는 Sound를 List에서 제거한다.
    /// </summary>
    /// <param name="audioSource"></param>
    public void RemoveActiveSound(AudioSource audioSource)
    {
        activeSound.Remove(audioSource);
    }

    public void AllSoundRemove()
    {
        foreach(var audio in activeSound)
        {
            if(audio != null)
            {
                Destroy(audio.gameObject);
            }
        }

        activeSound.Clear();
    }

    /// <summary>
    /// BGM Sound 중복 방지
    /// </summary>
    /// <param name="soundName">BGM Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(BGM soundName, AudioSource audioSource)
    {
        // Sound 길이만큼 Effect Sound가 들어오지 않게 한다.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeBGMSounds.Remove(soundName);
    }

    /// <summary>
    /// Effect Sound 중복 방지
    /// </summary>
    /// <param name="soundName">SFX Sound</param>
    /// <param name="audioSource">AudioSource</param>
    /// <returns></returns>
    private IEnumerator RemoveDuplicationSound(SFX soundName, AudioSource audioSource)
    {
        // Sound 길이만큼 Effect Sound가 들어오지 않게 한다.
        yield return new WaitForSeconds(audioSource.clip.length);
        activeSFXSounds.Remove(soundName);
    }


    /// <summary>
    /// 이름을 넣어서 BGM Sound를 찾는다.
    /// </summary>
    /// <param name="soundName">BGM 이름</param>
    /// <returns></returns>
    AudioSource FindBGMName(string soundName)
    {
        AudioSource bgmAudioSource = null;

        foreach (var bgm in bgmAudio)
        {
            if (bgm.name == soundName)
                return bgm;
        }

        return bgmAudioSource;
    }

    /// <summary>
    /// 이름을 넣어서 Effect Sound를 찾는다.
    /// </summary>
    /// <param name="soundName">SFX 이름</param>
    /// <returns></returns>
    AudioSource FindSFXName(string soundName)
    {
        AudioSource sfxAudioSource = null;

        foreach (var sfx in sfxAudio)
        {
            if (sfx.name == soundName)
                return sfx;
        }

        return sfxAudioSource;
    }
}

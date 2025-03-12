using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

// 카메라 흔들림 
[System.Serializable]
public struct CameraNoiseProfile
{
    // 인간일 때 기본상태
    public float normal_Idle_Amplitude;
    public float normal_Idle_Frequency;

    // vp일 때 기본상태
    public float vp_Idle_Amplitude;
    public float vp_Idle_Frequency;

    // 인간일 때 이동 상태
    public float normal_Move_Amplitude;
    public float normal_Move_Frequency;

    // vp일 때 이동 상태
    public float vp_Move_Amplitude;
    public float vp_Move_Frequency;

    // 인간일 때 달리기 상태
    public float normal_Run_Amplitude;
    public float normal_Run_Frequency;

    // 인간일 때 점프 상태
    public float normal_Jump_Amplitude;
    public float normal_Jump_Frequency;

    // VP일 때 점프 상태
    public float vp_Jump_Amplitude;
    public float vp_Jump_Frequency;

    // 인간일 때 앉기 상태
    public float normal_Sit_Amplitude;
    public float normal_Sit_Frequency;

    // 인간일 때 슬라이드 상태
    public float normal_Slide_Amplitude;
    public float normal_Slide_Frequency;

    // VP일 때 대쉬 돌진 상태
    public float vp_Dash_Amplitude;
    public float vp_Dash_Frequency;

    // VP일 때 내려찍기 상태
    public float vp_Slash_Amplitude;
    public float vp_Slash_Frequency;

    // 인간일 때 갈고리 상태
    public float normal_Grappling_Amplitude;
    public float normal_Grappling_Frequency;
}

public enum CameraSetting
{
    Shake,
    Wobble,
    Handheld_Normal_Mild,
    RifleNoise,
    ShotgunNoise,
    PistolNoise,
    VPLeftAttack,
    KnifeCutting_1,
    KnifeCutting_2,
    KnifePiercing,
}

// 이런식으로 관리하자.
public struct CameraInfomation
{
    public CameraSetting setting;
    public float amplitude;
    public float frequency;
}

public class CameraShake : MonoBehaviour, IListener
{
    // 반동 ㄱㄱ 카메라도 같이 흔들려야 하니까 부모에서 받아와야겠다.
    public CinemachineVirtualCamera playerCamera;
    private CinemachinePOV pov;
    private Vector3 currentRotation;

    // 카메라 흔들림
    private CinemachineBasicMultiChannelPerlin noise;
    public NoiseSettings noiseSettings;

    // Noise들이 담겨져 있다.
    public NoiseSettings pistolNoise;
    public NoiseSettings shotgunNoise;
    public NoiseSettings rifleNoise;
    public NoiseSettings shake;
    public NoiseSettings wobble;
    public NoiseSettings handheld_normal_mild;
    public NoiseSettings vpLeftAttack;
    public NoiseSettings knifeCuttingType1;
    public NoiseSettings knifeCuttingType2;
    public NoiseSettings knifePiercing;



    // 밖에서 더해준다.
    public Vector3 targetRotaion;   /// 인터페이스로 변경할 때 필요한 변수
    
    // Mouse 누르기 전의 FOV 값을 저장해야 한다.
    public bool isRecoil;       /// 인터페이스로 변경할 때 필요한 변수
    public bool isMouseDown;    /// 인터페이스로 변경할 때 필요한 변수
    public Vector3 mouseDistance;   /// 인터페이스로 변경할 때 필요한 변수
    public bool gunRecoil;      /// 인터페이스로 변경할 때 필요한 변수

    // 이전 좌표 Y,Z (Y,X) 좌표 값
    private float preRecoilYPosition;
    private float preRecoilZPosition;

    // 비어 있으면 Update가 실행되면 안 된다.
    public WeaponInformation weaponInformation;


    private void Start()
    {
        pov = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        noise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        EventManager.Instance.AddEvent(EventType.CameraShake, OnEvent);
    }

    private void Update()
    {
        if (weaponInformation.IsEmpty())
            return;

        if(isMouseDown) 
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            mouseDistance.x += mouseX;
            mouseDistance.y += mouseY;

            // 반동 제어하면 Rotation이 Zero가 되서 그 위치에 고정한다.
            //if (mouseX != 0 || mouseY != 0)
            if (mouseY != 0)
                targetRotaion = Vector3.zero;
        }

        // 에임이 회복됐으면 Zero이고 아니라면 Zero가 아니다.
        if (targetRotaion == Vector3.zero)
            gunRecoil = true;
        else gunRecoil = false;

        if (isRecoil)
        {
            // 여기가 다시 돌아오는 곳을 담당하는 곳 같다.
            float returnSpeed = weaponInformation.returnSpeed;
            targetRotaion = Vector3.Lerp(targetRotaion, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        float snappiness = weaponInformation.snappiness;
        float recoilSpeed = weaponInformation.recoilSpeed;

        // 현재 있던 Rotation 에서 target Rotation 이동.
        currentRotation = Vector3.Slerp(currentRotation, targetRotaion, snappiness * Time.deltaTime);

        // Local Rotation 좌표가 변경된다. 근데 우리는 cinemachine value를 조절해서 카메라를 움직여야한다.   
        Quaternion recoilGun = Quaternion.Euler(currentRotation);

        // 변화량을 더 해서 넣어야한다. -> 핵심, 근데 이거 아님 다른거 해야해

        // 현재 값
        float recoilYPosition = recoilGun.y;
        float recoilZPosition = recoilGun.z;

        // 현재 - 과거
        float diffentYPosition = recoilYPosition - preRecoilYPosition;
        float diffentZPosition = recoilZPosition - preRecoilZPosition;

        pov.m_VerticalAxis.Value -= diffentYPosition * recoilSpeed;
        pov.m_HorizontalAxis.Value -= diffentZPosition * recoilSpeed;

        // 과거 값
        preRecoilYPosition = recoilGun.y;
        preRecoilZPosition = recoilGun.z;
    }

    public void SetCameraNoise(float amplitude, float frequency)
    {
        //noise.m_NoiseProfile = noiseSettings;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
    }

    private void SetCameraNoiseSetting(CameraSetting camera, float amplitude, float frequency)
    {
        switch(camera)
        {
            case CameraSetting.Shake:
                noise.m_NoiseProfile = shake;
                break;
            case CameraSetting.Wobble: 
                noise.m_NoiseProfile = wobble;
                break;
            case CameraSetting.Handheld_Normal_Mild:
                noise.m_NoiseProfile = handheld_normal_mild;
                break;
            case CameraSetting.RifleNoise:
                noise.m_NoiseProfile = rifleNoise;
                break;
            case CameraSetting.PistolNoise:
                noise.m_NoiseProfile = pistolNoise;
                break;
            case CameraSetting.ShotgunNoise:
                noise.m_NoiseProfile = shotgunNoise;
                break;
            case CameraSetting.VPLeftAttack:
                noise.m_NoiseProfile = vpLeftAttack;
                break;
            case CameraSetting.KnifeCutting_1:
                noise.m_NoiseProfile = knifeCuttingType1;
                break;
            case CameraSetting.KnifeCutting_2:
                noise.m_NoiseProfile = knifeCuttingType2;
                break;
            case CameraSetting.KnifePiercing:
                noise.m_NoiseProfile = knifePiercing;
                break;
        }

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        //noise.mNoiseTime = 0f;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType) 
        {
            case EventType.CameraShake:
                {
                    CameraInfomation cameraInfomation = (CameraInfomation)param;
                    SetCameraNoiseSetting(cameraInfomation.setting, cameraInfomation.amplitude, cameraInfomation.frequency);
                }
                break;
        }
    }

}

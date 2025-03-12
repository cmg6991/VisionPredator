using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

// ī�޶� ��鸲 
[System.Serializable]
public struct CameraNoiseProfile
{
    // �ΰ��� �� �⺻����
    public float normal_Idle_Amplitude;
    public float normal_Idle_Frequency;

    // vp�� �� �⺻����
    public float vp_Idle_Amplitude;
    public float vp_Idle_Frequency;

    // �ΰ��� �� �̵� ����
    public float normal_Move_Amplitude;
    public float normal_Move_Frequency;

    // vp�� �� �̵� ����
    public float vp_Move_Amplitude;
    public float vp_Move_Frequency;

    // �ΰ��� �� �޸��� ����
    public float normal_Run_Amplitude;
    public float normal_Run_Frequency;

    // �ΰ��� �� ���� ����
    public float normal_Jump_Amplitude;
    public float normal_Jump_Frequency;

    // VP�� �� ���� ����
    public float vp_Jump_Amplitude;
    public float vp_Jump_Frequency;

    // �ΰ��� �� �ɱ� ����
    public float normal_Sit_Amplitude;
    public float normal_Sit_Frequency;

    // �ΰ��� �� �����̵� ����
    public float normal_Slide_Amplitude;
    public float normal_Slide_Frequency;

    // VP�� �� �뽬 ���� ����
    public float vp_Dash_Amplitude;
    public float vp_Dash_Frequency;

    // VP�� �� ������� ����
    public float vp_Slash_Amplitude;
    public float vp_Slash_Frequency;

    // �ΰ��� �� ���� ����
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

// �̷������� ��������.
public struct CameraInfomation
{
    public CameraSetting setting;
    public float amplitude;
    public float frequency;
}

public class CameraShake : MonoBehaviour, IListener
{
    // �ݵ� ���� ī�޶� ���� ������ �ϴϱ� �θ𿡼� �޾ƿ;߰ڴ�.
    public CinemachineVirtualCamera playerCamera;
    private CinemachinePOV pov;
    private Vector3 currentRotation;

    // ī�޶� ��鸲
    private CinemachineBasicMultiChannelPerlin noise;
    public NoiseSettings noiseSettings;

    // Noise���� ����� �ִ�.
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



    // �ۿ��� �����ش�.
    public Vector3 targetRotaion;   /// �������̽��� ������ �� �ʿ��� ����
    
    // Mouse ������ ���� FOV ���� �����ؾ� �Ѵ�.
    public bool isRecoil;       /// �������̽��� ������ �� �ʿ��� ����
    public bool isMouseDown;    /// �������̽��� ������ �� �ʿ��� ����
    public Vector3 mouseDistance;   /// �������̽��� ������ �� �ʿ��� ����
    public bool gunRecoil;      /// �������̽��� ������ �� �ʿ��� ����

    // ���� ��ǥ Y,Z (Y,X) ��ǥ ��
    private float preRecoilYPosition;
    private float preRecoilZPosition;

    // ��� ������ Update�� ����Ǹ� �� �ȴ�.
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

            // �ݵ� �����ϸ� Rotation�� Zero�� �Ǽ� �� ��ġ�� �����Ѵ�.
            //if (mouseX != 0 || mouseY != 0)
            if (mouseY != 0)
                targetRotaion = Vector3.zero;
        }

        // ������ ȸ�������� Zero�̰� �ƴ϶�� Zero�� �ƴϴ�.
        if (targetRotaion == Vector3.zero)
            gunRecoil = true;
        else gunRecoil = false;

        if (isRecoil)
        {
            // ���Ⱑ �ٽ� ���ƿ��� ���� ����ϴ� �� ����.
            float returnSpeed = weaponInformation.returnSpeed;
            targetRotaion = Vector3.Lerp(targetRotaion, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        float snappiness = weaponInformation.snappiness;
        float recoilSpeed = weaponInformation.recoilSpeed;

        // ���� �ִ� Rotation ���� target Rotation �̵�.
        currentRotation = Vector3.Slerp(currentRotation, targetRotaion, snappiness * Time.deltaTime);

        // Local Rotation ��ǥ�� ����ȴ�. �ٵ� �츮�� cinemachine value�� �����ؼ� ī�޶� ���������Ѵ�.   
        Quaternion recoilGun = Quaternion.Euler(currentRotation);

        // ��ȭ���� �� �ؼ� �־���Ѵ�. -> �ٽ�, �ٵ� �̰� �ƴ� �ٸ��� �ؾ���

        // ���� ��
        float recoilYPosition = recoilGun.y;
        float recoilZPosition = recoilGun.z;

        // ���� - ����
        float diffentYPosition = recoilYPosition - preRecoilYPosition;
        float diffentZPosition = recoilZPosition - preRecoilZPosition;

        pov.m_VerticalAxis.Value -= diffentYPosition * recoilSpeed;
        pov.m_HorizontalAxis.Value -= diffentZPosition * recoilSpeed;

        // ���� ��
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

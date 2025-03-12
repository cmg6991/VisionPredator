using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(3)]
public class VPRenderFeature : MonoBehaviour, IListener
{
    private bool isVPState;
    private bool isFirstProgress;
    private bool isSecondProgress;

    private float totalTime;

    // Render Object
    public RenderObjects vpState;                           // VP 상태
    public RenderObjects npcStencil;                        // NPC Stencil
    public RenderObjects objectStencil;                     // Object Stencil
    public RenderObjects scanner;                           // Scanner
    public RenderObjects npcVPState;                        // NPC Outline 제거
    public FullScreenPassRendererFeature fullScreenOutline; // Full Screen Outline

    private GameObject bloodImage;

    // Volume 처리
    private Volume globalVolume;
    private LiftGammaGain liftGammaGain;
    private MotionBlur motionBlur;
    private Vignette vignette;

    private TransitionAnimator animator;
    private GameObject fadeInOutObject;

    public float fadeSpeed = 10f;
    public float keepTime = 1.0f;

    private bool isStart;
    private bool isDead;

    public bool isInvincibleState;
    public bool isGameClear;

    AnimationInformation Dinfo;

    private bool isOnce;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
        EventManager.Instance.AddEvent(EventType.PlayerDead, OnEvent);

        globalVolume = FindObjectOfType<Volume>();
        globalVolume.profile.TryGet(out liftGammaGain);
        globalVolume.profile.TryGet(out motionBlur);
        globalVolume.profile.TryGet(out vignette);

        bloodImage = GameObject.Find("Blood Image");
        isOnce = false;
        bloodImage.SetActive(false);

        if (liftGammaGain == null)
            Debug.Log("None Global Volume -> Lift Gamma Gain");

        if(motionBlur == null)
            Debug.Log("None Global Volume -> Motion Blur");

        if(vignette == null)
            Debug.Log("None Global Volume -> Vignette");

        GameObject vpState = Resources.Load<GameObject>("EffectPrefab/Transitions Plus");
        fadeInOutObject = Object.Instantiate(vpState);
        animator = fadeInOutObject.GetComponent<TransitionAnimator>();
        fadeInOutObject.SetActive(false);
        SwitchRenderFeature(true);
    }

    // Update에 너무 많은 관여를 하니 Invoke를 하는 것이 좋을수도 있다.

    private void Update()
    {
        if (!isStart)
            return;

        // 무조건 progress 0 -> 1로 시작한다.
        if (animator.progress >= 1)
            isFirstProgress = true;

        // 첫번째 실행 완료 1초 후에 두번째 실행
        if (isFirstProgress && !isSecondProgress)
        {
            totalTime += Time.deltaTime;
            isInvincibleState = true;

            if (totalTime > keepTime)
            {
                if(isGameClear)
                {
                    SceneManager.LoadScene("GameClear");
                    return;
                }

                if (!isDead)
                    ReversePlay();
                else
                {
                    if(!isOnce)
                    {
                        totalTime = 0f;
                        // 죽었을 때 뜨는 UI 확인하자. 이것도 1로 변경
                        GameObject youDiedPrefab = Resources.Load<GameObject>("UI/Dead");
                        if (youDiedPrefab != null)
                        {
                            Instantiate(youDiedPrefab);
                        }
                        else Debug.Log("None Prefab");
                        isOnce = true;
                    }
                }
            }
        }

        // 두번째 실행 완료 1초 후에 Object false
        if(isSecondProgress)
        {
            totalTime += Time.deltaTime;

            if (totalTime > keepTime)
            {
                fadeInOutObject.SetActive(false);
                isInvincibleState = false;
                isStart = false;
            }
        }
    }

    public void FadeInFadeOut()
    {
        fadeInOutObject.SetActive(true);
        isFirstProgress = false;
        isSecondProgress = false;
        isStart = true;
        isInvincibleState = true;
        totalTime = 0f;
        Play();
    }

    private void Play()
    {
        animator.profile.invert = false;
        animator.Play();
    }

    // 이때 화면이 보이기 시작한다. 이곳에서 vpState에 따라 true가 될지 false가 될지 
    private void ReversePlay()
    {
        isSecondProgress = true;
        animator.profile.invert = true;
        animator.Play();
        totalTime = 0f;
        if (isVPState)
            SwitchRenderFeature(true);
        else
            SwitchRenderFeature(false);
    }
  

    private void SwitchRenderFeature(bool _isOn)
    {
        vpState.SetActive(_isOn);
        npcStencil.SetActive(_isOn);
        scanner.SetActive(_isOn);
        npcVPState.SetActive(_isOn);
        fullScreenOutline.SetActive(_isOn);
        objectStencil.SetActive(_isOn);
        bloodImage.SetActive(_isOn);

        EventManager.Instance.NotifyEvent(EventType.DoomOutline, _isOn);

        // !은 인간 상태일 때 
        liftGammaGain.active = !_isOn;
        motionBlur.active = !_isOn;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                    
                    FadeInFadeOut();
                }
                break;
            case EventType.PlayerDead:
                {
                    isDead = (bool)param;
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimationInformation
{
    public string stateName;
    public int layer;
    public float normalizedTime;
}

public class PlayerAnimator : MonoBehaviour, IListener
{
    // ÀÌ°Å private 
    public Animator[] animator;

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        EventManager.Instance.AddEvent(EventType.PlayerAnimator, OnEvent);
        EventManager.Instance.AddEvent(EventType.VPAnimator, OnEvent);
    }

    public float Speed
    {
        set => animator[0].SetFloat("Speed", value);
        get => animator[0].GetFloat("Speed");
    }

    public float Movement
    {
        set => animator[1].SetFloat("Movement", value);
        get => animator[1].GetFloat("Movement");
    }

    public void OnReload()
    {
        animator[0].SetTrigger("OnReload");
    }
    public void OnGrappling()
    {
        animator[0].SetTrigger("OnGrappling");
    }

    public void OnDash()
    {
        animator[1].SetTrigger("OnDash");
    }

    public bool IsCurrentAnimation(string name)
    {
        return animator[0].GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void Play(string  stateName, int layer, float normalizedTime)
    {
        animator[0].Play(stateName, layer, normalizedTime);
    }
    public void VPPlay(string stateName, int layer, float normalizedTime)
    {
        animator[1].Play(stateName, layer, normalizedTime);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerAnimator:
                {
                    AnimationInformation info = (AnimationInformation)param;
                    Play(info.stateName, info.layer, info.normalizedTime);
                }
                break;
            case EventType.VPAnimator:
                {
                    AnimationInformation info = (AnimationInformation)param;
                    VPPlay(info.stateName, info.layer, info.normalizedTime);
                }
                break;
        }
    }
}

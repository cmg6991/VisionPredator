using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialVPState : BaseState
{
    public TutorialVPState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (stateMachine.isVPState)
        {
            stateMachine.isVPState = false;

            if (stateMachine.VPStateRange.activeSelf)
                stateMachine.VPStateRange.SetActive(false);

            if (stateMachine.vpSound != null)
                SoundManager.Instance.DestroyObject(stateMachine.vpSound);
        }
        else
        {
            stateMachine.isVPState = true;

            if (!stateMachine.VPStateRange.activeSelf)
                stateMachine.VPStateRange.SetActive(true);
            stateMachine.vpSound = SoundManager.Instance.PlayAudioSourceBGMSound(BGM.VP_Am);


        }
        DataManager.Instance.isVPState = stateMachine.isVPState;
        EventManager.Instance.NotifyEvent(EventType.VPState, stateMachine.isVPState);
        SoundManager.Instance.PlayEffectSound(SFX.Change, stateMachine.transform);
    }

    public override void Tick()
    {
        if (stateMachine.isPause)
            return;
        stateMachine.SwitchPreviousState();
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {

    }
}

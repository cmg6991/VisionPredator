using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 상호작용하기 전에 Layer를 체크하는 State다.
/// </summary>
public class ObjectOpenState : BaseState
{
    public ObjectOpenState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private OpenWeaponCabinet weaponCabinet;
    private OpenItem openItem;
    private CloseDoorButton doorButton;
    // OpenItem

    public override void Enter()
    {
        if (stateMachine.objectTag == "Cabinet")
        {
            weaponCabinet = stateMachine.targetGameObject.GetComponent<OpenWeaponCabinet>();
            if (weaponCabinet != null)
            {
                if (!weaponCabinet.isControl)
                {
                    weaponCabinet.isControl = true;
                    SoundManager.Instance.PlayEffectSound(SFX.Interact, stateMachine.transform);
                }
            }
        }
        else if (stateMachine.objectTag == "Item")
        {
            // item open
            openItem = stateMachine.targetGameObject.GetComponent<OpenItem>();
            if (openItem != null)
            {
                if (!openItem.isControl)
                {
                    openItem.isControl = true;
                    SoundManager.Instance.PlayEffectSound(SFX.Interact, stateMachine.transform);
                }
            }
        }
        else if (stateMachine.objectTag == "Button")
        {
            doorButton = stateMachine.targetGameObject.GetComponent<CloseDoorButton>();
            if (doorButton != null)
            {
                if(!doorButton.isControl)
                {
                    doorButton.isControl = true;
                }
            }
        }

    }

    public override void Tick()
    {
        if (stateMachine.isTutorial)
        {
            stateMachine.SwitchState(new TutorialIdleState(stateMachine));
        }
        else
            stateMachine.SwitchState(new IdleState(stateMachine));
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {
        stateMachine.targetGameObject = null;
        stateMachine.objectTag = default;
    }
}

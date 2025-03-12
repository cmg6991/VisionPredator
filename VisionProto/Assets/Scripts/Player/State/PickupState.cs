using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PickupState : BaseState, IListener
{
    public PickupState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private GameObject gun;

    public override void Enter()
    {
        stateMachine.animator.OnReload();
        if (stateMachine.isEquiped)
            EventManager.Instance.NotifyEvent(EventType.Change);
//             //EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
//         else

        // interaction status에서 이미 마우스에 닿은 물체가 Gun이라고 인식했었다.
        gun = stateMachine.targetGameObject;

        // 해당하는 Script들을 활성화 한다. -> 그 총을 줍는다는 의미이다.
        MonoBehaviour[] gunScripts = gun.GetComponentsInParent<MonoBehaviour>();

        foreach (var script in gunScripts) 
        {
            // Statemachine에서 총을 장착하고 있다고 알려줘야 할 것 같은데?
            script.enabled = true;
        }
        stateMachine.isEquiped = true;
        EventManager.Instance.NotifyEvent(EventType.isEquiped, stateMachine.isEquiped);
    }

    public override void Tick()
    {
        if(stateMachine.isTutorial)
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
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType)
        {
            case EventType.isEquiped:
                {
                    stateMachine.isEquiped = (bool)param;
                }
                break;
        }
    }

}

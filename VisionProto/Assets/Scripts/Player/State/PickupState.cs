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

        // interaction status���� �̹� ���콺�� ���� ��ü�� Gun�̶�� �ν��߾���.
        gun = stateMachine.targetGameObject;

        // �ش��ϴ� Script���� Ȱ��ȭ �Ѵ�. -> �� ���� �ݴ´ٴ� �ǹ��̴�.
        MonoBehaviour[] gunScripts = gun.GetComponentsInParent<MonoBehaviour>();

        foreach (var script in gunScripts) 
        {
            // Statemachine���� ���� �����ϰ� �ִٰ� �˷���� �� �� ������?
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

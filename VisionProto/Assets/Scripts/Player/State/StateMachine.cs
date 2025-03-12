using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected IState currentState;
    protected IState previousState;
    public string state;

    public void SwitchState(IState state)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void SwitchPreviousState()
    {
        if(previousState != null)
        {
            IState tempState = previousState;
            previousState = currentState;
            currentState.Exit();
            currentState = tempState;
            currentState.Enter();
        }
    }

    protected void Update()
    {
        currentState?.Tick();
        state = currentState.ToString();
    }
    protected void FixedUpdate()
    {
        currentState?.FixedTick();
    }

    protected string GetState()
    {
        return currentState.ToString();
    }
}

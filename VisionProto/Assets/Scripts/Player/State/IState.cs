using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    public abstract void Enter();
    public abstract void Tick();
    public abstract void FixedTick();
    public abstract void Exit();
}
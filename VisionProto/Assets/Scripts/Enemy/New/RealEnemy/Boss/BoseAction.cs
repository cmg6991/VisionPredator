using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BTNode;

public abstract class BoseAction
{
    public abstract NodeState Execute();
    public abstract NodeState Stop();

    protected void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineRunner.Instance.StartCoroutine(coroutine);
    }
}

public class BIdle : BoseAction
{
    public override NodeState Execute() { return NodeState.Failure; }

    public override NodeState Stop() { return NodeState.Failure; }
}
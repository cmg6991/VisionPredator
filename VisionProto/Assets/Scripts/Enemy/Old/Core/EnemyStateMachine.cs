using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���¸� �����ϴ� Ŭ����
/// ������ �ʱ�ȭ, �߰�, �ٲ�, ���� �Լ��� �̰��� �ִ�.
/// 
/// �迹���� �ۼ�
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnemyStateMachine<T> where T : Enum
{
    private Dictionary<T, Action> stateDictionary = new Dictionary<T, Action>();
    public T currentState;

    public void Initialize(T initState)
    {
        currentState = initState;
    }

    //�̰� �ʿ��ұ�?
    public void AddState(T state, Action stateAction)
    {
        if (!stateDictionary.ContainsKey(state))
        {
            stateDictionary[state] = stateAction; //��� �� ������ �ȴ�.
            //stateDictionary.Add(state, stateAction);
        }
        else
        {
            Debug.LogWarning($"{state} ���´� �̹� �ִ�. ����!");
        }
    }

    public void ChangeState(T newState)
    {
        if (stateDictionary.ContainsKey(newState))
        {
            currentState = newState;
        }
        else
        {
            Debug.LogWarning($"{newState} ���´� �����ϴ�. �߰��Ͽ��°�?");
        }
    }

    public void UpdateCurrentState()
    {
        if (stateDictionary.TryGetValue(currentState, out Action action))
        {
            action?.Invoke();
        }
        else
        {
            Debug.LogWarning($"{currentState} ���¿� ����� �׼��� �ƿ� ����. �׼��� ���� �ϼ���");
        }
    }
}
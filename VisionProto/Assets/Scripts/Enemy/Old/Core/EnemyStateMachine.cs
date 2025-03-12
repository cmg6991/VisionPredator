using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적들의 상태를 관리하는 클래스
/// 상태의 초기화, 추가, 바꿈, 갱신 함수가 이곳에 있다.
/// 
/// 김예리나 작성
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

    //이거 필요할까?
    public void AddState(T state, Action stateAction)
    {
        if (!stateDictionary.ContainsKey(state))
        {
            stateDictionary[state] = stateAction; //사실 걍 업뎃도 된다.
            //stateDictionary.Add(state, stateAction);
        }
        else
        {
            Debug.LogWarning($"{state} 상태는 이미 있다. 하하!");
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
            Debug.LogWarning($"{newState} 상태는 없습니다. 추가하였는가?");
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
            Debug.LogWarning($"{currentState} 상태와 연결된 액션이 아예 없다. 액션을 연결 하세요");
        }
    }
}
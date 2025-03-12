using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BTNode;

public class RandomSelector : BTNode
{
    private List<BTNode> children = new List<BTNode>();
    private List<int> randomOrder = new List<int>();
    private int currentChild = 0;

    public void AddChild(BTNode node)
    {
        children.Add(node);
    }

    public override NodeState Execute()
    {
        if (currentChild >= randomOrder.Count)
        {
            Reset(); // 모두 순회했으면 리셋
            return NodeState.Failure;
        }

        NodeState result = children[randomOrder[currentChild]].Execute();

        if (result == NodeState.Running)
        {
            return NodeState.Running;
        }
        if (result == NodeState.Success)
        {
            Reset(); // 성공하면 초기화하고 성공 반환
            return NodeState.Success;
        }

        currentChild++; // 다음 노드로
        return Execute(); // 계속 다음 노드 실행
    }

    public override void Reset()
    {
        currentChild = 0;

        // 자식 노드들의 랜덤 순서 생성
        randomOrder.Clear();
        for (int i = 0; i < children.Count; i++)
        {
            randomOrder.Add(i);
        }

        // 리스트를 랜덤하게 섞음
        for (int i = 0; i < randomOrder.Count; i++)
        {
            int temp = randomOrder[i];
            int randomIndex = Random.Range(i, randomOrder.Count);
            randomOrder[i] = randomOrder[randomIndex];
            randomOrder[randomIndex] = temp;
        }

        // 모든 자식 노드 초기화
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}

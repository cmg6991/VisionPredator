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
            Reset(); // ��� ��ȸ������ ����
            return NodeState.Failure;
        }

        NodeState result = children[randomOrder[currentChild]].Execute();

        if (result == NodeState.Running)
        {
            return NodeState.Running;
        }
        if (result == NodeState.Success)
        {
            Reset(); // �����ϸ� �ʱ�ȭ�ϰ� ���� ��ȯ
            return NodeState.Success;
        }

        currentChild++; // ���� ����
        return Execute(); // ��� ���� ��� ����
    }

    public override void Reset()
    {
        currentChild = 0;

        // �ڽ� ������ ���� ���� ����
        randomOrder.Clear();
        for (int i = 0; i < children.Count; i++)
        {
            randomOrder.Add(i);
        }

        // ����Ʈ�� �����ϰ� ����
        for (int i = 0; i < randomOrder.Count; i++)
        {
            int temp = randomOrder[i];
            int randomIndex = Random.Range(i, randomOrder.Count);
            randomOrder[i] = randomOrder[randomIndex];
            randomOrder[randomIndex] = temp;
        }

        // ��� �ڽ� ��� �ʱ�ȭ
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}

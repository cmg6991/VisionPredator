/// <summary>
/// �⺻ ���
/// 
/// 2024.8.5
/// </summary>
public abstract class BTNode
{
    public enum NodeState
    {
        Running,
        Failure,
        Success 
    }

    public abstract NodeState Execute();
    public virtual void Reset() { }
    ///TO DO : stop quit fail �������� ��Ű�� �Լ��� �ʿ��ϰų� ������Ʈ������ �ʿ�.

}
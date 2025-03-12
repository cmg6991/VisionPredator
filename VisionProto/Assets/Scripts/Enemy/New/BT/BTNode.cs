/// <summary>
/// 기본 노드
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
    ///TO DO : stop quit fail 강제종료 시키는 함수가 필요하거나 업데이트내에서 필요.

}
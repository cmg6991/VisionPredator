using UnityEngine;

/// <summary>
/// ������ �����ϴ� ������ �������̽�
/// �Լ��� �ϳ��������� �������̽��� �����ؾ��ߴ�. �� ������ �ٲ����
/// 
/// �迹���� �ۼ�
/// </summary>
public enum DamageType
{
    ///�ѿ� ���� ���� 
}
public interface IDamageable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">���ط�</param>
    /// <param name="hitPoint">���� ��ġ</param>
    /// <param name="hitNormal">���� ��ġ�� ���� ����</param>
    /// <param name="source">���� �����ΰ�?</param>
    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal,  GameObject source); //DamageType damageType,
    public void Died();
}
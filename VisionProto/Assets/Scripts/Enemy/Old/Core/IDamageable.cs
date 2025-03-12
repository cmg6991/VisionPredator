using UnityEngine;

/// <summary>
/// 적들이 공유하는 데미지 인터페이스
/// 함수가 하나뿐이지만 인터페이스로 구현해야했다. 또 생각이 바뀔수도
/// 
/// 김예리나 작성
/// </summary>
public enum DamageType
{
    ///총에 따른 무언가 
}
public interface IDamageable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">피해량</param>
    /// <param name="hitPoint">맞은 위치</param>
    /// <param name="hitNormal">맞은 위치의 법선 벡터</param>
    /// <param name="source">누가 범인인가?</param>
    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal,  GameObject source); //DamageType damageType,
    public void Died();
}
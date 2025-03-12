using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, ISelectHandler
{
    /// <summary>
    /// 마우스가 선택했을 때
    /// </summary>
    public void OnSelect(BaseEventData baseEventData)
    {
        SoundManager.Instance.PlayEffectSound(SFX.UI_Click);
    }
}

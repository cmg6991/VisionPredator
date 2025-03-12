using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, ISelectHandler
{
    /// <summary>
    /// ���콺�� �������� ��
    /// </summary>
    public void OnSelect(BaseEventData baseEventData)
    {
        SoundManager.Instance.PlayEffectSound(SFX.UI_Click);
    }
}

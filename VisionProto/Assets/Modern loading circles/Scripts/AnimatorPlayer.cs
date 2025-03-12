using UnityEngine;

public class AnimatorPlayer : MonoBehaviour
{
    [SerializeField]
    string AnimationName;

    private void OnEnable() => GetComponent<Animator>().Play(AnimationName);
}

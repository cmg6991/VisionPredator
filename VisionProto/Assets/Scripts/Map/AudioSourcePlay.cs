using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePlay : MonoBehaviour
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(SoundDestroy(audioSource.clip.length));
    }

    private IEnumerator SoundDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        SoundManager.Instance.RemoveActiveSound(audioSource);
        Destroy(this.gameObject);
    }
}

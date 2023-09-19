using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] _audioSources;

    public AudioClip OnInteractClip;

    public AudioClip OnDoneInteractClip;

    private Queue<AudioSource> _sourcePool;

    private void Awake()
    {
        SingletonManager.Add(this);
        _sourcePool = new Queue<AudioSource>(_audioSources);
    }

    public void Play(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        if (_sourcePool.Count <= 0)
        {
            Debug.LogError("Out of audio source");
            return;
        }

        var audioSource = _sourcePool.Dequeue();
        audioSource.gameObject.SetActive(true);
        audioSource.enabled = true;
        audioSource.PlayOneShot(audioClip);

        DOVirtual.DelayedCall(audioClip.length, () =>
        {
            _sourcePool.Enqueue(audioSource);
            audioSource.gameObject.SetActive(false);
        });
    }

    public void PlayOnInteract()
    {
        Play(OnInteractClip);
    }

    public void PlayDoneInteract()
    {
        Play(OnDoneInteractClip);
    }
}

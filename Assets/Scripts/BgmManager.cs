using DG.Tweening;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    public float FadeDuration = 5;

    [SerializeField]
    private AudioClip[] _clips;

    private int index;

    private float _maxVolume;

    private void Awake()
    {
        _maxVolume = _audioSource.volume;
        SingletonManager.Add(this);
        NextSong();
    }

    private void NextSong()
    {
        _audioSource.clip = _clips[index];
        _audioSource.Play();
        var fadeDuration = Mathf.Min(FadeDuration, _audioSource.clip.length);

        DOTween.Sequence()
            .AppendInterval(_audioSource.clip.length - fadeDuration)
            .Append(_audioSource.DOFade(0, fadeDuration))
            .AppendCallback(NextSong)
            .Append(_audioSource.DOFade(_maxVolume, fadeDuration))
            .target = this;

        index = (index + 1) % _clips.Length;
    }

    public void Play(AudioClip clip)
    {
        DOTween.Kill(this);
        _audioSource.clip = clip;
        _audioSource.loop = true;
    }

    public void Play(AudioClip[] clips)
    {
        _clips = clips;
        index = 0;
        NextSong();
    }
}

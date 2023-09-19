using DG.Tweening;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _bgmSource;

    [SerializeField]
    private AudioClip[] _backgroundMusic;

    private int index;

    private void Awake()
    {
        SingletonManager.Add(this);
        NextSong();
    }

    private void NextSong()
    {
        _bgmSource.clip = _backgroundMusic[index];
        _bgmSource.Play();
        index = (index + 1) % _backgroundMusic.Length;
        DOVirtual.DelayedCall(_backgroundMusic[index].length, NextSong)
            .target = this;
    }

    public void Play(AudioClip clip)
    {
        DOTween.Kill(this);
        _bgmSource.clip = clip;
        _bgmSource.loop = true;
    }

    public void Play(AudioClip[] clips)
    {
        _backgroundMusic = clips;
        index = 0;
        NextSong();
    }
}

using System;
using DG.Tweening;
using UnityEngine;

public class TapRotatable : Interactable
{
    [Range(-360, 360)]
    public float Rotation = 45;

    private bool _isRotating = false;

    protected void Start()
    {
        Init();
    }

    public void Init()
    {
        enabled = true;
        GameController.Instance.OnLevelEnded.AddListener(OnLevelEnded);
    }

    private void OnLevelEnded(float completionRate, int levelIndex, int progress)
    {
        if (completionRate < 1)
        {
            return;
        }

        GameController.Instance.OnLevelEnded.RemoveListener(OnLevelEnded);
        DOTween.Kill(transform);
        DOTween.Kill(this);
        enabled = false;
    }

    protected override void OnDoneInteract()
    {
        if (_isRotating)
        {
            return;
        }

        _isRotating = true;
        SingletonManager.SoundManager.PlayDoneInteract();
        transform.DOLocalRotate(transform.eulerAngles + new Vector3(0, 0, Rotation), 0.15f, RotateMode.FastBeyond360)
            .OnComplete(FinishRotation);
    }

    private void FinishRotation()
    {
        SingletonManager.LevelController.CheckCompletionRate();
        _isRotating = false;
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
        GameController.Instance.OnLevelEnded.RemoveListener(OnLevelEnded);
    }
}

using System;
using DG.Tweening;
using UnityEngine;

public class TapRotatable : Interactable
{
    [Range(-360, 360)]
    public float Rotation = 45;

    private bool _isRotating = false;

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
    }
}

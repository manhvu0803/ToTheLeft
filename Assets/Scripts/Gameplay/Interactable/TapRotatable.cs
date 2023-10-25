using DG.Tweening;
using UnityEngine;

public class TapRotatable : Interactable
{
    [Range(-360, 360)]
    public float Rotation = 45;

    protected override void OnDoneInteract()
    {
        SingletonManager.SoundManager.PlayDoneInteract();
        transform.DOLocalRotate(transform.eulerAngles + new Vector3(0, 0, Rotation), 0.15f, RotateMode.FastBeyond360)
            .OnComplete(() => SingletonManager.LevelController.CheckCompletionRate());
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}

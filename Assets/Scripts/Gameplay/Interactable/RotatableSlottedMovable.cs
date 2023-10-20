using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatableSlottedMovable : SlottedMovable
{
    [Range(-360, 360)]
    public float Rotation = 45;

    private Vector3 _totalDelta = Vector3.zero;

    protected override void OnInteract(Vector3 delta, PointerEventData eventData)
    {
        _totalDelta += delta;
        base.OnInteract(delta, eventData);
    }

    protected override void OnDoneInteract()
    {
        if (_totalDelta.sqrMagnitude <= 0.001f)
        {
            SnapAndReturn();
            HideShadow();
            transform.DOLocalRotate(transform.eulerAngles + new Vector3(0, 0, Rotation), 0.15f, RotateMode.FastBeyond360)
                .OnComplete(() => Controller.CheckCompletionRate());
            SoundManager?.PlayDoneInteract();
        }
        else
        {
            base.OnDoneInteract();
        }

        _totalDelta = Vector3.zero;
    }
}

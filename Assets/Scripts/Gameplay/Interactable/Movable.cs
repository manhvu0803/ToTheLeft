using DG.Tweening;
using UnityEngine;

public class Movable : Interactable
{
    [Tooltip("Relative")]
    public Vector3 OffsetScaleOnDrag = new(0.25f, 0.25f, 0.25f);

    public Vector2 MaxLocalOffset = new(100, 100);

    [Tooltip("Is MaxLocalOffset absolute?")]
    public bool IsOffsetAbsolute = true;

    [Min(0)]
    public float SnapDistance = 0.1f;

    protected Vector3 OriginalScale;

    /// <summary>Starting local position</summary>
    private Vector3 _originalPosition;

    protected virtual void Start()
    {
        OriginalScale = transform.localScale;
        _originalPosition = transform.localPosition;
    }

    protected override void OnMouseDown()
    {
        if (!enabled)
        {
            return;
        }

        base.OnMouseDown();
        DOTween.Kill(transform, complete: true);
        transform.DOScale(OriginalScale + OffsetScaleOnDrag, 0.15f);
        transform.Translate(0, 0, -0.5f);
    }

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePostion)
    {
        transform.Translate(delta.x, delta.y, 0, Space.World);
        var position = transform.localPosition;

        if (IsOffsetAbsolute)
        {
            position.x = Mathf.Clamp(position.x, -MaxLocalOffset.x, MaxLocalOffset.x);
            position.y = Mathf.Clamp(position.y, -MaxLocalOffset.y, MaxLocalOffset.y);
        }
        else
        {
            position.x = Mathf.Clamp(position.x, _originalPosition.x - MaxLocalOffset.x, _originalPosition.x + MaxLocalOffset.x);
            position.y = Mathf.Clamp(position.y, _originalPosition.y - MaxLocalOffset.y, _originalPosition.y + MaxLocalOffset.y);
        }

        transform.localPosition = position;
    }

    protected override void OnDoneInteract()
    {
        SnapAndReturn();
        base.OnDoneInteract();
    }

    protected void SnapAndReturn()
    {
        if (((Vector2)transform.localPosition).sqrMagnitude <= SnapDistance * SnapDistance)
        {
            transform.DOLocalMove(Vector3.zero, 0.15f);
        }

        transform.DOScale(OriginalScale, 0.15f);
        transform.Translate(0, 0, 0.5f);
    }

    protected void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}

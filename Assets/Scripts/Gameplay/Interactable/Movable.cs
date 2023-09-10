using DG.Tweening;
using UnityEngine;

public class Movable : Interactable
{
    [Tooltip("Relative")]
    public Vector3 OffsetScaleOnDrag = new(0.25f, 0.25f, 0.25f);

    public Vector2 MaxLocalOffset = new(100, 100);

    [Min(0)]
    public float SnapDistance = 0.1f;

    private Vector3 _originalScale;

    protected void Start()
    {
        _originalScale = transform.localScale;
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        DOTween.Kill(transform, complete: true);
        transform.DOScale(_originalScale + OffsetScaleOnDrag, 0.15f);
        transform.Translate(0, 0, -0.5f);
    }

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePostion)
    {
        transform.Translate(delta.x, delta.y, 0, Space.World);
        var position = transform.localPosition;
        position.x = Mathf.Clamp(position.x, -MaxLocalOffset.x, MaxLocalOffset.x);
        position.y = Mathf.Clamp(position.y, -MaxLocalOffset.y, MaxLocalOffset.y);
        transform.localPosition = position;
    }

    protected override void OnMouseUp()
    {
        SnapAndReturn();
        Controller.CheckCompletionRate();
    }

    protected void SnapAndReturn()
    {
        if (((Vector2)transform.localPosition).sqrMagnitude <= SnapDistance * SnapDistance)
        {
            transform.localPosition = Vector3.zero;
        }

        transform.DOScale(_originalScale, 0.15f);
        transform.Translate(0, 0, 0.5f);
    }

    protected void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}

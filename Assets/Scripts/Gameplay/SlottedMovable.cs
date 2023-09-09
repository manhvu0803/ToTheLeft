using DG.Tweening;
using UnityEngine;

public class SlottedMovable : Interactable
{
    [Tooltip("Relative")]
    public Vector3 ScaleOnDrag = new(0.25f, 0.25f, 0.25f);

    public bool RotateOnMouseDown = false;

    private Vector3 _originalScale;

    private Slot _lastShowedSlot;

    protected override void Start()
    {
        base.Start();
        _originalScale = transform.localScale;
    }

    protected RaycastHit2D Raycast()
    {
        var slotLayers = SlotLevelController.Instance.SlotLayers;
        return Physics2D.Raycast(MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, slotLayers);
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        DOTween.Kill(transform, complete: true);
        transform.DOScale(_originalScale + ScaleOnDrag, 0.15f);
        transform.Translate(0, 0, -0.5f);
    }

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePostion)
    {
        transform.position += new Vector3(delta.x, delta.y, 0);
        var hit = Raycast();
        Slot slot = null;

        if (hit.collider != null && hit.collider.TryGetComponent(out slot))
        {
            SlotLevelController.Instance.UpdateSlot(transform, slot.transform, true);
            slot.Show();
        }

        if (_lastShowedSlot != null && _lastShowedSlot != slot)
        {
            _lastShowedSlot.Hide();
        }

        _lastShowedSlot = slot;
    }

    private void OnMouseUp()
    {
        transform.DOScale(_originalScale, 0.15f);
        transform.Translate(0, 0, 0.5f);

        if (_lastShowedSlot != null)
        {
            _lastShowedSlot.Hide();
            _lastShowedSlot = null;
        }

        var hit = Raycast();
        var hitTransform = (hit.collider != null) ? hit.collider.transform : null;
        var isSlotEmpty = SlotLevelController.Instance.UpdateSlot(transform, hitTransform);

        if (isSlotEmpty && hitTransform != null)
        {
            var position = hitTransform.position - new Vector3(0, 0, 0.5f);
            transform.DOMove(position, 0.15f);

            if (RotateOnMouseDown)
            {
                transform.DORotate(hitTransform.eulerAngles, 0.15f);
            }

            return;
        }

        if (RotateOnMouseDown)
        {
            transform.DORotate(new Vector3(0, 0, (Random.Range(0, 2) == 0) ? -20 : 20), 0.15f);
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}

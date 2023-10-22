using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlottedMovable : Movable
{
    #region Static
    private static SlotLevelController _controller;

    protected static SlotLevelController Controller
    {
        get
        {
            if (_controller == null)
            {
                _controller = SingletonManager.Get<SlotLevelController>();
            }

            return _controller;
        }
    }

    static protected RaycastHit2D Raycast()
    {
        var slotLayers = Controller.SlotLayers;
        return Physics2D.Raycast(MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, slotLayers);
    }
    #endregion

    public bool SnapToSlot = true;

    public bool RotateOnMouseUp = true;

    private Slot _lastShowedSlot;

    protected override void OnInteract(Vector3 delta, PointerEventData eventData)
    {
        base.OnInteract(delta, eventData);
        var hit = Raycast();
        Slot slot = null;

        if (hit.collider != null && hit.collider.TryGetComponent(out slot))
        {
            slot.Show();
        }

        if (_lastShowedSlot != null && _lastShowedSlot != slot)
        {
            _lastShowedSlot.Hide();
        }

        _lastShowedSlot = slot;
    }

    protected override void OnDoneInteract()
    {
        SnapAndReturn();
        HideShadow();

        var hit = Raycast();
        var hitTransform = (hit.collider != null) ? hit.collider.transform : null;
        var isSlotEmpty = Controller.UpdateSlot(transform, hitTransform);

        if (isSlotEmpty && hitTransform != null)
        {
            var position = hitTransform.position - new Vector3(0, 0, 0.5f);

            if (SnapToSlot)
            {
                transform.DOMove(position, 0.15f)
                    .OnComplete(() => Controller.CheckCompletionRate());
            }
            else
            {
                Controller.CheckCompletionRate();
            }

            if (RotateOnMouseUp)
            {
                transform.DORotate(hitTransform.eulerAngles, 0.15f);
            }

            return;
        }

        if (RotateOnMouseUp)
        {
            transform.DORotate(new Vector3(0, 0, (Random.Range(0, 2) == 0) ? -20 : 20), 0.15f);
        }

        Controller.CheckCompletionRate();
        SoundManager?.PlayDoneInteract();
    }

    protected void HideShadow()
    {
        if (_lastShowedSlot != null)
        {
            _lastShowedSlot.Hide();
            _lastShowedSlot = null;
        }
    }
}

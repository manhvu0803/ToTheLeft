using DG.Tweening;
using UnityEngine;

public class SlottedMovable : Movable
{
    #region Static
    private static SlotLevelController _controller;

    protected static new SlotLevelController Controller
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

    public bool RotateOnMouseUp = true;

    private Slot _lastShowedSlot;

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePostion)
    {
        base.OnInteract(delta, currentMousePostion);
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

        if (_lastShowedSlot != null)
        {
            _lastShowedSlot.Hide();
            _lastShowedSlot = null;
        }

        var hit = Raycast();
        var hitTransform = (hit.collider != null) ? hit.collider.transform : null;
        var isSlotEmpty = Controller.UpdateSlot(transform, hitTransform);
        SingletonManager.Get<SoundManager>()?.PlayDoneInteract();

        if (isSlotEmpty && hitTransform != null)
        {
            var position = hitTransform.position - new Vector3(0, 0, 0.5f);
            transform.DOMove(position, 0.15f)
                .OnComplete(() => Controller.CheckCompletionRate());

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
    }
}

using DG.Tweening;
using UnityEngine;

public class Level1Controller : SlottedLevelController
{
    private Transform _target;

    public override bool UpdateSlot(Transform target, Transform slot)
    {
        var result = base.UpdateSlot(target, slot);

        if (slot != null && SlotMap.TryGetValue(slot, out var slotTarget) && target == slotTarget)
        {
            _target = target;

            DOVirtual.DelayedCall(0.15f, () =>
            {
                var movable = target.GetComponent<SlottedMovable>();
                Destroy(movable);
                var rotatable = target.GetComponent<TapRotatable>();
                rotatable.enabled = true;
            });
        }

        return result;
    }

    protected override bool IsWinStateFufilled()
    {
        return _target.transform.eulerAngles == Vector3.zero;
    }
}

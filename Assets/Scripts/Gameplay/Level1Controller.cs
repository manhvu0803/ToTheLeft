using DG.Tweening;
using UnityEngine;

public class Level1Controller : FreeSlotLevelController
{
    private Transform _target;

    public override bool UpdateSlot(Transform target, Transform slot, bool isInteracting = false)
    {
        if (isInteracting)
        {
            return false;
        }

        var result = base.UpdateSlot(target, slot);

        if (slot != null && OccupantMap.TryGetValue(slot, out var slotTarget) && target == slotTarget)
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

    protected override float CompletionRate()
    {
        if (_target == null)
        {
            return 0;
        }

        // _target != null means the picture is in the right place, hence at least 50% complete (0.5f)
        // The other half is just how much farther from 0 degree the picture is
        return 0.5f + (1 - Vector3.Angle(_target.up, Vector3.up) / 180) / 2;
    }
}

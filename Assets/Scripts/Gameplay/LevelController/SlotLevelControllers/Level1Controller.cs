using DG.Tweening;
using UnityEngine;

public class Level1Controller : FreeSlotLevelController
{
    private Transform _target;

    public override bool UpdateSlot(Transform target, Transform slotTransform, bool isInteracting = false)
    {
        if (isInteracting)
        {
            return false;
        }

        var result = base.UpdateSlot(target, slotTransform);

        if (slotTransform != null
            && SlotTransforms.TryGetValue(slotTransform, out var slot)
            && slot.IsTarget(target))
        {
            _target = target;

            DOVirtual.DelayedCall(0.15f, () =>
            {
                Destroy(target.GetComponent<SlottedMovable>());
                var rotatable = target.GetComponent<TapRotatable>();
                rotatable.enabled = true;
            });
        }

        return result;
    }

    public override float CompletionRate()
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

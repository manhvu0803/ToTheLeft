using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeSlotLevelController : SlotLevelController
{
    protected readonly Dictionary<Transform, Transform> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    /// <summary>
    /// Try to put the target transform into slot and return the result
    /// </summary>
    /// <returns>true if the slot is empty, false otherwise</returns>
    public override bool UpdateSlot(Transform target, Transform slot, bool isInteracting = false)
    {
        if (isInteracting)
        {
            return false;
        }

        if (SlotMap.TryGetValue(target, out var oldSlot) && oldSlot != null)
        {
            OccupantMap[oldSlot] = null;
        }

        if (slot != null)
        {
            // slot is occupied
            if (OccupantMap.TryGetValue(slot, out var occupant) && occupant != null && occupant != target)
            {
                return false;
            }

            OccupantMap[slot] = target;
        }

        SlotMap[target] = slot;
        return true;
    }

    protected override bool IsWinStateFufilled()
    {
        foreach (var slot in Slots)
        {
            if (!OccupantMap.TryGetValue(slot.transform, out var occupant) || occupant != slot.Target)
            {
                return false;
            }
        }

        return true;
    }
}

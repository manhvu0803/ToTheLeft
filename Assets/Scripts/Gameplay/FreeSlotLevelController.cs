using System.Collections.Generic;
using UnityEngine;

public class FreeSlotLevelController : SlotLevelController
{
    protected readonly Dictionary<Transform, Transform> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    public List<string> Occs;

    public List<string> Slts;

    public void Write()
    {
        Occs = new List<string>(OccupantMap.Count);

        foreach (var pair in OccupantMap)
        {
            Occs.Add($"{pair.Key?.name ?? "null"} {pair.Value?.name ?? "null"}");
        }

        Slts = new List<string>(SlotMap.Count);

        foreach (var pair in SlotMap)
        {
            Slts.Add($"{pair.Key?.name ?? "null"} {pair.Value?.name ?? "null"}");
        }
    }

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
                Write();
                return false;
            }

            OccupantMap[slot] = target;
        }

        SlotMap[target] = slot;
        Write();
        return true;
    }

    protected override float CompletionRate()
    {
        var wrongSlotCount = 0;

        foreach (var slot in Slots)
        {
            if (!OccupantMap.TryGetValue(slot.transform, out var occupant) || occupant == null || occupant != slot.Target)
            {
                wrongSlotCount++;
            }
        }

        return 1 - (float)wrongSlotCount / Slots.Length;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeSlotLevelController : SlotLevelController
{
    [Serializable]
    public struct Pair
    {
        public Transform Slot;

        public Transform Target;
    }

    public Pair[] InitialPairs;

    protected readonly Dictionary<Transform, Transform> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

#if DEBUG || UNITY_EDITOR
    [Header("Debug")]
    public List<string> Occs;

    public List<string> Slts;

    public void Write()
    {
        Occs = new List<string>(OccupantMap.Count);

        foreach (var pair in OccupantMap)
        {
            Occs.Add($"{pair.Key?.name ?? "null"}-{pair.Value?.name ?? "null"}");
        }

        Slts = new List<string>(SlotMap.Count);

        foreach (var pair in SlotMap)
        {
            Slts.Add($"{pair.Key?.name ?? "null"}-{pair.Value?.name ?? "null"}");
        }
    }
#endif

    protected override void Start()
    {
        base.Start();

        foreach (var pair in InitialPairs)
        {
            UpdateSlot(pair.Target, pair.Slot);
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
            target.Translate(0, 0, -0.5f);
        }

        if (slot != null)
        {
            // slot is occupied
            if (OccupantMap.TryGetValue(slot, out var occupant) && occupant != null && occupant != target)
            {
                SlotMap[target] = null;
#if DEBUG || UNITY_EDITOR
                Write();
#endif
                return false;
            }

            OccupantMap[slot] = target;
        }

        SlotMap[target] = slot;
#if DEBUG || UNITY_EDITOR
        Write();
#endif
        return true;
    }

    public override float CompletionRate()
    {
        var correctSlotCount = 0;

        foreach (var slot in Slots)
        {
            if (OccupantMap.TryGetValue(slot.transform, out var occupant) && slot.IsTarget(occupant))
            {
                correctSlotCount++;
            }
        }

        return (float)correctSlotCount / Slots.Length;
    }
}

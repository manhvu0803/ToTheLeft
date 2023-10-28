using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeSlotLevelController : SlotLevelController
{
    [Serializable]
    public struct Pair
    {
        public Slot Slot;

        public Transform Target;
    }

    public Pair[] InitialPairs;

    /// <summary>
    /// Slot to occupant map
    /// </summary>
    protected readonly Dictionary<Transform, Transform> OccupantMap = new();

    /// <summary>
    /// Occupant to slot map
    /// </summary>
    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    protected override void OnValidate()
    {
        base.OnValidate();

        for (int i = 0; i < InitialPairs.Length; ++i)
        {
            var pair = InitialPairs[i];

            if (pair.Slot == null && pair.Slot is not null)
            {
                InitialPairs[i].Slot = pair.Slot.GetComponentInChildren<Slot>();
            }
        }
    }

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

        // In case some levels weren't reimported yet
        if (InitialPairs == null)
        {
            return;
        }

        foreach (var pair in InitialPairs)
        {
            UpdateSlot(pair.Target, pair.Slot.transform);
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

    public override void Hint()
    {
        var interactables = FindObjectsOfType<Interactable>();

        foreach (var interactable in interactables)
        {
            var currentSlot = GetSlot(interactable.transform);

            if (currentSlot != null && currentSlot.IsTarget(interactable.transform))
            {
                continue;
            }

            foreach (var slot in Slots)
            {
                if (slot.IsTarget(interactable.transform) && (currentSlot == null || currentSlot != slot))
                {
                    slot.Hint();
                    interactable.transform.DoScaleUpDown();
                    return;
                }
            }
        }
    }

    private Slot GetSlot(Transform occupant)
    {
        if (SlotMap.TryGetValue(occupant, out var slotTransform)
            && slotTransform != null
            && SlotTransforms.TryGetValue(slotTransform, out var slot))
        {
            return slot;
        }

        return null;
    }
}

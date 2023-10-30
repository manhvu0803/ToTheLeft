using System;
using UnityEngine;

public class SymmetricSlotLevelController : FreeSlotLevelController
{
    [Header("Symmetric slot")]
    [SerializeField]
    protected Slot[] LeftSlots;

    [SerializeField]
    protected Slot[] RightSlots;

    protected override void OnValidate()
    {
        if (LeftSlots.Length != RightSlots.Length)
        {
            Debug.LogWarning("The amount of left-side slots is different from right-side slots");
        }
    }

    public override float CompletionRate
    {
        get {
            var correctCount = 0;
            var limit = Math.Min(LeftSlots.Length, RightSlots.Length);

            for (int i = 0; i < limit; ++i)
            {
                if (OccupantMap.TryGetValue(LeftSlots[i].transform, out var left)
                    && left != null
                    && OccupantMap.TryGetValue(RightSlots[i].transform, out var right)
                    && right != null
                    && left.CompareTag(right.tag))
                {
                    correctCount++;
                }
            }

            foreach (var slot in Slots)
            {
                if (OccupantMap.TryGetValue(slot.transform, out var occupant) && occupant != null)
                {
                    correctCount++;
                }
            }

            return (float)correctCount / (RightSlots.Length + Slots.Length);
        }
    }
}
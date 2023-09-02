using System;
using System.Collections.Generic;
using UnityEngine;

public class SlottedLevelController : LevelController
{
    [Header("Slot")]
    private static SlottedLevelController _instance;

    public static SlottedLevelController Instance => _instance;

    public LayerMask SlotLayers;

    [SerializeField]
    protected Slot[] Slots;

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    protected readonly Dictionary<Transform, Transform> OccupantMap = new();


    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Try to put the target transform into slot and return the result
    /// </summary>
    /// <param name="target"></param>
    /// <param name="slot"></param>
    /// <returns>true if the slot is empty, false otherwise</returns>
    public virtual bool UpdateSlot(Transform target, Transform slot)
    {
        if (OccupantMap.TryGetValue(target, out var oldSlot) && oldSlot != null)
        {
            SlotMap[oldSlot] = null;
        }

        if (slot != null)
        {
            // slot is occupied
            if (SlotMap.TryGetValue(slot, out var occupant) && occupant != null && occupant != target)
            {
                return false;
            }

            SlotMap[slot] = target;
        }

        OccupantMap[target] = slot;
        return true;
    }

    protected override bool IsWinStateFufilled()
    {
        foreach (var slot in Slots)
        {
            if (!SlotMap.TryGetValue(slot.transform, out var occupant) || occupant != slot.Target)
            {
                return false;
            }
        }

        return true;
    }
}

using System.Collections.Generic;
using UnityEngine;

public abstract class SlotLevelController : LevelController
{
    public LayerMask SlotLayers;

    [SerializeField]
    protected Slot[] Slots;

    protected readonly Dictionary<Transform, Slot> SlotTransforms = new();

    protected virtual void Start()
    {
        foreach (var slot in Slots)
        {
            SlotTransforms.Add(slot.transform, slot);
        }
    }

    /// <summary>
    /// Try to put the target transform into slot and return the result
    /// </summary>
    /// <returns>true if the slot is empty, false otherwise</returns>
    public abstract bool UpdateSlot(Transform target, Transform slot, bool isInteracting = false);
}

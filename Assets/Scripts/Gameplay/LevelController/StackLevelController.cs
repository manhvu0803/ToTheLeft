using System.Collections.Generic;
using UnityEngine;

public class StackLevelController : SlotLevelController
{
    [SerializeField]
    private Transform[] _targets;

    protected readonly Dictionary<Transform, HashSet<Transform>> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    public override bool UpdateSlot(Transform target, Transform slotTransform, bool isInteracting = false)
    {
        if (isInteracting)
        {
            return false;
        }

        if (SlotMap.TryGetValue(target, out var oldSlot) && oldSlot != null && OccupantMap.TryGetValue(oldSlot, out var occupants))
        {
            occupants.Remove(target);

            foreach (var occupant in occupants)
            {
                occupant.Translate(0, 0, -0.5f);
            }
        }

        if (slotTransform != null)
        {
            if (!OccupantMap.TryGetValue(slotTransform, out occupants))
            {
                occupants = new HashSet<Transform>();
                OccupantMap.Add(slotTransform, occupants);
            }

            foreach (var occupant in occupants)
            {
                occupant.Translate(0, 0, 0.5f);
            }

            occupants.Add(target);
        }

        SlotMap[target] = slotTransform;
        target.parent = slotTransform;
        return true;
    }

    public override float CompletionRate()
    {
        var correctCount = 0;

        if (IsInCorrectSlot(_targets[0]))
        {
            correctCount++;
        }

        for (int i = 1; i < _targets.Length; ++i)
        {
            if (IsInCorrectSlot(_targets[i]) && _targets[i].position.z < _targets[i - 1].position.z)
            {
                correctCount++;
            }
        }

        return (float)correctCount / _targets.Length;
    }

    private bool IsInCorrectSlot(Transform transform)
    {
        return SlotMap.TryGetValue(transform, out var slotTransform)
            && slotTransform != null
            && SlotTransforms.TryGetValue(slotTransform, out var slot)
            && slot.IsTarget(transform);
    }
}

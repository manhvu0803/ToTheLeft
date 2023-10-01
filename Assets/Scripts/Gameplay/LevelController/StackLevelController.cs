using System.Collections.Generic;
using UnityEngine;

public class StackLevelController : SlotLevelController
{
    [SerializeField]
    private Transform[] _targets;

    // Use list to allow removing any item at any time
    protected readonly Dictionary<Transform, List<Transform>> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    protected readonly Dictionary<Transform, int> Positions = new();

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _targets.Length; ++i)
        {
            Positions[_targets[i]] = i;
        }
    }

    public override bool UpdateSlot(Transform target, Transform slotTransform, bool isInteracting = false)
    {
        if (isInteracting || !Positions.TryGetValue(target, out var targetCurrentPosition))
        {
            return false;
        }

        // The target is removed from the stack
        if (SlotMap.TryGetValue(target, out var oldSlot) && oldSlot != null && OccupantMap.TryGetValue(oldSlot, out var occupants))
        {
            occupants.Remove(target);
            target.Translate(0, 0, -0.5f);

            foreach (var occupant in occupants)
            {
                occupant.Translate(0, 0, -0.5f);
            }
        }

        if (slotTransform != null)
        {
            if (!OccupantMap.TryGetValue(slotTransform, out occupants))
            {
                occupants = new List<Transform>();
                OccupantMap.Add(slotTransform, occupants);
            }

            // If the object current top obbject in the stack is smaller (has higher correct position) than the current added object
            if (occupants.Count > 0 && Positions[occupants[^1]] > targetCurrentPosition)
            {
                return false;
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

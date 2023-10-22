using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MultiTargetSlotLevelController : SlotLevelController
{
    [Header("Targets")]
    [SerializeField]
    private Interactable[] _targets;

    protected readonly Dictionary<Transform, HashSet<Transform>> OccupantMap = new();

    protected readonly Dictionary<Transform, Transform> SlotMap = new();

    protected override void OnValidate()
    {
        base.OnValidate();
        Utils.Fill(ref _targets);
    }

    public override bool UpdateSlot(Transform target, Transform slotTransform, bool isInteracting = false)
    {
        if (isInteracting)
        {
            return false;
        }

        // The target is removed from the stack
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
        return true;
    }

    public override float CompletionRate()
    {
        var correctCount = 0;

        for (int i = 0; i < _targets.Length; ++i)
        {
            if (IsInCorrectSlot(_targets[i]))
            {
                correctCount++;
            }
        }

        return (float)correctCount / _targets.Length;
    }

    private bool IsInCorrectSlot(Interactable target)
    {
        return SlotMap.TryGetValue(target.transform, out var slotTransform)
            && slotTransform != null
            && SlotTransforms.TryGetValue(slotTransform, out var slot)
            && slot.IsTarget(target.transform);
    }

    public override void Hint()
    {
        for (int i = 0; i < _targets.Length; ++i)
        {
            if (!IsInCorrectSlot(_targets[i]))
            {
                _targets[i].transform.DoScaleUpDown();
                break;
            }
        }
    }
}

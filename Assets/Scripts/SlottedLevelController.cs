using System;
using System.Collections.Generic;
using UnityEngine;

public class SlottedLevelController : LevelController
{
    [Serializable]
    public struct Slot
    {
        public Transform Transform;

        public Transform Target;
    }

    private static SlottedLevelController _instance;

    public static SlottedLevelController Instance => _instance;

    [SerializeField]
    private Slot[] _slots;

    private readonly Dictionary<Transform, Transform> _slotMap = new();

    private readonly Dictionary<Transform, Transform> _occupantMap = new();

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
    public bool UpdateSlot(Transform target, Transform slot)
    {
        if (slot == null)
        {
            if (_occupantMap.TryGetValue(target, out slot) && slot != null)
            {
                _slotMap[slot] = null;
            }

            _occupantMap[target] = slot;
            return true;
        }

        if (_slotMap.TryGetValue(slot, out var occupant) && occupant != null && occupant != target)
        {
            return false;
        }

        if (_occupantMap.TryGetValue(target, out var oldSlot) && oldSlot != null)
        {
            _slotMap[oldSlot] = null;
        }

        _slotMap[slot] = target;
        _occupantMap[target] = slot;
        return true;
    }

    protected override bool IsWinStateFufilled()
    {
        foreach (var slot in _slots)
        {
            if (!_slotMap.TryGetValue(slot.Transform, out var occupant) || occupant != slot.Target)
            {
                return false;
            }
        }

        return true;
    }
}

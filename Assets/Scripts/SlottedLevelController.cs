using System;
using UnityEngine;

public class SlottedLevelController : LevelController
{
    [Serializable]
    public struct Slot
    {
        public Transform Transform;

        public Transform Target;
    }

    [SerializeField]
    private Slot[] _slots;

    protected override bool IsWinStateFufilled()
    {
        foreach (var slot in _slots)
        {
            if (slot.Target.position != slot.Transform.position)
            {
                return false;
            }
        }

        return true;
    }
}

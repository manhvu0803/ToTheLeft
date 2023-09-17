using UnityEngine;

public class MultiTargetSlot : Slot
{
    [SerializeField]
    private Transform[] _targets;

    public override bool IsTarget(Transform transform)
    {
        foreach (var target in _targets)
        {
            if (target == transform)
            {
                return true;
            }
        }

        return false;
    }
}
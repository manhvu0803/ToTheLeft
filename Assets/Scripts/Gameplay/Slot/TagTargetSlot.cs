using UnityEngine;

public class TagTargetSlot : Slot
{
    public string TargetTag;

    public override bool IsTarget(Transform transform)
    {
        if (transform == null)
        {
            return false;
        }

        return transform.CompareTag(TargetTag);
    }
}
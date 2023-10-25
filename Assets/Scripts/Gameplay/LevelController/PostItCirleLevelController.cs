using UnityEngine;

public class PostItCirleLevelController : FreeSlotLevelController
{
    public override float CompletionRate()
    {
        var correctSlotCount = 0;
        var correctRotationCount = 0;

        foreach (var slot in Slots)
        {
            if (OccupantMap.TryGetValue(slot.transform, out var occupant) && slot.IsTarget(occupant))
            {
                correctSlotCount++;

                if (Mathf.Abs(occupant.transform.eulerAngles.z - slot.transform.eulerAngles.z) <= 0.01f)
                {
                    correctRotationCount++;
                }
            }
        }

        return (float)(correctSlotCount + correctRotationCount) / (Slots.Length * 2);
    }
}

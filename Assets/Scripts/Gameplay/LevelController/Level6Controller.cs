using UnityEngine;

public class Level6Controller : LevelController
{
    [SerializeField]
    private Movable[] _targets;

    protected void OnValidate()
    {
        Utils.Fill(ref _targets);

        foreach (var target in _targets)
        {
            if (target == null)
            {
                continue;
            }

            target.MaxLocalOffset = new Vector2(1000, 0);
            target.SnapDistance = 0.4f;
            target.OffsetScaleOnDrag = Vector3.zero;
        }
    }

    public override float CompletionRate()
    {
        var correctCount = 0;

        foreach (var target in _targets)
        {
            if (((Vector2)target.transform.localPosition).sqrMagnitude <= 0.01f)
            {
                correctCount++;
            }
        }

        return (float)correctCount / _targets.Length;
    }
}

using UnityEngine;
using UnityEngine.Serialization;

public class FakeRotateLevelController : LevelController
{
    public float SnapDistance = 0.6f
    ;
    [SerializeField, FormerlySerializedAs("_cans")]
    private FakeRotatable[] _rotatables;

    private void OnValidate()
    {
        Utils.Fill(ref _rotatables);

        if (_rotatables != null)
        {
            foreach (var rotatable in _rotatables)
            {
                var offset = Random.Range(-3, 3f);
                rotatable.InitialOffset = offset + ((offset > 0) ? 2 : -2);
                rotatable.SnapDistance = SnapDistance;
            }
        }
    }

    public override float CompletionRate()
    {
        var completedCount = 0;

        foreach (var rotatable in _rotatables)
        {
            if (Mathf.Abs(rotatable.Offset) <= 0.1f)
            {
                completedCount++;
            }
        }

        return (float)completedCount / _rotatables.Length;
    }

    public override void Hint()
    {
        foreach (var rotatable in _rotatables)
        {
            if (Mathf.Abs(rotatable.Offset) > 0.1f)
            {
                rotatable.transform.DoScaleUpDown();
                break;
            }
        }
    }
}

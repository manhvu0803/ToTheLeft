using System;
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
                var offset = UnityEngine.Random.Range(-3, 3f);
                rotatable.InitialOffset = offset + ((offset > 0) ? 2 : -2);
                rotatable.SnapDistance = SnapDistance;
            }
        }
    }

    public override float CompletionRate
    {
        get {
            var completedCount = 0;

            try
            {
                foreach (var rotatable in _rotatables)
                {
                    if (Mathf.Abs(rotatable.Offset) <= 0.1f)
                    {
                        completedCount++;
                    }
                }
            }
            // In case the rotatables aren't ready
            catch (NullReferenceException)
            {
                return 0;
            }

            return (float)completedCount / _rotatables.Length;
        }
    }

    public override void Hint()
    {
        foreach (var rotatable in _rotatables)
        {
            if (Mathf.Abs(rotatable.Offset) > 0.1f)
            {
                if (rotatable.transform.parent != null)
                {
                    rotatable.transform.parent.DoScaleUpDown();
                }
                else
                {
                    rotatable.transform.DoScaleUpDown();
                }

                break;
            }
        }
    }
}

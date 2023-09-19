using UnityEngine;

public class Level9Controller : LevelController
{
    public float SnapDistance = 0.6f
    ;
    [SerializeField]
    private FakeRotatable[] _cans;

    private void OnValidate()
    {
        Utils.Fill(ref _cans);

        if (_cans != null)
        {
            foreach (var can in _cans)
            {
                var offset = Random.Range(-3, 3f);
                can.InitialOffset = offset + ((offset > 0) ? 2 : -2);
                can.SnapDistance = SnapDistance;
            }
        }
    }

    public override float CompletionRate()
    {
        var completedCount = 0;

        foreach (var can in _cans)
        {
            if (Mathf.Abs(can.Offset) <= 0.1f)
            {
                completedCount++;
            }
        }

        return (float)completedCount / _cans.Length;
    }
}

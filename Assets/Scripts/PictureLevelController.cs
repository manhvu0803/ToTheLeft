using UnityEngine;

public class PictureLevelController : LevelController
{
    [SerializeField]
    private Transform[] _pictures;

    protected override bool IsWinStateFufilled()
    {
        foreach (var target in _pictures)
        {
            if (Vector3.Angle(target.up, Vector3.up) >= 0.5f)
            {
                return false;
            }
        }

        return true;
    }
}

using UnityEngine;

public class PictureLevelController : LevelController
{
    [SerializeField]
    private Transform[] _pictures;

    [SerializeField]
    private Transform _level;

    protected override bool IsWinStateFufilled()
    {
        if (_level != null && _level.position != Vector3.zero)
        {
            return false;
        }

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

using UnityEngine;

public class PictureLevelController : LevelController
{
    [SerializeField]
    private Transform[] _pictures;

    protected void Start()
    {
        OnCompletionRateChanged.AddListener(Check);
    }

    private void Check(float completionRate)
    {
        if (completionRate < 1)
        {
            return;
        }

        foreach (var picture in _pictures)
        {
            picture.GetComponent<Collider2D>().enabled = false;
        }
    }

    public override float CompletionRate
    {
        get {
            // How much farther all the pictures is from 0 degree
            var angleSum = 0f;

            foreach (var target in _pictures)
            {
                angleSum += Vector3.Angle(target.up, Vector3.up);
            }

            return 1 - angleSum / 180 / _pictures.Length;
        }
    }

    public override void Hint()
    {
        foreach (var target in _pictures)
        {
            if (Vector3.Angle(target.up, Vector3.up) > 0.001f)
            {
                target.DoScaleUpDown();
                break;
            }
        }
    }
}

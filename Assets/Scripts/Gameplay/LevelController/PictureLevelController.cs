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
            var correctCount = 0;

            foreach (var target in _pictures)
            {
                if (target.transform.eulerAngles.z <= 0.0001f)
                {
                    correctCount++;
                }
            }

            return (float)correctCount / _pictures.Length;
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

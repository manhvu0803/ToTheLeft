using DG.Tweening;
using UnityEngine;

public class TapRotatable : MonoBehaviour
{
    [Range(-360, 360)]
    public float Rotation = 45;

    public new bool enabled = true;

    private void OnMouseUpAsButton()
    {
        if (!enabled)
        {
            return;
        }

        transform.DOLocalRotate(transform.eulerAngles + new Vector3(0, 0, Rotation), 0.15f, RotateMode.FastBeyond360);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}

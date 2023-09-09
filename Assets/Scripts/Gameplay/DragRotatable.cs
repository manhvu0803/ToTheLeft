using UnityEngine;

public class DragRotatable : Interactable
{
    [Min(0)]
    public float SnapAngle = 0.5f;

    public float MaxAngle = 90;

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePositon)
    {
        if (Vector3.Angle(transform.up, Vector3.up) <= SnapAngle)
        {
            transform.up = Vector3.up;
        }

        transform.Rotate(0, 0, delta.x);
        var angle = transform.localEulerAngles;
        angle.z = Utils.Clamp360Angle(angle.z, MaxAngle);
        transform.localEulerAngles = angle;
    }
}

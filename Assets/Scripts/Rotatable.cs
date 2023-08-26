using UnityEngine;

public class Rotatable : Interactable
{
    [Min(0)]
    public float SnapAngle = 0.5f;

    public float MaxAngle = 90;

    protected override void OnInteract(Vector2 delta)
    {
        if (Vector3.Angle(transform.up, Vector3.up) <= SnapAngle)
        {
            transform.up = Vector3.up;
        }

        transform.Rotate(0, 0, delta.x);
        var angle = transform.localEulerAngles;
        angle.z = Clamp360Angle(angle.z, MaxAngle);
        transform.localEulerAngles = angle;
    }

    private static float Clamp360Angle(float value, float clampValue)
    {
        if (value > clampValue && value <= 180)
        {
            return clampValue;
        }

        if (value < 360 - clampValue && value > 180)
        {
            return -clampValue;
        }

        return value;
    }
}

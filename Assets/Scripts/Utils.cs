using UnityEngine;

public static class Utils
{
    public static void Fill<T>(this Component component, ref T target) where T : Component
    {
        if (target == null)
        {
            target = component.GetComponent<T>();
        }
    }

    public static void FillFromChildren<T>(this Component component, ref T target) where T : Component
    {
        if (target == null)
        {
            target = component.GetComponentInChildren<T>();
        }
    }

    /// <summary>
    /// Clamp a 360 angle value to -clampValue and clampValue
    /// </summary>
    /// <param name="value">Value larger than 360 will be modulo to 360</param>
    /// <param name="clampValue">Value larger than 360 will be modulo to 180</param>
    /// <returns></returns>
    public static float Clamp360Angle(this float value, float clampValue)
    {
        value %= 360;
        clampValue %= 180;

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
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class Utils
{
    public delegate void SetAction(ref Vector3 vector);

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

    public static void Fill<T>(ref T[] target) where T : Component
    {
        if (target == null || target.Length <= 0)
        {
            target = Object.FindObjectsOfType<T>();
        }
    }

    public static void Fill<T>(ref List<T> target) where T : Component
    {
        if (target == null || target.Count <= 0)
        {
            target = new List<T>(Object.FindObjectsOfType<T>());
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

    public static void SetX(this Transform transform, float x)
    {
        var position = transform.position;
        position.x = x;
        transform.position = position;
    }

    public static void SetY(this Transform transform, float y)
    {
        var position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public static void SetZ(this Transform transform, float z)
    {
        var position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static void DoScaleUpDown(this Transform transform, float multiplier = 1.25f, float duration = 0.2f, int times = 2)
    {
        transform.DOScale(transform.localScale * multiplier, duration).SetLoops(times * 2, LoopType.Yoyo);
    }
}
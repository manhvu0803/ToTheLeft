using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class SingletonManager
{
    private static readonly Dictionary<Type, object> Singletons = new();

    // Using generics will force type inference, which pass param as the type of super class, not current class
    // So we pass object instead
    public static void Add(object singleton)
    {
        var type = singleton.GetType();

#if UNITY_EDITOR || DEBUG
        if (Singletons.TryGetValue(type, out var instance) && instance != null)
        {
            Debug.LogWarning($"Overriding {type.Name} in singleton manager");
        }
#endif

        while (type != null && type != typeof(MonoBehaviour))
        {
            Singletons[type] = singleton;
            type = type.BaseType;
        }
    }

    public static T Get<T>()
    {
        Singletons.TryGetValue(typeof(T), out var value);
        return (T)value;
    }

    public static void Remove(object singleton)
    {
        var type = singleton.GetType();

        while (type != null && type != typeof(MonoBehaviour))
        {
            Singletons[type] = null;
            type = type.BaseType;
        }
    }
}

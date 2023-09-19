using System;
using System.Collections.Generic;
using UnityEngine;

public static class SingletonManager
{
    private static readonly Dictionary<Type, object> Singletons = new();

    // Using generics will force type inference, which pass param as the type of super class, not current class
    // So we pass object instead
    public static void Add(object singleton)
    {
        var type = singleton.GetType();

#if UNITY_EDITOR || DEBUG
        if (Singletons.ContainsKey(type))
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
}

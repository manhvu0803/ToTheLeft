using System;
using System.Collections.Generic;
using UnityEngine;

public static class SingletonManager
{
    private static readonly Dictionary<Type, object> Singletons = new();

    // Using generics will force type inference make param always be type of calling class
    public static void Add(object singleton)
    {
        var type = singleton.GetType();

        while (type != typeof(MonoBehaviour))
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

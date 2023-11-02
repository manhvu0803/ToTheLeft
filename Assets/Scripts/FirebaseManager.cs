using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseApp App { get; private set; }

    public static FirebaseRemoteConfig RemoteConfig { get; private set; }

    public static bool IsRemoteConfigReady { get; private set; } = false;

    private static readonly Dictionary<string, object> Defaults = new();

    private static void InitRemoteConfig()
    {
        RemoteConfig = FirebaseRemoteConfig.DefaultInstance;
        RemoteConfig.SetDefaultsAsync(Defaults);
        RemoteConfig.FetchAndActivateAsync()
            .ContinueWithOnMainThread(task => IsRemoteConfigReady = true);
    }

    protected void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                InitRemoteConfig();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }
}
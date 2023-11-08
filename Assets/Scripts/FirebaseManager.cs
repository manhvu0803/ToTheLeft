using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Crashlytics;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseApp App { get; private set; }

    public static FirebaseRemoteConfig RemoteConfig { get; private set; }

    public static bool IsRemoteConfigReady { get; private set; } = false;

    private static readonly Dictionary<string, object> Defaults = new()
    {
        { "AdsExtraTime", 15 },
        { "AdsExtraHintCount", 1 },
        { "NewLevelConmpletedBonusHintCount", 1 },
        { "AllowProgressBar", true },
        { "InterstitalAdsLevelPacing", 1 },
        { "InterstitalAdsTimePacing", 30 }
    };

    public static float[] LevelTimeLimits { get; private set; }

    public static float AdsExtraTime { get; private set; }

    public static int AdsExtraHintCount { get; private set; }

    public static int NewLevelConmpletedBonusHintCount { get; private set; }

    public static bool AllowProgressBar { get; private set; }

    public static float InterstitalAdsTimePacing { get; private set; }

    public static int InterstitalAdsLevelPacing { get; private set; }

    private static void InitRemoteConfig()
    {
        RemoteConfig = FirebaseRemoteConfig.DefaultInstance;
        RemoteConfig.SetDefaultsAsync(Defaults);
        RemoteConfig.FetchAndActivateAsync()
            .ContinueWithOnMainThread(ReadRemoteConfig);
    }

    private static void ReadRemoteConfig(Task<bool> task)
    {
        Debug.Log("Fetch and activate: " + task.Result);

        try
        {
            var timeLimitConfig = RemoteConfig.GetValue("LevelTimeLimit");
            Debug.Log($"Firebase source: {timeLimitConfig.Source}");
            LevelTimeLimits = Utils.ArrayFromJson<float>(timeLimitConfig.StringValue);
            AdsExtraTime = (float)RemoteConfig.GetValue("AdsExtraTime").DoubleValue;
            AdsExtraHintCount = (int)RemoteConfig.GetValue("AdsExtraHintCount").LongValue;
            NewLevelConmpletedBonusHintCount = (int)RemoteConfig.GetValue("NewLevelConmpletedBonusHintCount").LongValue;
            AllowProgressBar = RemoteConfig.GetValue("AllowProgressBar").BooleanValue;
            InterstitalAdsLevelPacing = (int)RemoteConfig.GetValue("InterstitalAdsLevelPacing").LongValue;
            InterstitalAdsTimePacing = (float)RemoteConfig.GetValue("InterstitalAdsTimePacing").DoubleValue;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        IsRemoteConfigReady = true;
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
                IsRemoteConfigReady = true;
            }
        });
    }
}
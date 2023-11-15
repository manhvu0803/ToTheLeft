using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Crashlytics;
using UnityEngine;
using System;

public class FirebaseManager : MonoBehaviour
{
    public static bool IsRemoteConfigReady { get; private set; } = false;

    private static readonly Dictionary<string, object> Defaults = new()
    {
        { "AdsExtraTime", 15 },
        { "AdsExtraHintCount", 1 },
        { "NewLevelConmpletedBonusHintCount", 1 },
        { "AllowProgressBar", true },
        { "InterstitalAdsLevelPacing", 1 },
        { "InterstitalAdsTimePacing", 30 },
        { "HintButtonFlashDelay", 5 },
        { "AllowMoreTimeButton", false }
    };

    public static float[] LevelTimeLimits { get; private set; }

    public static float AdsExtraTime { get; private set; }

    public static int AdsExtraHintCount { get; private set; }

    public static int NewLevelConmpletedBonusHintCount { get; private set; }

    public static bool AllowProgressBar { get; private set; }

    public static float InterstitalAdsTimePacing { get; private set; }

    public static int InterstitalAdsLevelPacing { get; private set; }

    public static float HintButtonFlashDelay { get; private set; }

    public static bool AllowMoreTimeButton { get; private set; }

    private async static void InitRemoteConfig()
    {
        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        await remoteConfig.SetDefaultsAsync(Defaults);
        await remoteConfig.FetchAsync(TimeSpan.Zero);
        var justActivated = await remoteConfig.ActivateAsync();
        Debug.Log("Firebase fetch and activate: " + justActivated);
        GetRemoteConfigs();
        IsRemoteConfigReady = true;
    }

    private static void GetRemoteConfigs()
    {
        try
        {
            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var timeLimitConfig = remoteConfig.GetValue("LevelTimeLimit");
            LevelTimeLimits = Utils.ArrayFromJson<float>(timeLimitConfig.StringValue);
            AdsExtraTime = (float)remoteConfig.GetValue("AdsExtraTime").DoubleValue;
            AdsExtraHintCount = (int)remoteConfig.GetValue("AdsExtraHintCount").LongValue;
            NewLevelConmpletedBonusHintCount = (int)remoteConfig.GetValue("NewLevelConmpletedBonusHintCount").LongValue;
            AllowProgressBar = remoteConfig.GetValue("AllowProgressBar").BooleanValue;
            InterstitalAdsLevelPacing = (int)remoteConfig.GetValue("InterstitalAdsLevelPacing").LongValue;
            InterstitalAdsTimePacing = (float)remoteConfig.GetValue("InterstitalAdsTimePacing").DoubleValue;
            HintButtonFlashDelay = (float)remoteConfig.GetValue("HintButtonFlashDelay").DoubleValue;
            AllowMoreTimeButton = remoteConfig.GetValue("AllowMoreTimeButton").BooleanValue;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    protected void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
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
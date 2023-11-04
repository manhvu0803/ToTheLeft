using System;
using System.Collections;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    #region Static
    public static readonly string IronSourceKey = "1c438d4bd";

    private static Action OnCompleteAction;

    private static Action<IronSourceError> OnErrorAction;

    public static AdsManager Instance { get; private set; }
    public static bool IsReady { get; private set; } = false;

    private static int NextLevelCount = 0;

    private static float LastAdsShowTime;

    private static void OnLoadingLevelComplete()
    {
        NextLevelCount++;

        if (NextLevelCount >= FirebaseManager.InterstitalAdsLevelPacing
            && Time.realtimeSinceStartup - LastAdsShowTime >= FirebaseManager.InterstitalAdsTimePacing)
        {
            ShowInterstitalAd();
        }
    }

    public static void ShowRewardedAd(Action onComplete, Action<IronSourceError> onError = null)
    {
        if (!IronSource.Agent.isRewardedVideoAvailable() || !IsReady)
        {
            Debug.Log("No ads available");
            onError?.Invoke(new IronSourceError(-1, "No ads available"));
            return;
        }

        OnCompleteAction = onComplete;
        OnErrorAction = onError;
        IronSource.Agent.showRewardedVideo();
    }

    public static void ShowInterstitalAd(Action onComplete = null, Action<IronSourceError> onError = null)
    {
        if (!IronSource.Agent.isInterstitialReady() || !IsReady)
        {
            Debug.Log("Interstitial ads isn't ready");
            IronSource.Agent.loadInterstitial();
            onError?.Invoke(new IronSourceError(-1, "Interstitial ads isn't ready"));
            return;
        }

        OnCompleteAction = onComplete;
        OnErrorAction = onError;
        IronSource.Agent.showInterstitial();

    }

    private static void InvokeSuccess(IronSourceAdInfo info)
    {
        LastAdsShowTime = Time.realtimeSinceStartup;
        OnCompleteAction?.Invoke();
        ClearCallbacks();
    }

    private static void InvokeError(IronSourceError error, IronSourceAdInfo info)
    {
        print(error);
        print(info);
        OnErrorAction?.Invoke(error);
        ClearCallbacks();
    }

    private static void ClearCallbacks()
    {
        OnCompleteAction = null;
        OnErrorAction = null;
    }

    private static void OnSdkInitializationCompleted()
    {
        print("IronSource init completed");
        IronSource.Agent.validateIntegration();
        IronSource.Agent.loadInterstitial();
        IsReady = true;
    }
    #endregion

    #region Unity callbacks
    protected void Awake()
    {
        Instance = this;
    }

    protected IEnumerator Start()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompleted;

        // Rewarded
        IronSourceRewardedVideoEvents.onAdRewardedEvent += (placement, info) => InvokeSuccess(info);
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += InvokeError;

        // Interstital
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InvokeSuccess;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InvokeError;

        IronSource.Agent.init(IronSourceKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

        yield return new WaitUntil(() => GameController.Instance != null);

        GameController.Instance.OnLoadingLevelComplete.AddListener(OnLoadingLevelComplete);
    }

    protected void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
    #endregion
}

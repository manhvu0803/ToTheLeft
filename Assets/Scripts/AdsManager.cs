using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static readonly string IronSourceKey = "1c438d4bd";

    private static Action _onCompleteAction;

    private static Action _onErrorAction;

    public static AdsManager Instance { get; private set; }

    public static void ShowRewardedAd(Action onComplete, Action onError = null)
    {
        if (!IronSource.Agent.isRewardedVideoAvailable())
        {
            Debug.Log("No ads available");
            onError?.Invoke();
            return;
        }

        _onCompleteAction = onComplete;
        _onErrorAction = onError;
    }

    protected void Awake()
    {
        Instance = this;
        IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        IronSource.Agent.init(IronSourceKey, IronSourceAdUnits.REWARDED_VIDEO);
    }

    private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
    {
        print(placement);
        print(info);
    }

    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
    {
        _onCompleteAction?.Invoke();
        _onCompleteAction = null;
        _onErrorAction = null;
    }

    private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
    {
        print(error);
        print(info);
        _onErrorAction?.Invoke();
        _onCompleteAction = null;
        _onErrorAction = null;
    }

    private void OnSdkInitializationCompletedEvent()
    {
        print("IronSource init completed");
        IronSource.Agent.validateIntegration();
    }

    protected void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}

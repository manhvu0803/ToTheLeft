using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class LevelController : MonoBehaviour
{
    private static readonly float Epsilon = 0.0001f;

    public UnityEvent<float> OnCompletionRateChanged;

    private float _lastCompletionRate;

    protected void Awake()
    {
        SingletonManager.Add(this);
    }

    public void CheckCompletionRate()
    {
        var completionRate = CompletionRate;
#if UNITY_EDITOR || DEBUG
        print("Completion rate: " + completionRate);
#endif

        if (Mathf.Abs(completionRate - _lastCompletionRate) >= Epsilon)
        {
            try
            {
                OnCompletionRateChanged?.Invoke(completionRate);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        _lastCompletionRate = completionRate;

        if (1 - completionRate <= Epsilon)
        {
            enabled = false;

            if (GameController.Instance != null)
            {
                GameController.Instance.CompleteLevel(completionRate);
            }
        }
    }

    public abstract float CompletionRate { get; }

    public abstract void Hint();

    protected virtual void OnDestroy()
    {
        SingletonManager.Remove(this);
    }
}

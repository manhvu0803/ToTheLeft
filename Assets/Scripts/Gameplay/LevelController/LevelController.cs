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
        var completionRate = CompletionRate();
#if UNITY_EDITOR || DEBUG
        print("Completion rate: " + completionRate);
#endif

        if (Mathf.Abs(completionRate - _lastCompletionRate) >= Epsilon)
        {
            OnCompletionRateChanged?.Invoke(completionRate);
        }

        _lastCompletionRate = completionRate;

        if (1 - completionRate <= Epsilon)
        {
            enabled = false;

            if (GameController.Instance != null)
            {
                GameController.Instance.CompleteLevel();
            }
        }
    }

    public abstract float CompletionRate();

    public abstract void Hint();

    protected virtual void OnDestroy()
    {
        SingletonManager.Remove(this);
    }
}

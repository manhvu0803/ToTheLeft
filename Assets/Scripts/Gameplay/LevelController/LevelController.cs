using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class LevelController : MonoBehaviour
{
    private static readonly float Epsilon = 0.01f;

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
            StartCoroutine(WinCorountine());
        }
    }

    private IEnumerator WinCorountine()
    {
        yield return new WaitForSeconds(0.5f);

        enabled = false;
        print("Win");

        if (GameController.Instance != null)
        {
            GameController.Instance.CompleteLevel();
        }
    }

    public abstract float CompletionRate();
}

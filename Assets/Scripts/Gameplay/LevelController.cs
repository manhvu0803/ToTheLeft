using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class LevelController : MonoBehaviour
{
    private static readonly float Epsilon = 0.01f;

    public float WinCheckInterval = 1;

    public UnityEvent<float> OnCompletionRateChanged;

    private float _lastWinCheckTime;

    private float _lastCompletionRate;

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0) || _lastWinCheckTime + WinCheckInterval > Time.time)
        {
            return;
        }

        _lastWinCheckTime = Time.time;
        var completionRate = CompletionRate();

        if (Mathf.Abs(completionRate - _lastCompletionRate) > Epsilon)
        {
            OnCompletionRateChanged?.Invoke(completionRate);
        }

        _lastCompletionRate = completionRate;

        if (completionRate - 1 <= Epsilon)
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

    protected abstract float CompletionRate();
}

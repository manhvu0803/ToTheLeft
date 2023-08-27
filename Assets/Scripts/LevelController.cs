using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    public float WinCheckInterval = 1;

    public UnityEvent OnLevelComplete;

    private bool _isCheckingWinState;

    private float _lastWinCheckTime;

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0) || _lastWinCheckTime + WinCheckInterval > Time.time)
        {
            return;
        }

        _lastWinCheckTime = Time.time;

        if (IsWinStateFufilled())
        {
            if (!_isCheckingWinState)
            {
                StartCoroutine(WinCheck());
            }
        }
        else if (_isCheckingWinState)
        {
            StopAllCoroutines();
            _isCheckingWinState = false;
        }
    }

    private IEnumerator WinCheck()
    {
        _isCheckingWinState = true;
        yield return new WaitForSeconds(0.5f);

        if (IsWinStateFufilled())
        {
            enabled = false;
            OnLevelComplete?.Invoke();
            GameController.Instance?.CompleteLevel();
        }

        _isCheckingWinState = false;
    }

    protected virtual bool IsWinStateFufilled()
    {
        return false;
    }
}

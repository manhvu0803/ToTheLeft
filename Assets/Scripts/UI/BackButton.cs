using UnityEngine;
using UnityEngine.UI;

public class BackButton : Button
{
    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        if (GameController.Instance != null)
        {
            GameController.Instance.OnLevelEnded.AddListener(OnLevelEnded);
            GameController.Instance.OnLoadingLevelComplete.AddListener(OnLoadingLevelComplete);
        }
    }

    private void OnLevelEnded(float completionRate, int levelIndex, int progress)
    {
        enabled = false;
        interactable = false;
    }

    private void OnLoadingLevelComplete()
    {
        enabled = true;
        interactable = true;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onClick?.Invoke();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        if (GameController.Instance != null)
        {
            GameController.Instance.OnLevelEnded.RemoveListener(OnLevelEnded);
            GameController.Instance.OnLoadingLevelComplete.RemoveListener(OnLoadingLevelComplete);
        }
    }
}

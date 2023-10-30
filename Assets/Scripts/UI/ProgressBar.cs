using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private TMP_Text _text;

    private IEnumerator Start()
    {
        yield return null;

        if (GameController.Instance != null)
        {
            GameController.Instance.OnLoadingLevelComplete.AddListener(SubscribeToLevel);
        }
        else
        {
            SubscribeToLevel();
        }
    }

    private void SubscribeToLevel()
    {
        gameObject.SetActive(GameController.Instance == null);
        var levelController = SingletonManager.Get<LevelController>();
        levelController.OnCompletionRateChanged.AddListener(UpdateProgress);
        UpdateProgress(levelController.CompletionRate);
    }

    public void UpdateProgress(float progress)
    {
        if (_text != null)
        {
            _text.text = $"[{Mathf.RoundToInt(progress * 100)}%]";
        }

        if (progress >= 1)
        {
            SingletonManager
                .Get<LevelController>()
                .OnCompletionRateChanged
                .RemoveListener(UpdateProgress);
        }

        _slider.DOValue(progress, 1)
            .SetEase(Ease.OutElastic);
    }
}

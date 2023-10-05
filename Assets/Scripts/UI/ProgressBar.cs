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
        var controller = SingletonManager.Get<LevelController>();
        controller.OnCompletionRateChanged.AddListener(UpdateProgress);
        UpdateProgress(controller.CompletionRate());
    }

    public void UpdateProgress(float progress)
    {
        if (_text != null)
        {
            _text.text = $"[{Mathf.RoundToInt(progress * 100)}%]";
        }

        _slider.DOValue(progress, 1)
            .SetEase(Ease.OutElastic);
    }
}

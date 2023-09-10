using System.Collections;
using System.Collections.Generic;
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
        SingletonManager.Get<LevelController>().OnCompletionRateChanged.AddListener(UpdateProgress);
        UpdateProgress(0);
    }

    public void UpdateProgress(float progress)
    {
        if (_text != null)
        {
            _text.text = $"[{progress * 100:N0}%]";
        }

        _slider.DOValue(progress, 0.75f)
            .SetEase(Ease.OutElastic);
    }
}

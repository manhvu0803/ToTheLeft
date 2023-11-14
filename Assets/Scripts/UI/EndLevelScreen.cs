using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class EndLevelScreen : MonoBehaviour
{
    [Serializable]
    public struct CompletionLevel
    {
        public Image Star;

        public string Message;

        [HideInInspector]
        public Vector3 StarPosition;
    }

    [SerializeField]
    private CompletionLevel[] _completionLevels;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private TMP_Text _completeMessage;

    [SerializeField]
    private Button _moreTimeButton;

    [SerializeField]
    private Button _tryAgainButton;

    [SerializeField]
    private TMP_Text _nextLevelText;

    [SerializeField]
    private GameObject _addHintVFX;

    [SerializeField]
    private GameObject _noCompletionVFX;

    [Range(0, 5)]
    public float AppearanceDelay = 3;

    private void OnValidate()
    {
        this.Fill(ref _canvasGroup);
        this.FillFromChildren(ref _continueButton);
    }

    public void Init()
    {
        var controller = GameController.Instance;
        controller.OnLevelEnded.AddListener(DelayedAppear);
        controller.OnLoadingNextLevel.AddListener(Disappear);
        _noCompletionVFX.TrySetActive(false);

        for (int i = 0; i < _completionLevels.Length; ++i)
        {
            var level = _completionLevels[i];

            if (level.Star != null)
            {
                _completionLevels[i].StarPosition = level.Star.transform.localPosition;
            }
        }
    }

    public void Appear(float completionRate)
    {
        DelayedAppear(completionRate, 0, 0);
    }

    public void DelayedAppear(float completionRate, int levelIndex, int progress)
    {
        _moreTimeButton.gameObject.SetActive(completionRate < 1);
        _tryAgainButton.gameObject.SetActive(completionRate < 1);
        _nextLevelText.gameObject.SetActive(completionRate >= 1);
        _continueButton.interactable = completionRate >= 1;
        _addHintVFX.TrySetActive(false);
        _noCompletionVFX.TrySetActive(completionRate <= 0);

        if (completionRate < 1)
        {
            Appear(completionRate, levelIndex, progress);
            return;
        }

        DOVirtual.DelayedCall(AppearanceDelay, () => Appear(completionRate, levelIndex, progress), ignoreTimeScale: true)
            .SetRecyclable(true)
            .target = this;
    }

    public void Appear(float completionRate, int levelIndex, int progress)
    {
        if (_addHintVFX != null
            && completionRate >= 1
            && FirebaseManager.AdsExtraHintCount > 0
            && levelIndex >= progress)
        {
            _addHintVFX.SetActive(true);
            _addHintVFX.GetComponentInChildren<TMP_Text>().text = $"+{FirebaseManager.AdsExtraHintCount}";
        }

        completionRate = Mathf.Clamp01(completionRate);
        var completeLevel = Mathf.RoundToInt(completionRate * _completionLevels.Length);
        print("Complete level: " + completeLevel);

        if (_completeMessage != null)
        {
            _completeMessage.text = _completionLevels[Mathf.Max(0, completeLevel - 1)].Message;
        }

        var sequence = DOTween.Sequence();
        sequence.SetRecyclable(true);
        sequence.target = this;

        for (int i = 0; i < _completionLevels.Length; i++)
        {
            if (_completionLevels[i].Star == null)
            {
                continue;
            }

            var star = _completionLevels[i].Star.transform;

            if (i < completeLevel)
            {
                star.localScale = Vector3.zero;
                star.transform.localPosition = _completionLevels[i].StarPosition - new Vector3(0, 200, 0);
                sequence.Insert(0.25f + i * 0.5f, star.DOScale(1, 0.75f).SetEase(Ease.OutBack));
                sequence.Insert(0.15f + i * 0.5f, star.DOLocalMoveY(star.localPosition.y + 200, 0.75f));
                star.gameObject.SetActive(true);
                continue;
            }

            star.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
    }

    private void Disappear()
    {
        _continueButton.interactable = false;
        _canvasGroup.DOFade(0, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }

    protected void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
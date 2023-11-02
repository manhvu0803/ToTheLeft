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

    [Range(0, 5)]
    public float AppearanceDelay = 3;

    private void OnValidate()
    {
        this.Fill(ref _canvasGroup);
        this.FillFromChildren(ref _continueButton);

        for (int i = 0; i < _completionLevels.Length; ++i)
        {
            var level = _completionLevels[i];

            if (level.Star != null)
            {
                _completionLevels[i].StarPosition = level.Star.transform.localPosition;
            }
        }
    }

    public void Init()
    {
        var controller = GameController.Instance;
        controller.OnLevelEnded.AddListener(DelayedAppear);
        controller.OnLoadingNextLevel.AddListener(Disappear);
    }

    public void DelayedAppear(float completionRate)
    {
        _moreTimeButton.gameObject.SetActive(completionRate < 1);
        _tryAgainButton.gameObject.SetActive(completionRate < 1);
        _nextLevelText.gameObject.SetActive(completionRate >= 1);
        _continueButton.interactable = completionRate >= 1;

        if (completionRate < 1)
        {
            Appear(completionRate);
            return;
        }

        DOVirtual.DelayedCall(AppearanceDelay, () => Appear(completionRate), ignoreTimeScale: true)
            .SetRecyclable(true)
            .target = this;
    }

    public void Appear(float completionRate)
    {
        if (_addHintVFX != null && GameController.Instance.Progress <= GameController.Instance.LevelIndex)
        {
            _addHintVFX.SetActive(true);
        }

        completionRate = Mathf.Clamp01(completionRate);
        var completeLevel = Mathf.RoundToInt(completionRate * _completionLevels.Length);
        print(completeLevel);

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
                star.transform.localPosition = _completionLevels[i].StarPosition - new Vector3(0, 400, 0);
                sequence.Insert(0.25f + i * 0.5f, star.DOScale(1, 0.75f).SetEase(Ease.OutBack));
                sequence.Insert(0.15f + i * 0.5f, star.DOMoveY(star.position.y + 400, 0.75f));
                star.gameObject.SetActive(true);
                continue;
            }

            star.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _continueButton.interactable = true;
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
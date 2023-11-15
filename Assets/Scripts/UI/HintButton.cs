using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintButton : Button
{
    private int _hintAmount = 3;

    [SerializeField]
    private TMP_Text _amountText;

    [SerializeField]
    private GameObject _addHintVFX;

    private bool _hintAddedLastLevel;

    private float _maxCompletionRate = 0;

    private float _lastHintFlashTime = 0;

    public int HintAmount
    {
        get => _hintAmount;
        private set
        {
            _hintAmount = value;
            PlayerPrefs.SetInt("hintAmount", _hintAmount);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.FillFromChildren(ref _amountText);
    }
#endif

    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        UpdateHintAmount(PlayerPrefs.GetInt("hintAmount", 3));
        onClick.AddListener(ShowHint);

        if (GameController.Instance != null)
        {
            GameController.Instance.OnLevelEnded.AddListener(OnLevelEnded);
            GameController.Instance.OnLoadingLevelComplete.AddListener(OnLoadingComplete);
        }
        else
        {
            SingletonManager.LevelController.OnCompletionRateChanged.AddListener(OnCompletionRateChange);
        }
    }

    private void OnCompletionRateChange(float completionRate)
    {
        if (completionRate > _maxCompletionRate)
        {
            _maxCompletionRate = completionRate;
            _lastHintFlashTime = Time.realtimeSinceStartup;
        }
    }

    private void OnLevelEnded(float completionRate, int levelIndex, int progress)
    {
        SingletonManager.LevelController.OnCompletionRateChanged.RemoveListener(OnCompletionRateChange);
        _addHintVFX.SetActive(false);

        if (completionRate >= 1 && levelIndex >= progress)
        {
            _hintAddedLastLevel = true;
            HintAmount += FirebaseManager.NewLevelConmpletedBonusHintCount;
        }
    }

    private void OnLoadingComplete()
    {
        if (!_hintAddedLastLevel)
        {
            return;
        }

        var hintAmountText = _addHintVFX.GetComponentInChildren<TMP_Text>();

        if (hintAmountText != null)
        {
            hintAmountText.text = $"+{FirebaseManager.AdsExtraHintCount}";
        }

        _hintAddedLastLevel = false;
        _addHintVFX.SetActive(true);
        _addHintVFX.transform.localPosition = -transform.position;
        SingletonManager.LevelController.OnCompletionRateChanged.AddListener(OnCompletionRateChange);
        _maxCompletionRate = 0;
        _lastHintFlashTime = Time.realtimeSinceStartup;

        DOTween.Sequence()
            .Append(_addHintVFX.transform.DOLocalMove(Vector3.zero, 0.75f))
            .AppendCallback(() => _addHintVFX.SetActive(false))
            .AppendCallback(() => UpdateHintAmount(HintAmount))
            .Append(transform.DOScale(1.25f, 0.25f).SetLoops(2, LoopType.Yoyo))
            .SetRecyclable(true)
            .target = this;
    }

    private void UpdateHintAmount(int amount)
    {
        HintAmount = Mathf.Max(0, amount);

        if (_amountText != null)
        {
            _amountText.text = HintAmount.ToString();
        }
    }

    private void ShowHint()
    {
        if (HintAmount > 0)
        {
            UpdateHintAmount(HintAmount - 1);
            SingletonManager.LevelController.Hint();
            return;
        }

        AdsManager.ShowRewardedAd(() => UpdateHintAmount(HintAmount + FirebaseManager.AdsExtraHintCount));
    }

    protected void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        if (Time.realtimeSinceStartup - _lastHintFlashTime > FirebaseManager.HintButtonFlashDelay)
        {
            transform.DoScaleUpDown();
            _lastHintFlashTime = Time.realtimeSinceStartup;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveAllListeners();
        DOTween.Kill(transform);
    }
}

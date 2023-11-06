using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintButton : Button
{
    private int _hintAmount = 3;

    [SerializeField]
    private TMP_Text _amountText;

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
            GameController.Instance.OnNewLevelComplete.AddListener(level => UpdateHintAmount(HintAmount + 1));
        }
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveAllListeners();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintButton : Button
{
    [SerializeField]
    private TMP_Text _amountText;

    protected override void Start()
    {
        base.Start();
        _amountText.text = GameController.Instance.HintAmount.ToString();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        if (GameController.Instance != null)
        {
            onClick.AddListener(ShowHint);
        }
        else
        {
            onClick.AddListener(SingletonManager.Get<LevelController>().Hint);
        }
    }

    private void ShowHint()
    {
        GameController.Instance.ShowHint();
        _amountText.text = GameController.Instance.HintAmount.ToString();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveAllListeners();
    }
}

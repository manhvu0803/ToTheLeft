using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EndLevelScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Button _continueButton;

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
        controller.OnLevelComplete.AddListener(DelayedAppear);
        controller.OnLoadingNextLevel.AddListener(Disappear);
    }

    private void DelayedAppear()
    {
        DOVirtual.DelayedCall(AppearanceDelay, Appear);
    }

    private void Appear()
    {
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
}
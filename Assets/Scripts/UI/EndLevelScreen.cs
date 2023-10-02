using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class EndLevelScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Button _continueButton;

    private void OnValidate()
    {
        this.Fill(ref _canvasGroup);
        this.FillFromChildren(ref _continueButton);
    }

    private IEnumerator Start()
    {
        GameController.Instance.OnLevelComplete.AddListener(DelayedAppear);
        GameController.Instance.OnLoadingNextLevel.AddListener(Disappear);

        // DOTween need this to be able to smoothly rewinded the 1st time
        yield return null;
        yield return null;

        gameObject.SetActive(false);
    }

    private void DelayedAppear()
    {
        DOVirtual.DelayedCall(3, Appear);
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
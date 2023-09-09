using UnityEngine;
using DG.Tweening;
using System.Collections;

public class EndLevelScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private void OnValidate()
    {
        this.Fill(ref _canvasGroup);
    }

    private IEnumerator Start()
    {
        GameController.Instance.OnLevelComplete.AddListener(Appear);
        GameController.Instance.OnLoadingNextLevel.AddListener(Disappear);

        // DOTween need this to be able to smoothly rewinded the 1st time
        yield return null;
        yield return null;

        gameObject.SetActive(false);
    }

    private void Appear()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.gameObject.SetActive(true);
    }

    private void Disappear()
    {
        _canvasGroup.DOFade(0, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
using UnityEngine;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private void OnValidate()
    {
        this.Fill(ref _canvasGroup);
    }

    private void Start()
    {
        GameController.Instance.OnLoadingNextLevel.AddListener(Appear);
        GameController.Instance.OnLoadingLevelComplete.AddListener(Disappear);
        gameObject.SetActive(false);
    }

    private void Appear()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.5f);
    }

    private void Disappear()
    {
        _canvasGroup.DOFade(0, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
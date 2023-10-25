using DG.Tweening;
using UnityEngine;

public abstract class Slot : MonoBehaviour
{
    public bool HideRenderer = true;

    [field: SerializeField]
    public SpriteRenderer Renderer { get; private set; }

    private float _originalAlpha;

    private void OnValidate()
    {
        if (Renderer == null)
        {
            Renderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected virtual void Start()
    {
        if (!HideRenderer)
        {
            return;
        }

        Renderer.enabled = false;
        _originalAlpha = Renderer.color.a;
        Renderer.SetAlpha(0);
    }

    public void Show(float tweenDuration = 0.2f)
    {
        if (!HideRenderer)
        {
            return;
        }

        DOTween.Kill(Renderer);
        Renderer.enabled = true;
        Renderer.DOFade(_originalAlpha, tweenDuration);
    }

    public void Hide(float tweenDuration = 0.2f)
    {
        if (!HideRenderer)
        {
            return;
        }

        DOTween.Kill(Renderer);
        Renderer.DOFade(0, tweenDuration)
            .OnComplete(() => Renderer.enabled = false);
    }

    public void Hint()
    {
        print("Hint");
        Show();
        DOTween.Kill(transform, complete: true);
        transform.DoScaleUpDown()
            .OnComplete(() => Hide());
    }

    private void OnDestroy()
    {
        DOTween.Kill(Renderer);
        DOTween.Kill(transform);
    }

    public abstract bool IsTarget(Transform transform);
}

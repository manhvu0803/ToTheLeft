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

        var color = Renderer.color;
        color.a = 0;
        Renderer.color = color;
    }

    public void Show()
    {
        if (HideRenderer)
        {
            Renderer.enabled = true;
            Renderer.DOFade(_originalAlpha, 0.2f);
        }
    }

    public void Hide()
    {
        if (HideRenderer)
        {
            Renderer.DOFade(0, 0.2f)
                .OnComplete(() => Renderer.enabled = false);
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(Renderer);
    }

    public abstract bool IsTarget(Transform transform);
}

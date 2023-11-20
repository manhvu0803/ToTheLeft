using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PushOutMovable : Movable
{
    [field: Header("Push-out movable")]
    [field: SerializeField]
    public SpriteRenderer Renderer { get; private set; }

    public Sprite CustomBoundSprite;

    public Bounds Bounds => Renderer.bounds;

    private SpriteRenderer _extraRenderer;

    private Vector3 _originalScale;

    protected void OnValidate()
    {
        if (Renderer == null)
        {
            Renderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected override void Start()
    {
        base.Start();
        _extraRenderer = Instantiate(Renderer, transform);
        _extraRenderer.transform.Translate(0, 0, -0.1f);
        _originalScale = _extraRenderer.transform.localScale;

        if (CustomBoundSprite != null)
        {
            Renderer.sprite = CustomBoundSprite;
        }

        // We don't need to show this renderer, just need it to calculate bounds
        Renderer.enabled = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        base.OnPointerDown(eventData);
        DOTween.Kill(transform, complete: false);
        DOTween.Kill(_extraRenderer.transform, complete: true);
        _extraRenderer.transform.DOScale(_originalScale + OffsetScaleOnDrag, 0.15f);
    }

    protected override void OnDoneInteract()
    {
        DOTween.Kill(transform, complete: true);
        _extraRenderer.transform.DOScale(_originalScale, 0.15f);
        base.OnDoneInteract();
    }
}

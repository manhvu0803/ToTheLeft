using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteChangable : Interactable
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public Sprite InteractSprite;

    private Sprite _originalSprite;

    protected void OnValidate()
    {
        this.FillFromChildren(ref _spriteRenderer);
    }

    protected void Start()
    {
        _originalSprite = _spriteRenderer.sprite;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        base.OnPointerDown(eventData);
        _spriteRenderer.sprite = InteractSprite;
    }

    protected override void OnDoneInteract()
    {
        base.OnDoneInteract();
        _spriteRenderer.sprite = _originalSprite;
    }
}

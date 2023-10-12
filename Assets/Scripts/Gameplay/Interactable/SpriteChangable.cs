using UnityEngine;

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

    protected override void OnMouseDown()
    {
        if (!enabled)
        {
            return;
        }

        base.OnMouseDown();
        _spriteRenderer.sprite = InteractSprite;
    }

    protected override void OnDoneInteract()
    {
        base.OnDoneInteract();
        _spriteRenderer.sprite = _originalSprite;
    }
}

using UnityEngine;

public class Cleanable : IndirectInteractable
{
    [Range(0, 1)]
    public float InitialCleanLevel = 0;

    [Range(0, 1), Tooltip("The amount of clean level increase for every interaction")]
    public float InteractLevel = 0.01f;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private float _cleanLevel;

    protected virtual void OnValidate()
    {
        this.FillFromChildren(ref _spriteRenderer);
    }

    protected virtual void Start()
    {
        _cleanLevel = InitialCleanLevel;
        UpdateRenderer();
    }

    public override void Interact()
    {
        _cleanLevel = Mathf.Min(1, _cleanLevel + InteractLevel);
        SingletonManager.Get<LevelController>().CheckCompletionRate();
        SingletonManager.Get<SoundManager>()?.PlayOnInteract();
        UpdateRenderer();
    }

    private void UpdateRenderer()
    {
        _spriteRenderer.color = new Color(_cleanLevel, _cleanLevel, _cleanLevel);
    }

    public override float CompletionRate => _cleanLevel;
}

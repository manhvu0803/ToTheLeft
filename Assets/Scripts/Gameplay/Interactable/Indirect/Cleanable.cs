using UnityEngine;

public class Cleanable : IndirectInteractable
{
    [Range(0, 1)]
    public float InitialCleanLevel = 0;

    [Range(0, 1), Tooltip("The amount of clean level increase for every interaction")]
    public float InteractLevel = 0.01f;

    [SerializeField]
    protected SpriteRenderer SpriteRenderer;

    protected float CleanLevel;

    protected virtual void OnValidate()
    {
        this.FillFromChildren(ref SpriteRenderer);
    }

    protected virtual void Start()
    {
        CleanLevel = InitialCleanLevel;
        UpdateRenderer();
    }

    public override void Interact()
    {
        CleanLevel = Mathf.Min(1, CleanLevel + InteractLevel);
        SingletonManager.Get<LevelController>().CheckCompletionRate();
        SingletonManager.Get<SoundManager>()?.PlayOnInteract();
        UpdateRenderer();
    }

    protected virtual void UpdateRenderer()
    {
        SpriteRenderer.color = new Color(CleanLevel, CleanLevel, CleanLevel);
    }

    public override float CompletionRate => CleanLevel;
}

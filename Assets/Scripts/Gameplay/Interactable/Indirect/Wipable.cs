public class Wipable : Cleanable
{
    protected override void UpdateRenderer()
    {
        var color = SpriteRenderer.color;
        color.a = 1 - CleanLevel;
        SpriteRenderer.color = color;
    }
}

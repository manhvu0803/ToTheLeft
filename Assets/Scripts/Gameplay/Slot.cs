using DG.Tweening;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [field: SerializeField]
    public SpriteRenderer Renderer { get; private set; }

    [field: SerializeField]
    public Transform Target { get; private set; }

    [SerializeField]
    private Transform[] _targets;

    private float _originalAlpha;

    private void OnValidate()
    {
        if (Renderer == null)
        {
            Renderer = GetComponentInChildren<SpriteRenderer>();
        }

        if ((_targets == null || _targets.Length <= 0) && Target != null)
        {
            _targets = new Transform[] { Target };
        }
    }

    private void Start()
    {
        Renderer.enabled = false;
        _originalAlpha = Renderer.color.a;

        var color = Renderer.color;
        color.a = 0;
        Renderer.color = color;
    }

    public void Show()
    {
        Renderer.enabled = true;
        Renderer.DOFade(_originalAlpha, 0.2f);
    }

    public void Hide()
    {
        Renderer.DOFade(0, 0.2f)
            .OnComplete(() => Renderer.enabled = false);
    }

    public bool IsTarget(Transform transform)
    {
        if (Target == transform)
        {
            return true;
        }

        foreach (var target in _targets)
        {
            if (target == transform)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        DOTween.Kill(Renderer);
    }
}

using DG.Tweening;
using UnityEngine;

/// <summary>
/// An Interactable that mimic rotatation on sprites by using 2 masked sprites and move them horizontally
/// </summary>
public class FakeRotatable : Interactable
{
    [Min(0)]
    public float SnapDistance = 0.4f;

    public float InitialOffset = 1;

    [SerializeField]
    private SpriteRenderer _spritePrefab;

    private SpriteRenderer _mainSprite;

    private SpriteRenderer _extraSprite;

    public float Offset => _mainSprite.transform.localPosition.x;

    private void SetExtraSpritePosition(int side)
    {
        _extraSprite.transform.position = _mainSprite.transform.position + side * new Vector3(_mainSprite.bounds.size.x, 0);
    }

    private void Start()
    {
        _mainSprite = Instantiate(_spritePrefab, transform);
        _extraSprite = Instantiate(_spritePrefab, transform);

        var position = _mainSprite.transform.localPosition;
        position.x = InitialOffset;
        _mainSprite.transform.localPosition = position;

        SetExtraSpritePosition(1);
    }

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePosition)
    {
        _mainSprite.transform.Translate(delta.x, 0, 0);

        if (_mainSprite.transform.localPosition.x < 0)
        {
            SetExtraSpritePosition(1);
        }
        else
        {
            SetExtraSpritePosition(-1);
        }

        if (Mathf.Abs(_extraSprite.transform.localPosition.x) < Mathf.Abs(_mainSprite.transform.localPosition.x))
        {
            (_mainSprite, _extraSprite) = (_extraSprite, _mainSprite);
        }
    }

    protected override void OnDoneInteract()
    {
        if (Mathf.Abs(_mainSprite.transform.localPosition.x) > SnapDistance)
        {
            base.OnDoneInteract();
            return;
        }

        _mainSprite.transform.DOLocalMoveX(0, 0.15f)
            .OnUpdate(() => SetExtraSpritePosition(1))
            .OnComplete(base.OnDoneInteract);
    }

    private void OnDestroy()
    {
        DOTween.Kill(_mainSprite.transform);
    }
}

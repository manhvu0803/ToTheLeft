using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This could be use to toggle show/hide anything, not just dropdowns
/// </summary>
public class DropdownButton : Selectable, IPointerClickHandler
{
    [Header("Dropdown")]
    [SerializeField]
    private RectTransform _content;

    public bool ScaleOnChange = true;

    public bool FadeOnChange = true;

    [Range(0, 1), Tooltip("Duration of fade in/fade out animation in seconds")]
    public float AnimationDuration = 0.15f;

    private CanvasGroup _contentGroup;

    private bool _isDropping = false;

    private Vector3 _originalScale;

    public bool IsDropping
    {
        get => _isDropping;
        set
        {
            if (_isDropping != value)
            {
                DoAnimation(value);
            }

            _isDropping = value;
        }
    }

    private void DoAnimation(bool isDropping)
    {
        DOTween.Kill(this);
        DOTween.Kill(transform);

        if (isDropping)
        {
            _content.gameObject.SetActive(true);
        }

        var sequence = DOTween.Sequence();
        sequence.target = this;
        sequence.SetRecyclable(true);

        if (FadeOnChange)
        {
            sequence.Append(_contentGroup.DOFade(isDropping ? 1 : 0, AnimationDuration));
        }

        if (ScaleOnChange)
        {
            sequence.Insert(0, _content.DOScale(isDropping ? _originalScale : Vector3.zero, AnimationDuration));
        }

        if (!isDropping)
        {
            sequence.AppendCallback(() => _content.gameObject.SetActive(isDropping));
        }
    }

    protected override void Start()
    {
        base.Start();
        _originalScale = transform.localScale;
        IsDropping = false;

        if (FadeOnChange && !_content.TryGetComponent(out _contentGroup))
        {
            Debug.LogWarning("DropdownButton target doesn't have CanvasGroup while FadeOnChange is true");
            _contentGroup = _content.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsDropping = !IsDropping;
    }
}

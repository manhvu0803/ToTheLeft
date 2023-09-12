using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Level1Tutorial : MonoBehaviour
{
    [SerializeField]
    private Image _pointer;

    [SerializeField]
    private Movable _movable;

    [SerializeField]
    private TapRotatable _rotatable;

    private bool _isStep2 = false;

    private void Start()
    {
        if (GameController.Instance.Progress >= 1)
        {
            Destroy(_pointer.gameObject);
            Destroy(this);
            return;
        }

        _movable.OnPointerDrag.AddListener(Step1);
        _pointer.transform.position = Camera.main.WorldToScreenPoint(_movable.transform.position);
        DOTween.Sequence()
            .AppendInterval(0.25f)
            .Append(_pointer.transform.DOScale(new Vector3(0.65f, 0.65f, 0.65f), 0.5f))
            .Append(_pointer.transform.DOMove(_pointer.transform.position + new Vector3(200, -200, 0), 1))
            .SetLoops(-1)
            .target = _pointer.transform;

        _rotatable.OnPointerUp.AddListener(Disable);
    }

    private void Update()
    {
        if (_rotatable.enabled && !_isStep2)
        {
            _isStep2 = true;
            _pointer.gameObject.SetActive(true);
            _pointer.transform.position = Camera.main.WorldToScreenPoint(_movable.transform.position);
            DOTween.Sequence()
                .Append(_pointer.transform.DOScale(new Vector3(0.65f, 0.65f, 0.65f), 0.5f))
                .Append(_pointer.transform.DOShakeRotation(0.25f))
                .Append(_pointer.transform.DOScale(Vector3.one, 0.5f))
                .SetLoops(-1)
                .target = _pointer.transform;
        }
    }

    private void Step1()
    {
        DOTween.Kill(_pointer.transform);
        _movable.OnPointerDrag.RemoveListener(Step1);
        _pointer.gameObject.SetActive(false);
    }

    private void Disable()
    {
        _rotatable.OnPointerUp.RemoveListener(Disable);
        DOTween.Kill(_pointer.transform);
        _pointer.gameObject.SetActive(false);
        enabled = false;
    }
}

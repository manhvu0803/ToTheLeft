using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Level2Tutorial : MonoBehaviour
{
    [SerializeField]
    private Image _pointer;

    [SerializeField]
    private Movable _movable;

    private void Start()
    {
        if (GameController.Instance.Progress >= 2)
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
            .Append(_pointer.transform.DOMove(_pointer.transform.position + new Vector3(0, -200, 0), 1))
            .SetLoops(-1)
            .target = _pointer.transform;
    }

    private void Step1()
    {
        DOTween.Kill(_pointer.transform);
        _movable.OnPointerDrag.RemoveListener(Step1);
        _pointer.gameObject.SetActive(false);
        enabled = false;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class Slidable : Interactable
{
    public Vector2 MaxOffset = new(2, 2);

    [Range(0, 1)]
    public float LerpRate = 0.5f;

    [Min(0)]
    public float SnapDistance = 0.1f;

    private Vector3 _targetPosition;

    protected override void OnInteract(Vector3 delta, PointerEventData eventData)
    {
        _targetPosition = transform.position + new Vector3(delta.x, delta.y, 0);
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -MaxOffset.x, MaxOffset.x);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, -MaxOffset.y, MaxOffset.y);
    }

    protected void Start()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, LerpRate);

        if (transform.localPosition.sqrMagnitude <= SnapDistance * SnapDistance)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}

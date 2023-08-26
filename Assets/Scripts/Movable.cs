using UnityEngine;

public class Movable : Interactable
{
    public Vector2 MaxOffset = new(2, 2);

    [Range(0, 1)]
    public float LerpRate = 0.5f;

    [Min(0)]
    public float SnapDistance = 0.1f;

    private Vector3 _targetPosition;

    protected override void OnInteract(Vector2 delta)
    {
        _targetPosition = transform.position + (Vector3)delta;
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -MaxOffset.x, MaxOffset.x);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, -MaxOffset.y, MaxOffset.y);
    }

    private void Start()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, LerpRate);

        if ((transform.localPosition - Vector3.zero).sqrMagnitude <= SnapDistance * SnapDistance)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}

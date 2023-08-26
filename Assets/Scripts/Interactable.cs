using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float SpeedMultiplier = 50;

    private Vector3 _lastMousePosition;

    protected virtual void OnMouseDown()
    {
        _lastMousePosition = Input.mousePosition;
    }

    protected virtual void OnMouseDrag()
    {
        var currentMousePosition = Input.mousePosition;
        var delta = currentMousePosition - _lastMousePosition;
        OnInteract(SpeedMultiplier * new Vector2(delta.x / Screen.width, delta.y / Screen.height));
        OnRawInteract(SpeedMultiplier * (Vector2)delta);
        _lastMousePosition = currentMousePosition;
    }

    /// <summary>
    /// Normalized delta
    /// </summary>
    /// <param name="delta"></param>
    protected virtual void OnInteract(Vector2 delta) {}

    protected virtual void OnRawInteract(Vector2 delta) {}
}

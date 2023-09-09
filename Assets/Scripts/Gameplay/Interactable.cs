using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    static protected Camera MainCamera;

    public float SpeedMultiplier = 1;

    protected Vector3 LastMousePosition { get; private set; }

    protected virtual void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }
    }

    protected virtual void OnMouseDown()
    {
        if (!enabled)
        {
            return;
        }

        LastMousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    protected virtual void OnMouseDrag()
    {
        if (!enabled)
        {
            return;
        }

        var currentMousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        var delta = currentMousePosition - LastMousePosition;
        OnInteract(SpeedMultiplier * delta, currentMousePosition);
        LastMousePosition = currentMousePosition;
    }

    protected abstract void OnInteract(Vector3 delta, Vector3 currentMousePosition);
}

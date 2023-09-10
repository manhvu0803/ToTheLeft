using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    #region Static
    static private Camera _mainCamera;

    private static LevelController _controller;

    static protected Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            return _mainCamera;
        }
    }

    protected static LevelController Controller
    {
        get
        {
            if (_controller == null)
            {
                _controller = SingletonManager.Get<LevelController>();
            }

            return _controller;
        }
    }
    #endregion

    protected Vector3 LastMousePosition { get; private set; }

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

        if (delta.sqrMagnitude >= 0.000001f)
        {
            OnInteract(delta, currentMousePosition);
        }

        LastMousePosition = currentMousePosition;
    }

    protected virtual void OnMouseUp()
    {
        Controller.CheckCompletionRate();
    }

    protected virtual void OnInteract(Vector3 delta, Vector3 currentMousePosition) { }
}

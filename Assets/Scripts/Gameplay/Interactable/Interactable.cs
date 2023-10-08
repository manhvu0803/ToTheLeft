using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    #region Static
    static private Camera _mainCamera;

    private static LevelController _controller;

    private static SoundManager _soundManager;

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

    protected static LevelController LevelController
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

    protected static SoundManager SoundManager
    {
        get
        {
            if (_soundManager == null)
            {
                _soundManager = SingletonManager.Get<SoundManager>();
            }

            return _soundManager;
        }
    }
    #endregion

    public UnityEvent OnPointerDown;

    public UnityEvent OnPointerDrag;

    public UnityEvent OnPointerUp;

    protected Vector3 LastMousePosition { get; private set; }

    protected virtual void OnMouseDown()
    {
        if (!enabled)
        {
            return;
        }

        SoundManager?.PlayOnInteract();
        LastMousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        OnPointerDown?.Invoke();
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
        OnPointerDrag?.Invoke();
    }

    protected void OnMouseUp()
    {
        if (!enabled)
        {
            return;
        }

        OnDoneInteract();
        OnPointerUp?.Invoke();
    }

    protected virtual void OnDoneInteract()
    {
        SoundManager?.PlayDoneInteract();
        LevelController.CheckCompletionRate();
    }

    protected virtual void OnInteract(Vector3 delta, Vector3 currentMousePosition) { }
}

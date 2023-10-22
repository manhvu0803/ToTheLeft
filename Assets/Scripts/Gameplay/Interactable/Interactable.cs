using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class Interactable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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

    public UnityEvent OnDown;

    public UnityEvent OnObjectDragged;

    public UnityEvent OnUp;

    protected Vector3 LastMousePosition { get; private set; }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        SoundManager?.PlayOnInteract();
        LastMousePosition = eventData.pointerCurrentRaycast.worldPosition;
        OnDown?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        var currentMousePosition = eventData.pointerCurrentRaycast.worldPosition;
        var delta = currentMousePosition - LastMousePosition;

        if (delta.sqrMagnitude >= 0.000001f)
        {
            OnInteract(delta, eventData);
        }

        LastMousePosition = currentMousePosition;
        OnObjectDragged?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        OnDoneInteract();
        OnUp?.Invoke();
    }

    protected virtual void OnDoneInteract()
    {
        SoundManager?.PlayDoneInteract();
        LevelController.CheckCompletionRate();
    }

    protected virtual void OnInteract(Vector3 delta, PointerEventData eventData) { }
}

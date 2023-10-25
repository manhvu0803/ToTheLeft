using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class Interactable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public UnityEvent OnDown;

    public UnityEvent OnObjectDragged;

    public UnityEvent OnUp;

    protected Vector2 LastMousePosition { get; private set; }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        SingletonManager.SoundManager.PlayOnInteract();
        LastMousePosition = eventData.pointerCurrentRaycast.worldPosition;
        OnDown?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        var currentMousePosition = (Vector2)SingletonManager.MainCamera.ScreenToWorldPoint(eventData.position);
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
        SingletonManager.SoundManager.PlayDoneInteract();
        SingletonManager.LevelController.CheckCompletionRate();
    }

    protected virtual void OnInteract(Vector3 delta, PointerEventData eventData) { }
}

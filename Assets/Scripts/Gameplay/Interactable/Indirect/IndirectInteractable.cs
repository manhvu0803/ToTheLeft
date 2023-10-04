using UnityEngine;

public abstract class IndirectInteractable : MonoBehaviour
{
    public abstract void Interact();

    public abstract float CompletionRate { get; }
}

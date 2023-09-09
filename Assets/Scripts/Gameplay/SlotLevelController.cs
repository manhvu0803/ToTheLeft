using UnityEngine;

public abstract class SlotLevelController : LevelController
{
    [Header("Slot")]
    private static SlotLevelController _instance;

    public static SlotLevelController Instance => _instance;

    public LayerMask SlotLayers;

    [SerializeField]
    protected Slot[] Slots;

    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Try to put the target transform into slot and return the result
    /// </summary>
    /// <returns>true if the slot is empty, false otherwise</returns>
    public abstract bool UpdateSlot(Transform target, Transform slot, bool isInteracting = false);
}

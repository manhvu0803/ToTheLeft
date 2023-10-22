using UnityEngine;

public class IndirectInteractLevelController : LevelController
{
    [SerializeField]
    private IndirectInteractable[] _interactables;

    protected virtual void OnValidate()
    {
        Utils.Fill(ref _interactables);
    }

    public override float CompletionRate()
    {
        float totalRate = 0;

        foreach (var interactable in _interactables)
        {
            totalRate += interactable.CompletionRate;
        }

        return totalRate / _interactables.Length;
    }

    public override void Hint()
    {
        var directInteractable = FindObjectOfType<Interactable>();
        directInteractable.transform.DoScaleUpDown();

        foreach (var interactable in _interactables)
        {
            if (interactable.CompletionRate < 1)
            {
                interactable.transform.DoScaleUpDown();
            }
        }
    }
}

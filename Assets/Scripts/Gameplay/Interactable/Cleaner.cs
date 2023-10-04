using UnityEngine;

public class Cleaner : Movable
{
    private float _lastDirection = 0;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[4];

    private Cleanable _currentCleanable;

    protected override void OnInteract(Vector3 delta, Vector3 currentMousePostion)
    {
        base.OnInteract(delta, currentMousePostion);

        // The current direction is different from last direction OR this is the first interaction
        if (_lastDirection * delta.x < 0 || _lastDirection == 0)
        {
            _currentCleanable = null;
        }

        if (_currentCleanable == null)
        {
            var objectCount = Physics2D.RaycastNonAlloc(currentMousePostion, Vector2.zero, hits);

            // This does not account for multiple Cleanable bjects in the result
            for (int i = 0; i < objectCount; ++i)
            {
                if (hits[i].collider.TryGetComponent(out _currentCleanable))
                {
                    _currentCleanable.Interact();
                    break;
                }
            }
        }

        if (delta.x != 0)
        {
            _lastDirection = delta.x;
        }
    }
}

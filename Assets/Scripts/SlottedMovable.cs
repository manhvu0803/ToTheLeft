using UnityEngine;

public class SlottedMovable : Interactable
{
    private static Camera MainCamera;

    public LayerMask SlotLayers;

    private void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        transform.Translate(0, 0, -0.5f);
    }

    protected override void OnRawInteract(Vector2 delta)
    {
        transform.position += (Vector3)delta;
    }

    private void OnMouseUp()
    {
        transform.Translate(0, 0, 0.5f);
        var hit = Physics2D.Raycast(MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, SlotLayers);

        if (hit.collider != null)
        {
            transform.SetPositionAndRotation(hit.collider.transform.position, hit.collider.transform.rotation);
            return;
        }

        var random = Random.Range(0, 2);
        transform.localEulerAngles = new Vector3(0, 0, (random == 0) ? -20 : 20);
    }
}

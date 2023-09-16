using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidthCamera : MonoBehaviour {

    [Tooltip("Set this to the in-world distance between the left & right edges of your scene")]
    public float SceneWidth = 10;

    [SerializeField, HideInInspector]
    private Camera _camera;

    private void OnValidate()
    {
        this.Fill(ref _camera);
    }

    private void Start()
    {
        float unitsPerPixel = SceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        _camera.orthographicSize = desiredHalfHeight;
    }

#if UNITY_EDITOR
    private void Update()
    {
        Start();
    }
#endif
}
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [SerializeField]
    private RectTransform ProgressButton;

    protected void Start()
    {
        ProgressButton.TrySetActive(FirebaseManager.AllowProgressBar);
    }
}

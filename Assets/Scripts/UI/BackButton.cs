using UnityEngine;
using UnityEngine.UI;

public class BackButton : Button
{
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onClick?.Invoke();
        }
    }
}

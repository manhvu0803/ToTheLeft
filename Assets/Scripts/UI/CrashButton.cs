using System;
using UnityEngine;
using UnityEngine.UI;

public class CrashButton : Button
{
    protected override void Start()
    {
#if TEST_BUILD
        base.Start();
        if (Application.isPlaying)
        {
            onClick.AddListener(Crash);
        }
#else
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
#endif
    }

    private void Crash()
    {
        throw new Exception("Crash button pressed");
    }
}

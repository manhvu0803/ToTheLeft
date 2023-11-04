using System;
using UnityEngine.UI;

public class CrashButton : Button
{
    protected override void Start()
    {
#if TEST_BUILD
        base.Start();
        onClick.AddListener(Crash);
#else
        Destroy(gameObject);
#endif
    }

    private void Crash()
    {
        throw new Exception("Crash button pressed");
    }
}

using UnityEngine.UI;

public class HintButton : Button
{
    protected override void Start()
    {
        base.Start();

        if (GameController.Instance != null)
        {
            onClick.AddListener(GameController.Instance.ShowHint);
        }
        else
        {
            onClick.AddListener(SingletonManager.Get<LevelController>().Hint);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveAllListeners();
    }
}

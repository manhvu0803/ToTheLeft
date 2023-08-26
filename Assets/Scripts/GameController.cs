using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    [SerializeField]
    private string[] _levels;

    [SerializeField]
    private CanvasGroup _loadingGroup;

    [SerializeField]
    private CanvasGroup _endLevelGroup;

    private int _levelIndex = -1;

    public static GameController Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void CompleteLevel()
    {
        _endLevelGroup.alpha = 1;
        _endLevelGroup.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        _loadingGroup.gameObject.SetActive(true);
        _loadingGroup.DOFade(1, 0.5f);
        _endLevelGroup.DOFade(0, 0.5f)
            .OnComplete(() => _endLevelGroup.gameObject.SetActive(false));

        if (_levelIndex + 1 >= _levels.Length)
        {
            return;
        }

        _levelIndex++;

        if (_levelIndex > 0)
        {
            StartCoroutine(UnloadAndLoad(_levels[_levelIndex - 1], _levels[_levelIndex ]));
        }
        else
        {
            StartCoroutine(Load(_levels[_levelIndex ]));
        }
    }

    private IEnumerator UnloadAndLoad(string oldLevelName, string newLevelName)
    {
        print("Begin unload");
        var operation = SceneManager.UnloadSceneAsync(oldLevelName);

        while (!operation.isDone)
        {
            yield return null;
        }

        StartCoroutine(Load(newLevelName));
    }

    private IEnumerator Load(string levelName)
    {
        print("Begin load");
        var operation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        _loadingGroup.DOFade(0, 0.5f)
            .OnComplete(() => _loadingGroup.gameObject.SetActive(false));
    }
}

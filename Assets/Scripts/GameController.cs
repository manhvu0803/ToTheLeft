using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance => _instance;

    [SerializeField]
    private string[] _levels;

    public ReadOnlyCollection<string> Levels => Array.AsReadOnly(_levels);

    public UnityEvent OnLevelComplete;

    public UnityEvent OnLoadingNextLevel;

    public UnityEvent OnLoadingLevelComplete;

    public int Progress { get; private set; }

    private int _levelIndex = -1;

    private void Awake()
    {
        _instance = this;
        Progress = PlayerPrefs.GetInt("progress", 0);
    }

    public void LoadLevel(int level)
    {
        if (level < 0 || level >= _levels.Length)
        {
            Debug.LogError($"Level {level} doesn't exist");
            return;
        }

        StartCoroutine(UnloadAndLoad(level));
    }

    public void CompleteLevel()
    {
        OnLevelComplete?.Invoke();
        Progress = Mathf.Max(_levelIndex + 1, Progress);
        PlayerPrefs.SetInt("progress", Progress);
    }

    public void NextLevel()
    {
        if (_levelIndex + 1 >= _levels.Length)
        {
            return;
        }

        if (_levelIndex < 0)
        {
            StartCoroutine(UnloadAndLoad(Mathf.Min(Progress, _levels.Length - 1)));
            return;
        }

        StartCoroutine(UnloadAndLoad(_levelIndex + 1));
    }

    private IEnumerator UnloadAndLoad(int level)
    {
        OnLoadingNextLevel?.Invoke();

        if (_levelIndex >= 0 && _levelIndex < _levels.Length)
        {
            var operation = SceneManager.UnloadSceneAsync(_levels[_levelIndex]);

            while (!operation.isDone)
            {
                yield return null;
            }
        }

        _levelIndex = level;
        StartCoroutine(Load(_levels[level]));
    }

    private IEnumerator Load(string levelName)
    {
        var operation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        OnLoadingLevelComplete?.Invoke();
    }
}

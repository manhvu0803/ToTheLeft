using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance => _instance;

    public GameObject FirstScreen;

    [SerializeField]
    private List<string> _levels;

    public ReadOnlyCollection<string> Levels => _levels.AsReadOnly();

    public UnityEvent OnLevelComplete;

    public UnityEvent OnLoadingNextLevel;

    public UnityEvent OnLoadingLevelComplete;

    public int Progress { get; private set; }

    private int _levelIndex = -1;

    private bool _isCurrentLevelComplete = false;

    private LevelController _currentLevelController;

    [field: SerializeField]
    public ProgressBar ProgressBar { get; private set; }

    [field: SerializeField]
    private EndLevelScreen _endLevelScreen;

    public LevelController CurrentLevelController
    {
        get
        {
            if (_currentLevelController == null)
            {
                _currentLevelController = SingletonManager.Get<LevelController>();
            }

            return _currentLevelController;
        }
    }

    private void OnValidate()
    {
        ProgressBar = Utils.FindIfNull(ProgressBar);
        Utils.Find(ref _endLevelScreen);
    }

    private void Awake()
    {
        _instance = this;
        Progress = PlayerPrefs.GetInt("progress", 0);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        _endLevelScreen.Init();
    }

    public void LoadLevel(int level)
    {
        if (level < 0 || level >= _levels.Count)
        {
            Debug.LogError($"Level {level} doesn't exist");
            return;
        }

        StartCoroutine(UnloadAndLoad(level));
    }

    public void CompleteLevel()
    {
        if (_isCurrentLevelComplete)
        {
            return;
        }

        print("Level complete");
        _isCurrentLevelComplete = true;
        OnLevelComplete?.Invoke();
        Progress = Mathf.Max(_levelIndex + 1, Progress);
        PlayerPrefs.SetInt("progress", Progress);
    }

    public void NextLevel()
    {
        if (_levelIndex + 1 >= _levels.Count)
        {
            return;
        }

        if (_levelIndex < 0)
        {
            StartCoroutine(UnloadAndLoad(Mathf.Min(Progress, _levels.Count - 1)));
            return;
        }

        StartCoroutine(UnloadAndLoad(_levelIndex + 1));
    }

    private IEnumerator UnloadAndLoad(int level)
    {
        OnLoadingNextLevel?.Invoke();

        if (_levelIndex >= 0 && _levelIndex < _levels.Count)
        {
            yield return SceneManager.UnloadSceneAsync(_levels[_levelIndex]);
            yield return Resources.UnloadUnusedAssets();
        }

        _levelIndex = level;
        StartCoroutine(Load(_levels[level]));
    }

    private IEnumerator Load(string levelName)
    {
        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        _isCurrentLevelComplete = false;
        OnLoadingLevelComplete?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        if (_levelIndex >= 0 && _levelIndex < _levels.Count)
        {
            SceneManager.UnloadSceneAsync(_levels[_levelIndex]);
        }

        _levelIndex = -1;
        FirstScreen.SetActive(true);
    }

    public void ShowHint()
    {
        CurrentLevelController.Hint();
    }

    public void ShowProgressBar()
    {
        ProgressBar.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh level list")]
    private void RefreshLevelList()
    {
        _levels.Clear();

        foreach(var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled || scene.path.Contains("Main"))
            {
                continue;
            }

            // Remove "Assets/" at start and ".unity" at the end before adding to list of levels
            _levels.Add(scene.path[7..scene.path.IndexOf(".unity")]);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public GameObject FirstScreen;

    [SerializeField]
    private List<string> _levels;

    public ReadOnlyCollection<string> Levels => _levels.AsReadOnly();

    public UnityEvent<float> OnLevelEnded;

    public UnityEvent OnLoadingNextLevel;

    public UnityEvent OnLoadingLevelComplete;

    public UnityEvent OnTimeLimitReached;

    public UnityEvent<int> OnNewLevelComplete;

    public int Progress { get; private set; }

    public int LevelIndex { get; private set; } = -1;

    private bool _isCurrentLevelComplete = true;

    private float _timeLeft;

    [field: SerializeField]
    public ProgressBar ProgressBar { get; private set; }

    [SerializeField]
    private EndLevelScreen _endLevelScreen;

    [SerializeField]
    private Image _timerCircle;

    private float[] _levelTimeLimits;

    private float CurrentTimeLimit
    {
        get
        {
            if (LevelIndex < 0)
            {
                return -1;
            }

            if (_levelTimeLimits != null && LevelIndex < _levelTimeLimits.Length)
            {
                return _levelTimeLimits[LevelIndex];
            }

            return 60;
        }
    }

    private void OnValidate()
    {
        ProgressBar = Utils.FindIfNull(ProgressBar);
        Utils.Find(ref _endLevelScreen);
    }

    private void Awake()
    {
        Instance = this;
        Progress = PlayerPrefs.GetInt("progress", 0);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        _endLevelScreen.Init();
        _timerCircle.fillAmount = 0;
        StartCoroutine(ReadRemoteConfig());
    }

    private IEnumerator ReadRemoteConfig()
    {
        yield return new WaitUntil(() => FirebaseManager.IsRemoteConfigReady);
        var timeLimitConfig = FirebaseManager.RemoteConfig.GetValue("LevelTimeLimit");
        _levelTimeLimits = Utils.FromJson<float>(timeLimitConfig.StringValue);
    }

    private void Update()
    {
        if (CurrentTimeLimit <= 0 || _isCurrentLevelComplete || SingletonManager.LevelController == null)
        {
            return;
        }

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0)
        {
            CompleteLevel(SingletonManager.LevelController.CompletionRate);
        }
        else
        {
            var ratio = _timeLeft / CurrentTimeLimit;
            _timerCircle.fillAmount = ratio;

            if (ratio > 0.5f)
            {
                _timerCircle.color = Color.green;
            }
            else if (ratio > 0.25f)
            {
                _timerCircle.color = Color.yellow;
            }
            else
            {
                _timerCircle.color = Color.red;
            }
        }
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

    public void CompleteLevel(float completionRate)
    {
        if (_isCurrentLevelComplete)
        {
            return;
        }

        print("Level complete");
        _isCurrentLevelComplete = true;

        if (LevelIndex >= Progress)
        {
            Progress = LevelIndex + 1;

            try
            {
                OnNewLevelComplete?.Invoke(Progress);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        PlayerPrefs.SetInt("progress", Progress);
        OnLevelEnded?.Invoke(completionRate);
    }

    public void AddMoreTime()
    {
        _timeLeft = 15;
        _isCurrentLevelComplete = false;
    }

    public void Retry()
    {
        StartCoroutine(UnloadAndLoad(LevelIndex));
    }

    public void NextLevel()
    {
        if (LevelIndex + 1 >= _levels.Count)
        {
            return;
        }

        if (LevelIndex < 0)
        {
            StartCoroutine(UnloadAndLoad(Mathf.Min(Progress, _levels.Count - 1)));
            return;
        }

        StartCoroutine(UnloadAndLoad(LevelIndex + 1));
    }

    private IEnumerator UnloadAndLoad(int level)
    {
        if (LevelIndex >= 0 && LevelIndex < _levels.Count)
        {
            yield return SceneManager.UnloadSceneAsync(_levels[LevelIndex]);
        }

        LevelIndex = level;
        StartCoroutine(Load(_levels[level]));
        OnLoadingNextLevel?.Invoke();
    }

    private IEnumerator Load(string levelName)
    {
        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        _timeLeft = CurrentTimeLimit;
        _isCurrentLevelComplete = false;
        _timerCircle.fillAmount = 1;
        _timerCircle.color = Color.green;

        yield return Resources.UnloadUnusedAssets();

        OnLoadingLevelComplete?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        if (LevelIndex >= 0 && LevelIndex < _levels.Count)
        {
            SceneManager.UnloadSceneAsync(_levels[LevelIndex]);
        }

        LevelIndex = -1;
        FirstScreen.SetActive(true);
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

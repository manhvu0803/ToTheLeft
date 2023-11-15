using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


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

    public UnityEvent OnFirstLoadingComplete;

    public UnityEvent<float, int, int> OnLevelEnded;

    public UnityEvent OnLoadingNextLevel;

    public UnityEvent OnLoadingLevelComplete;

    public UnityEvent OnTimeLimitReached;

    public static int Progress { get; private set; }

    public static int LevelIndex { get; private set; } = -1;

    private bool _isCurrentLevelComplete = true;

    private float _timeLeft;

    [field: SerializeField]
    public ProgressBar ProgressBar { get; private set; }

    [SerializeField]
    private UITimer _timer;

    private float TimeLeft
    {
        get => _timeLeft;
        set
        {
            _timeLeft = value;
            _timer.TimeLeft = value;
        }
    }

    private float CurrentTimeLimit
    {
        get
        {
            if (LevelIndex < 0)
            {
                return -1;
            }

            var timeLimits = FirebaseManager.LevelTimeLimits;

            if (timeLimits != null && LevelIndex < timeLimits.Length)
            {
                return timeLimits[LevelIndex];
            }

            return 60;
        }
    }

    private void OnValidate()
    {
        ProgressBar = Utils.FindIfNull(ProgressBar);
    }

    private void Awake()
    {
        Instance = this;
        Progress = PlayerPrefs.GetInt("progress", 0);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        StartCoroutine(FirstLoading());
    }

    private IEnumerator FirstLoading()
    {
        yield return new WaitUntil(() => FirebaseManager.IsRemoteConfigReady);
        OnFirstLoadingComplete?.Invoke();
    }

    private void Update()
    {
        if (CurrentTimeLimit <= 0 || _isCurrentLevelComplete || SingletonManager.LevelController == null)
        {
            return;
        }

        TimeLeft -= Time.deltaTime;

        if (TimeLeft <= 0)
        {
            CompleteLevel(SingletonManager.LevelController.CompletionRate);
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

        try
        {
            OnLevelEnded?.Invoke(completionRate, LevelIndex, Progress);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        if (LevelIndex >= Progress)
        {
            Progress = LevelIndex + 1;
            PlayerPrefs.SetInt("progress", Progress);
        }
    }

    public void AddMoreTime()
    {
        AdsManager.ShowRewardedAd(() =>
        {
            TimeLeft = FirebaseManager.AdsExtraTime;
            _isCurrentLevelComplete = false;
            SingletonManager.LevelController.enabled = true;
            ProgressBar.Init();

            // TODO: Change FindObject to something else
            FindObjectOfType<Physics2DRaycaster>().enabled = true;
            var rotatables = FindObjectsOfType<TapRotatable>();

            foreach (var rotatable in rotatables)
            {
                rotatable.Init();
            }
        });
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
            yield return SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Additive);
            yield return Resources.UnloadUnusedAssets();
            yield return SceneManager.UnloadSceneAsync("Empty");
        }

        LevelIndex = level;
        StartCoroutine(Load(_levels[level]));
        OnLoadingNextLevel?.Invoke();
    }

    private IEnumerator Load(string levelName)
    {
        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        TimeLeft = CurrentTimeLimit;
        _timer.TimeLimit = TimeLeft;
        _isCurrentLevelComplete = false;
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
            if (!scene.enabled || !scene.path.Contains("Levels"))
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

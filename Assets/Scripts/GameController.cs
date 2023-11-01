using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine.UI;


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

    public int Progress { get; private set; }

    private int _levelIndex = -1;

    private bool _isCurrentLevelComplete = true;

    private float _timeLeft;

    [field: SerializeField]
    public ProgressBar ProgressBar { get; private set; }

    [SerializeField]
    private EndLevelScreen _endLevelScreen;

    [SerializeField]
    private Image _timerCircle;

    private float[] _levelTimeLimits;

    private int _hintAmount = 3;

    public int HintAmount
    {
        get => _hintAmount;
        private set
        {
            _hintAmount = value;
            PlayerPrefs.SetInt("hintAmount", _hintAmount);
        }
    }

    private float CurrentTimeLimit
    {
        get
        {
            if (_levelTimeLimits != null && _levelIndex < _levelTimeLimits.Length)
            {
                return _levelTimeLimits[_levelIndex];
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
        _hintAmount = PlayerPrefs.GetInt("hintAmount", 3);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        _endLevelScreen.Init();
        StartCoroutine(ReadRemoteConfig());
    }

    private IEnumerator ReadRemoteConfig()
    {
        yield return new WaitUntil(() => FirebaseManager.IsRemoteConfigReady);
        var timeLimitConfig = FirebaseManager.RemoteConfig.GetValue("LevelTimeLimit");
        _levelTimeLimits = JsonUtility.FromJson<float[]>(timeLimitConfig.StringValue);
    }

    private void Update()
    {
        if (CurrentTimeLimit < 0 || _isCurrentLevelComplete)
        {
            _timerCircle.gameObject.SetActive(false);
            return;
        }

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0)
        {
            CompleteLevel(SingletonManager.LevelController.CompletionRate);
            OnTimeLimitReached?.Invoke();
        }
        else
        {
            _timerCircle.gameObject.SetActive(true);
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

        if (_levelIndex >= Progress)
        {
            HintAmount++;
            Progress = _levelIndex + 1;
        }

        PlayerPrefs.SetInt("progress", Progress);
        OnLevelEnded?.Invoke(completionRate);
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
        if (_levelIndex >= 0 && _levelIndex < _levels.Count)
        {
            yield return SceneManager.UnloadSceneAsync(_levels[_levelIndex]);
        }

        _levelIndex = level;
        StartCoroutine(Load(_levels[level]));
        OnLoadingNextLevel?.Invoke();
    }

    private IEnumerator Load(string levelName)
    {
        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        yield return Resources.UnloadUnusedAssets();
        _isCurrentLevelComplete = false;

        if (_levelTimeLimits != null && _levelIndex < _levelTimeLimits.Length)
        {
            _timeLeft = _levelTimeLimits[_levelIndex];
        }
        else
        {
            _timeLeft = 60;
        }

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
        if (HintAmount > 0)
        {
            HintAmount--;
            SingletonManager.LevelController.Hint();
            return;
        }

        AdsManager.ShowRewardedAd(() => HintAmount++, () => HintAmount++);
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

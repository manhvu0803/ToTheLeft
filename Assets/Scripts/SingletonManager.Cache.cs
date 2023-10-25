using UnityEngine;

public static partial class SingletonManager
{
    static private Camera _mainCamera;

    private static LevelController _levelController;

    private static SoundManager _soundManager;

    static public Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            return _mainCamera;
        }
    }

    public static LevelController LevelController
    {
        get
        {
            GetSingleton(ref _levelController);
            return _levelController;
        }
    }

    public static SoundManager SoundManager
    {
        get
        {
            GetSingleton(ref _soundManager);
            return _soundManager;
        }
    }

    private static void GetSingleton<T>(ref T target)
    {
        if (target != null)
        {
            return;
        }

        target = Get<T>();
    }
}
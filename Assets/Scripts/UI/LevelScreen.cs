using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelScreen : MonoBehaviour
{
    public TextButton ButtonPrefab;

    [SerializeField]
    private RectTransform _container;

    private readonly List<TextButton> _buttons = new();

    private void Start()
    {
        gameObject.SetActive(false);

        var levels = GameController.Instance.Levels;
        GameController.Instance.OnLoadingNextLevel.AddListener(() => gameObject.SetActive(false));

        for (int i = 0; i < levels.Count; ++i)
        {
            CreateButton(i);
        }
    }

    private void OnEnable()
    {
        var limit = Math.Min(GameController.Instance.Progress + 1, _buttons.Count);

        for (int i = 0; i < limit; ++i)
        {
            _buttons[i].Button.interactable = true;
        }
    }

    private void CreateButton(int level)
    {
        var button = Instantiate(ButtonPrefab, _container);
        button.Init($"Level {level + 1}", () => GameController.Instance.LoadLevel(level));
        button.Button.interactable = level <= GameController.Instance.Progress;
        _buttons.Add(button);
    }
}
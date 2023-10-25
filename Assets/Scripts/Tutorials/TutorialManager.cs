using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Serializable]
    public struct TutorialStep
    {
        public GameObject Container;
    }

    public bool AutoStart = true;

    public bool DisableEverythingOnDone = true;

    [Min(0), Tooltip("If user has already play past this level, disable this tutorial")]
    public int LevelProgress = 1;

    [SerializeField]
    private List<TutorialStep> _steps;

    private int _index = -1;

    public int CurrentTutorialIndex => _index;

    protected virtual void Start()
    {
        if (GameController.Instance?.Progress >= LevelProgress)
        {
            Destroy(this);
            return;
        }

        if (AutoStart)
        {
            NextStep();
        }
    }

    public void NextStep()
    {
        if (_index >= _steps.Count - 1)
        {
#if UNITY_EDITOR
            print("Tutorial is done");
#endif
            if (DisableEverythingOnDone)
            {
                _steps[^1].Container.SetActive(false);
                gameObject.SetActive(false);
            }

            return;
        }

        _index++;

        if (_index > 0)
        {
            _steps[_index - 1].Container.SetActive(false);
        }

        _steps[_index].Container.SetActive(true);
    }

    [ContextMenu("Take children as steps")]
    public void TakeChildrenAsSteps()
    {
        _steps.Clear();

        foreach (Transform child in transform)
        {
            _steps.Add(new TutorialStep() { Container = child.gameObject });
        }
    }
}

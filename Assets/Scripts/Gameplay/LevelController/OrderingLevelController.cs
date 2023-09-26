using System;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class OrderingLevelController : LevelController
{
    private static int CompareX(Component a, Component b)
    {
        var aX = a.transform.position.x;
        var bX = b.transform.position.x;

        if (aX < bX)
        {
            return -1;
        }

        if (bX < aX)
        {
            return 1;
        }

        return 0;
    }

    public float Spacing = 0.1f;

    public bool ControlYPosition = true;

    [SerializeField]
    private PushOutMovable[] _targets;

    private PushOutMovable[] _currentTargets;

    private int _currentIndex;

    private PushOutMovable CurrentTarget => _currentTargets[_currentIndex];

    private PushOutMovable CurrentLeft => _currentTargets[_currentIndex - 1];

    private PushOutMovable CurrentRight => _currentTargets[_currentIndex + 1];

    private float _totalSize;

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        if (_targets == null)
        {
            return;
        }

        foreach (var target in _targets)
        {
            if (target == null || target.Renderer == null)
            {
                continue;
            }

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Gizmos.DrawSphere(target.Bounds.min, 0.1f);
            Gizmos.DrawSphere(target.Bounds.max, 0.1f);
            Gizmos.DrawSphere(target.transform.position, 0.1f);
            Handles.Label(target.Bounds.min, target.Bounds.min.ToString());
            Handles.Label(target.Bounds.max, target.Bounds.max.ToString());
            Handles.Label(target.transform.position, target.transform.position.ToString());
        }
    }
#endif

    protected void OnValidate()
    {
        Utils.Fill(ref _targets);
    }

    protected void Start()
    {
        _currentTargets = new PushOutMovable[_targets.Length];
        _targets.CopyTo(_currentTargets, 0);
        Array.Sort(_currentTargets, CompareX);
        _totalSize = (_targets.Length - 1) * Spacing;

        foreach (var target in _targets)
        {
            target.OnPointerDown.AddListener(() => OnStartMoving(target));
            target.OnPointerDrag.AddListener(OnMove);
            target.OnPointerUp.AddListener(OnDoneMove);
            _totalSize += target.Bounds.size.x;

            if (ControlYPosition)
            {
                target.transform.position = transform.position;
            }
        }

        _currentTargets[0].transform.SetX(transform.position.x - _totalSize / 2 + _currentTargets[0].Bounds.extents.x);

        for (int i = 1; i < _currentTargets.Length; ++i)
        {
            var current = _currentTargets[i];
            current.transform.SetX(_currentTargets[i - 1].Bounds.max.x + Spacing + current.Bounds.extents.x);
        }
    }

    private void OnStartMoving(PushOutMovable target)
    {
        DOTween.Kill(target.transform);

        for (int i = 0; i < _currentTargets.Length; ++i)
        {
            if (_currentTargets[i] == target)
            {
                _currentIndex = i;
                break;
            }
        }
    }

    private void OnMove()
    {
        var spaceNeeded = CurrentTarget.Bounds.size.x + Spacing;

        if (_currentIndex > 0 && CompareX(CurrentTarget, CurrentLeft) < 0)
        {
            var x = CurrentLeft.Bounds.min.x + spaceNeeded + CurrentLeft.Bounds.extents.x;
            Swap(ref _currentIndex, x, -1);
            return;
        }

        if (_currentIndex < _currentTargets.Length - 1 && CompareX(CurrentTarget, CurrentRight) > 0)
        {
            var x = CurrentRight.Bounds.max.x - spaceNeeded - CurrentRight.Bounds.extents.x;
            Swap(ref _currentIndex, x, 1);
        }
    }

    private void OnDoneMove()
    {
        var restPosition = transform.position;

        if (_currentIndex > 0)
        {
            restPosition.x = CurrentLeft.Bounds.max.x + Spacing + CurrentTarget.Bounds.extents.x;
        }
        else
        {
            restPosition.x = transform.position.x - _totalSize / 2 + CurrentTarget.Bounds.extents.x;
        }

        if (ControlYPosition)
        {
            CurrentTarget.transform.DOMove(restPosition, 0.15f);
        }
        else
        {
            CurrentTarget.transform.DOMoveX(restPosition.x, 0.15f);
        }
    }

    private void Swap(ref int index, float positionX, int dir)
    {
        (_currentTargets[index], _currentTargets[index + dir]) = (_currentTargets[index + dir], _currentTargets[index]);
        _currentTargets[index].transform.DOMoveX(positionX, 0.15f);
        index += dir;
    }

    public override float CompletionRate()
    {
        var correctCount = 0;

        for (int i = 0; i < _targets.Length; ++i)
        {
            if (_targets[i] == _currentTargets[i])
            {
                correctCount++;
            }
        }

        return (float)correctCount / _targets.Length;
    }
}
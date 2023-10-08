using System;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class OrderingLevelController : LevelController
{
    [Serializable]
    public enum Axis { X, Y, Z };

    public bool ControlFullPosition = true;

    public Axis CompareAxis = Axis.X;

    public float Spacing = 0.1f;

    [SerializeField]
    private PushOutMovable[] _targets;

    private PushOutMovable[] _currentTargets;

    private int _currentIndex;

    private PushOutMovable CurrentTarget => _currentTargets[_currentIndex];

    private PushOutMovable CurrentLeft => _currentTargets[_currentIndex - 1];

    private PushOutMovable CurrentRight => _currentTargets[_currentIndex + 1];

    private float _totalSize;

    private float GetValue(Vector3 vector)
    {
        return CompareAxis switch
        {
            Axis.X => vector.x,
            Axis.Y => vector.y,
            _ => vector.z
        };
    }

    private Vector3 NewValue(Vector3 vector3, float value)
    {
        switch (CompareAxis)
        {
            case Axis.X:
                vector3.x = value;
                break;
            case Axis.Y:
                vector3.y = value;
                break;
            default:
                vector3.z = value;
                break;
        }

        return vector3;
    }

    private int Compare(Component a, Component b)
    {
        var valueA = GetValue(a.transform.position);
        var valueB = GetValue(b.transform.position) ;

        if (valueA < valueB)
        {
            return -1;
        }

        if (valueB < valueA)
        {
            return 1;
        }

        return 0;
    }

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
        Array.Sort(_currentTargets, Compare);
        _totalSize = (_targets.Length - 1) * Spacing;

        foreach (var target in _targets)
        {
            target.OnPointerDown.AddListener(() => OnStartMoving(target));
            target.OnPointerDrag.AddListener(OnMove);
            target.OnPointerUp.AddListener(OnDoneMove);
            _totalSize += GetValue(target.Bounds.size);

            if (ControlFullPosition)
            {
                target.transform.position = transform.position;
            }
        }

        var firstTransform = _currentTargets[0].transform;
        var position = NewValue(firstTransform.position, GetValue(transform.position) - _totalSize / 2 + GetValue(_currentTargets[0].Bounds.extents));
        firstTransform.position = position;

        for (int i = 1; i < _currentTargets.Length; ++i)
        {
            var current = _currentTargets[i];
            position = NewValue(current.transform.position, GetValue(_currentTargets[i - 1].Bounds.max) + Spacing + GetValue(current.Bounds.extents));
            current.transform.position = position;
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
        var spaceNeeded = GetValue(CurrentTarget.Bounds.size) + Spacing;

        if (_currentIndex > 0 && Compare(CurrentTarget, CurrentLeft) < 0)
        {
            var position = GetValue(CurrentLeft.Bounds.min) + spaceNeeded + GetValue(CurrentLeft.Bounds.extents);
            Swap(ref _currentIndex, position, -1);
            return;
        }

        if (_currentIndex < _currentTargets.Length - 1 && Compare(CurrentTarget, CurrentRight) > 0)
        {
            var position = GetValue(CurrentRight.Bounds.max) - spaceNeeded - GetValue(CurrentRight.Bounds.extents);
            Swap(ref _currentIndex, position, 1);
        }
    }

    private void OnDoneMove()
    {
        float restPosition;

        if (_currentIndex > 0)
        {
            restPosition = GetValue(CurrentLeft.Bounds.max) + Spacing + GetValue(CurrentTarget.Bounds.extents);
        }
        else
        {
            restPosition = GetValue(transform.position) - _totalSize / 2 + GetValue(CurrentTarget.Bounds.extents);
        }

        if (ControlFullPosition)
        {
            var position = NewValue(transform.position, restPosition);
            CurrentTarget.transform.DOMove(position, 0.15f);
        }
        else
        {
            DoMove(CurrentTarget.transform, restPosition);
        }
    }

    private void Swap(ref int index, float position, int dir)
    {
        (_currentTargets[index], _currentTargets[index + dir]) = (_currentTargets[index + dir], _currentTargets[index]);
        DoMove(_currentTargets[index].transform, position);
        index += dir;
    }

    private void DoMove(Transform target, float position)
    {
        switch (CompareAxis)
        {
            case Axis.X:
                target.DOMoveX(position, 0.15f);
                break;
            case Axis.Y:
                target.DOMoveY(position, 0.15f);
                break;
            case Axis.Z:
                target.DOMoveZ(position, 0.15f);
                break;
        }
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
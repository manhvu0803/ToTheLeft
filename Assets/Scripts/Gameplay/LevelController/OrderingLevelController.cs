using System;
using System.Collections;
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

    private PushOutMovable[] _correctOrderedTargets;

    private int _currentIndex;

    private PushOutMovable CurrentTarget => _targets[_currentIndex];

    private PushOutMovable CurrentLeft => _targets[_currentIndex - 1];

    private PushOutMovable CurrentRight => _targets[_currentIndex + 1];

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

    protected IEnumerator Start()
    {
        // We need to wait for the PushOutMovables to initialize first
        yield return null;

        _correctOrderedTargets = new PushOutMovable[_targets.Length];
        _targets.CopyTo(_correctOrderedTargets, 0);
        Array.Sort(_targets, Compare);
        _totalSize = (_targets.Length - 1) * Spacing;

        foreach (var target in _targets)
        {
            target.OnDown.AddListener(() => OnStartMoving(target));
            target.OnDrag.AddListener(OnMove);
            target.OnUp.AddListener(OnDoneMove);
            _totalSize += GetValue(target.Bounds.size);

            if (ControlFullPosition)
            {
                target.transform.position = transform.position;
            }
        }

        var firstTransform = _targets[0].transform;
        firstTransform.position = NewValue(firstTransform.position, GetValue(transform.position) - _totalSize / 2 + GetValue(_targets[0].Bounds.extents));;

        for (int i = 1; i < _targets.Length; ++i)
        {
            var current = _targets[i];
            var position = NewValue(current.transform.position, GetValue(_targets[i - 1].Bounds.max) + Spacing + GetValue(current.Bounds.extents));
            position.z = i * -0.1f;
            current.transform.position = position;
        }
    }

    private void OnStartMoving(PushOutMovable target)
    {
        DOTween.Kill(target.transform);

        for (int i = 0; i < _targets.Length; ++i)
        {
            if (_targets[i] == target)
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

        if (_currentIndex < _targets.Length - 1 && Compare(CurrentTarget, CurrentRight) > 0)
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
            DOTween.Kill(CurrentLeft.transform, complete: true);
            restPosition = GetValue(CurrentLeft.Bounds.max) + Spacing + GetValue(CurrentTarget.Bounds.extents);
        }
        else
        {
            restPosition = GetValue(transform.position) - _totalSize / 2 + GetValue(CurrentTarget.Bounds.extents);
        }

        if (ControlFullPosition)
        {
            var position = NewValue(transform.position, restPosition);
            position.z = _currentIndex * -0.1f;
            CurrentTarget.transform.DOMove(position, 0.15f);
        }
        else
        {
            CurrentTarget.transform.SetZ(_currentIndex * -0.1f);
            print("RestPosition: " + restPosition);
            DoMove(CurrentTarget.transform, restPosition);
        }
    }

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

    private void Swap(ref int index, float position, int dir)
    {
        (_targets[index], _targets[index + dir]) = (_targets[index + dir], _targets[index]);
        DoMove(_targets[index].transform, position);
        _targets[index].transform.SetZ(index * -0.1f);
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
            if (_correctOrderedTargets[i] == _targets[i])
            {
                correctCount++;
            }
        }

        return (float)correctCount / _targets.Length;
    }
}
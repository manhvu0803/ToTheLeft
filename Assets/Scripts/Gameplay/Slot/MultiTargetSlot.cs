using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetSlot : Slot
{
    [SerializeField]
    private Transform[] _targets;

    private HashSet<Transform> _targetSet;

    public ReadOnlySpan<Transform> Targets => _targets;

    protected override void Start()
    {
        base.Start();
        _targetSet = new HashSet<Transform>(_targets);
    }

    public override bool IsTarget(Transform transform)
    {
        return _targetSet.Contains(transform);
    }
}
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

[Obsolete]
public class OrderedSlotLevelController : SlotLevelController
{
    [SerializeField]
    private Transform[] _targets;

    private readonly Dictionary<Transform, int> _bookIndexMap = new();

    private readonly Dictionary<Transform, int> _slotIndexMap = new();

    protected override void Start()
    {
        base.Start();

        if (_targets.Length != Slots.Length)
        {
            Debug.LogWarning("The number of books and slots are not equal");
        }

        for (int i = 0; i < Slots.Length; ++i)
        {
            _targets[i].position = Slots[i].transform.position - new Vector3(0, 0, 0.5f);
            _bookIndexMap[_targets[i]] = i;
            _slotIndexMap[Slots[i].transform] = i;
        }
    }

    public override bool UpdateSlot(Transform book, Transform slot, bool isInteracting = false)
    {
        if (book == null || !_bookIndexMap.TryGetValue(book, out var bookIndex))
        {
            return false;
        }

        if (slot == null || !_slotIndexMap.TryGetValue(slot, out var slotIndex))
        {
            return false;
        }

        if (bookIndex > slotIndex)
        {
            SwapForward(slotIndex, bookIndex);
        }
        else if (bookIndex < slotIndex)
        {
            SwapBackward(bookIndex, slotIndex);
        }

        _targets[slotIndex] = book;
        _bookIndexMap[book] = slotIndex;
        return true;
    }

    private void SwapForward(int startIndex, int endIndex)
    {
        var current = _targets[startIndex];

        for (int i = startIndex; i < endIndex; ++i)
        {
            DOTween.Kill(current.transform, true);
            var position = Slots[i + 1].transform.position - new Vector3(0, 0, 0.5f);
            current.transform.DOMove(position, 0.2f);
            _bookIndexMap[current] = i + 1;
            (_targets[i + 1], current) = (current, _targets[i + 1]);
        }
    }

    private void SwapBackward(int startIndex, int endIndex)
    {
        for (int i = startIndex + 1; i < endIndex + 1; ++i)
        {
            DOTween.Kill(_targets[i].transform, true);
            var position = Slots[i - 1].transform.position - new Vector3(0, 0, 0.5f);
            _targets[i].transform.DOMove(position, 0.2f);
            _bookIndexMap[_targets[i]] = i - 1;
            _targets[i - 1] = _targets[i];
        }
    }

    public override float CompletionRate
    {
        get {
            int completeSlotCount = 0;
            for (int i = 0; i < Slots.Length; ++i)
            {
                if (Slots[i].IsTarget(_targets[i]))
                {
                    completeSlotCount++;
                }
            }

            return (float)completeSlotCount / Slots.Length;
        }
    }

    public override void Hint()
    {
        throw new NotImplementedException();
    }
}

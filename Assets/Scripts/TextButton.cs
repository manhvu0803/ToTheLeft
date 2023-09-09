using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    [field: SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private Button _button;

    public TMP_Text Text => _text;

    public Button Button => _button;

    private void OnValidate()
    {
        this.FillFromChildren(ref _text);
        this.FillFromChildren(ref _button);
    }

    public void Init(string text, Action onClick = null)
    {
        _text.text = text;
        _button.onClick.AddListener(() => onClick?.Invoke());
    }
}

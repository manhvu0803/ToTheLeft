using TMPro;
using UnityEngine;

public class TextTimer : UITimer
{
    [SerializeField]
    private TMP_Text _text;

    public override float TimeLeft
    {
        get => base.TimeLeft;
        set
        {
            base.TimeLeft = value;
            var min = (int)value / 60;
            var sec = (int)value % 60;
            _text.text = $"{(min < 10 ? "0" : "")}{min} : {(sec < 10 ? "0" : "")}{sec}";
        }
    }
}

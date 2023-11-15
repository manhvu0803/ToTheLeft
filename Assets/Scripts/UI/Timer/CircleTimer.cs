using UnityEngine;
using UnityEngine.UI;

public class CircleTimer : UITimer
{
    [SerializeField]
    private Image _timerCircle;

    public override float TimeLeft
    {
        get => base.TimeLeft;
        set
        {
            base.TimeLeft = value;
            var ratio = value / TimeLimit;
            _timerCircle.fillAmount = ratio;

            if (ratio > 0.5f)
            {
                _timerCircle.color = Color.green;
            }
            else if (ratio > 0.25f)
            {
                _timerCircle.color = Color.yellow;
            }
            else
            {
                _timerCircle.color = Color.red;
            }
        }
    }
}
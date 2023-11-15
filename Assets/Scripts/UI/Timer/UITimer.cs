using UnityEngine;

public abstract class UITimer : MonoBehaviour
{
    public virtual float TimeLimit { get; set; }

    public virtual float TimeLeft { get; set; }
}

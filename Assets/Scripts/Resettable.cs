using UnityEngine;
using UnityEngine.Events;

public class Resettable : MonoBehaviour
{
    public UnityEvent onReset;

    public void Reset()
    {
        onReset.Invoke();
    }
}
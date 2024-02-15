using UnityEngine;
using UnityEngine.Events;

public class ActivationEvents : MonoBehaviour
{
    public UnityEvent onEnabled;
    public UnityEvent onDisabled;

    void OnEnable()
    {
        onEnabled.Invoke();
    }

    void OnDisable()
    {
        onDisabled.Invoke();
    }
}
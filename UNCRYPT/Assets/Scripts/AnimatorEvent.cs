using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvent : MonoBehaviour
{
    public UnityEvent onExecute;
    
    public void Invoke()
    {
        onExecute?.Invoke();
    }      
}
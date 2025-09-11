using UnityEngine;

//todo delete
public class EventInvoker : MonoBehaviour
{
    [SerializeField] private string eventName;

    public void TriggerEvent<T>(T eventData)
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(eventName, eventData);
        }
    }
}

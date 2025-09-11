using UnityEngine;

//todo delete
public class EventSubscriber<T> : MonoBehaviour
{
    [SerializeField] private string eventName;

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe<T>(eventName, OnEventTriggered);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<T>(eventName, OnEventTriggered);
        }
    }

    private void OnEventTriggered(T eventData)
    {
        Debug.Log($"{eventName} was triggered for {gameObject.name}");
    }
}

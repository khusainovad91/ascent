using System;
using System.Collections.Generic;

//todo persistent ли нужен тут?
public class EventManager : PersistentSingleton<EventManager>
{
    private Dictionary<string, Delegate> events;
    private new void Awake()
    {
        base.Awake();
        events = new Dictionary<string, Delegate>();
    }

    // Подписаться на собтыие
    public void Subscribe(string eventName, Action listener)
    {
        if (!events.ContainsKey(eventName))
        {
            events[eventName] = null;
        }
        events[eventName] = (Action) events[eventName] + listener;
    }

    //получить последние данные
    public T GetLastEventData<T>(string eventName)
    {
        if (events.TryGetValue(eventName, out var data) && data is T typedData)
        {
            return typedData;
        }
        return default;
    }

    // Подписаться на событие с обобщенными параметрами
    public void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (!events.ContainsKey(eventName))
        {
            events[eventName] = null;
        }
        events[eventName] = (Action<T>) events[eventName] + listener;
    }

    public void Unsubscribe(string eventName, Action listener)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = (Action)events[eventName] - listener;

            if (events[eventName] == null)
            {
                events.Remove(eventName);
            }
        }
    }

    // Отписаться от события с обобщенными параметрами
    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = (Action<T>)events[eventName] - listener;

            if (events[eventName] == null)
            {
                events.Remove(eventName);
            }
        }
    }

    // Вызвать событие параметризованное
    public void TriggerEvent<T>(string eventName, T eventData)
    {
        if (events.ContainsKey(eventName)) 
        {
            (events[eventName] as Action<T>)?.Invoke(eventData);
        }
    }

    // Вызвать событие
    public void TriggerEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            (events[eventName] as Action)?.Invoke();
        }
    }
}

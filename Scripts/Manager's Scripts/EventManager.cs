using System;
using System.Collections.Generic;

//todo persistent �� ����� ���?
public class EventManager : PersistentSingleton<EventManager>
{
    private Dictionary<string, Delegate> events;
    private new void Awake()
    {
        base.Awake();
        events = new Dictionary<string, Delegate>();
    }

    // ����������� �� �������
    public void Subscribe(string eventName, Action listener)
    {
        if (!events.ContainsKey(eventName))
        {
            events[eventName] = null;
        }
        events[eventName] = (Action) events[eventName] + listener;
    }

    //�������� ��������� ������
    public T GetLastEventData<T>(string eventName)
    {
        if (events.TryGetValue(eventName, out var data) && data is T typedData)
        {
            return typedData;
        }
        return default;
    }

    // ����������� �� ������� � ����������� �����������
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

    // ���������� �� ������� � ����������� �����������
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

    // ������� ������� �����������������
    public void TriggerEvent<T>(string eventName, T eventData)
    {
        if (events.ContainsKey(eventName)) 
        {
            (events[eventName] as Action<T>)?.Invoke(eventData);
        }
    }

    // ������� �������
    public void TriggerEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            (events[eventName] as Action)?.Invoke();
        }
    }
}

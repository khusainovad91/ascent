using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConditionHandler: NetworkBehaviour
{
    public HashSet<Condition> conditions { get; private set; }
    
    //public ConditionHandler()
    //{
    //    conditions = new HashSet<Condition>();
    //}

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        conditions = new HashSet<Condition>();
    }

    public bool ContainsCondition<T>() 
    {
        return conditions.Any(c => c is T);
    }

    //Метод добавляеи в список новый кондишн, заодно устанавливает линк в сам кондишн этот кондишн хэндлер
    [Rpc(SendTo.Server)]
    public void AddConditionRpc(ConditionType conditionType)
    {
        var condition = this.conditions.FirstOrDefault(condition => condition.Type == conditionType);
        if (condition != null)
        {
            return;
        }

        if (!IsServer) 
        {
            return;
            //create condition (instantiate and spawn)
        }
        var conditionPrefab = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[1].PrefabList[(int)conditionType].Prefab;

        var newConditionGO = GameObject.Instantiate(conditionPrefab);
        NetworkObject newConditionNO = newConditionGO.GetComponent<NetworkObject>();
        newConditionNO.Spawn();
        newConditionGO.GetComponent<Condition>().SetConditionHandlerRpc(new NetworkBehaviourReference(this));
        newConditionNO.TrySetParent(this.transform, true);
        newConditionGO.GetComponent<Condition>().SetConditionHandlerRpc(this);
        //в клиентском Rpc расставялем в HashSet по NOR (по референсам)
        AddConditionClientRpc(new NetworkObjectReference(newConditionNO));
    }

    [Rpc(SendTo.Everyone)]
    private void AddConditionClientRpc(NetworkObjectReference conditionNOR)
    {
        conditionNOR.TryGet(out NetworkObject conditionNO);
        var condition = conditionNO.GetComponent<Condition>();
        this.conditions.Add(condition);
        EventManager.Instance.TriggerEvent<Condition>("OnConditionAdd", condition);
    }


    public void RemoveConditionByType<T>()
    {
        var condition = conditions.First(condition => condition is T);
        condition.DeleteThisCondition();
    }

    [Rpc(SendTo.Server)]
    public void RemoveConditionRpc(ConditionType conditionType) {
        var condition = this.conditions.FirstOrDefault(condition => condition.Type == conditionType);

        if (condition != null) {
            RemoveConditionClientRpc(new NetworkObjectReference(condition.gameObject));
            condition.NetworkObject.Despawn();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveConditionClientRpc(NetworkObjectReference conditionNOR)
    {
        conditionNOR.TryGet(out NetworkObject conditionNO);
        var condition = conditionNO.GetComponent<Condition>();
        EventManager.Instance.TriggerEvent<Condition>("OnConditionRemove", condition);
        this.conditions.Remove(condition);
    }

    [Rpc(SendTo.Server)]
    public void RemoveAllConditionsRpc()
    {
        foreach (var condition in conditions)
        {
            EventManager.Instance.TriggerEvent<Condition>("OnConditionRemove", condition);
            condition.GetComponent<NetworkObject>().Despawn();
        }
        RemoveAllConditionsClientRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveAllConditionsClientRpc()
    {
        conditions.Clear();
    }

    public T GetConditionByType<T>()
    {
        return conditions.OfType<T>().FirstOrDefault();
    }
}

public enum ConditionType
{
    Tired = 0,
    Rested = 1,
    Fainted = 2,
    Stuned = 3
}
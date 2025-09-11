#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class NetworkPrefabHashViewer
{
    [MenuItem("Tools/Print Network Prefab Hashes")]
    public static void PrintHashes()
    {
        var config = Object.FindFirstObjectByType<NetworkManager>()?.NetworkConfig;
        if (config == null)
        {
            Debug.LogError("NetworkManager или его конфигурация не найдены.");
            return;
        }

        foreach (var list in config.Prefabs.NetworkPrefabsLists)
        {
            foreach (var entry in list.PrefabList)
            {
                if (entry.Prefab != null && entry.Prefab.TryGetComponent<NetworkObject>(out var netObj))
                {
                    Debug.Log($"Name: {entry.Prefab.name}, Hash: {netObj.PrefabIdHash}");
                }
            }
        }
    }
}
#endif
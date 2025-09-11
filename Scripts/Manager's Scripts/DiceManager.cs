using Unity.Netcode;
using UnityEngine;

public class DiceManager : NetworkBehaviour//PersistentSingleton<DiceManager>
{
    public static DiceManager Instance;
    public NetworkPrefabsList AttackingDicePrefabs;
    public NetworkPrefabsList DeffendingDicePrefabs;
    public NetworkPrefabsList FateDicePrefabs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    //public override void OnNetworkSpawn()
    //{
    //    Instance = this;
    //    base.OnNetworkSpawn();
    //    //NetworkObject netObj = GetComponent<NetworkObject>();
    //    //netObj.DontDestroyWithOwner = true;
    //    DontDestroyOnLoad(Instance);
    //    //if (Instance != null)
    //    //{
    //    //    Destroy(Instance);
    //    //}
    //    Debug.Log("kek " + AttackingDicePrefabs.PrefabList[0].Prefab.name);
    //}
}

public enum AttackingDice
{
    WhiteDice = 0,
    GreenDice = 1,
    RedDice = 2
}

public enum DeffendingDice
{
    LightDice = 0,
    MidDice = 1,
    StrongDice = 2
}

public enum FateDice
{
    LightFateDice = 0,
    HardFateDice = 1
}
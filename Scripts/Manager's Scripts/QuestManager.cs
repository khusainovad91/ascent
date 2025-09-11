using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class QuestManager : NetworkBehaviour
{
    public static QuestManager Instance;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Instance = this;
        DontDestroyOnLoad(this);
    }

}

using System;
using Unity.Netcode;
using UnityEngine;

public class KillNMonstersQuest : Quest
{ 
    [SerializeField]
    private int _initialCounter;
    private NetworkVariable<int> _counter = new NetworkVariable<int>(99, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private string _monsterName;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        EventManager.Instance.Subscribe<EnemyObject>("EnemyDied", OnEnemyDeath); 
        _counter.Value = _initialCounter;
    }

    private void OnEnemyDeath(EnemyObject enemy)
    {
        if (IsServer)
        {
            if (enemy.Name == _monsterName)
            {
                _counter.Value--;
                UpdateUiClientRpc($"����� {_monsterName}: {_counter.Value}");
            }

            if (_counter.Value == 0)
            {
                _isCompleted.Value = true;
                UpdateUiClientRpc($"<s>����� {_monsterName}</s>: {_counter.Value}");
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventManager.Instance.Unsubscribe<EnemyObject>("EnemyDied", OnEnemyDeath);
    }
}

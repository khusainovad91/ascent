using ExtensionMethods;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemiesManager : NetworkBehaviour
{
    public static EnemiesManager Instance;

    [SerializeField]
    private List<EnemyObject> enemies;
    private EnemyObject currentEnemy;

    //4 testing purpose
    [SerializeField]
    private TileMapManager startingEnemiesTile;
    //4 testing purpose
    [SerializeField]
    private EnemyObject firstEnemy;

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        base.OnNetworkSpawn();
        enemies = new List<EnemyObject>();
    }

    //todo сценарий игры будет вызывать
    [Rpc(SendTo.Server)]
    public void AddNewEnemiesRpc() //List<EnemyObject> groupOfEnemyPrefabs //tileToSpawnEnemeies
    {
        if (startingEnemiesTile == null)
        {
            return;
        }
        if (!IsServer)
        {
            return;    
        }
        
        InstantiateEnemiesAndSpawn(firstEnemy, 3, startingEnemiesTile);
    }

    private void InstantiateEnemiesAndSpawn(EnemyObject enemyPrefab, int amount, TileMapManager spawnTile)
    {
        for (int i = 0; i < amount; i++)
        {
            EnemyObject enemyPrefabClone = Instantiate(enemyPrefab);
            var enemyNetworkObject = enemyPrefabClone.GetNetworkObject();
            enemyNetworkObject.Spawn();

            NetworkObjectReference enemyNetworkObjectReference = new NetworkObjectReference(enemyNetworkObject);
            SendEnemyRefereneceToClientListRpc(enemyNetworkObjectReference);

            BoardManager.Instance.AddEnemyOnBoard(enemyPrefabClone, startingEnemiesTile);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SendEnemyRefereneceToClientListRpc(NetworkObjectReference enemyNetworkObjectReference)
    {
        enemyNetworkObjectReference.TryGet(out NetworkObject enemyNetworkObject);
        var enemyObject = enemyNetworkObject.GetComponent<EnemyObject>();
        //Debug.Log("Я выполнился на клиенте");
        //Debug.Log($"[Клиент] Получен NetworkObjectReference с ID: {enemyNetworkObjectReference.NetworkObjectId}");
        //Debug.Log($"[Сервер] Спауним врага с ID: {enemyNetworkObject.NetworkObjectId}");
        enemies.Add(enemyObject);
        //Debug.Log("Количество врагов: " + enemies.Count);
        if (enemyObject == null) Debug.LogError("[Клиент] EnemyObject не найден на объекте!");
    }
    
    public void GetLogs()
    {
        UnityEngine.Debug.Log("Количество врагов на клиенте: " + enemies.Count);
        foreach (var enemy in enemies)
        {
            Debug.Log("Враг находится на клетке: " + enemy.CurrentCell);
        }
    }

    //Выполняется только на сервере
    public IEnumerator HandleEnemiesTurn()
    {
        enemies.Shuffle();
        foreach (var enemy in enemies)
        { 
            yield return enemy.EnemyController.StartTurn();
        }

        Debug.Log("Все враги завершили ходы!");
        if (GameManager.Instance.Heroes.Where(hero => hero.CurrentState != HeroState.Fainted 
            && hero.CurrentState != HeroState.Dead).Count() != 0)
        {
            GameManager.Instance.ChangeState(GameState.HeroTurn);
        }
    }

    public void RemoveEnemy(EnemyObject enemyObject)
    {
        if (IsServer)
        {
            enemies.Remove(enemyObject);
        }
    }
}

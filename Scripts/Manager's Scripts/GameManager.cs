using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour //NetworkPersistentSingleton<GameManager>
{
    public static GameManager Instance;
    //Должен быть не лист героев а Dictionary, где NetworkObjectReference - идентификатор NetworkObject 
    public List<HeroData> Heroes;
    public Dictionary<HeroData, NetworkObjectReference> heroToNODict;
    private int _endedTurn = 0;

    //todo delete
    public bool IsAnythingIsMovingNow = false;
    public NetworkVariable<GameState> StateOfGame = new NetworkVariable<GameState>(GameState.Menu, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        Instance = this;
        //ActiveSceneSynchronization = true
        NetworkObject netObj = GetComponent<NetworkObject>();
        netObj.DontDestroyWithOwner = true;
        DontDestroyOnLoad(Instance);
        Heroes = new List<HeroData>();
        heroToNODict = new Dictionary<HeroData, NetworkObjectReference>();
        ChangeState(GameState.SelectCharacterScreen);
        EventManager.Instance?.Subscribe<HeroData>("OnHeroStateChange", HandleHeroStateChange);
    }


    public void ChangeState(GameState newState)
    {
        if (StateOfGame.Value == newState || !IsServer) return;
        //ChangeStateServerRpc(newState);
        StateOfGame.Value = newState;

        switch (newState)
        {
            case GameState.Menu:
                HandleMenu();
                break;
            case GameState.SelectCharacterScreen:
                HandleSelectCharacterScreen();
                break;
            case GameState.Starting:
                HandleStartingRpc();
                break;
            case GameState.SpawningHeroes:
                HandleSpawningHeroesRpc();
                break;
            case GameState.SpawningEnemies:
                HandleSpawningEnemiesRpc();
                break;
            case GameState.HeroTurn:
                HandleHeroTurnRpc();
                break;
            case GameState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case GameState.Win:
                //todo
                HandleWinRpc();
                Debug.Log("WIN");
                break;
            case GameState.Lose:
                //todo
                HandleLooseRpc();
                Debug.Log("LOOSE");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void HandleWinRpc()
    {
        EventManager.Instance.TriggerEvent("OnHeroesWon");
    }

    [Rpc(SendTo.Everyone)]
    private void HandleLooseRpc()
    {
        EventManager.Instance.TriggerEvent("OnEnemiesWon");
    }

    private void HandleSelectCharacterScreen()
    {
        
    }

    private void HandleMenu()
    {

    }

    [Rpc(SendTo.Everyone)]
    private void HandleStartingRpc()
    {
        StartCoroutine(Wait4BoardManagerToSpawn());
    }

    private IEnumerator Wait4BoardManagerToSpawn()
    {
        yield return new WaitUntil(() => BoardManager.Instance != null && BoardManager.Instance.IsSpawned);
        BoardManager.Instance.Init();
        yield return new WaitUntil(() => BoardManager.Instance.IsReady);
        EventManager.Instance.TriggerEvent("GameStarting");

        UiManager.Instance.SetUpTopPanel(Heroes);
        UiManager.Instance.TurnOnTopPanel();

        ChangeState(GameState.SpawningHeroes);
    }


    [Rpc(SendTo.Everyone)]
    private void HandleSpawningHeroesRpc()
    {
        LeanTween.delayedCall(1f, () => BoardManager.Instance.PlaceHeroesOnABoard());
        LeanTween.delayedCall(2f, () => EventManager.Instance.TriggerEvent("HeroesSpawned"));
        LeanTween.delayedCall(2f, () => CameraController.Instance.MoveCameraToTargetRpc(Heroes.First().FieldHero.GetNetworkObjectReference(), 0.6f));

        //todo исправить, может быть время загрузки больше 1f и тогда базовые кубики защиты опять сломаются
        //EventManager.Instance.TriggerEvent("HeroesSpawned");
        ChangeState(GameState.SpawningEnemies);
    }

    [Rpc(SendTo.Everyone)]
    private void HandleSpawningEnemiesRpc()
    {
        //4 testing purpose
        //todo make normal spawn
        if (!IsServer)
        {
            return;
        }
        StartCoroutine(Wait4EnemiesManagerToSpawn());
    }

    [Rpc(SendTo.Everyone)]
    private void HandleHeroTurnRpc()
    {
        EnemiesManager.Instance.GetLogs();
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Free);

        EventManager.Instance.TriggerEvent("OnRoundStart");

        //if (!IsServer) return;
        foreach (var hero in Heroes)
        {
            //todo questionable
            EventManager.Instance.TriggerEvent<HeroData>("OnHeroTurn", hero);
        }
    }

    private IEnumerator Wait4EnemiesManagerToSpawn()
    {
        yield return new WaitUntil(() => EnemiesManager.Instance != null && EnemiesManager.Instance.IsSpawned);
        EnemiesManager.Instance.AddNewEnemiesRpc();
        ChangeState(GameState.HeroTurn);
    }

    //Выполняется только на сервере
    private void HandleEnemyTurn()
    {
        if (!IsServer) return;
        foreach (var hero in Heroes)
        {
            hero.FieldHero.GetComponent<Outline>().enabled = false;
        }

        SendOnEnemyTurnEventToClientsRpc();
        StartCoroutine(EnemiesManager.Instance.HandleEnemiesTurn());
    }

    [Rpc(SendTo.Everyone)]
    private void SendOnEnemyTurnEventToClientsRpc()
    {
        EventManager.Instance.TriggerEvent("OnEnemyTurn");
        Debug.Log("Время хода врагов");
    }

    private void HandleHeroStateChange(HeroData heroData)
    {
        var heroesAlive = Heroes.Where(hero => hero.CurrentState != HeroState.Fainted && hero.CurrentState != HeroState.Dead);
        Debug.Log("Героев в живых: " + heroesAlive.Count());

        if (heroesAlive.Count() == 0)
        {
            ChangeState(GameState.Lose);
        }

        if (heroData.CurrentState == HeroState.EndedTurn)
        {
            Debug.Log("endedTurn: " + _endedTurn);
            _endedTurn++;
            if (heroesAlive.Count() == _endedTurn)
            {
                this.ChangeState(GameState.EnemyTurn);
                _endedTurn = 0;
            }
        }
    }

    public void AddHero(HeroData heroData)
    {
        Heroes.Add(heroData);
    }

    private void InstantiateHeroes()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (var heroData in Heroes)
        {
            GameObject heroPrefab = Instantiate(heroData.FieldHero.gameObject);
            var heroNetworkObject = heroPrefab.GetComponent<NetworkObject>();
            heroNetworkObject.SpawnWithOwnership(heroData.PlayerId);
            //heroNetworkObject.ChangeOwnership(heroData.PlayerId);
            heroPrefab.gameObject.GetComponent<Outline>().OutlineColor = heroData.PlayerColor;

            NetworkObjectReference heroReference = new NetworkObjectReference(heroNetworkObject);
            SendReferenceRpc(heroData.HeroNumber, heroReference);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SendReferenceRpc(int heroNumber, NetworkObjectReference heroNetworkReference)
    {
        HeroData heroData = Heroes.First(it => it.HeroNumber == heroNumber);
        heroNetworkReference.TryGet(out NetworkObject heroNetworkObject);
        Debug.Log(heroNetworkObject.GetComponent<FieldHero>());

        heroData.SetUpFieldHero(heroNetworkObject.GetComponent<FieldHero>());
        heroNetworkObject.GetComponent<FieldHero>().SetupHeroData(heroData); //пробую засетапить херодату в филд хиро хотя нахера? есть же ссылки на нетворк объекты

        Debug.Log("Номер добавленного героя: " + heroData.HeroNumber);
        Debug.Log("Имя добавленного героя: " + heroNetworkObject.GetComponent<FieldHero>().name);
        StartCoroutine(WaitForHero(heroData, heroNetworkReference));
    }

    private IEnumerator WaitForHero(HeroData heroData, NetworkObjectReference heroReference)
    {
        NetworkObject heroNetworkObject;

        // Ждём, пока объект появится у клиента
        while (!heroReference.TryGet(out heroNetworkObject))
        {
            yield return null;
        }

        Debug.Log($"[Клиент] Герой найден! Имя: {heroNetworkObject.gameObject.name}");

        // Добавляем в локальный словарь
        heroToNODict.Add(heroData, heroNetworkObject);
    }

    internal void StartFirstLevel()
    {
        LeanTween.delayedCall(1f, () =>
        {
            InstantiateHeroes();
            ChangeState(GameState.Starting);
        }); //непонятная хуйня, Grid не успеват заинстантиейтится, пришлось поставить делей
    }

    //Достать ссылку на FieldHero/NetworkOject по его номеру, указанному в StartGameButton
    public NetworkObjectReference GetHeroNetworkObjectReferenceByNumber(int number)
    {
        return heroToNODict.First((it) => it.Key.HeroNumber == number).Value;
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance?.Unsubscribe<HeroData>("OnHeroStateChange", HandleHeroStateChange);
        base.OnNetworkDespawn();
    }
}

[Serializable]
public enum GameState
{
    Menu,// = 0
    LoadingScreen,
    SelectCharacterScreen,
    Starting,
    SpawningHeroes,
    SpawningEnemies,
    HeroTurn,
    EnemyTurn,
    Win,
    Lose// = 6
}

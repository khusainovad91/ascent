using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.Linq;
using System;
using Unity.VisualScripting;
using JetBrains.Annotations;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UIElements;

public class BoardManager : NetworkBehaviour//Singleton<BoardManager>
{
    public static BoardManager Instance;
    [NonSerialized]
    public Grid grid;
    public List<TileMapManager> tiles;
    [SerializeField] private TileMapManager starterTile;
    [NonSerialized]
    public bool IsReady = false;
    public Dictionary<Vector3Int, Cell> CellsInBoard { get; private set; }

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(this);
        base.OnNetworkSpawn();
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //new void Awake()
    //{
    //    base.Awake();
    //    Init();
    //}

    [Rpc(SendTo.Server)]
    public void ClearCellServerRpc(Vector3Int coords)
    {
        ClearCellClientRpc(coords);
    }

    [Rpc(SendTo.Everyone)]
    private void ClearCellClientRpc(Vector3Int coords)
    {
        Cell cell = CellsInBoard[coords];
        cell.isOcupied = false;
        cell.objectOnTile = null;
    }

    [Rpc(SendTo.Server)]
    public void OcupieCellServerRpc(Vector3Int coords, NetworkObjectReference fieldObjectReference)
    {
        Debug.Log("Test2");
        OcupieCellClientRpc(coords, fieldObjectReference);
    }

    [Rpc(SendTo.Everyone)]
    private void OcupieCellClientRpc(Vector3Int coords, NetworkObjectReference fieldObjectReference)
    {
        Debug.Log("Test3");
        Cell cell = CellsInBoard[coords];
        fieldObjectReference.TryGet(out NetworkObject fieldNetworkObject);
        FieldObject fieldObject = fieldNetworkObject.GetComponent<FieldObject>();
        Debug.Log(fieldObject.name + " окупировал " + cell);
        cell.isOcupied = true;
        cell.objectOnTile = fieldObject;
        fieldObject.CurrentCell = cell;
    }


    public void Init()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        tiles = grid.GetComponentsInChildren<TileMapManager>(false).ToList<TileMapManager>();
        Debug.Log("Board Manager Inited");

        CellsInBoard = new Dictionary<Vector3Int, Cell>();

        foreach (var tile in tiles)
        {
            tile.CreateCellsInTile();
            Debug.Log("CellsInTile: " + tile.cellsInTileDict.Count);
            tile.AddCellsFromTileMapToBoard();
        }

        AddNeighboursToCells();
        IsReady = true;
    }

    public void AddNeighboursToCells()
    {
        Debug.Log("cellsInBoard " + CellsInBoard.Count);
        foreach (var cell in CellsInBoard)
        {
            List<Tuple<int, int>> possibleCoords = UtilClass.FindPossibleNeighbourCoord(cell.Value);
            foreach (var tuple in possibleCoords)
            {
                Cell existingNeighbour;
                if (CellsInBoard.TryGetValue(new Vector3Int(tuple.Item1, cell.Value.coords.y, tuple.Item2), out existingNeighbour)) {
                    //Debug.Log("tuple: " + tuple);
                    //if (checkWalls(cell.Value, existingNeighbour))
                    //{
                        cell.Value.neighbours.Add(existingNeighbour);
                    //}
                }
            }
        }
    }

    public void PlaceHeroesOnABoard()
    {
        if (!IsServer)
        {
            return;
        }

        List<HeroData> heroes = GameManager.Instance.Heroes;

        if (starterTile == null)
        {
            throw new Exception("Остутствует стартовый тайл или не проставлен тег стартового тайла");
        }

        for (int i = 0; i < heroes.Count; i++)
        {
            var heroData = heroes[i];
            Cell randomStarterCell = GetRandomNotOcupiedCellFromTileMap(starterTile);
            Vector3Int coords = randomStarterCell.coords;

            SpawnHeroClientRpc(heroData.HeroNumber, randomStarterCell.coords);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SpawnHeroClientRpc(int heroNumber, Vector3Int coords)
    {
        Debug.Log("Попытался достать героя с номером: " + heroNumber);
        Debug.Log(GameManager.Instance);
        var networkObjectReference = GameManager.Instance.GetHeroNetworkObjectReferenceByNumber(heroNumber);
        networkObjectReference.TryGet(out NetworkObject networkObjectFieldHero);

        var hero = networkObjectFieldHero.GetComponent<FieldHero>();

        BoardManager.Instance.OcupieCellServerRpc(coords, networkObjectReference);
        //var cell = CellsInBoard[position];
        //cell.OcupieCell(hero);
        var cell = BoardManager.Instance.CellsInBoard[coords];
        hero.CurrentCell = cell;
        //hero.OcupiedCells.Clear(); // на всякий случай
        //hero.OcupiedCells.Add(cell);

        hero.transform.position = coords + new Vector3(0.5f, Constants.LIL_ABOVE_FLOOR, 0.5f);
        hero.gameObject.SetActive(true);
    }

    internal void AddEnemyOnBoard(EnemyObject enemyPrefabClone, TileMapManager tileToSpawnEnemeies)
    {
        Cell randomCell = GetRandomNotOcupiedCellFromTileMap(tileToSpawnEnemeies);
        Vector3Int coords = randomCell.coords;
        PlaceEnemyOnCellRpc(enemyPrefabClone.GetNetworkObjectReference(), coords);
    }

    [Rpc(SendTo.Everyone)]
    private void PlaceEnemyOnCellRpc(NetworkObjectReference networkObjectReference, Vector3Int coords)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        var enemy = networkObject.GetComponent<EnemyObject>();

        enemy.transform.position = coords + new Vector3(0.5f, Constants.LIL_ABOVE_FLOOR, 0.5f);
        enemy.LookOn(GameManager.Instance.Heroes[
            new System.Random().Next(0, GameManager.Instance.Heroes.Count)]
            .FieldHero.transform.position);
        BoardManager.Instance.OcupieCellServerRpc(coords, networkObjectReference);
        //var cell = CellsInBoard[coords];
        //cell.OcupieCell(enemy);
    }

    //Взять случайную незанятуб ячейку из стартового тайла
    private static Cell GetRandomNotOcupiedCellFromTileMap(TileMapManager starterTile)
    {
        var notOcupiedCerllsInTile = starterTile.cellsInTileDict.Where(item => item.Value.isOcupied == false).ToDictionary(i => i.Key, i => i.Value);
        int r = UnityEngine.Random.Range(0, notOcupiedCerllsInTile.Count);
        return notOcupiedCerllsInTile.ElementAt(r).Value;
    }
}

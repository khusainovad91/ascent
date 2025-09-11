using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.EventSystems;
using ExtensionMethods;
using Unity.VisualScripting;
using Unity.Netcode;
using System;

public class HeroControllerManager : NetworkBehaviour//PersistentSingleton<HeroControllerManager>
{
    public static HeroControllerManager Instance;
    public FieldHero FieldHero;
    private PathFinder _pathFinder;

    [SerializeField] private GameObject _pathNumber;
    //4 testing purpose maded public
    [SerializeField] public GameObject FloorNumberPrefab;
    [SerializeField] public GameObject _possibleCellsToGoPrefab;
    [SerializeField] private Camera _currentCamera;

    private GameObject _possibleCellsToGo;
    private Vector3Int _whereToGo;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<FieldHero>("event_SelectHero", SetUpSelectedHero);
        EventManager.Instance.Subscribe("event_UnselectHero", UnSelectHero);
        EventManager.Instance.Subscribe<HeroData>("OnMpChanged", HandleMpChange);
        EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", HandleStateChange);
        EventManager.Instance.Subscribe("OnEnemyTurn", UnSelectHero);
        EventManager.Instance.Subscribe("RecalculateMovement", RecalculateMovement);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe("event_UnselectHero", UnSelectHero);
            EventManager.Instance.Unsubscribe<FieldHero>("event_SelectHero", SetUpSelectedHero);
            EventManager.Instance.Unsubscribe<HeroData>("OnMpChanged", HandleMpChange);
            EventManager.Instance.Unsubscribe<HeroData>("OnHeroStateChange", HandleStateChange);
            EventManager.Instance.Unsubscribe("OnEnemyTurn", UnSelectHero);
            EventManager.Instance.Unsubscribe("RecalculateMovement", RecalculateMovement);
        }
    }

    private void RecalculateMovement()
    {
        if (FieldHero == null || GameManager.Instance.StateOfGame.Value != GameState.HeroTurn || FieldHero.HeroData.CurrentState == HeroState.Moving) return;

        ClearPossibleCellsToGo();
        InitializePathFinder();
    }

    private void SetUpSelectedHero(FieldHero hero)
    {
        FieldHero = hero;
        StartCoroutine(CameraController.Instance.MoveCameraToTarget(FieldHero.transform, 1f));

        if (!hero.IsOwner)
        {
            return;
        }

        InitializePathFinder();
    }

    public void UnSelectHero()
    {
        ClearUiOnFloor();
        FieldHero = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //Board = BoardManager.Instance.CellsInBoard;
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Update()
    {
        if (FieldHero == null) return;
        if (!FieldHero.IsOwner)
        {
            return;
        }
        switch (SelectControllerManager.Instance.currentMode)
        {
            case SelectionMode.Free:
                if (FieldHero.HeroData.CurrentState == HeroState.Idle)
                {
                    ShowPossibleCellsToGo(_possibleCellsToGoPrefab, FieldHero.HeroData.Stats.MovementPoints);
                    HandlePathSelection();
                } else
                {
                    ClearPossibleCellsToGo();
                }
                break;
            case SelectionMode.Hero:
                break;
        }
    }

    //private void HandleStateChange(HeroData heroData)
    //{
    //    if (_fieldHero?.HeroData != heroData) return;

    //    if (heroData.CurrentState == HeroState.Idle)
    //    {
    //        InitializePathFinder();
    //    }
    //}

    private void HandleStateChange(HeroData heroData)
    {
        if (FieldHero?.HeroData != heroData) return;

        if (heroData.CurrentState == HeroState.Idle)
        {
            InitializePathFinder();
        }
    }

    public void ClearUiOnFloor()
    {
        ClearPossibleCellsToGo();
        ClearPossiblePath();
    }

    private void HandlePathSelection()
    {
        var mouseCell = GetMousePositionCell();
        if (mouseCell == null || mouseCell.isOcupied || GameManager.Instance.IsAnythingIsMovingNow) return;

        ShowPossiblePath(mouseCell, FieldHero.HeroData.Stats.MovementPoints);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SelectPath(mouseCell);
        }
    }

    private void InitializePathFinder()
    {
        _pathFinder = new PathFinder(FieldHero.CurrentCell, FieldHero.HeroData.Stats.MovementPoints);

        if (FieldHero.HeroData.Stats.MovementPoints > 0)
        {
            ShowPossibleCellsToGo(_possibleCellsToGoPrefab, FieldHero.HeroData.Stats.MovementPoints);
        }
    }

    private void HandleMpChange(HeroData heroData)
    {
        if (FieldHero == null || FieldHero.HeroData != heroData || FieldHero.HeroData.CurrentState == HeroState.Moving) return;

        ClearPossibleCellsToGo();
        InitializePathFinder();
    }


    //показывает возможные клетки для передвижения
    public void ShowPossibleCellsToGo(GameObject pointerPrefab, int movementPoints)
    {
        if (!FieldHero.IsOwner) return;
        _pathFinder.UpdateNotOcupiedCellsInRange(FieldHero.CurrentCell);
        if (_possibleCellsToGo != null)
        {
            GameObject.Destroy(_possibleCellsToGo);
        }
        _possibleCellsToGo = new GameObject("possibleCellsToGo");

        foreach (var item in _pathFinder.cellsToChooseFrom)
        {
            var path = _pathFinder.FindPath(FieldHero.CurrentCell, item.Value);
            bool isPathPossible = path != null;
            if (isPathPossible && path.Count() <= movementPoints + 1)
            {
                GameObject pointer = GameObject.Instantiate(pointerPrefab, item.Value.coords.CenterOfCell(), pointerPrefab.transform.rotation);
                pointer.transform.parent = _possibleCellsToGo.transform;
            }
        }
    }

    private void ShowPossiblePath(Cell mouseCell, int _movementPoints)
    {
        var path = _pathFinder.FindPathMaxLengthUpdateCells(BoardManager.Instance.CellsInBoard[FieldHero.CurrentCell.coords], BoardManager.Instance.CellsInBoard[mouseCell.coords], _movementPoints);

        if (mouseCell.isOcupied || !_pathFinder.cellsToChooseFrom.ContainsKey(mouseCell.coords))
        {
            ClearPossiblePath();
        }

        if (this._whereToGo == mouseCell.coords || path == null)
            return;

        _pathNumber.transform.position = this.FieldHero.CurrentCell.coords;
        ClearPossiblePath();
        var renderer = FloorNumberPrefab.GetComponent<Renderer>();
        renderer.sharedMaterial.color = FieldHero.HeroData.PlayerColor;

        //показываем новый путь
        GameObject previousTrack = null;
        for (int i = 0; i < path.Count; i++)
        {
            var go = Instantiate(
                FloorNumberPrefab,
                path[i].Cell.coords.CenterOfCell() + new Vector3(0.0f, Constants.LIL_ABOVE_FLOOR, 0.0f),
                FloorNumberPrefab.transform.rotation,
                _pathNumber.transform
            );
            if (i == 0)
            {
                go.GetComponent<Track>().FaceOut(FieldHero.gameObject);
            } else
            {
                if (previousTrack != null && previousTrack != go)
                {
                    go.GetComponent<Track>().FaceOut(previousTrack);
                }
            }
            previousTrack = go;
            go.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (i + 1).ToString();
            go.transform.GetChild(0).AddComponent<FaceCameraFromFloor>();
        }

        this._whereToGo = mouseCell.coords;
    }

    //уничтожает объект хранящий возможные клетки для передвижения
    public void ClearPossibleCellsToGo()
    {
        if (_possibleCellsToGo != null)
        {
            GameObject.Destroy(_possibleCellsToGo);
        }
    }

    //чистим путь
    private void ClearPossiblePath()
    {
        foreach (Transform item in _pathNumber.transform)
        {
            Destroy(item.gameObject);
        }
    }

    private Cell GetMousePositionCell()
    {
        var ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Constants.everyThingMask) && hit.collider.CompareTag("Tile"))
        {
            var coordinates = Vector3Int.FloorToInt(hit.point);
            if (BoardManager.Instance.CellsInBoard.TryGetValue(coordinates, out Cell possibleCell))
            {
                return possibleCell;
            }
        }
        else
        {
            ClearUiOnFloor();
        }

        return null;
    }

    private void SelectPath(Cell mouseCell)
    {
        var chosenPath = _pathFinder.FindPathMaxLengthUpdateCells(FieldHero.CurrentCell, mouseCell, FieldHero.HeroData.Stats.MovementPoints);
        if (chosenPath == null) return;
        if (_pathFinder.IsCellInPath(mouseCell, chosenPath))
        {
            var nor = FieldHero.GetNetworkObjectReference();
            Coords[] coords = chosenPath.ToVector3List().ToCoords();
            //CommandManager.Instance.EnqeueueMoveHeroCommandRpc(nor, coords);
            //todo delete
            MoveHeroCommand moveCommand = new MoveHeroCommand(FieldHero, chosenPath.Select(node => node.Cell).ToList());
            CommandManager.Instance.AddCommand(moveCommand);
            ClearUiOnFloor();
        }
    }

}


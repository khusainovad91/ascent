using UnityEngine;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;

public class FieldStaticObject : FieldObject
{
    public bool isRandomRotated;
    [SerializeField]
    private bool _isRandomPlaced;
    [SerializeField]
    private TileMapManager _tileMap;

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe("GameStarting", PlaceObjectOnAField);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe("GameStarting", PlaceObjectOnAField);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Obstacles");
        //toddo delete
        //transform.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Obstacles");
        ////сперва запишется в Board manager доска потом выполнится эта функция
        ////TODO точно переделать, на разных компах может сцена дольше загружаться чем 0.2f
        //Invoke("PlaceObjectOnAField", 0.2f);
    }

    private void PlaceObjectOnAField()
    {
        if (!IsServer) return;
        if (_isRandomPlaced && _tileMap != null)
        {
            switch (Size)
            {
                case 1:
                    this.OcupiedCells.Add(_tileMap.GetRandomNonOcupiedCellInTile());
                    CurrentCell.PlaceObjectInACell(this);
                    break;
                case 2:
                    this.OcupiedCells.AddRange(_tileMap.GetRandomNotOcupiedCellsInTile(2));
                    break;
                default:
                    break;
            }
            
            //todo delete
            //else if (Size == 1)
            //{

            //} else return;
            //this.OcupiedCells.Add(_tileMap.GetRandomNonOcupiedCellInTile());
            //this.CurrentCell = _tileMap.GetRandomNonOcupiedCellInTile();
            //CurrentCell.PlaceObjectInACell(this);
        } 
        else {
            if (Size <= 1)
                {
                    OcupiedCells.Add(this.PlaceObjectInACell());
                } else
                {
                    var coordsOfObject = UtilClass.GetRenderCoordinates(this.gameObject);
                foreach (var coords in coordsOfObject)
                {
                    this.PlaceObjectOnABoardByCoordinates(coords);
                    Debug.Log("Coords: " + this.name + "\n coords" + coords);
                }
            }
            if (CurrentCell == null)
            {
                Debug.Log("Предмет не на поле, уничтожаю: " + this.name);
                GameObject.Destroy(this.gameObject);
            }
        }

        if (isRandomRotated)
        {
            this.transform.Find("Visuals").transform.RotateAround(this.transform.position, Vector3.up, Random.Range(0, 360));
        }
    }

}

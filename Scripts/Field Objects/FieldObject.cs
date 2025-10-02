using ExtensionMethods;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(RightClickHandler))]
[RequireComponent(typeof(ConditionHandler))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(NetworkObject))]
public class FieldObject : NetworkBehaviour
{
    public string Name;
    public bool IsSolid;
    public int Size = 1;
    //public List<Cell> OcupiedCells = new();
    public Cell CurrentCell; //=> OcupiedCells.FirstOrDefault();
    //public Cell CurrentCell;
    [NonSerialized]
    public Outline Outline;
    public GameObject Model { get; private set; }
    [field: SerializeField]
    public Color defaultColor {  get; protected set; }

    [NonSerialized]
    public NetworkVariable<bool> isTargeted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected bool isMovable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Outline = GetComponent<Outline>();
        Outline.enabled = false;
    }

    private void OnEnable()
    {
        Model = transform.Find("Model").gameObject;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        if (CurrentCell != null)
        {
            BoardManager.Instance.ClearCellServerRpc(CurrentCell.coords);
            EventManager.Instance.TriggerEvent("RecalculateMovement");
            //CurrentCell.ClearCell();
        }
    }

    public void SetDefaultColor()
    {
        Outline.OutlineColor = defaultColor;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public NetworkObjectReference GetNetworkObjectReference()
    {
        try
        {
            return new NetworkObjectReference(this.gameObject);
        } catch (ArgumentException e)
        {
            Debug.LogError($"{this.name} /// {this.transform.position}");
            throw e;
        }
    }

    public void LookOn(Vector3 there)
    {
        Vector3 direction = there - transform.position;
        direction.y = 0; // Фиксируем поворот только по горизонтали
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //this.Model.transform.LeanRotate(targetRotation.eulerAngles, 0.2f)
        //        .setEase(LeanTweenType.easeOutQuad);
        LookOnRpc(targetRotation.eulerAngles);
    }

    [Rpc(SendTo.Server)]
    private void LookOnRpc(Vector3 eulerAngles)
    {
        LookOnClientRpc(eulerAngles);
    }

    [Rpc(SendTo.Everyone)]
    private void LookOnClientRpc(Vector3 eulerAngles)
    {
        if (!IsOwner) return;
        this.Model.transform.LeanRotate(eulerAngles, 0.2f)
            .setEase(LeanTweenType.easeOutQuad);
        this.GetComponent<DeffendingDicePool>().transform.LeanRotate(eulerAngles, 0.2f)
            .setEase(LeanTweenType.easeOutQuad);
        this.GetComponent<AttackingDicePool>().transform.LeanRotate(eulerAngles, 0.2f)
            .setEase(LeanTweenType.easeOutQuad);
    }

    [Rpc(SendTo.Server)]
    public void SetIsTargetedRpc(bool isTargeted)
    {
        this.isTargeted.Value = isTargeted;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        BoardManager.Instance.ClearCellServerRpc(CurrentCell.coords);
    }

    public void OnMouseEnter()
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) return;
        if (HeroControllerManager.Instance.FieldHero == this || isTargeted.Value)
        {
            return;
        }
        this.Outline.OutlineColor = Color.white;
        this.Outline.enabled = true;
    }

    public void OnMouseExit()
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) return;
        if (HeroControllerManager.Instance.FieldHero == this)
        {
            return;
        }
        this.Outline.enabled = false;
    }

    [Rpc(SendTo.Server)]
    public void SetCellRpc()
    {
        SetCellClientRpc(this.transform.position);
    }

    [Rpc(SendTo.Everyone)]
    private void SetCellClientRpc(Vector3 coords)
    {
        BoardManager.Instance.CellsInBoard.TryGetValue(this.GetVector3IntPosition(), out var currentCell);
        if (currentCell != null)
        {
            this.CurrentCell = currentCell;
            //OcupiedCells.Add(currentCell);
        }
    }
}

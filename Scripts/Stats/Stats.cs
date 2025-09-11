using System.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

//?убрать MonoBehavior?
public abstract class Stats: NetworkBehaviour
{
    [field:SerializeField]
    public int maxActionsAmount { get; protected set; }
    [field:SerializeField]
    public int MaxHP { get; protected set; }
    [field:SerializeField]
    public int Hp { get; protected set; }
    [field: SerializeField]
    public int Speed { get; protected set; }
    //Очки передвижения
    [field:SerializeField]
    public int MovementPoints { get; protected set; }
    [field:SerializeField]
    public int ActionsAmount { get; protected set; }

    //public abstract void ChangeHealthRpc(int hp);

    //public abstract void ChangeMovementPointsRpc(int mp);

    //public abstract void ChangeActionsAmountRpc(int actionsAmount);

    [Rpc(SendTo.Everyone)]
    public void DropActionsRpc()
    {
        this.ActionsAmount = 0;
    }

    [Rpc(SendTo.Everyone)]
    public void DropMovementPointRpc()
    {
        this.MovementPoints = 0;
    }
}

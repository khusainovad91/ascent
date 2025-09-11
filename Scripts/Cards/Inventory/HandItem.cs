using UnityEngine;

public abstract class HandItem : InventoryItem
{
    [field: SerializeField]
    public int handsAmount { get; private set; }
}

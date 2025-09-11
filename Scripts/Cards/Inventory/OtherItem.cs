using UnityEngine;

//Герой может носить до двух любых
public class OtherItem : InventoryItem
{
    [field: SerializeField]
    public int amountOfSpace { get; private set; }
}

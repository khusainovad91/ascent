using UnityEngine;

//Все оружие должно наследоваться от этого класса
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Items/Weapon")]
public class WeaponItem : HandItem
{
    //сумма максимальной дальности на костях + дальность с sun/skull
    [field: SerializeField] public int MaxWeaponRange {  get; private set; }
    [field: SerializeField] public bool isMelee { get; private set; }
    private void OnEnable()
    {
        UiCard.GetComponent<UiCardAttack>().WeaponItem = this;
    }
}

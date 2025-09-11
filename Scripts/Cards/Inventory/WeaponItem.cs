using UnityEngine;

//��� ������ ������ ������������� �� ����� ������
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Items/Weapon")]
public class WeaponItem : HandItem
{
    //����� ������������ ��������� �� ������ + ��������� � sun/skull
    [field: SerializeField] public int MaxWeaponRange {  get; private set; }
    [field: SerializeField] public bool isMelee { get; private set; }
    private void OnEnable()
    {
        UiCard.GetComponent<UiCardAttack>().WeaponItem = this;
    }
}

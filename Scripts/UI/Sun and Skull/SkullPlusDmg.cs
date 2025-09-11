using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkullPlusDmg : Skull
{
    [SerializeField]
    private int _dmg;
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_hac == null)
        {
            return;
        }

        if (_hac.skulls - _skullCost < 0) return;

        if (_hac.usedAbilities.Contains(this))
        {
            return;
        }

        base.OnPointerClick(eventData);
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Dice);
        _hac.skulls -= _skullCost;

        AddDmg(_dmg);
    }

    private void AddDmg(int dmg)
    {
        _hac.usedAbilities.Add(this);
        _hac.bonusDmg += _dmg;
    }
}

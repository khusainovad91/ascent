using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SunPlusRange : Sun
{
    [SerializeField]
    private int _range;
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_hac == null)
        {
            return;
        }

        if (_hac.suns - _sunCost < 0) return;

        if (_hac.usedAbilities.Contains(this))
        {
            return;
        }

        base.OnPointerClick(eventData);
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Dice);
        _hac.suns -= _sunCost;

        AddRange(_range);
    }

    private void AddRange(int range)
    {
        _hac.usedAbilities.Add(this);
        _hac.bonusRange = _range;
    }
}

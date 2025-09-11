using UnityEngine;
using UnityEngine.UI;

//todo delete
[RequireComponent (typeof(Button))]
public class PatchButton : UiButton
{
    private new void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(PatchWounds);
    }

    private void PatchWounds()
    {
        if (_heroData.Stats.ActionsAmount <= 0) return;
        //todo добавить в описание
        //todo что-то другое может сделать с этой механикой?
        if (_heroData.Stats.Hp < _heroData.Stats.MaxHP / 2)
        {
            _heroData.Stats.ChangeActionsAmountRpc(-1);
            _heroData.Stats.ChangeHealthRpc(+2);
        }
    }
}

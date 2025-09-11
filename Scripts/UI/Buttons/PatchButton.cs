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
        //todo �������� � ��������
        //todo ���-�� ������ ����� ������� � ���� ���������?
        if (_heroData.Stats.Hp < _heroData.Stats.MaxHP / 2)
        {
            _heroData.Stats.ChangeActionsAmountRpc(-1);
            _heroData.Stats.ChangeHealthRpc(+2);
        }
    }
}

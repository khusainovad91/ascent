using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class RestButton : UiButton
{
    private new void Awake()
    {
        base.Awake();
        this.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Rest()));
    }

    private IEnumerator Rest()
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) { yield break; }
        if (_heroData.CurrentState != HeroState.Idle) { yield break; }
        if (_heroData.Stats.ActionsAmount <= 0) yield break;

        yield return UtilClass.PlayClickAnimation(this.gameObject);

        _heroData.Stats.ChangeActionsAmountRpc(-1);
        _heroData.FieldHero.GetComponent<ConditionHandler>().AddConditionRpc(ConditionType.Rested);
        this.gameObject.SetActive(false);
    }
}

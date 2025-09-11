using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class MoveButton : UiButton
{
    [SerializeField] GameObject fatigueToMpButton;

    private new void Awake()
    {
        base.Awake();
        this.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(AddMovementPoints()));
    }

    private IEnumerator AddMovementPoints()
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) { yield break; }
        if (_heroData.CurrentState != HeroState.Idle) { yield break; }

        if (_heroData.Stats.ActionsAmount <= 0) yield break;
        yield return UtilClass.PlayClickAnimation(this.gameObject);

        _heroData.Stats.ChangeActionsAmountRpc(-1);
        _heroData.Stats.ChangeMovementPointsRpc(_heroData.Stats.Speed);
        //enum Conidtion.Tired, int duration
        _heroData.FieldHero.GetComponent<ConditionHandler>().AddConditionRpc(ConditionType.Tired);
        this.gameObject.SetActive(false);
        fatigueToMpButton.SetActive(true);
        fatigueToMpButton.GetComponent<FatigueToMpButton>().SetUp(_heroData);
        
    }
}

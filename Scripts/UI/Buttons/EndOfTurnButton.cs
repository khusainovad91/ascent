using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndOfTurnButton : UiButton
{
    private new void Awake()
    {
        base.Awake();
        this.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(EndTurn()));
    }

    private IEnumerator EndTurn()
    {
        if (_heroData.CurrentState != HeroState.Idle) { yield break; }
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) { yield break; }

        _heroData.ChangeState(HeroState.EndedTurn);

        yield return UtilClass.PlayClickAnimation(this.gameObject);

        foreach (var uiCard in _heroData.InventoryData.CachedAllCards.Where(uiCard => uiCard.AnimateInHeroEnd))
        {
            yield return uiCard.AnimateInEnd();
        }


        EventManager.Instance.TriggerEvent("event_UnselectHero");

        yield return null;
    }
}

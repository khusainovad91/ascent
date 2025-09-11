using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReactButton : MonoBehaviour
{
    HeroData _heroData;

    public void SetUp(HeroData heroData)
    {
        _heroData = heroData;
    }

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(EndReactPhase()));
    }

    //todo remake
    private IEnumerator EndReactPhase()
    {
        yield return UtilClass.PlayClickAnimation(this.gameObject);

        EventManager.Instance.TriggerEvent("event_UnselectHero");
        _heroData.ChangeState(HeroState.EndedTurn);
        UiManager.Instance.HandleReact();
    }
}

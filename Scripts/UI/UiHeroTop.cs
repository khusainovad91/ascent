using UnityEngine;
using UnityEngine.EventSystems;

public class UiHeroTop : MonoBehaviour, IPointerClickHandler
{
    private HeroData _heroData;
    public void SetUpUiHeroTop(HeroData heroData)
    {
        _heroData = heroData;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free || GameManager.Instance.StateOfGame.Value != GameState.HeroTurn)
        {
            //CameraController.Instance.MoveCameraToTarget(_heroData.FieldHero.transform, 1f);
            return;
        }
        EventManager.Instance.TriggerEvent("event_UnselectHero");
        EventManager.Instance.TriggerEvent("event_SelectHero", _heroData.FieldHero);
    }
}

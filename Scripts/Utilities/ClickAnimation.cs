using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAnimation : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UtilClass.PlayClickAnimation(this.gameObject, 0.9f, Constants.CLICK_ANIMATION_TIME);
    }
}

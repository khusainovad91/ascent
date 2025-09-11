using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UiActiveCard : UiCard, IPointerEnterHandler, IPointerExitHandler
{
    //-----------------------------------
    // ACTIVE CARD ANIMATIONS
    //-----------------------------------
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (GetComponent<CardFlip>().IsFaceUp)
        {
            transform.SetAsLastSibling();
            var rectTransform = ChildCardForAnimation.GetComponent<RectTransform>();
            rectTransform.LeanScale(new Vector3(2, 2, 2), 0.2f).setEase(LeanTweenType.easeOutBack);
            rectTransform.LeanMove(new Vector3(0, 70), 0.2f);
        }
    }

    // Метод для уменьшения карты
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<CardFlip>().IsFaceUp)
        {
            var rectTransform = ChildCardForAnimation.GetComponent<RectTransform>();
            rectTransform.LeanScale(new Vector3(1, 1, 1), 0.2f).setEase(LeanTweenType.easeOutBack);
            rectTransform.LeanMove(new Vector3(0, 0), 0.2f);
        }
    }

}

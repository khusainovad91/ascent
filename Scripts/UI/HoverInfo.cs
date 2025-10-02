using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] string text;
    private bool isMouseOverAnObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverInfoDescription.Instance.ToggleOn();
        HoverInfoDescription.Instance.SetText(text);
        isMouseOverAnObject = true;
    }

    public void Update()
    {
        if (isMouseOverAnObject)
        {
            Vector2 mousePos = GetMousePositionUi();
            HoverInfoDescription.Instance.SetPosition(mousePos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverInfoDescription.Instance.ToggleOff();
        isMouseOverAnObject = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HoverInfoDescription.Instance.ToggleOff();
    }

    private Vector2 GetMousePositionUi()
    {
        Vector2 screenPosition = Input.mousePosition;

        RectTransform rectTransform = UiManager.Instance?.GetComponent<RectTransform>() ?? HoverInfoDescription.Instance.transform.parent.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, // Целевой RectTransform
            screenPosition, // Экранные координаты
            this.gameObject.GetComponentInParent<Canvas>().worldCamera, // Камера, используемая для рендеринга Canvas
            out Vector2 localPosition // Выходные локальные координаты
        );

        return localPosition;
    }


}

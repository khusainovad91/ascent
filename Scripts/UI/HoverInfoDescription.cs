using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfoDescription : PersistentSingleton<HoverInfoDescription>
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject tmp;
    //Отступы
    public Vector2 padding = new Vector2(20, 10);

    // Отступ от курсора мыши
    private Vector2 offset = new Vector2(20, 20);
    // Отступ от краёв экрана

    private new void Awake()
    {
        base.Awake();
        ToggleOff();
    }

    // Метод для установки текста
    public void SetText(string _text)
    {
        tmp.GetComponent<TMP_Text>().text = _text;
        SetSize();
    }

    void SetSize()
    {
        float prefferedHeight = tmp.GetComponent<TMP_Text>().preferredHeight;
        float prefferedWidth = tmp.GetComponent<TMP_Text>().preferredWidth;
        Vector2 newSize = new Vector2(prefferedWidth, prefferedHeight) + padding;
        this.GetComponent<RectTransform>().sizeDelta = newSize;
        tmp.GetComponent<RectTransform>().sizeDelta = newSize;
        background.GetComponent<RectTransform>().sizeDelta = newSize + newSize / 4 + new Vector2(0, 10);
    }

    public void ToggleOn() {
        background.SetActive(true);
        tmp.SetActive(true);
    }

    public void ToggleOff()
    {
        background.SetActive(false);
        tmp.SetActive(false);
    }

    // Метод для установки позиции UiDescription
    public void SetPosition(Vector2 mousePos)
    {
        // Получаем размеры UiDescription
        Vector2 size = GetComponent<RectTransform>().sizeDelta;

        // Устанавливаем позицию UiDescription
        GetComponent<RectTransform>().anchoredPosition = mousePos + offset;

        // Корректируем позицию, чтобы UiDescription не выходил за пределы экрана
        ClampToScreen(size);
    }

    // Метод для ограничения позиции в пределах экрана
    private void ClampToScreen(Vector2 size)
    {
        // Получаем RectTransform родительского Canvas
        RectTransform parentRect = GetComponent<RectTransform>().parent as RectTransform;

        // Получаем границы экрана в локальных координатах Canvas
        Vector2 minPosition = parentRect.rect.min + new Vector2(padding.x, padding.y);
        Vector2 maxPosition = parentRect.rect.max - new Vector2(size.x + padding.x, size.y + padding.y);

        // Текущая позиция UiDescription
        Vector2 clampedPosition = GetComponent<RectTransform>().anchoredPosition;

        // Ограничиваем позицию по X
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);

        // Ограничиваем позицию по Y
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minPosition.y, maxPosition.y);

        // Применяем новую позицию
        GetComponent<RectTransform>().anchoredPosition = clampedPosition;
    }
}

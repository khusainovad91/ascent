using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfoDescription : PersistentSingleton<HoverInfoDescription>
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject tmp;
    //�������
    public Vector2 padding = new Vector2(20, 10);

    // ������ �� ������� ����
    private Vector2 offset = new Vector2(20, 20);
    // ������ �� ���� ������

    private new void Awake()
    {
        base.Awake();
        ToggleOff();
    }

    // ����� ��� ��������� ������
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

    // ����� ��� ��������� ������� UiDescription
    public void SetPosition(Vector2 mousePos)
    {
        // �������� ������� UiDescription
        Vector2 size = GetComponent<RectTransform>().sizeDelta;

        // ������������� ������� UiDescription
        GetComponent<RectTransform>().anchoredPosition = mousePos + offset;

        // ������������ �������, ����� UiDescription �� ������� �� ������� ������
        ClampToScreen(size);
    }

    // ����� ��� ����������� ������� � �������� ������
    private void ClampToScreen(Vector2 size)
    {
        // �������� RectTransform ������������� Canvas
        RectTransform parentRect = GetComponent<RectTransform>().parent as RectTransform;

        // �������� ������� ������ � ��������� ����������� Canvas
        Vector2 minPosition = parentRect.rect.min + new Vector2(padding.x, padding.y);
        Vector2 maxPosition = parentRect.rect.max - new Vector2(size.x + padding.x, size.y + padding.y);

        // ������� ������� UiDescription
        Vector2 clampedPosition = GetComponent<RectTransform>().anchoredPosition;

        // ������������ ������� �� X
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);

        // ������������ ������� �� Y
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minPosition.y, maxPosition.y);

        // ��������� ����� �������
        GetComponent<RectTransform>().anchoredPosition = clampedPosition;
    }
}

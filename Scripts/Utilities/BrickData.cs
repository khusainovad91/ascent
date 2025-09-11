using UnityEngine;

[RequireComponent(typeof(Outline))]
public class BrickData : MonoBehaviour
{
    public Cell cell;
    public Color defaultColor = Color.white;
    private Outline _outline;

    private void OnEnable()
    {
        _outline = GetComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineWidth = 5;
        _outline.enabled = false;
    }

    public void OnMouseEnter()
    {
        _outline.OutlineColor = Color.white;
        _outline.enabled = true;
    }

    public void OnMouseExit()
    {
        _outline.enabled = false;
    }
}

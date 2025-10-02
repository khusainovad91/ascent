using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(TextMeshProUGUI))]
public class WhatToDoText : Singleton<WhatToDoText>
{
    private TextMeshProUGUI _text;

    protected override void Awake()
    {
        base.Awake();
        _text = this.GetComponent<TextMeshProUGUI>();
        this.gameObject.SetActive(false);
    }

    public void SetTextAndEnable(string text)
    {
        _text.SetText(text);
        this.gameObject.SetActive(true);
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 1f).setLoopPingPong();
    }

    public void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
        this.gameObject.SetActive(false);
    }
}

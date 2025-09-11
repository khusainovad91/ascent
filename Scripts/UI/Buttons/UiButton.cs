using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public abstract class UiButton : MonoBehaviour
{
    protected HeroData _heroData;
    private Color _defaultBackgroundColor;

    public void SetUp(HeroData heroData)
    {
        this._heroData = heroData;
    }

    protected void Awake()
    {
        _defaultBackgroundColor = GetComponent<Image>().color;
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null) 
        {
            EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", HandleStateChange);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnHeroStateChange", HandleStateChange);
        }
    }

    private void HandleStateChange(HeroData heroData)
    {
        if (this._heroData == heroData)
        {
            if (heroData.CurrentState == HeroState.RollingDice)
            {
                TuggleOff();
            }
            else
            {
                TuggleOn();
            }
        }
    }

    protected void TuggleOn()
    {
        //LeanTween.color(this.gameObject, _defaultBackgroundColor, 1f);
        GetComponent<Button>().interactable = true;
    }

    protected void TuggleOff()
    {
        //this.GetComponent<Image>().color = UtilClass.DarkenColor(_defaultBackgroundColor);
        //LeanTween.color(this.GetComponent<Image>(), UtilClass.DarkenColor(_defaultBackgroundColor), 1f);
        GetComponent<Button>().interactable = false;
    }
}

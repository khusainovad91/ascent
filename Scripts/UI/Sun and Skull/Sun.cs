using UnityEngine;
using UnityEngine.EventSystems;
using ExtensionMethods;
using TMPro;
using System.Xml;


public abstract class Sun : SunAndSkull
{
    [SerializeField] protected int _sunCost;

    protected new virtual void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.Subscribe<HeroAttackCommand>("ChooseSunSkull", SetUpHac);
    }

    protected new virtual void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.Unsubscribe<HeroAttackCommand>("ChooseSunSkull", SetUpHac);
    }

    private void SetUpHac(HeroAttackCommand hac)
    {
        if (this._heroData != hac.FieldHero.HeroData || hac.suns <= 0)
        {
            return;
        }
        HighlightThis();
        _hac = hac;
    }

}

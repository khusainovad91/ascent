using UnityEngine;

public class Skull : SunAndSkull
{
    [SerializeField] protected int _skullCost;

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
        if (this._heroData != hac.FieldHero.HeroData || hac.skulls <= 0)
        {
            return;
        }
        HighlightThis();
        _hac = hac;
    }
}
